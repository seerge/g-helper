# Fork Release Pipeline Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make `vamsi-alluri/g-helper` merge upstream automatically, publish its own unsigned releases, and have G-Helper's in-app updater offer those releases instead of upstream's.

**Architecture:** Three new files and two one-line edits to existing ones. A POSIX shell script derives the fork version from git history (so nothing in the repo records a version that upstream could conflict with). A reusable release workflow publishes an unsigned single-file build plus a zip. A scheduled sync workflow merges upstream, gates on compilation, tags, and then calls the release workflow as a job in the same run.

**Tech Stack:** GitHub Actions (`windows-2022` for .NET work, `ubuntu-latest` for git/shell), .NET 10 (`net10.0-windows`), `gh` CLI, bash, `actionlint` for YAML validation, PowerShell `[version]` for ordering checks.

**Spec:** `docs/superpowers/specs/2026-10-15-fork-release-pipeline-design.md`

## Global Constraints

- Fork repo is `vamsi-alluri/g-helper`; upstream is `https://github.com/seerge/g-helper`.
- **Never edit `<AssemblyVersion>` in `app/GHelper.csproj` (line 22, currently `0.288`).** Version is injected at publish time only, via `-p:AssemblyVersion=`.
- Release tags are `v` followed by 2-4 numeric components and nothing else — `System.Version` must parse them.
- Releases must be normal releases: not draft, not prerelease. `releases/latest` excludes both.
- A tag pushed with the default `GITHUB_TOKEN` cannot trigger another workflow run. Never rely on `push: tags` for sync-created tags.
- The release's primary asset must be a real `.zip`; `AutoUpdate()` runs `Expand-Archive` on it.
- `dotnet-version: '10.0.x'` and `runs-on: windows-2022` for any job that builds, matching `.github/workflows/build.yml`.
- Fork builds are unsigned. No SignPath steps, no attestation steps, in any new workflow.
- This repo has **no test project**. Do not add one. Tests here are shell-level assertions against real git repos, `actionlint` on YAML, and PowerShell `[version]` comparisons — all runnable locally.

## Review Focus

These are failure modes the spec implies but that no task's happy path exercises. Each has a test pinned to the task that owns the code.

1. **A shallow checkout makes `git rev-list --count upstream/main..HEAD` silently wrong**, returning a small or zero count and producing a version that loses to the already-installed one, so the updater goes quiet forever. Covered in Task 3, Step 7.
2. **`FORK_REV` of `0`** (fork identical to upstream) would produce tag `v0.288.0`, which `System.Version` reads as equal to a `0.288.0.0` assembly — an update that is offered, downloads, and changes nothing. The script must refuse. Covered in Task 3, Step 5.
3. **A 2-component `AssemblyVersion` like `0.288` compared against `0.288.N`**: `System.Version` gives `0.288` a `Build` of `-1`, so ordering works — but only if nothing pads it to `0.288.0`. Covered in Task 3, Step 9.
4. **Two syncs on the same day, or a re-run after a partial failure**, would try to push a tag that already exists and abort the run after `main` was already pushed. Covered in Task 5, Step 9.
5. **A conflict issue opened on every single daily run** once upstream diverges, producing 30 identical issues a month. Dedup must be verified, not assumed. Covered in Task 5, Step 7.

---

### Task 1: Retarget the in-app updater at this fork

This is the urgent safety fix: today a fork build downloads upstream's zip and `Expand-Archive -Force` overwrites the install directory, destroying any local feature. Do this task first and commit it on its own.

**Files:**
- Modify: `app/AutoUpdate/AutoUpdateControl.cs:15` and `app/AutoUpdate/AutoUpdateControl.cs:77`
- Test: none (no test project; verified by grep assertion and a compile)

**Interfaces:**
- Consumes: nothing.
- Produces: nothing other tasks reference. The repo slug `vamsi-alluri/g-helper` appears in workflows too, but as literal text, not shared code.

