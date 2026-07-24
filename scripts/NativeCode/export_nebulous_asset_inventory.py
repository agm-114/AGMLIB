#!/usr/bin/env python3
"""Build a searchable metadata inventory of NEBULOUS Unity data.

This intentionally inventories names, types, logical asset paths, serialized
script identities, file hashes, and prefab-dump structure. It does not export
textures, meshes, audio, video, or other copyrighted payload bytes.
"""

from __future__ import annotations

import argparse
import base64
import csv
import gc
import hashlib
import json
import os
import re
import sys
import time
from collections import Counter, defaultdict
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Any, Iterable


@dataclass
class FileRecord:
    family: str
    relative_path: str
    size: int
    modified_utc: str
    sha256: str
    unity_scanned: bool = False
    unity_error: str = ""


def sha256_file(path: Path, chunk_size: int = 8 * 1024 * 1024) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        while chunk := stream.read(chunk_size):
            digest.update(chunk)
    return digest.hexdigest().upper()


def utc_stamp(path: Path) -> str:
    return time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime(path.stat().st_mtime))


def pptr_path_id(value: Any) -> int | None:
    if not isinstance(value, dict):
        return None
    raw = value.get("m_PathID")
    return int(raw) if raw is not None else None


def safe_name(obj: Any) -> str:
    try:
        value = obj.peek_name()
        return "" if value is None else str(value)
    except Exception:
        return ""


def safe_typetree(obj: Any) -> dict[str, Any] | None:
    try:
        value = obj.read_typetree()
        return value if isinstance(value, dict) else None
    except Exception:
        return None


def discover_files(game_root: Path) -> tuple[list[tuple[str, Path]], list[Path]]:
    data_root = game_root / "Nebulous_Data"
    candidates: list[tuple[str, Path]] = []
    raw_only: list[Path] = []

    def add_dir(family: str, directory: Path, predicate) -> None:
        if not directory.exists():
            return
        for path in sorted(p for p in directory.rglob("*") if p.is_file()):
            if predicate(path):
                candidates.append((family, path))
            else:
                raw_only.append(path)

    add_dir(
        "runtime-asset-bundle",
        game_root / "Assets" / "AssetBundles",
        lambda p: not p.name.endswith(".manifest") and p.name != "AssetBundles",
    )
    add_dir(
        "legacy-com-asset-bundle",
        game_root / "Assets" / "ComAssetBundles",
        lambda p: True,
    )
    add_dir(
        "addressables-bundle",
        data_root / "StreamingAssets" / "aa",
        lambda p: p.suffix.lower() == ".bundle",
    )

    if data_root.exists():
        root_names = {
            "globalgamemanagers",
            "globalgamemanagers.assets",
            "resources.assets",
        }
        for path in sorted(p for p in data_root.iterdir() if p.is_file()):
            if (
                path.name in root_names
                or path.name.startswith("sharedassets")
                and path.suffix.lower() == ".assets"
                or re.fullmatch(r"level\d+", path.name)
            ):
                candidates.append(("player-data", path))
            else:
                raw_only.append(path)

    for directory in (
        game_root / "Assets" / "Resources",
        data_root / "StreamingAssets" / "Localization",
        data_root / "Resources",
    ):
        if directory.exists():
            raw_only.extend(sorted(p for p in directory.rglob("*") if p.is_file()))

    # Preserve metadata alongside the scanned containers.
    for directory in (
        game_root / "Assets" / "AssetBundles",
        data_root / "StreamingAssets" / "aa",
    ):
        if directory.exists():
            raw_only.extend(
                sorted(
                    p
                    for p in directory.rglob("*")
                    if p.is_file() and p not in {item[1] for item in candidates}
                )
            )

    deduped_candidates: list[tuple[str, Path]] = []
    seen: set[Path] = set()
    for family, path in candidates:
        resolved = path.resolve()
        if resolved not in seen:
            seen.add(resolved)
            deduped_candidates.append((family, resolved))

    deduped_raw: list[Path] = []
    for path in raw_only:
        resolved = path.resolve()
        if resolved not in seen:
            seen.add(resolved)
            deduped_raw.append(resolved)

    return deduped_candidates, deduped_raw


