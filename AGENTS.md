# AGENTS.md — ProphetsWay.Hasher

<!-- ═══════════════════════════════════════════════════════════════════════
     BEGIN SHARED BLOCK
     Generated from prophets-pipelines/conventions/AGENTS.shared.md
     DO NOT EDIT BY HAND — run /sync-agents-md to regenerate.
     ═══════════════════════════════════════════════════════════════════════ -->

## About This Codebase

`ProphetsWay.*` is a family of small, focused .NET libraries by G. Gordon Nasseri, published to
NuGet under the `ProphetsWay.` prefix and hosted at `github.com/ProphetManX`. Each library lives
in its own repository with its own version line, changelog, and pipeline.

### The Two Families

| Family | Repos | Purpose |
|---|---|---|
| **Utility** | Utilities, Logger, Hasher | Standalone helpers with no dependency on each other |
| **Data Access** | BaseDataAccess, EFTools | A layered DAL-decoupling paradigm; EFTools implements BaseDataAccess |

`ProphetsWay.Example` is a reference implementation, not a published package.
`prophets-pipelines` holds shared Azure DevOps YAML templates and this conventions file.

## Naming

**Display vs. codified.** The organization name is written two ways, and the distinction matters:

- **Display name** — `Prophet's Way`, with the apostrophe. Used in `<Company>`, prose, README text, and anything a human reads.
- **Codified** — `ProphetsWay`, no apostrophe or space. Used in namespaces, package IDs, assembly names, repo names, and the Azure DevOps org.

| Thing | Rule | Example |
|---|---|---|
| Repository | `ProphetsWay.<Library>` | `ProphetsWay.Logger` |
| Package ID | matches repository | `ProphetsWay.Logger` |
| Assembly name | matches repository | `ProphetsWay.Logger` |
| Library project folder | matches repository | `ProphetsWay.Logger/` |
| Test project | `<Library>.Tests` — **plural** | `ProphetsWay.Logger.Tests` |
| Example project | `<Library>.Example` | `ProphetsWay.Logger.Example` |
| `<Company>` | display name | `Prophet's Way` |
| `<Authors>` | `G. Gordon Nasseri` | |
| `<Product>` | library name without prefix | `Logger` |

### Namespaces

The rule is **family-dependent**. Do not "correct" one family to match the other.

- **Utility family** shares one root namespace regardless of assembly name:
  `ProphetsWay.Utilities`, with sub-namespaces for areas (`ProphetsWay.Utilities.LoggerDestinations`).
  A consumer adds one `using ProphetsWay.Utilities;` and reaches every utility library.
  This is why `ProphetsWay.Logger.dll` declares `namespace ProphetsWay.Utilities` — intentional, not a bug.
- **Data Access family** uses per-library namespaces: `ProphetsWay.BaseDataAccess`, `ProphetsWay.EFTools`
  (plus key-type sub-namespaces `.Guid`, `.Int`, `.Long`). These are an architectural paradigm, not
  utilities, and are kept separately addressable.
- **Test projects always use their own namespace**, `<AssemblyName>.Tests` — never the shared root.

## Target Frameworks

```xml
<!-- default for a published library -->
<TargetFrameworks>netstandard2.0;net10.0</TargetFrameworks>

<!-- only when a framework-conditional dependency or API requires it -->
<TargetFrameworks>netstandard2.0;net48;net10.0</TargetFrameworks>

<!-- test projects — netstandard2.0 is not a valid test target -->
<TargetFrameworks>net48;net10.0</TargetFrameworks>
```

.NET ships every November: **even-numbered = LTS (3 years), odd-numbered = STS (18 months)**.
.NET 10 is the current LTS (Nov 2025 → ~Nov 2028). **.NET 8 and .NET 9 both go end of life on
10 November 2026** — `net8.0`/`net9.0` are now debt, as are `netcoreapp*`, `net5.0`–`net7.0`,
and anything below `net48`.