- [ ] **Step 1: Confirm the current state is the broken one**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
grep -n "seerge" app/AutoUpdate/AutoUpdateControl.cs
```

Expected: exactly two lines, 15 and 77, both naming `seerge/g-helper`. If the line numbers differ, upstream has moved them — use the real numbers and carry on.

- [ ] **Step 2: Add the repo slug as a single constant**

Insert into the `AutoUpdateControl` class, immediately above the `versionUrl` field at line 15:

```csharp
        // Fork: updates come from this repository, not upstream. Changing this
        // to seerge/g-helper will make Update overwrite the fork's own build.
        const string RepoSlug = "vamsi-alluri/g-helper";
```

- [ ] **Step 3: Point the releases page at the fork**

Replace line 15:

```csharp
        public string versionUrl = "https://github.com/" + RepoSlug + "/releases";
```

- [ ] **Step 4: Point the API check at the fork**

Replace the URL on line 77:

```csharp
                    var json = await httpClient.GetStringAsync("https://api.github.com/repos/" + RepoSlug + "/releases/latest");
```

- [ ] **Step 5: Assert no upstream reference survives**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
if grep -n "seerge" app/AutoUpdate/AutoUpdateControl.cs; then
  echo "FAIL: upstream reference still present"; exit 1
else
  echo "PASS: no upstream reference"
fi
```

Expected: `PASS: no upstream reference`.

- [ ] **Step 6: Verify it compiles**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
dotnet build app/GHelper.sln
```

Expected: `Build succeeded`. A `CS0236` here means the `const` was placed inside a method rather than at class scope — move it to class scope.

- [ ] **Step 7: Commit**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git add app/AutoUpdate/AutoUpdateControl.cs
git commit -m "Point auto-update at this fork instead of upstream

An unmodified fork build checks seerge/g-helper for updates and
Expand-Archive -Force overwrites the install directory with upstream's
build, destroying any local feature on the first Update click."
```

---

### Task 2: Stop upstream's signed release workflow from running in the fork

**Files:**
- Modify: `.github/workflows/release.yml:14`
- Test: none (one-line guard; verified by reading the rendered condition)

**Interfaces:**
- Consumes: nothing.
- Produces: nothing.

- [ ] **Step 1: Confirm the job shape before editing**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
sed -n '12,16p' .github/workflows/release.yml
```

Expected: `jobs:`, then `  release:`, then `    runs-on: windows-2022`.

- [ ] **Step 2: Add the repository guard**

In `.github/workflows/release.yml`, the `release` job becomes:

```yaml
jobs:
  release:
    if: github.repository == 'seerge/g-helper'
    runs-on: windows-2022
```

Nothing else in the file changes. The guard is preferred over deleting the file so the fork keeps diffing cleanly against upstream.

- [ ] **Step 3: Verify the guard is present and the YAML still parses**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
grep -n "github.repository == 'seerge/g-helper'" .github/workflows/release.yml
pwsh -NoProfile -Command "python -c 'import sys,yaml; yaml.safe_load(open(sys.argv[1]))' .github/workflows/release.yml" 2>/dev/null \
  || python -c "import sys,yaml; yaml.safe_load(open('.github/workflows/release.yml')); print('YAML OK')"
```

Expected: the grep prints line 14, and `YAML OK`. If neither python nor pyyaml is available, skip the parse check — `actionlint` in Task 4 will cover all workflow files including this one.

- [ ] **Step 4: Commit**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git add .github/workflows/release.yml
git commit -m "Guard upstream release workflow to upstream repo

In this fork the job fails on missing SIGNPATH_* secrets, and its
tag-matches-version check rejects the fork's three-component tags."
```

---

### Task 3: Version derivation script

The one piece with real logic, so it lives in its own file and gets real tests. Keeping it out of workflow YAML is what makes it testable at all.

**Files:**
- Create: `.github/scripts/fork-version.sh`
- Test: `.github/scripts/test-fork-version.sh`

**Interfaces:**
- Consumes: nothing.
- Produces: `.github/scripts/fork-version.sh`, run as `bash .github/scripts/fork-version.sh <upstream-ref>`. Prints exactly one line, the version, e.g. `0.288.4`, to stdout and exits `0`. On any failure it prints a message to stderr and exits non-zero. Tasks 4 and 5 call it and read stdout.

- [ ] **Step 1: Write the failing test**

Create `.github/scripts/test-fork-version.sh`:

```bash
#!/usr/bin/env bash
# Tests for fork-version.sh. Builds throwaway git repos in a temp dir.
set -u

