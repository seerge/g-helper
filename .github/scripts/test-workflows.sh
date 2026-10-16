#!/usr/bin/env bash
# Invariant checks for the fork release pipeline's workflows.
#
# These are static assertions, not a live run: each one pins a property whose
# absence has already cost a wedged pipeline or a silent failure. They exist
# because YAML that parses is not YAML that works — actionlint passes on every
# bug these catch.
set -u

SYNC=".github/workflows/sync-upstream.yml"
REL=".github/workflows/release-fork.yml"
PASS=0
FAIL=0

ok()   { echo "PASS: $1"; PASS=$((PASS + 1)); }
bad()  { echo "FAIL: $1 — $2"; FAIL=$((FAIL + 1)); }

has() { # name, file, regex
  if grep -qE "$3" "$2"; then ok "$1"; else bad "$1" "no match for /$3/ in $2"; fi
}

# 1. The release gate must verify the managed AssemblyVersion, which is what
#    AutoUpdateControl compares. Verifying only the Win32 FileVersion lets a
#    build ship whose assembly version never moved, producing an update that
#    is offered, downloads, and changes nothing — forever.
# Matched on a non-comment line only: GetAssemblyName currently appears in a
# comment explaining why it fails on the apphost, which would pass vacuously.
if grep -E 'GetAssemblyName' "$REL" | grep -qvE '^\s*#'; then
  ok "release verifies managed AssemblyVersion from GHelper.dll"
else
  bad "release verifies managed AssemblyVersion from GHelper.dll" "GetAssemblyName appears only in comments"
fi
has "release reads the managed dll, not just the apphost" "$REL" \
  'win-x64/GHelper\.dll'

# 2. The shallow guard must sit in front of the behind-count, not only inside
#    fork-version.sh. A shallow checkout makes the count read 0, which exits
#    green, skips everything, and never releases again.
has "sync asserts a full checkout before counting commits" "$SYNC" \
  'is-shallow-repository'

# 3. The version must be derived before main is pushed, so a refusal from
#    fork-version.sh leaves the remote untouched.
# Scoped to the tag step: fork-version.sh is also called by the recover step,
# which sits after the push, so a whole-file scan would read the wrong pair.
if awk '/id: tag/ {t=NR} t && !d && /fork-version\.sh/ {d=NR} t && !p && /git push origin HEAD:main/ {p=NR} END {exit !(d && p && d < p)}' "$SYNC"; then
  ok "version is derived before main is pushed"
else
  bad "version is derived before main is pushed" "push appears at or before the fork-version.sh call"
fi

# 4. Overlapping runs both merge the same base and race to push; the loser
#    dies on a rejected push and reports nothing.
has "sync serialises itself with a concurrency group" "$SYNC" \
  '^concurrency:'

# 5. Most of the ways this job can fail skip the conflict/build report step
#    entirely, leaving only a red run nobody is watching.
has "sync reports any other failure to the run summary" "$SYNC" \
  'if: failure\(\)'

# 6. Closing the tracking issue inside the sync job closes it while the
#    release job can still fail.
if grep -qE '^  close-issue:' "$SYNC" && grep -qE 'needs: \[sync, release\]' "$SYNC"; then
  ok "issue close waits on the release job"
else
  bad "issue close waits on the release job" "expected a close-issue job with needs: [sync, release]"
fi
if awk '/^  sync:/{s=1} /^  [a-z-]+:$/ && !/^  sync:/{s=0} s && /gh issue close/{found=1} END{exit found}' "$SYNC"; then
  ok "sync job no longer closes the issue itself"
else
  bad "sync job no longer closes the issue itself" "gh issue close still inside the sync job"
fi

# 7. The gate must build what the release actually publishes. A Debug,
#    RID-less `dotnet build` can pass while the Release single-file publish
#    fails, which wedges the pipeline after main and the tag are pushed.
has "build gate matches the release publish configuration" "$SYNC" \
  'dotnet publish app/GHelper\.sln --configuration Release --runtime win-x64 -p:PublishSingleFile=true'
if grep -qE 'run: dotnet build app/GHelper\.sln\s*$' "$SYNC"; then
  bad "build gate is not a bare Debug dotnet build" "gate still runs 'dotnet build app/GHelper.sln'"
else
  ok "build gate is not a bare Debug dotnet build"
fi

# 8. Recovery: if a previous run pushed a tag but the release never
#    published, every later run sees behind=0 and does nothing forever.
has "sync can recover a tag that has no release" "$SYNC" \
  'id: recover'

# 9. Inputs must not be interpolated directly into shell bodies of a
#    token-bearing step; pwsh expands $(...) inside double quotes.
if grep -qE 'gh release create "v\$\{\{ inputs\.version \}\}"' "$REL"; then
  bad "release passes version via env, not inline interpolation" "inputs.version interpolated into the gh release create body"
else
  ok "release passes version via env, not inline interpolation"
fi

echo "---"
echo "passed: $PASS  failed: $FAIL"
[ "$FAIL" -eq 0 ]