1. **LTS only.** Never target an STS release in a published library — an 18-month window means
   re-cutting the list every year and stranding someone each time. Never target a preview.
2. **`netstandard2.0` is permanent.** It is an API contract, not a runtime, so it cannot expire.
   It is the reach floor: consumable by .NET Framework 4.6.1+ (painless from 4.7.2 up) and by every
   .NET Core/5+ runtime. It is also the last .NET Standard version Framework supports —
   `netstandard2.1` deliberately excluded it.
3. **`net48` is conditional, not default.** `netstandard2.0` already reaches .NET Framework 4.8, so
   an explicit `net48` target earns its place only when the repo has a framework-conditional
   *dependency* or needs an API `netstandard2.0` does not expose. `ProphetsWay.EFTools` qualifies —
   its EF6 branch is keyed on `net4*`. Most repos do not. Justify it per repo.
   (.NET Framework 4.8 is the final Framework version; it ships as a Windows component and inherits
   the OS lifecycle, so it has no standalone EOL date.)
4. **Carry exactly one modern TFM** unless something concrete requires two. Every extra target
   multiplies build time.
5. **Test projects name runtimes directly**, since `netstandard2.0` cannot be a test target. A
   `net48` test target is how .NET Framework behavior is *verified*, which is distinct from a
   library merely *supporting* it: `Activator.CreateInstance<T>()` wraps a throwing constructor on
   .NET Framework and does not on .NET Core, so `ProphetsWay.Example.Tests` must keep `net48` or its
   exception-passthrough regression guard stops guarding anything.
6. **Canonical dotted monikers** — `net10.0`, never `net100`. The undotted form parses, but it is
   non-standard and inconsistent across the repos.
7. **`LangVersion`:** `netstandard2.0` defaults to C# 7.3, and that constraint applies to all shared
   code in a multi-targeted project. This is why nullable reference types do not work in these
   libraries regardless of what a csproj claims.
8. **Dropping `net8.0`/`net9.0` is not a breaking change** while `netstandard2.0` remains — those
   consumers still install and still resolve an asset.
9. **Adding a TFM is a MINOR bump, never a patch.** A new target silently repoints existing
   consumers to a *different assembly* — a .NET 10 consumer that resolved the `netstandard2.0` asset
   starts binding the `net10.0` one, a different compilation with different BCL bindings and no
   netstandard shims. A patch must be safe to take without reading the notes.

## Packaging Metadata

Required in every **published** library's `.csproj`. If a repo is not published to NuGet, these
are optional — but they become mandatory the moment publishing is on the table.

```xml
<PackageId>ProphetsWay.Thing</PackageId>
<Product>Thing</Product>
<Authors>G. Gordon Nasseri</Authors>
<Company>Prophet's Way</Company>
<Description>...</Description>
<RepositoryType>git</RepositoryType>
<RepositoryUrl>https://github.com/ProphetManX/ProphetsWay.Thing</RepositoryUrl>
<PackageLicenseExpression>MIT</PackageLicenseExpression>
<PackageRequireLicenseAcceptance>true</PackageRequireLicenseAcceptance>
<PackageIcon>profile.png</PackageIcon>
<PackageReadmeFile>README.md</PackageReadmeFile>
<PackageTags>...</PackageTags>
```

Paired with the item group that actually packs those files — declaring `PackageIcon` or
`PackageReadmeFile` without the matching `<None Pack="true">` packs nothing and fails the build:

```xml
<ItemGroup>
  <None Include="..\CHANGELOG.md" Link="CHANGELOG.md" Pack="true" PackagePath="" />
  <None Include="..\README.md" Link="README.md" Pack="true" PackagePath="" />
  <Content Include="..\profile.png" Link="profile.png" Pack="true" PackagePath="" />
</ItemGroup>
```

**An empty self-closing element is not a value.** `<PackageId />` silently falls back to
`AssemblyName` and leaves the nuget.org listing without a license, readme, or source link.
Treat empty stubs as missing.