def scan_unity_container(
    UnityPy: Any,
    game_root: Path,
    family: str,
    path: Path,
    object_type_rows: list[dict[str, Any]],
    named_rows: list[dict[str, Any]],
    script_rows: list[dict[str, Any]],
    behaviour_rows: list[dict[str, Any]],
    logical_rows: list[dict[str, Any]],
) -> tuple[bool, str]:
    relative = path.relative_to(game_root).as_posix()
    print(f"[unity] {family}: {relative}", flush=True)
    try:
        environment = UnityPy.load(str(path))
        objects = list(environment.objects)
        counts = Counter(obj.type.name for obj in objects)
        for type_name, count in sorted(counts.items()):
            object_type_rows.append(
                {
                    "family": family,
                    "container": relative,
                    "type": type_name,
                    "count": count,
                }
            )

        script_map: dict[int, dict[str, Any]] = {}
        for obj in objects:
            if obj.type.name != "MonoScript":
                continue
            tree = safe_typetree(obj) or {}
            row = {
                "family": family,
                "container": relative,
                "path_id": obj.path_id,
                "name": tree.get("m_Name") or safe_name(obj),
                "assembly": tree.get("m_AssemblyName", ""),
                "namespace": tree.get("m_Namespace", ""),
                "class": tree.get("m_ClassName", ""),
            }
            script_rows.append(row)
            script_map[obj.path_id] = row

        for obj in objects:
            type_name = obj.type.name
            name = safe_name(obj)
            if name:
                named_rows.append(
                    {
                        "family": family,
                        "container": relative,
                        "path_id": obj.path_id,
                        "type": type_name,
                        "name": name,
                    }
                )
            if type_name != "MonoBehaviour":
                continue
            tree = safe_typetree(obj) or {}
            script_id = pptr_path_id(tree.get("m_Script"))
            script = script_map.get(script_id or 0, {})
            behaviour_rows.append(
                {
                    "family": family,
                    "container": relative,
                    "path_id": obj.path_id,
                    "name": tree.get("m_Name") or name,
                    "enabled": tree.get("m_Enabled", ""),
                    "game_object_path_id": pptr_path_id(tree.get("m_GameObject")) or "",
                    "script_path_id": script_id or "",
                    "assembly": script.get("assembly", ""),
                    "namespace": script.get("namespace", ""),
                    "class": script.get("class", ""),
                    "typetree_available": bool(tree),
                }
            )

        for logical_path, pointer in sorted(
            environment.container.items(), key=lambda item: item[0]
        ):
            try:
                target = pointer.deref()
                target_type = target.type.name
                target_id = target.path_id
                target_name = safe_name(target)
            except Exception:
                target_type = ""
                target_id = getattr(pointer, "path_id", "")
                target_name = ""
            logical_rows.append(
                {
                    "family": family,
                    "container": relative,
                    "logical_path": logical_path,
                    "target_type": target_type,
                    "target_path_id": target_id,
                    "target_name": target_name,
                }
            )

        del objects
        del environment
        gc.collect()
        return True, ""
    except Exception as exc:
        gc.collect()
        message = f"{type(exc).__name__}: {exc}"
        print(f"[unity:error] {relative}: {message}", flush=True)
        return False, message


TOP_VALUE = re.compile(r"^(source|category|save_key|name|type): '(.*)'$")
COMPONENT_TYPE = re.compile(r"^    type: '(.*)'$")
SERIALIZED_TYPE = re.compile(r"^\s+\$type: '(.*)'$")
REFERENCE = re.compile(r"\$ref:([^:']+):([^']*)")
HIERARCHY_LAYER = re.compile(r"^    layer: (-?\d+)$")