SCRIPT="$(cd "$(dirname "$0")" && pwd)/fork-version.sh"
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
    git add -A
    git -c user.email=t@t -c user.name=t commit -qm upstream
    git checkout -q -b main
    local i=0
    while [ "$i" -lt "$fork_commits" ]; do
      echo "$i" > "fork-$i.txt"
      git add -A
      git -c user.email=t@t -c user.name=t commit -qm "fork $i"
      i=$((i + 1))
    done
  )
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

# 3. Zero fork commits must fail, not emit 0.288.0
d="$(make_repo 0 0.288)"
out="$(cd "$d" && bash "$SCRIPT" upstream-main 2>/dev/null)"
rc=$?
check "zero fork commits exits non-zero" "nonzero" "$([ "$rc" -ne 0 ] && echo nonzero || echo zero)"
check "zero fork commits prints nothing" "" "$out"
rm -rf "$d"

# 4. A shallow repo must fail loudly rather than undercount
d="$(make_repo 3 0.288)"
shallow="$(mktemp -d)"
git clone -q --depth 1 "file://$d" "$shallow/repo" 2>/dev/null
(cd "$shallow/repo" && git fetch -q --depth 1 origin upstream-main:upstream-main 2>/dev/null)
rc=0
(cd "$shallow/repo" && bash "$SCRIPT" upstream-main >/dev/null 2>&1) || rc=$?
check "shallow repo exits non-zero" "nonzero" "$([ "$rc" -ne 0 ] && echo nonzero || echo zero)"
rm -rf "$d" "$shallow"

echo "---"
echo "passed: $PASS  failed: $FAIL"
[ "$FAIL" -eq 0 ]
```

- [ ] **Step 2: Run the test to verify it fails**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
bash .github/scripts/test-fork-version.sh
```

Expected: every case FAILs, because `fork-version.sh` does not exist yet. Output ends with a non-zero `failed:` count.

- [ ] **Step 3: Write the minimal implementation**

Create `.github/scripts/fork-version.sh`:

```bash
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
```

- [ ] **Step 4: Run the test to verify it passes**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
bash .github/scripts/test-fork-version.sh
```

Expected: `passed: 5  failed: 0`.

- [ ] **Step 5: Confirm the zero-revision guard against the real repo**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git rev-list --count HEAD..HEAD
bash .github/scripts/fork-version.sh HEAD; echo "exit=$?"
```

Expected: the count is `0`, and the script prints `no fork-only commits; nothing to release` to stderr with `exit=1`. This is Review Focus item 2: `v0.288.0` would be offered as an update, download, and change nothing.

- [ ] **Step 6: Confirm it produces a real version against real history**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git remote add upstream https://github.com/seerge/g-helper 2>/dev/null || true
git fetch -q upstream main
bash .github/scripts/fork-version.sh upstream/main
```

Expected: one line like `0.288.3` — `0.288` from the csproj, and a count matching the commits on this branch that upstream lacks (the spec commit, plus Tasks 1-3). Cross-check with `git rev-list --count upstream/main..HEAD`.

- [ ] **Step 7: Confirm the shallow-repository guard**

This is Review Focus item 1 — the failure that would quietly kill updates forever.

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
TMP="$(mktemp -d)"
git clone -q --depth 1 "file://$PWD" "$TMP/shallow"
cd "$TMP/shallow" && git rev-parse --is-shallow-repository
bash .github/scripts/fork-version.sh HEAD; echo "exit=$?"
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper && rm -rf "$TMP"
```

Expected: `true`, then `shallow repository; checkout needs fetch-depth: 0` on stderr with `exit=1`.