Versioning is owned by the pipeline. Leave `<Version />`, `<AssemblyVersion />`,
`<FileVersion />`, and `<InformationalVersion />` empty in the csproj — `app-variables.yml`
supplies them at build time.

## Testing

- **xUnit** — the test framework. Do not introduce NUnit or MSTest.
- **Shouldly** — assertion style. Prefer `result.ShouldBe(...)` over `Assert.Equal`.
  FluentAssertions 8.x requires a paid commercial license; do not add it to any project.
- **coverlet.collector** — coverage.
- **Moq** — only where a test genuinely needs a mock; most of these libraries do not.
- Test class names mirror the type under test: `HasherTests`, `FileDestinationTests`.
- Tests requiring a local database set `LocalTestsOnly: 'yes'` in `app-variables.yml` so CI skips them.

## Pipelines

Every repo consumes the shared templates in `prophets-pipelines` via two root files:

| File | Purpose |
|---|---|
| `app-variables.yml` | Per-repo values — `Major`/`Minor`/`Patch`, `TargetProject`, `Product`, `RepoName`, `PostTargetToNuGet`, `LocalTestsOnly` |
| `local-pipeline.yml` | Thin wrapper pulling `prophets-pipelines` stage templates |

`Major`/`Minor`/`Patch` are bumped **by hand** in `app-variables.yml` as work proceeds. The
pipeline appends build metadata to produce alpha/beta/release packages.

## Repo Layout

```
ProphetsWay.Thing/
├─ AGENTS.md                 ← this file
├─ README.md                 ← packed into the nupkg
├─ CHANGELOG.md              ← packed into the nupkg
├─ LICENSE                   ← MIT
├─ profile.png               ← NuGet icon
├─ app-variables.yml
├─ local-pipeline.yml
├─ ProphetsWay.Thing.sln
├─ ProphetsWay.Thing/        ← library
├─ ProphetsWay.Thing.Tests/  ← xUnit
└─ docs/                     ← agent-generated analysis
  ├─ repo-profile.md
  ├─ purpose-and-scope.md
  ├─ nuget-extraction-proposal.md
  └─ feature-requests.md    ← durable request and decision index
```

These artifacts are generated by agents and committed. `feature-requests.md` becomes applicable once
the first request is captured; an empty repo need not carry a placeholder.

**`docs/architecture.md` and per-project `docs/requirements.md` are `n/a` for a utility or reference
library** — its architecture lives in `AGENTS.md`, the README, and XML `<remarks>`. They apply to
multi-project **application** solutions only. Do not report either as missing from a library repo.

## Solution Layout

For multi-project application solutions, the rule is **base name = contracts, suffix = swappable
implementation**. Business logic has one implementation and needs no split; a DAL has many and does.

| Project | Contains |
|---|---|
| `<Solution>.Core` | Domain models, business logic interfaces, and their implementation |
| `<Solution>.DataAccess` | DAL contracts only — interfaces and entities |
| `<Solution>.DataAccess.<Provider>` | One DAL implementation: `.MSSQL`, `.PostgreSQL`, `.MySQL`, `.NoDB`, `.EF` |
| `<Solution>.Database` | The `.sqlproj` database project |
| `<Solution>.Api` | Service endpoints |
| `<Solution>.Web` | Web UI |
| `<Solution>.Win` | Desktop UI |
| `<Project>.Tests` | xUnit tests for that project — `<Solution>.Core.Tests` |

**The suffix list is open.** A new provider or UI technology gets a new suffix following the same
shape (`.DataAccess.Cosmos`, `.Mobile`, `.Cli`). Do not invent a new *pattern* — extend this one.

A contracts project must never reference an implementation project, and must never expose a type
from a specific technology (`DbContext`, `SqlConnection`, `HttpContext`) in its public surface.
That rule is what makes the DAL swappable, and it is the whole point of the paradigm.

### Database Projects

