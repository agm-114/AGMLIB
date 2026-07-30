# AGMLIB CI Scripts

These scripts are the reusable boundary between GitHub Actions and NEBULOUS.
Keep workflow YAML declarative and put installation, staging, validation, launch,
timeout, and evidence-collection logic here so each step can be reproduced
locally with PowerShell 7.

- Treat Steam application `2353090` as the anonymous dedicated-server package.
- Treat Steam application `887570` and workshop item `2960504230` as AGMLIB's
  workshop identity.
- Overlay repository-built binaries only inside an isolated CI download. Never
  mutate a developer's installed game or subscribed workshop content.
- Every launched server must have a finite timeout and be stopped in `finally`.
- Long-running server checks must emit milestone notices and periodic
  heartbeats, and must fail on a bounded no-output stall before the global
  timeout.
- Stream named lifecycle events for mod download/load, dedicated-server
  readiness, match scene/map loading, fleet transfer/spawn, and gameplay start.
  Keep these operator-facing events separate from required pass/fail assertions.
- Bound time spent in one lifecycle phase independently of raw log activity;
  repetitive Unity output must not keep a frozen match alive.
- On fresh runners, warm SteamCMD app metadata in a separate invocation before
  installing app `2353090`; keep install retries bounded.
- Preserve the full Unity log, stdout, stderr, prefab manifest, and JSON summary
  on failure.
- Integration support must be opt-in. Do not enable test transport or fixtures
  in normal Release gameplay.
- The headless match test stages `AGMLIB.CI.TestSupport.dll` only into the
  isolated workshop download and activates it only with
  `AGMLIB_CI_AUTOSTART_MATCH=1`. A passing match test must reach the native
  `GO!` gameplay milestone with one ready stock-fleet bot on each team.
- Use `AGMLIB_PREFAB_DUMP_IMMEDIATE=1` only with the isolated Debug integration
  dump. Dedicated-server startup does not reliably raise the post-load event
  used by interactive clients.
- Keep `.github/workflows/workshop-compatibility.yml` manual-only. Do not add
  `push`, `pull_request`, or scheduled triggers. Its checked-in
  `WorkshopCompatibilityCatalog.json` is the reviewed set of major non-fleet
  mods and their additional declared Workshop dependencies.
- Run one Workshop compatibility target per isolated matrix job. Require every
  downloaded mod manifest to report a successful load, require the stock
  two-bot match to reach native `GO!`, and keep matrix fail-fast disabled so
  one incompatible mod does not hide the remaining results.
- Export the pristine Workshop file inventory, SHA-256 hashes, assemblies, and
  copied `ModInfo.xml` files before overlaying the repository AGMLIB build.
  Upload those structures together with the post-load prefab YAML, `mods.yaml`,
  lifecycle logs, match summary, and generated server config as seven-day
  artifacts. Do not upload the downloaded mod payloads themselves.
