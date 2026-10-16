#!/usr/bin/env bash
# Prints <upstream AssemblyVersion>.<fork commit count>, e.g. 0.288.4.
#
# The fork never stores a version of its own: app/GHelper.csproj keeps
# upstream's AssemblyVersion byte-for-byte, so upstream's frequent version
# bumps never conflict. The fork revision is derived from history instead.
set -euo pipefail

UPSTREAM_REF="${1:-upstream/main}"
CSPROJ="app/GHelper.csproj"

if [ ! -f "$CSPROJ" ]; then
  echo "fork-version: $CSPROJ not found; run from the repo root" >&2
  exit 1
fi

# A shallow clone makes rev-list undercount, which would emit a version that
# loses to the installed one and silently stop all future updates.
if [ "$(git rev-parse --is-shallow-repository)" = "true" ]; then
  echo "fork-version: shallow repository; checkout needs fetch-depth: 0" >&2
  exit 1
fi

if ! git rev-parse --verify --quiet "$UPSTREAM_REF" >/dev/null; then
  echo "fork-version: ref '$UPSTREAM_REF' not found; fetch upstream first" >&2
  exit 1
fi

ASSEMBLY_VERSION="$(sed -n 's|.*<AssemblyVersion>\(.*\)</AssemblyVersion>.*|\1|p' "$CSPROJ" | head -n1)"
if [ -z "$ASSEMBLY_VERSION" ]; then
  echo "fork-version: no <AssemblyVersion> in $CSPROJ" >&2
  exit 1
fi

FORK_REV="$(git rev-list --count "$UPSTREAM_REF..HEAD")"
if [ "$FORK_REV" -eq 0 ]; then
  echo "fork-version: no fork-only commits; nothing to release" >&2
  exit 1
fi

echo "${ASSEMBLY_VERSION}.${FORK_REV}"