New `.sqlproj` projects use the **`Microsoft.Build.Sql`** SDK — SDK-style, cross-platform, and
buildable with `dotnet build`:

```xml
<Project Sdk="Microsoft.Build.Sql/<version>">
```

The legacy SSDT format (`ToolsVersion="4.0"`, the 2003 MSBuild namespace, `TargetFrameworkVersion`,
plus `.dbmdl`/`.jfm` sidecar files) requires Visual Studio on Windows and cannot be built by the
.NET CLI. Existing legacy projects are **debt to migrate** — the `.sql` files carry over unchanged;
the project header and sidecars are what change.

## Code Style

- Tabs for indentation in `.csproj` and `.cs`.
- Braces on their own line (Allman).
- Interfaces prefixed `I`. Abstract bases prefixed `Base` or `Root`.
- Public API surface gets XML doc comments; internals do not need them.
- No `.editorconfig` exists yet — style is convention, not enforced. Match surrounding code.

## Rules for Agents

- **Never edit `.cs`, `.csproj`, `.sln`, or `.yml`** unless the human explicitly asks in that turn.
  Propose changes as fenced snippets labeled `PROPOSED — not applied`.
- **Exception — the TDD agents.** `Interface Architect`, `Test Designer`, `Implementer`, and
  `Refactorer` exist to write code; invoking one *is* the explicit ask. Each is restricted to one
  kind of file, and those restrictions are load-bearing:

  | Agent | May write |
  |---|---|
  | `Interface Architect` | Interfaces and their supporting types — never tests, never implementations |
  | `API Designer` | HTTP contracts and `docs/api/` — never implementations |
  | `Test Designer` | `*Tests.cs` only |
  | `Implementer` | Implementation `.cs` only — **never** a test file |
  | `Refactorer` | Implementation `.cs` only, behavior-preserving — **never** a test file |
  | `Modernizer` | `.csproj` / `.sqlproj` build and packaging config — never versions, never namespaces |
  | `Pipeline Engineer` | `.yml` / `.yaml` only — never versions, secrets, project files, or Markdown |
  | `Changelog Author` | `CHANGELOG.md` only |
  | `Threat Modeler`, `Security Reviewer` | `docs/security/` only — read-only on source |

  If an agent edits a test to make it pass, the workflow has failed. Report it rather than
  accepting the green build.
- **Never bump a version** in `app-variables.yml`. That is a human decision.
- **Never invent an Azure DevOps `definitionId`.** Badge URLs must be copied from a file that
  already exists in the repo. If one is missing, ask.
- **Feature requests are shared-capture, single-owner triage.** The owner or any agent may append a
  `Proposed` entry to `docs/feature-requests.md`, but must read the index first and extend an existing
  entry instead of duplicating it. Only `Purpose Refiner` may change status. Never delete or renumber
  entries; rejected requests remain with their reasoning, and new numbers increase monotonically.
- **A namespace change is a binary-breaking change.** Never make one casually; it requires a major
  version bump and a CHANGELOG entry.
- **Affirming an inherited claim is not verifying it.** Before restating any existing claim in a
  README, `AGENTS.md`, or doc as still accurate, open the artifact it describes. A claim that has
  survived several passes has been *copied* several times, not *checked* several times. Say which
  file you opened.
- **When a hygiene fix has a teaching cost, genericize rather than delete** — replace the
  machine-specific value, keep the artifact — and record the declined option in the
  `docs/feature-requests.md` entry so it is not re-proposed later as unfinished work.
- Respect the family split above. `ProphetsWay.EFTools` living outside `ProphetsWay.Utilities`
  is correct, not drift.
- Deviations from these conventions are listed per-repo below. They are known, not overlooked —
  do not re-report them as discoveries.

<!-- ═══════════════════════════════════════════════════════════════════════
     END SHARED BLOCK
     ═══════════════════════════════════════════════════════════════════════ -->

---

## This Repo

