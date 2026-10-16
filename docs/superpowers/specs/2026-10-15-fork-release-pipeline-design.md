# Fork Release Pipeline — Design

Date: 2026-10-15
Repo: `vamsi-alluri/g-helper` (fork of `seerge/g-helper`)
Status: approved design, ready for implementation planning

## Purpose

Let this fork carry local features while still tracking upstream, so that:

1. Upstream changes land in the fork automatically.
2. Each sync produces an installable release in *this* fork.
3. G-Helper's existing in-app updater offers that release and applies it when
   the user clicks Update.

Success means: seerge pushes to `main`; within a day the fork has merged it,
built it and published a release; and the next G-Helper launch shows
`Download update: 0.288.0 → 0.288.N` and updates from this fork.

## Why this is needed first

`app/AutoUpdate/AutoUpdateControl.cs` hardcodes upstream in two places:

- `versionUrl = "https://github.com/seerge/g-helper/releases"` (line 18)
- `https://api.github.com/repos/seerge/g-helper/releases/latest` (line 77)

A fork build therefore checks upstream's releases, and `AutoUpdate()`
downloads upstream's zip and runs `Expand-Archive -Force` over the install
directory. Any local feature is destroyed by one Update click. This pipeline
must exist before local features are written.

## Constraints

- No SignPath credentials in this fork, so fork builds are unsigned.
- Upstream edits `app/GHelper.csproj` and `.github/workflows/release.yml`
  routinely; anything this fork changes in them is permanent merge surface.
- The in-app updater parses tags with `System.Version`, so release tags must
  be `v` plus 2-4 numeric components and nothing else.
- Current upstream `AssemblyVersion` is `0.288` (`app/GHelper.csproj:22`).
- **A tag pushed using the default `GITHUB_TOKEN` does not start another
  workflow run.** GitHub suppresses this deliberately to prevent recursive
  runs; `workflow_dispatch` and `repository_dispatch` are the only exceptions.
  The sync workflow therefore cannot rely on pushing a tag to trigger the
  release workflow, which is why the release workflow is reusable and is
  invoked directly (see component 3).

## Versioning

**The fork never edits `AssemblyVersion`.** That line is upstream's most
frequently touched (see commit `e897b278`, "Version bump"); editing it would
conflict on every upstream bump and turn "auto-release on clean merge" into
"conflict issue on every sync".

Instead the fork revision is derived at build time:

    FORK_REV = git rev-list --count upstream/main..HEAD

the number of commits the fork has that upstream does not. The published
version is `<upstream AssemblyVersion>.<FORK_REV>`, injected via
`-p:AssemblyVersion=` at publish time only.

Properties:

- Deterministic, no state file, nothing in the repo to conflict over.
- Monotonic: `FORK_REV` only grows, since history only grows. Each sync adds a
  merge commit, so each sync yields a fresh version.
- Ordered correctly against upstream: `new Version("0.288.5")` compares
  greater than `new Version("0.288")` (whose `Build` is `-1`) and less than
  `new Version("0.289")`. At upstream `0.289` the fork publishes `0.289.12`,
  still between `0.289` and `0.290`.
- `FORK_REV == 0` means no fork-only commits; no release is published.

## Components

### 1. Retarget the in-app updater

File: `app/AutoUpdate/AutoUpdateControl.cs` (modifies existing code)

Replace the two upstream literals with constants naming
`vamsi-alluri/g-helper`:

- the releases page URL used by `LoadReleases()`
- the `releases/latest` API URL used by `CheckForUpdatesAsync()`

This is the only edit to existing C# source. Keep it to those two lines so the
conflict, if upstream ever touches them, is trivial.

### 2. Upstream sync workflow

File: `.github/workflows/sync-upstream.yml` (new file, no conflict surface)

Triggers: daily `schedule` cron, plus `workflow_dispatch`.
Permissions: `contents: write`, `issues: write`.

Steps:

1. `actions/checkout@v7` with `fetch-depth: 0` (full history is required for
   `git rev-list --count`).
2. Add remote `upstream` pointing at `https://github.com/seerge/g-helper`, and
   fetch it.
3. If `upstream/main` has no commits absent from `main`, exit successfully.
4. `git merge --no-edit upstream/main`.
   - On conflict: `git merge --abort`, then open an issue labelled
     `upstream-conflict` (deduplicated — do not open a second issue while one
     is already open), and stop without pushing.
5. Build gate: `dotnet build app/GHelper.sln` on `windows-2022` with
   `actions/setup-dotnet@v5` and `dotnet-version: 10.0.x`, matching
   `build.yml`.
   - On failure: open an issue, stop without pushing. The merge is never
     pushed unless it compiles.
6. Push `main`.
7. Compute `FORK_REV` and read `AssemblyVersion` out of `app/GHelper.csproj`,
   giving the version `<AssemblyVersion>.<FORK_REV>`. Create and push the tag
   `v<version>` — for the release's provenance, not as a trigger.