def scan_prefab_dump(
    prefab_root: Path, allowed_sources: set[str] | None = None
) -> tuple[list[dict[str, Any]], Counter, Counter, Counter]:
    rows: list[dict[str, Any]] = []
    component_counts: Counter = Counter()
    serialized_type_counts: Counter = Counter()
    reference_type_counts: Counter = Counter()
    if not prefab_root.exists():
        return rows, component_counts, serialized_type_counts, reference_type_counts

    for path in sorted(prefab_root.rglob("*.yaml")):
        if path.name in {"manifest.yaml", "mods.yaml"}:
            continue
        values: dict[str, str] = {}
        components: Counter = Counter()
        layers: Counter = Counter()
        references: Counter = Counter()
        hierarchy_nodes = 0
        in_hierarchy = False
        with path.open("r", encoding="utf-8", errors="replace") as stream:
            for line in stream:
                text = line.rstrip("\r\n")
                match = TOP_VALUE.match(text)
                if match and match.group(1) not in values:
                    values[match.group(1)] = match.group(2)
                if text == "hierarchy:":
                    in_hierarchy = True
                elif text == "components:":
                    in_hierarchy = False
                elif in_hierarchy and text.startswith("  - path: "):
                    hierarchy_nodes += 1
                if match := COMPONENT_TYPE.match(text):
                    components[match.group(1)] += 1
                    component_counts[match.group(1)] += 1
                if match := SERIALIZED_TYPE.match(text):
                    serialized_type_counts[match.group(1)] += 1
                for match in REFERENCE.finditer(text):
                    references[match.group(1)] += 1
                    reference_type_counts[match.group(1)] += 1
                if match := HIERARCHY_LAYER.match(text):
                    layers[int(match.group(1))] += 1
        if allowed_sources and values.get("source", "") not in allowed_sources:
            continue
        rows.append(
            {
                "path": path.relative_to(prefab_root).as_posix(),
                **values,
                "hierarchy_nodes": hierarchy_nodes,
                "component_count": sum(components.values()),
                "component_types": len(components),
                "components": dict(sorted(components.items())),
                "layers": dict(sorted(layers.items())),
                "references": dict(sorted(references.items())),
            }
        )
    return rows, component_counts, serialized_type_counts, reference_type_counts


def addressables_summary(game_root: Path) -> dict[str, Any]:
    catalog = game_root / "Nebulous_Data" / "StreamingAssets" / "aa" / "catalog.json"
    if not catalog.exists():
        return {}
    with catalog.open("r", encoding="utf-8") as stream:
        data = json.load(stream)
    return {
        "path": catalog.relative_to(game_root).as_posix(),
        "locator_id": data.get("m_LocatorId"),
        "provider_ids": data.get("m_ProviderIds", []),
        "internal_ids": data.get("m_InternalIds", []),
        "internal_id_prefixes": data.get("m_InternalIdPrefixes", []),
        "resource_type_count": len(data.get("m_resourceTypes", [])),
        "encoded_key_bytes": len(base64.b64decode(data.get("m_KeyDataString", "") or "")),
        "encoded_bucket_bytes": len(base64.b64decode(data.get("m_BucketDataString", "") or "")),
        "encoded_entry_bytes": len(base64.b64decode(data.get("m_EntryDataString", "") or "")),
    }


def localization_summary(game_root: Path) -> list[dict[str, Any]]:
    root = game_root / "Nebulous_Data" / "StreamingAssets" / "Localization"
    rows: list[dict[str, Any]] = []
    if not root.exists():
        return rows
    for path in sorted(root.rglob("*.json")):
        try:
            with path.open("r", encoding="utf-8-sig") as stream:
                data = json.load(stream)
            count = len(data) if isinstance(data, (dict, list)) else 1
            kind = type(data).__name__
        except Exception as exc:
            count = 0
            kind = f"error:{type(exc).__name__}"
        rows.append(
            {
                "path": path.relative_to(game_root).as_posix(),
                "locale": path.parent.name,
                "file": path.name,
                "json_kind": kind,
                "entry_count": count,
            }
        )
    return rows