- [ ] **Step 8: Make the scripts executable**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git update-index --chmod=+x .github/scripts/fork-version.sh
git update-index --chmod=+x .github/scripts/test-fork-version.sh
```

These are added to the index in Step 10; run `git add` first if `update-index` complains the files are untracked.

- [ ] **Step 9: Pin the version-ordering assumption the whole scheme rests on**

Review Focus item 3. `System.Version` must sort `0.288.N` above upstream's `0.288` and below `0.289`. PowerShell's `[version]` is `System.Version`, so this tests the real type:

```bash
pwsh -NoProfile -Command '
  $ok = $true
  if (-not ([version]"0.288.1" -gt [version]"0.288"))   { $ok=$false; "FAIL: 0.288.1 should beat 0.288" }
  if (-not ([version]"0.288.9" -lt [version]"0.289"))   { $ok=$false; "FAIL: 0.288.9 should lose to 0.289" }
  if (-not ([version]"0.289.2" -gt [version]"0.288.9")) { $ok=$false; "FAIL: upstream bump should win" }
  if (([version]"0.288").Build -ne -1)                  { $ok=$false; "FAIL: 0.288 Build should be -1" }
  if ($ok) { "PASS: ordering holds" } else { exit 1 }'
```

Expected: `PASS: ordering holds`. The `Build -eq -1` check is the load-bearing one: it is why a 2-component upstream version loses to a 3-component fork version.

- [ ] **Step 10: Commit**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git add .github/scripts/fork-version.sh .github/scripts/test-fork-version.sh
git commit -m "Add fork version derivation script

Derives <upstream AssemblyVersion>.<fork commit count> from git history so
the fork never stores a version of its own, keeping upstream's frequent
AssemblyVersion bumps conflict-free. Refuses to run on a shallow clone or
with zero fork commits, both of which would emit a version that loses to
the installed build and silently end updates."
```

---

### Task 4: Reusable release workflow

**Files:**
- Create: `.github/workflows/release-fork.yml`

**Interfaces:**
- Consumes: `.github/scripts/fork-version.sh` is **not** used here — the version arrives as an input, so this workflow stays usable by hand.
- Produces: a workflow callable as `uses: ./.github/workflows/release-fork.yml` with inputs `version` (required, string, e.g. `0.288.4`) and `ref` (required, string — the tag or sha to build). Task 5 calls it with exactly these two inputs.

- [ ] **Step 1: Install actionlint, the only real test available for workflow YAML**

```bash
mkdir -p "$HOME/.local/bin"
cd "$HOME/.local/bin"
curl -sSfL https://raw.githubusercontent.com/rhysd/actionlint/main/scripts/download-actionlint.bash | bash
export PATH="$HOME/.local/bin:$PATH"
actionlint --version
```

Expected: a version number. If the download is blocked, fall back to `pwsh -NoProfile -Command "Get-Content <file> | Out-Null"` plus a YAML parse, and rely on the real `workflow_dispatch` run in Step 5 as the test.

- [ ] **Step 2: Write the failing test**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
export PATH="$HOME/.local/bin:$PATH"
actionlint .github/workflows/release-fork.yml
```

Expected: FAIL — the file does not exist. `actionlint: could not read .github/workflows/release-fork.yml`.

- [ ] **Step 3: Write the workflow**

Create `.github/workflows/release-fork.yml`:

```yaml
name: Release (fork)

# Deliberately NOT triggered by `push: tags`. A tag pushed with the default
# GITHUB_TOKEN cannot start another workflow run, so a tag trigger would look
# correct and never fire. sync-upstream.yml calls this via workflow_call.
on:
  workflow_call:
    inputs:
      version:
        description: 'Version to publish, e.g. 0.288.4'
        required: true
        type: string
      ref:
        description: 'Tag or sha to build'
        required: true
        type: string
  workflow_dispatch:
    inputs:
      version:
        description: 'Version to publish, e.g. 0.288.4'
        required: true
        type: string
      ref:
        description: 'Tag or sha to build'
        required: true
        type: string

permissions:
  contents: write