8. A second job, `needs:` the first, calls `release-fork.yml` via `uses:` with
   the version as an input. This is what actually publishes, because the
   pushed tag cannot trigger it (see Constraints).

### 3. Fork release workflow

File: `.github/workflows/release-fork.yml` (new file, no conflict surface)

Triggers, in this order of importance:

- `workflow_call` with a required `version` input — how the sync workflow
  invokes it, and the path that matters. Keeping this reusable avoids needing
  a PAT purely to make a tag push observable.
- `workflow_dispatch` with the same `version` input, for publishing by hand.

Deliberately **not** `push: tags`, since that would never fire for
sync-created tags and would only create the illusion of a working trigger.

Permissions: `contents: write`.

Steps:

1. Checkout at the tag or ref for the requested version, then
   `actions/setup-dotnet@v5` with `10.0.x`.
2. Take the version from the `version` input.
3. Publish:

       dotnet publish app/GHelper.sln --configuration Release
         --runtime win-x64 -p:PublishSingleFile=true --no-self-contained
         -p:AssemblyVersion=<version>

4. `Compress-Archive` the published `GHelper.exe` into `GHelper.zip`.
   **Required**: `CheckForUpdatesAsync()` scans assets for a
   `browser_download_url` containing `.zip` and only falls back to
   `assets[0]`; `AutoUpdate()` then runs `Expand-Archive` on whatever it
   downloaded, so the primary asset must be a real zip.
5. `gh release create` with both `GHelper.exe` and `GHelper.zip`, as a
   **normal release — not a draft, not a prerelease**. The updater reads
   `releases/latest`, which excludes drafts and prereleases.

No SignPath submission, no build-provenance attestation.

### 4. Guard upstream's signed release workflow

File: `.github/workflows/release.yml` (modifies existing code)

Add `if: github.repository == 'seerge/g-helper'` to the `release` job so it
does not run in this fork, where it would fail on the missing
`SIGNPATH_API_TOKEN`, `SIGNPATH_ORG_ID`, `SIGNPATH_PROJECT` and
`SIGNPATH_POLICY` secrets, and where its "Check tag matches version" step
would reject the fork's three-component tag.

One added line, chosen over deleting the file so the fork keeps diffing
cleanly against upstream.

## Data flow

    seerge/g-helper main
            |
            |  daily cron
            v
    sync-upstream.yml --conflict-----> issue (upstream-conflict), stop
            |          --build fails-> issue, stop
            | clean + compiles
            v
      push main, push tag v0.288.N
            |
            | uses: release-fork.yml (workflow_call, same run)
            v
    release-fork.yml --> unsigned exe + zip --> GitHub release (latest)
            |
            v
    G-Helper AutoUpdateControl (<=12h later, or next launch)
            |
            v
      version label shows update --> user clicks --> zip from THIS fork

## Verification

End-to-end, done once at implementation time:

1. Confirm `sync-upstream.yml` runs clean via `workflow_dispatch` with no
   upstream changes pending (expect: early exit, no tag, no release).
2. Publish a real release at `v0.288.N` from the fork, and confirm the release
   job ran **in the same workflow run** as the sync job — not as a separate
   run, which would mean the trigger path is wrong.
3. Run a locally built G-Helper whose `AssemblyVersion` is lower, and confirm
   the label becomes `Download update: 0.288.0 → 0.288.N`.
4. Click Update and confirm the download URL in `Logger` output points at
   `vamsi-alluri/g-helper`, and that the app relaunches on the new version.
5. Force a conflict deliberately (edit a line upstream also changed) and
   confirm an `upstream-conflict` issue appears and nothing is pushed.

Testing notes:

- `AppConfig` keys `skip_updates` and `skip_version` suppress the update
  prompt; clear them when testing.
- The 12-hour throttle lives in a `static long lastUpdate` initialised to 0,
  so it is bypassed on each fresh process launch.

## Accepted trade-offs

- **Unsigned builds.** SmartScreen will warn on first launch after each
  update. Accepted: personal fork, no code-signing certificate.
- **Unreviewed upstream changes ship automatically.** The only gate is that
  the merge is conflict-free and compiles. No one has run the build before it
  reaches the user's machine. Accepted deliberately in exchange for a
  hands-off pipeline; the conflict and build-failure paths stop the bad cases
  that are detectable.
- **Fork revision counts merge commits.** `FORK_REV` increments on syncs that
  carry no local change of their own. This is intended: it guarantees a
  distinct version per sync.

## Out of scope

- The automatic dark/light theme switching feature. It is the reason this
  pipeline is being built first, but it gets its own spec.
- Code signing.
- Any change to `build.yml` or `codeql.yml`; both run harmlessly in the fork.