def write_csv(path: Path, rows: list[dict[str, Any]]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    if not rows:
        path.write_text("", encoding="utf-8")
        return
    columns: list[str] = []
    for row in rows:
        for key in row:
            if key not in columns:
                columns.append(key)
    with path.open("w", newline="", encoding="utf-8") as stream:
        writer = csv.DictWriter(stream, fieldnames=columns)
        writer.writeheader()
        writer.writerows(rows)


def markdown_summary(index: dict[str, Any]) -> str:
    file_records = index["files"]
    scanned = [row for row in file_records if row["unity_scanned"]]
    errors = [row for row in file_records if row["unity_error"]]
    family_counts = Counter(row["family"] for row in file_records)
    family_bytes = Counter()
    for row in file_records:
        family_bytes[row["family"]] += row["size"]
    object_counts = Counter()
    for row in index["object_type_counts"]:
        object_counts[row["type"]] += row["count"]
    prefab_counts = Counter(row.get("source", "") for row in index["prefabs"])
    category_counts = Counter(row.get("category", "") for row in index["prefabs"])

    lines = [
        "# Vanilla asset, bundle, resource, and prefab inventory",
        "",
        "Generated metadata only. Payload bytes such as textures, meshes, audio,",
        "video, and fonts are not exported.",
        "",
        f"- Game root: `{index['game_root']}`",
        f"- Generated UTC: `{index['generated_utc']}`",
        f"- Files hashed: {len(file_records):,}",
        f"- Unity containers scanned: {len(scanned):,}",
        f"- Unity scan errors: {len(errors):,}",
        f"- Unity objects indexed: {sum(object_counts.values()):,}",
        f"- Logical bundle entries indexed: {len(index['logical_assets']):,}",
        f"- Named Unity objects indexed: {len(index['named_assets']):,}",
        f"- MonoBehaviour instances indexed: {len(index['mono_behaviours']):,}",
        f"- Registered/referenced prefab YAML files indexed: {len(index['prefabs']):,}",
        "",
        "## File families",
        "",
        "| Family | Files | Bytes |",
        "|---|---:|---:|",
    ]
    for family in sorted(family_counts):
        lines.append(
            f"| {family} | {family_counts[family]:,} | {family_bytes[family]:,} |"
        )

    lines.extend(
        [
            "",
            "## Largest Unity object classes",
            "",
            "| Type | Objects |",
            "|---|---:|",
        ]
    )
    for type_name, count in object_counts.most_common(40):
        lines.append(f"| `{type_name}` | {count:,} |")

    lines.extend(
        [
            "",
            "## Prefab snapshot",
            "",
            "### Sources",
            "",
            "| Source | Prefabs |",
            "|---|---:|",
        ]
    )
    for source, count in sorted(prefab_counts.items()):
        lines.append(f"| {source or '(unknown)'} | {count:,} |")
    lines.extend(["", "### Categories", "", "| Category | Prefabs |", "|---|---:|"])
    for category, count in sorted(category_counts.items()):
        lines.append(f"| {category or '(unknown)'} | {count:,} |")

    lines.extend(
        [
            "",
            "## Addressables",
            "",
            f"- Internal IDs: {len(index['addressables'].get('internal_ids', [])):,}",
            f"- Resource types: {index['addressables'].get('resource_type_count', 0):,}",
            f"- Localization JSON files: {len(index['localization']):,}",
            f"- Localization entries: {sum(row['entry_count'] for row in index['localization']):,}",
            "",
            "## Search products",
            "",
            "- `index.json`: complete machine-readable metadata index.",
            "- `files.csv`: hashes and scan status for every included file.",
            "- `object-types.csv`: Unity object counts per container.",
            "- `logical-assets.csv`: logical bundle/addressable asset paths.",
            "- `named-assets.csv`: named Unity objects.",
            "- `mono-scripts.csv`: assembly/namespace/class identities.",
            "- `mono-behaviours.csv`: serialized script instances and GameObject IDs.",
            "- `prefabs.json`: prefab identity, hierarchy, component, layer, and reference summary.",
            "",
            "The registered-prefab YAML remains the stronger source for fully resolved",
            "native component fields and hierarchy paths. The raw Unity inventory adds",
            "unregistered assets, scenes, resources, addressables, and asset-path context.",
        ]
    )
    if errors:
        lines.extend(["", "## Scan errors", ""])
        for row in errors:
            lines.append(f"- `{row['relative_path']}`: {row['unity_error']}")
    return "\n".join(lines) + "\n"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--game-root", type=Path, required=True)
    parser.add_argument("--prefab-root", type=Path)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--summary", type=Path)
    parser.add_argument("--unitypy-path", type=Path)
    parser.add_argument("--prefab-source", action="append")
    parser.add_argument("--skip-unity", action="store_true")
    args = parser.parse_args()

    game_root = args.game_root.resolve()
    output = args.output.resolve()
    prefab_root = args.prefab_root.resolve() if args.prefab_root else Path()
    if args.unitypy_path:
        sys.path.insert(0, str(args.unitypy_path.resolve()))
    UnityPy = None
    if not args.skip_unity:
        import UnityPy as imported_unitypy

        UnityPy = imported_unitypy

    candidates, raw_only = discover_files(game_root)
    file_records: list[FileRecord] = []
    object_type_rows: list[dict[str, Any]] = []
    named_rows: list[dict[str, Any]] = []
    script_rows: list[dict[str, Any]] = []
    behaviour_rows: list[dict[str, Any]] = []
    logical_rows: list[dict[str, Any]] = []

    candidate_paths = {path for _, path in candidates}
    for family, path in candidates:
        record = FileRecord(
            family=family,
            relative_path=path.relative_to(game_root).as_posix(),
            size=path.stat().st_size,
            modified_utc=utc_stamp(path),
            sha256=sha256_file(path),
        )
        if UnityPy is not None:
            record.unity_scanned, record.unity_error = scan_unity_container(
                UnityPy,
                game_root,
                family,
                path,
                object_type_rows,
                named_rows,
                script_rows,
                behaviour_rows,
                logical_rows,
            )
        file_records.append(record)

    for path in raw_only:
        if path in candidate_paths:
            continue
        if "Localization" in path.parts:
            family = "localization"
        elif path.is_relative_to(game_root / "Assets" / "Resources"):
            family = "loose-resource"
        elif path.suffix.lower() in {".ress", ".resource"}:
            family = "streamed-resource-payload"
        else:
            family = "metadata-or-player-data"
        file_records.append(
            FileRecord(
                family=family,
                relative_path=path.relative_to(game_root).as_posix(),
                size=path.stat().st_size,
                modified_utc=utc_stamp(path),
                sha256=sha256_file(path),
            )
        )

    prefabs, component_counts, serialized_type_counts, reference_type_counts = (
        scan_prefab_dump(
            prefab_root,
            set(args.prefab_source) if args.prefab_source else None,
        )
    )
    index = {
        "schema_version": 1,
        "generated_utc": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
        "game_root": str(game_root),
        "prefab_root": str(prefab_root) if args.prefab_root else "",
        "unitypy_version": getattr(UnityPy, "__version__", "") if UnityPy else "",
        "files": [asdict(row) for row in file_records],
        "object_type_counts": object_type_rows,
        "logical_assets": logical_rows,
        "named_assets": named_rows,
        "mono_scripts": script_rows,
        "mono_behaviours": behaviour_rows,
        "prefabs": prefabs,
        "prefab_component_counts": dict(component_counts.most_common()),
        "prefab_serialized_type_counts": dict(serialized_type_counts.most_common()),
        "prefab_reference_type_counts": dict(reference_type_counts.most_common()),
        "addressables": addressables_summary(game_root),
        "localization": localization_summary(game_root),
    }

    output.mkdir(parents=True, exist_ok=True)
    with (output / "index.json").open("w", encoding="utf-8") as stream:
        json.dump(index, stream, indent=2, ensure_ascii=False)
        stream.write("\n")
    with (output / "prefabs.json").open("w", encoding="utf-8") as stream:
        json.dump(prefabs, stream, indent=2, ensure_ascii=False)
        stream.write("\n")
    write_csv(output / "files.csv", index["files"])
    write_csv(output / "object-types.csv", object_type_rows)
    write_csv(output / "logical-assets.csv", logical_rows)
    write_csv(output / "named-assets.csv", named_rows)
    write_csv(output / "mono-scripts.csv", script_rows)
    write_csv(output / "mono-behaviours.csv", behaviour_rows)
    summary_text = markdown_summary(index)
    (output / "summary.md").write_text(summary_text, encoding="utf-8")
    if args.summary:
        args.summary.parent.mkdir(parents=True, exist_ok=True)
        args.summary.write_text(summary_text, encoding="utf-8")
    print(f"[done] {output}", flush=True)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
