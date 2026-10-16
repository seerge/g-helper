#!/usr/bin/env bash
# Tests for fork-version.sh. Builds throwaway git repos in a temp dir.
#
# Note on the failure cases: asserting only "exits non-zero" is not enough,
# because a missing script also exits non-zero. Each failure case asserts the
# specific diagnostic, so the test can tell "script refused correctly" from
# "script is not there".
set -u

SCRIPT="$(cd "$(dirname "$0")" && pwd)/fork-version.sh"
GITQ="git -c core.autocrlf=false -c core.safecrlf=false -c user.email=t@t -c user.name=t"
PASS=0
FAIL=0

check() {
  local name="$1" expected="$2" actual="$3"
  if [ "$expected" = "$actual" ]; then
    echo "PASS: $name"
    PASS=$((PASS + 1))
  else
    echo "FAIL: $name — expected '$expected', got '$actual'"
    FAIL=$((FAIL + 1))
  fi
}

check_contains() {
  local name="$1" needle="$2" haystack="$3"
  case "$haystack" in
    *"$needle"*)
      echo "PASS: $name"
      PASS=$((PASS + 1))
      ;;
    *)
      echo "FAIL: $name — expected stderr containing '$needle', got '$haystack'"
      FAIL=$((FAIL + 1))
      ;;
  esac
}

# Builds a repo with an upstream branch carrying <version> in the csproj,
# then $1 extra fork commits on main.
make_repo() {
  local fork_commits="$1" version="$2" dir
  dir="$(mktemp -d)"
  (
    cd "$dir" || exit 1
    git init -q -b upstream-main
    mkdir -p app
    printf '<Project>\n  <PropertyGroup>\n    <AssemblyVersion>%s</AssemblyVersion>\n  </PropertyGroup>\n</Project>\n' "$version" > app/GHelper.csproj
    $GITQ add -A
    $GITQ commit -qm upstream
    git checkout -q -b main
    local i=0
    while [ "$i" -lt "$fork_commits" ]; do
      echo "$i" > "fork-$i.txt"
      $GITQ add -A
      $GITQ commit -qm "fork $i"
      i=$((i + 1))
    done
  ) >/dev/null 2>&1
  echo "$dir"
}

# 1. Three fork commits over upstream 0.288 gives 0.288.3
d="$(make_repo 3 0.288)"
check "three fork commits" "0.288.3" "$(cd "$d" && bash "$SCRIPT" upstream-main 2>/dev/null)"
rm -rf "$d"

# 2. A different upstream version is picked up from the csproj
d="$(make_repo 2 0.289)"
check "reads AssemblyVersion" "0.289.2" "$(cd "$d" && bash "$SCRIPT" upstream-main 2>/dev/null)"
rm -rf "$d"

# 3. Zero fork commits must fail with the right reason, not emit 0.288.0
d="$(make_repo 0 0.288)"
out="$(cd "$d" && bash "$SCRIPT" upstream-main 2>/dev/null)"
err="$(cd "$d" && bash "$SCRIPT" upstream-main 2>&1 >/dev/null)"
rc=0
(cd "$d" && bash "$SCRIPT" upstream-main >/dev/null 2>&1) || rc=$?
check "zero fork commits exits non-zero" "nonzero" "$([ "$rc" -ne 0 ] && echo nonzero || echo zero)"
check "zero fork commits prints nothing on stdout" "" "$out"
check_contains "zero fork commits explains why" "no fork-only commits" "$err"
rm -rf "$d"

# 4. A shallow repo must fail loudly rather than undercount
d="$(make_repo 3 0.288)"
shallow="$(mktemp -d)"
git clone -q --depth 1 "file://$d" "$shallow/repo" >/dev/null 2>&1
err="$(cd "$shallow/repo" && bash "$SCRIPT" HEAD 2>&1 >/dev/null)"
rc=0
(cd "$shallow/repo" && bash "$SCRIPT" HEAD >/dev/null 2>&1) || rc=$?
check "shallow repo exits non-zero" "nonzero" "$([ "$rc" -ne 0 ] && echo nonzero || echo zero)"
check_contains "shallow repo explains why" "shallow repository" "$err"
rm -rf "$d" "$shallow"

# 5. A missing upstream ref must be reported, not treated as zero commits.
#    The needle names the ref, because a bare "not found" also matches the
#    "app/GHelper.csproj not found" diagnostic — it would pass whether the
#    script refused for the right reason or because of the wrong cwd.
d="$(make_repo 3 0.288)"
err="$(cd "$d" && bash "$SCRIPT" no-such-ref 2>&1 >/dev/null)"
rc=0
(cd "$d" && bash "$SCRIPT" no-such-ref >/dev/null 2>&1) || rc=$?
check "missing ref exits non-zero" "nonzero" "$([ "$rc" -ne 0 ] && echo nonzero || echo zero)"
check_contains "missing ref explains why" "ref 'no-such-ref' not found" "$err"
rm -rf "$d"

# 6. The ordering the whole version scheme rests on. Pinned here because the
#    comparison is between the tag string and the four-component version the
#    assembly actually reports: <AssemblyVersion>0.288</AssemblyVersion>
#    reports as 0.288.0.0, so a fork's 0.288.N must beat that and lose to a
#    later 0.289.0.0. A `Build` of -1 is a property of a parsed string only
#    and is NOT what the updater compares against.
if command -v pwsh >/dev/null 2>&1; then
  ord="$(pwsh -NoProfile -Command '
    $r = @()
    if (([version]"0.288.9").CompareTo([version]"0.288.0.0") -ne  1) { $r += "fork build must beat installed 0.288.0.0" }
    if (([version]"0.288.9").CompareTo([version]"0.289.0.0") -ne -1) { $r += "upstream bump must beat fork build" }
    if (([version]"0.288.0").CompareTo([version]"0.288.0.0") -ne -1) { $r += "0.288.0 must be lower than 0.288.0.0" }
    if (([version]"0.288.9").CompareTo([version]"0.288.9.0") -ne -1) { $r += "a fork build must not re-offer itself" }
    if ($r.Count -eq 0) { "ok" } else { $r -join "; " }' 2>/dev/null)"
  check "System.Version ordering holds" "ok" "$ord"
else
  echo "SKIP: System.Version ordering (pwsh not available)"
fi

echo "---"
echo "passed: $PASS  failed: $FAIL"
[ "$FAIL" -eq 0 ]