**Family:** Utility · **Published:** **yes, as `ProphetsWay.Hasher`** — `build-test-deploy-release.yml`
packs alpha/beta/release nupkgs and pushes all three to an external NuGet feed, and cuts GitHub
releases alongside them. The version line is `2.1.0` (`MajorVersion`/`MinorVersion`/`PatchVersion` in
`azure-pipelines.yml`).

Generates hashes and checksums against files and streams. The smallest library in the workspace —
three source files, `Hasher.cs`, `HashFactory.cs`, `HashTypes.cs` — and the one with the tightest,
clearest purpose.

### Key Types

| Type | Role |
|---|---|
| `Hasher` | `public static`; extension methods over `Stream` and `FileInfo` plus a filename overload — `GenerateHash`, `GenerateHashes`, `GenerateHashesAsync`, and three `VerifyHash` overloads. One `VerifyHash` infers the algorithm from the checksum's character length |
| `HashFactory` | `public static`; `GetHasher()` maps one `HashTypes` value to a `HashAlgorithm`, `GetHashers()` expands a bitwise-combined value into a dictionary of them |
| `HashTypes` | **`[Flags]`** enum — `MD5`, `SHA1`, `SHA256`, `SHA384`, `SHA512`, plus `RIPEMD160` **only on .NET Framework targets** (see deviation 9) |

Tests live in `ProphetsWay.Hasher.Tests` — `HasherTests` and `HashFactoryTests` (all `[Theory]`), with
`TestFiles` an `internal static` holder of expected checksums for the three committed fixture files
`TestFileA.txt`, `TestFileB.bmp`, `TestFileC.bin`. Framework is xUnit 2.4.1 + FluentAssertions 5.10.3.

### Known Deviations