jobs:
  release:
    runs-on: windows-2022
    steps:
    - uses: actions/checkout@v7
      with:
        ref: ${{ inputs.ref }}

    - name: Setup dotnet
      uses: actions/setup-dotnet@v5
      with:
        dotnet-version: '10.0.x'

    - name: Publish
      run: |
        dotnet publish app/GHelper.sln --configuration Release --runtime win-x64 -p:PublishSingleFile=true --no-self-contained -p:AssemblyVersion=${{ inputs.version }}

    - name: Confirm the built exe carries the injected version
      shell: pwsh
      run: |
        $exe = "app/bin/x64/Release/net10.0-windows/win-x64/publish/GHelper.exe"
        if (-not (Test-Path $exe)) { throw "Publish produced no exe at $exe" }
        $actual = [Reflection.AssemblyName]::GetAssemblyName((Resolve-Path $exe)).Version
        $expected = [version]"${{ inputs.version }}"
        Write-Host "exe version: $actual / expected: $expected"
        if ($actual.Major -ne $expected.Major -or $actual.Minor -ne $expected.Minor -or $actual.Build -ne $expected.Build) {
          throw "exe is $actual but should be $expected; -p:AssemblyVersion did not take"
        }

    - name: Create ZIP
      shell: pwsh
      run: |
        Compress-Archive -Path app/bin/x64/Release/net10.0-windows/win-x64/publish/GHelper.exe -DestinationPath GHelper.zip -Force

    - name: Publish release
      env:
        GH_TOKEN: ${{ github.token }}
      shell: pwsh
      run: |
        # Not --draft and not --prerelease: releases/latest, which the in-app
        # updater reads, excludes both.
        gh release create "v${{ inputs.version }}" `
          app/bin/x64/Release/net10.0-windows/win-x64/publish/GHelper.exe `
          GHelper.zip `
          --title "G-Helper ${{ inputs.version }} (fork)" `
          --notes "Unsigned fork build of upstream + local changes. SmartScreen will warn on first launch." `
          --target "${{ inputs.ref }}" `
          --latest
```

- [ ] **Step 4: Run the test to verify it passes**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
export PATH="$HOME/.local/bin:$PATH"
actionlint .github/workflows/release-fork.yml && echo "ACTIONLINT CLEAN"
```

Expected: `ACTIONLINT CLEAN`, no output from actionlint itself.

- [ ] **Step 5: Assert the tag trigger is absent**

If a future edit adds `push: tags`, the pipeline appears to work while publishing nothing. Pin it:

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
if grep -qE '^\s*(push|tags)\s*:' .github/workflows/release-fork.yml; then
  echo "FAIL: release-fork.yml must not use a push/tags trigger"; exit 1
else
  echo "PASS: no tag trigger"
fi
```

Expected: `PASS: no tag trigger`.

- [ ] **Step 6: Commit**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git add .github/workflows/release-fork.yml
git commit -m "Add reusable fork release workflow

Publishes an unsigned single-file build plus the zip the in-app updater
needs, as a normal (non-draft, non-prerelease) release so releases/latest
returns it. Reachable via workflow_call and workflow_dispatch only: a tag
pushed with GITHUB_TOKEN cannot trigger a workflow run."
```

---

### Task 5: Upstream sync workflow

**Files:**
- Create: `.github/workflows/sync-upstream.yml`

**Interfaces:**
- Consumes: `.github/scripts/fork-version.sh` (Task 3), and `./.github/workflows/release-fork.yml` with inputs `version` and `ref` (Task 4).
- Produces: nothing other tasks consume. This is the top of the chain.

- [ ] **Step 1: Write the failing test**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
export PATH="$HOME/.local/bin:$PATH"
actionlint .github/workflows/sync-upstream.yml
```

Expected: FAIL — file does not exist.

- [ ] **Step 2: Write the workflow**

Create `.github/workflows/sync-upstream.yml`:

```yaml
name: Sync upstream

on:
  schedule:
    # 05:30 UTC daily. Cron in Actions is best-effort and can run late.
    - cron: '30 5 * * *'
  workflow_dispatch:

permissions:
  contents: write
  issues: write

