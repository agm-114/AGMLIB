# AGMLIB Agent Notes

## Line Endings

- Preserve the existing line-ending style when editing files.
- C# and VB files in this repo use Windows CRLF line endings, matching `.editorconfig` (`end_of_line = crlf`).
- Do not use editing commands or scripts that rewrite only part of a file with LF line endings. When a full-file rewrite is unavoidable, normalize the entire file to the repo's expected line endings before finishing.
- Before finishing any change to a `.cs` or `.vb` file, verify the touched file does not contain mixed line endings.

## Code Style

- Prefer concrete game/domain types over `object` where possible.
- Use `object` only at unavoidable reflection, Harmony, serialization, or unknown nested-type boundaries. Convert back to typed helpers immediately after that boundary.
- Keep Harmony patch entry points small and put reusable behavior in typed helper methods.