| # | Deviation | Severity |
|---|---|---|
| 1 | **Namespace is `ProphetsWay.Hasher`, not `ProphetsWay.Utilities`** | **Deliberate — do not "fix" it.** `CHANGELOG.md` v2.0.0 records the change *to* `ProphetsWay.Hasher` as intentional cleanup: "This was an artifact from when the code was broken out of a single project." It is a real divergence from the shared block's Utility-family namespace rule, but it is an owner decision already paid for with a major version bump, and reverting it would be binary-breaking a second time. Only three library files declare it, so the mechanical change stays trivial — the cost is the consumers, not the edit. **Open question for the owner:** which is authoritative, the family rule or the v2.0.0 decision? |
| 2 | **Packaging metadata is empty stubs in the csproj — but the pipeline fills most of it** | **Medium, not High.** `<PackageId />`, `<Product />`, `<PackageLicenseUrl />`, `<RepositoryUrl />`, `<RepositoryType />`, `<PackageTags />`, `<PackageProjectUrl />`, `<PackageIconUrl />`, `<PackageRequireLicenseAcceptance />` are all self-closing. However `build-test-deploy-release.yml` runs `Assembly-Info-NetCore@2` before packing and injects `PackageId` (`ProphetsWay.Hasher`), `Product` (`Hasher`), `Authors`, `Company`, `Description`, `PackageLicenseUrl`, `RepositoryUrl`, `RepositoryType`, and every version field. **What the listing genuinely lacks: tags, project URL, icon, and readme** — no `PackageTags`, no `PackageProjectUrl`, no `PackageIcon`, no `profile.png` in the repo, no `PackageReadmeFile`, and no `<ItemGroup>` at all to pack README or CHANGELOG. It also uses the **deprecated `PackageLicenseUrl`** rather than `PackageLicenseExpression` (NuGet emits `NU5125`). |
| 3 | **Legacy standalone pipeline** — `azure-pipelines.yml`, `build-and-test.yml`, `build-test-deploy-release.yml`, `version-variables.yml` | **High.** All four confirmed present. The only repo still on the pre-shared-template pipeline. Every other repo uses `app-variables.yml` + `local-pipeline.yml` against `prophets-pipelines`. Migrating means replacing four files with two. |
| 4 | `<Company>` is `ProphetsWay` **in two places** | Low. Should be the display form, `Prophet's Way`. Set both in `ProphetsWay.Hasher.csproj` and in the `Assembly-Info-NetCore@2` task in `build-test-deploy-release.yml` — fixing only the csproj changes nothing, because the pipeline value wins. |
| 5 | `<Product />` is an empty stub in the csproj | Low, and **cosmetic only** — the pipeline injects `Product: 'Hasher'` at build time, so the shipped package is correct. |
| 6 | TFM list is **14 targets**: `net451;net452;net46;net461;net471;net48;netstandard2.0;netstandard2.1;netcoreapp2.0;netcoreapp2.1;netcoreapp2.2;netcoreapp3.0;netcoreapp3.1;net50` | Medium. Everything except `netstandard2.0`/`2.1` is end-of-life. No `net8.0`/`net9.0`/`net10.0`. `net50` is written in the non-canonical undotted form. |
| 7 | Test dependencies are the oldest in the workspace | Low. Verified exactly: `Microsoft.NET.Test.Sdk` 16.6.1, `xunit` 2.4.1, `xunit.runner.visualstudio` 2.4.2, `FluentAssertions` 5.10.3, `coverlet.collector` 1.3.0 — versus 17.13.0 / 2.9.3 / 8.2.0 / 6.0.4 in EFTools. **5.10.3 predates the 8.x paid commercial license, so there is no licensing exposure**; it is drift from the house Shouldly convention, not a legal problem. |
| 8 | csproj mixes 2-space and tab indentation | Cosmetic. Both csproj files open `<PropertyGroup>` at two spaces and indent its contents with tabs. |
| 9 | **`RIPEMD160` is compiled in conditionally, and the `#if` names a TFM the library does not target** | Low, but a trap. `HashTypes.cs`, `HashFactory.cs`, and `Hasher.cs` all guard it with `#if NET451 \|\| NET452 \|\| NET46 \|\| NET461 \|\| NET471 \|\| NET472 \|\| NET48`. The library has no `net472` target, so that arm is dead. The practical effect is that **`HashTypes` has a different set of members depending on which asset a consumer resolves** — six on .NET Framework, five everywhere else. Any documentation of the enum must say so. |
| 10 | **`<Authors>` and `<Company>` are each declared twice — the second is an empty stub that wins** | Low here, unlike in `ProphetsWay.Utilities`. `<Authors>G. Gordon Nasseri</Authors>` and `<Company>ProphetsWay</Company>` appear first, then the "Assembly Override Properties" block re-declares `<Authors />` and `<Company />`; MSBuild takes the last assignment. Inert in practice — the pipeline overwrites both before packing — but a local `dotnet pack` produces a package with no author. |
| 11 | `CHANGELOG.md`'s newest entry is **v2.0.0** while the pipeline is building **2.1.0** | Low. `Changelog Author`'s territory; recorded so it is not re-reported. |

### Security Note

This library produces hashes. If any consumer uses it for **password storage**, that is a
vulnerability — general-purpose hashes (MD5, SHA-*) are unsuitable for credentials regardless of
salting. If documenting this library, state plainly what it is for (file integrity, checksums,
deduplication) and what it is **not** for (password hashing — use PBKDF2, bcrypt, scrypt, or
Argon2).

`HashTypes` declares six members, and **three of them are cryptographically broken**: `MD5`, `SHA1`,
and `RIPEMD160` (the last present only on the .NET Framework assets — see deviation 9). Flag all
three as unsuitable for security purposes, fit only for non-adversarial integrity checks. `SHA256`,
`SHA384`, and `SHA512` are sound. Note also that `VerifyHash(Stream, string)` **infers the algorithm
from the checksum's length**, so a 32-character input silently selects MD5 and a 40-character input
selects SHA1 — an attacker-supplied checksum picks its own algorithm.