jobs:
  sync:
    runs-on: windows-2022
    outputs:
      released: ${{ steps.tag.outputs.released }}
      version: ${{ steps.tag.outputs.version }}
      ref: ${{ steps.tag.outputs.ref }}
    steps:
    - uses: actions/checkout@v7
      with:
        # Required: fork-version.sh refuses to run on a shallow clone, because
        # an undercount would emit a version that loses to the installed one.
        fetch-depth: 0

    - name: Fetch upstream
      shell: bash
      run: |
        git remote add upstream https://github.com/seerge/g-helper || true
        git fetch upstream main

    - name: Check for new upstream commits
      id: check
      shell: bash
      run: |
        behind="$(git rev-list --count HEAD..upstream/main)"
        echo "behind=$behind" >> "$GITHUB_OUTPUT"
        echo "main is $behind commits behind upstream/main"

    - name: Merge upstream
      id: merge
      if: steps.check.outputs.behind != '0'
      shell: bash
      run: |
        git config user.name "github-actions[bot]"
        git config user.email "41898282+github-actions[bot]@users.noreply.github.com"
        if git merge --no-edit upstream/main; then
          echo "conflict=false" >> "$GITHUB_OUTPUT"
        else
          git merge --abort
          echo "conflict=true" >> "$GITHUB_OUTPUT"
        fi

    - name: Setup dotnet
      if: steps.merge.outputs.conflict == 'false'
      uses: actions/setup-dotnet@v5
      with:
        dotnet-version: '10.0.x'

    - name: Build gate
      id: build
      if: steps.merge.outputs.conflict == 'false'
      continue-on-error: true
      run: dotnet build app/GHelper.sln

    - name: Open issue on conflict or build failure
      if: steps.merge.outputs.conflict == 'true' || steps.build.outcome == 'failure'
      env:
        GH_TOKEN: ${{ github.token }}
      shell: bash
      run: |
        if [ "${{ steps.merge.outputs.conflict }}" = "true" ]; then
          title="Upstream sync blocked: merge conflict"
          body="\`git merge upstream/main\` conflicted and was aborted. Nothing was pushed. Merge locally, resolve, push, then re-run this workflow."
        else
          title="Upstream sync blocked: build failure"
          body="\`upstream/main\` merged cleanly but \`dotnet build app/GHelper.sln\` failed, so the merge was not pushed. See the run log."
        fi
        # Dedup: one open issue at a time, or a diverged upstream produces an
        # identical issue every single day.
        existing="$(gh issue list --label upstream-sync --state open --json number --jq '.[0].number')"
        if [ -n "$existing" ]; then
          gh issue comment "$existing" --body "Still blocked as of $(date -u +%Y-%m-%dT%H:%MZ). Run: ${{ github.server_url }}/${{ github.repository }}/actions/runs/${{ github.run_id }}"
        else
          gh label create upstream-sync --description "Automated upstream sync needs attention" --color B60205 2>/dev/null || true
          gh issue create --title "$title" --label upstream-sync --body "$body

Run: ${{ github.server_url }}/${{ github.repository }}/actions/runs/${{ github.run_id }}"
        fi
        exit 1

    - name: Push main and tag
      id: tag
      if: steps.merge.outputs.conflict == 'false' && steps.build.outcome == 'success'
      env:
        GH_TOKEN: ${{ github.token }}
      shell: bash
      run: |
        git push origin HEAD:main
        version="$(bash .github/scripts/fork-version.sh upstream/main)"
        echo "derived version: $version"
        # Re-runs and same-day syncs must not abort after main was pushed.
        if git ls-remote --exit-code --tags origin "refs/tags/v$version" >/dev/null 2>&1; then
          echo "tag v$version already exists; nothing new to release"
          echo "released=false" >> "$GITHUB_OUTPUT"
          exit 0
        fi
        git tag "v$version"
        git push origin "v$version"
        echo "released=true" >> "$GITHUB_OUTPUT"
        echo "version=$version" >> "$GITHUB_OUTPUT"
        echo "ref=v$version" >> "$GITHUB_OUTPUT"

    - name: Close the sync issue now that sync succeeded
      if: steps.build.outcome == 'success' && steps.merge.outputs.conflict == 'false'
      env:
        GH_TOKEN: ${{ github.token }}
      shell: bash
      run: |
        existing="$(gh issue list --label upstream-sync --state open --json number --jq '.[0].number')"
        if [ -n "$existing" ]; then
          gh issue close "$existing" --comment "Upstream sync succeeded in run ${{ github.run_id }}."
        fi

  release:
    needs: sync
    if: needs.sync.outputs.released == 'true'
    uses: ./.github/workflows/release-fork.yml
    with:
      version: ${{ needs.sync.outputs.version }}
      ref: ${{ needs.sync.outputs.ref }}
    permissions:
      contents: write
```

- [ ] **Step 3: Run the test to verify it passes**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
export PATH="$HOME/.local/bin:$PATH"
actionlint .github/workflows/sync-upstream.yml && echo "ACTIONLINT CLEAN"
```

