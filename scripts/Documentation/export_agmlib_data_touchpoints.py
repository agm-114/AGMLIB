#!/usr/bin/env python3
"""Map AGMLIB source files to native types and vanilla serialized evidence.

This is a navigation aid. A lexical type-name match is not proof that a member
or prefab is used at runtime; the report makes that limitation explicit.
"""

from __future__ import annotations

import argparse
import csv
import json
import re
from collections import Counter, defaultdict
from pathlib import Path
from typing import Any


TOKEN = re.compile(r"\b[A-Za-z_][A-Za-z0-9_]*\b")


def load_json(path: Path) -> Any:
    with path.open("r", encoding="utf-8-sig") as stream:
        return json.load(stream)


def load_csv(path: Path) -> list[dict[str, str]]:
    with path.open("r", encoding="utf-8-sig", newline="") as stream:
        return list(csv.DictReader(stream))


def native_type_map(index: dict[str, Any]) -> dict[str, set[str]]:
    result: dict[str, set[str]] = defaultdict(set)
    for row in index.get("indexedTypes", []):
        if row.get("compilerGenerated"):
            continue
        full_name = row.get("fullName", "")
        if not full_name:
            continue
        simple = full_name.rsplit(".", 1)[-1].split("+", 1)[0]
        if simple and "<" not in simple:
            result[simple].add(full_name)
    return result


def prefab_maps(
    asset_index: dict[str, Any],
) -> tuple[Counter, Counter, dict[str, list[str]]]:
    component_counts = Counter(asset_index.get("prefab_component_counts", {}))
    root_counts: Counter = Counter()
    examples: dict[str, list[str]] = defaultdict(list)
    for prefab in asset_index.get("prefabs", []):
        path = prefab.get("path", "")
        root_type = prefab.get("type", "")
        if root_type:
            root_counts[root_type] += 1
            if len(examples[root_type]) < 3:
                examples[root_type].append(path)
        for component in prefab.get("components", {}):
            if len(examples[component]) < 3:
                examples[component].append(path)
    return component_counts, root_counts, examples


def behaviour_counts(rows: list[dict[str, str]]) -> Counter:
    counts: Counter = Counter()
    for row in rows:
        if row.get("family") not in {
            "runtime-asset-bundle",
            "addressables-bundle",
            "player-data",
        }:
            continue
        class_name = row.get("class", "")
        namespace = row.get("namespace", "")
        if class_name:
            if namespace:
                counts[f"{namespace}.{class_name}"] += 1
            else:
                counts[class_name] += 1
    return counts


def classify(path: Path, repository_root: Path) -> str:
    relative = path.relative_to(repository_root / "AGMLIB")
    return relative.parts[0] if len(relative.parts) > 1 else "(root)"


def markdown(
    repository_root: Path,
    type_names: dict[str, set[str]],
    prefab_component_counts: Counter,
    prefab_root_counts: Counter,
    prefab_examples: dict[str, list[str]],
    raw_counts: Counter,
) -> str:
    def evidence_count(counter: Counter, name: str) -> int:
        return counter[name] + sum(counter[full] for full in type_names[name])

    source_rows: list[dict[str, Any]] = []
    type_source_counts: Counter = Counter()
    for source in sorted((repository_root / "AGMLIB").rglob("*.cs")):
        text = source.read_text(encoding="utf-8-sig", errors="replace")
        tokens = set(TOKEN.findall(text))
        matches = sorted(tokens.intersection(type_names))
        for name in matches:
            type_source_counts[name] += 1
        serialized_matches = [
            name
            for name in matches
            if evidence_count(prefab_component_counts, name) > 0
            or evidence_count(prefab_root_counts, name) > 0
            or evidence_count(raw_counts, name) > 0
        ]
        source_rows.append(
            {
                "path": source.relative_to(repository_root).as_posix(),
                "area": classify(source, repository_root),
                "native": matches,
                "serialized": serialized_matches,
            }
        )

    touched_native = sorted({name for row in source_rows for name in row["native"]})
    touched_serialized = sorted(
        {name for row in source_rows for name in row["serialized"]}
    )
    lines = [
        "# AGMLIB native/data touchpoint matrix",
        "",
        "Generated lexical navigation aid. Type-name presence does not prove runtime",
        "use, member compatibility, authority, or prefab correctness. Read the source,",
        "native consumer, and serialized graph before making a behavioral claim.",
        "",
        f"- AGMLIB C# files: {len(source_rows):,}",
        f"- Distinct pinned native type names mentioned: {len(touched_native):,}",
        f"- Mentioned native types with vanilla serialized evidence: {len(touched_serialized):,}",
        "",
        "## Highest-frequency serialized touchpoints",
        "",
        "| Native type | AGMLIB files | Registered roots | Registered prefab components | Raw serialized instances | Example vanilla prefab dumps |",
        "|---|---:|---:|---:|---:|---|",
    ]
    ranked = sorted(
        touched_serialized,
        key=lambda name: (
            -type_source_counts[name],
            -evidence_count(prefab_root_counts, name),
            -evidence_count(prefab_component_counts, name),
            -evidence_count(raw_counts, name),
            name,
        ),
    )
    for name in ranked:
        full_names = type_names[name]
        roots = evidence_count(prefab_root_counts, name)
        components = evidence_count(prefab_component_counts, name)
        raw = evidence_count(raw_counts, name)
        examples: list[str] = []
        for key in [name, *sorted(full_names)]:
            for item in prefab_examples.get(key, []):
                if item not in examples and len(examples) < 3:
                    examples.append(item)
        example_text = "<br>".join(f"`{value}`" for value in examples)
        lines.append(
            f"| `{name}` | {type_source_counts[name]} | {roots} | {components} | {raw} | {example_text} |"
        )

    lines.extend(
        [
            "",
            "## Per-source navigation",
            "",
            "| AGMLIB source | Native type names | With vanilla serialized evidence |",
            "|---|---:|---|",
        ]
    )
    for row in source_rows:
        serialized = ", ".join(f"`{name}`" for name in row["serialized"])
        lines.append(
            f"| `{row['path']}` | {len(row['native'])} | {serialized} |"
        )
    return "\n".join(lines) + "\n"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--repository-root", type=Path, required=True)
    parser.add_argument("--native-index", type=Path, required=True)
    parser.add_argument("--asset-index", type=Path, required=True)
    parser.add_argument("--mono-behaviours", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()

    repository_root = args.repository_root.resolve()
    type_names = native_type_map(load_json(args.native_index))
    prefab_component_counts, prefab_root_counts, prefab_examples = prefab_maps(
        load_json(args.asset_index)
    )
    raw_counts = behaviour_counts(load_csv(args.mono_behaviours))
    output = markdown(
        repository_root,
        type_names,
        prefab_component_counts,
        prefab_root_counts,
        prefab_examples,
        raw_counts,
    )
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(output, encoding="utf-8")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