Expected: `ACTIONLINT CLEAN`.

- [ ] **Step 4: Lint every workflow together**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
export PATH="$HOME/.local/bin:$PATH"
actionlint && echo "ALL WORKFLOWS CLEAN"
```

Expected: `ALL WORKFLOWS CLEAN`. This also covers the Task 2 edit to `release.yml`.

- [ ] **Step 5: Assert the release job is called, not triggered**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
grep -n "uses: ./.github/workflows/release-fork.yml" .github/workflows/sync-upstream.yml \
  && echo "PASS: release invoked via workflow_call"
```

Expected: the grep matches and prints `PASS`. If this line is ever replaced by a tag push alone, the chain breaks silently.

- [ ] **Step 6: Commit**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git add .github/workflows/sync-upstream.yml
git commit -m "Add scheduled upstream sync workflow

Merges upstream/main daily, gates on dotnet build, pushes and tags, then
calls release-fork.yml in the same run. Opens one deduplicated issue on
conflict or build failure and never pushes an unbuildable merge."
```

- [ ] **Step 7: Verify issue dedup against the live repo**

Review Focus item 5 — without this, a diverged upstream opens an identical issue every day.

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
gh label create upstream-sync --description "Automated upstream sync needs attention" --color B60205 2>/dev/null || echo "label exists"
gh issue create --title "Test: sync dedup" --label upstream-sync --body "Delete me."
gh issue list --label upstream-sync --state open --json number --jq '.[0].number'
```

Expected: the final command prints exactly one issue number — the same lookup the workflow uses to decide between commenting and creating. Then clean up:

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
gh issue close "$(gh issue list --label upstream-sync --state open --json number --jq '.[0].number')" --comment "Test issue, closing."
```

- [ ] **Step 8: Push the branch and open a PR to make the workflows reachable**

Workflows only run from the default branch for `schedule`, and `workflow_dispatch` needs the file present on a branch GitHub can see.

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git push -u origin feature/fork-release-pipeline
gh pr create --title "Fork release pipeline" --body "Implements docs/superpowers/specs/2026-10-15-fork-release-pipeline-design.md

Retargets auto-update at this fork, adds upstream sync and unsigned fork
releases, and guards upstream's signed release workflow.

🤖 Generated with [Claude Code](https://claude.com/claude-code)"
```

- [ ] **Step 9: Verify the duplicate-tag path**

Review Focus item 4. After the PR is merged to `main`, run the sync twice in a row:

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
gh workflow run sync-upstream.yml --ref main
sleep 90 && gh run list --workflow sync-upstream.yml --limit 2
gh workflow run sync-upstream.yml --ref main
sleep 90 && gh run list --workflow sync-upstream.yml --limit 2
```

Expected: the first run may release; the second must **succeed** with `tag vX already exists; nothing new to release` in the "Push main and tag" step and no `release` job, rather than failing on a duplicate tag push.

---

### Task 6: End-to-end verification

No new files. This is the task that proves the chain works, and it is the only one that exercises the actual updater.

**Files:**
- Modify: none permanently.

**Interfaces:**
- Consumes: everything from Tasks 1-5.
- Produces: a confirmed working pipeline.

- [ ] **Step 1: Confirm a no-op sync exits cleanly**

With `main` already level with upstream:

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
gh workflow run sync-upstream.yml --ref main
sleep 90
gh run view --workflow sync-upstream.yml --log | grep -E "commits behind|released="
```

Expected: `main is 0 commits behind upstream/main`, no merge, no tag, no release job, run concludes successfully.

- [ ] **Step 2: Publish a release by hand**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
VERSION="$(bash .github/scripts/fork-version.sh upstream/main)"
echo "publishing $VERSION"
git tag "v$VERSION" && git push origin "v$VERSION"
gh workflow run release-fork.yml --ref main -f version="$VERSION" -f ref="v$VERSION"
sleep 180
gh release view "v$VERSION" --json tagName,isDraft,isPrerelease,assets \
  --jq '{tag:.tagName, draft:.isDraft, prerelease:.isPrerelease, assets:[.assets[].name]}'
```

Expected: `draft:false`, `prerelease:false`, assets containing both `GHelper.exe` and `GHelper.zip`. A `true` on either flag means `releases/latest` will not return it and the updater will never see it.

- [ ] **Step 3: Confirm the API the updater actually reads**

```bash
curl -s https://api.github.com/repos/vamsi-alluri/g-helper/releases/latest \
  | python -c "import json,sys; d=json.load(sys.stdin); print('tag:', d['tag_name']); print('zip:', [a['browser_download_url'] for a in d['assets'] if '.zip' in a['browser_download_url']])"
```

Expected: the tag just published, and exactly one `.zip` URL under `vamsi-alluri/g-helper`. This is the precise shape `CheckForUpdatesAsync()` parses.

- [ ] **Step 4: Build a deliberately older local app**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
dotnet publish app/GHelper.sln --configuration Release --runtime win-x64 \
  -p:PublishSingleFile=true --no-self-contained -p:AssemblyVersion=0.288.1
```

Use a fork version above `0.288.1` in Step 2 so this build is genuinely behind.

- [ ] **Step 5: Clear the settings that suppress the prompt**

`skip_updates` and `skip_version` in `AppConfig` will silently hide the update. The 12-hour throttle is a `static long lastUpdate` initialised to `0`, so a fresh launch always checks.

```bash
cat "$APPDATA/GHelper/config.json" 2>/dev/null | python -m json.tool | grep -E "skip_updates|skip_version" \
  || echo "no skip_* keys set"
```

Expected: `no skip_* keys set`. If either is present, remove the key before testing.

- [ ] **Step 6: Confirm the update is offered and points at the fork**

Launch the published exe from Step 4, open the G-Helper window, and read the version label.

Expected: `Download update: 0.288.1 → <version from Step 2>`. Then check the log:

```bash
grep -iE "github.com|releases" "$APPDATA/GHelper/log.txt" | tail -20
```

Expected: the logged download URL contains `vamsi-alluri/g-helper` and **not** `seerge`. This is the whole point of Task 1 — if it says `seerge` here, stop, because clicking Update will overwrite the fork.

- [ ] **Step 7: Apply the update and confirm the version moved**

Click the update label and accept. G-Helper downloads the zip, waits for the process to exit, expands it in place, and relaunches.

Expected: the relaunched app's version label reads the new version with no update offered. SmartScreen may warn, because fork builds are unsigned — expected, not a failure.

- [ ] **Step 8: Force a real conflict and confirm nothing is pushed**

Spec verification item 5. Do this on a throwaway branch so `main` is never
endangered. Pick a line upstream actively changes — `AssemblyVersion` is the
reliable one:

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git checkout -b test/force-conflict main
sed -i 's|<AssemblyVersion>0.288</AssemblyVersion>|<AssemblyVersion>0.288-conflict-test</AssemblyVersion>|' app/GHelper.csproj
git commit -aqm "TEST ONLY: provoke a conflict on the AssemblyVersion line"
git push -u origin test/force-conflict
gh workflow run sync-upstream.yml --ref test/force-conflict
sleep 90
gh run view --workflow sync-upstream.yml --log | grep -iE "conflict=|merge" | head
```

Expected: `conflict=true`, an issue appears under the `upstream-sync` label,
the run ends failed, and **no tag or release is created**. Confirm nothing was
pushed to `main`:

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git fetch origin main
git log --oneline origin/main -1
```

Expected: unchanged from before the test. Note that this only produces a
conflict once upstream has itself touched that line since your branch point;
if the merge succeeds instead, upstream hasn't moved, so re-run this step
after the next upstream version bump.

Clean up:

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git checkout feature/fork-release-pipeline
git push origin --delete test/force-conflict
git branch -D test/force-conflict
gh issue close "$(gh issue list --label upstream-sync --state open --json number --jq '.[0].number')" --comment "Conflict path verified; closing test issue."
```

- [ ] **Step 9: Record the result**

```bash
cd /c/Users/vamsi/source/repos/vamsi-alluri/g-helper
git commit --allow-empty -m "Verify fork release pipeline end to end

Confirmed: no-op sync exits clean; manual release publishes a non-draft
release with both exe and zip; releases/latest returns it; an older local
build is offered the update, downloads from vamsi-alluri/g-helper, applies
it and relaunches on the new version; a forced conflict opens one issue and
pushes nothing."
```
