# Codehaks.Pagination — Modernization & Professionalization Plan

**Goal:** bring this repository up to a professional, maintainable, and reusable standard,
measured against the *Modern Web Dev (.NET)* handbook.

**Repo today:** a reusable NuGet library (`Codehaks.Pagination` — a Razor `<Pagination>` tag
helper + a `PaginationService<T>`) targeting `netstandard2.0`, plus a sample web app
(`PeopleApp` / `Demo.csproj`) on `netcoreapp2.2` using EF Core + SQLite.

## How to read this document

Tasks are grouped by priority (P0 → P5) and numbered sequentially. Each task lists the
**what**, the **why** (with the handbook rule/chapter it satisfies), and concrete **steps**.

Handbook references use the rule IDs from the Constitution (`R1`–`R21`) and chapter numbers.

### Framing note — library vs. web app

The handbook is written for **net10 ASP.NET Core web apps backed by Postgres**. This repo is
a **reusable NuGet library** plus a **sample app**. Therefore:

- **Substrate rules apply in full** — target framework, nullable, analyzers, Central Package
  Management, tests, CI, `src/`+`tests/` layout, naming, structured logging.
- **Web/data rules (R6–R13: Postgres, EF Core, Identity, JWT, minimal APIs) apply only to the
  sample** — and several conflict with the sample's job of being a tiny, self-contained demo.
  Those tensions are flagged in the relevant tasks and require an explicit decision.

### Open decisions (resolve before coding)

- **D1 — Library target:** multi-target (`net10.0;netstandard2.0`) for broad consumer reach,
  or net10-only per the handbook default? (affects Task 1)
- **D2 — Sample data stack:** keep SQLite for self-contained simplicity (documented deviation
  from `R11`), or move the sample to Postgres + `docker compose` per the handbook? (affects
  Tasks 22, 28)

---

## P0 — Substrate & build hygiene

> Highest leverage. These are the non-negotiable hard rules (`R1`) and the tooling chapter (12).
> Nothing here is optional in a handbook-conformant repo.

### 1. Retarget both projects to net10 / C# 14
- **What:** Library is `netstandard2.0`; sample is `netcoreapp2.2` (EOL since Dec 2019).
  Move to `net10.0`.
- **Why:** `R1` pins .NET 10 / C# 14 as the substrate. `netcoreapp2.2` is unsupported.
- **Steps:**
  1. Decide **D1** (multi-target vs net10-only).
  2. Set `<TargetFramework>net10.0</TargetFramework>` (or `<TargetFrameworks>net10.0;netstandard2.0</TargetFrameworks>` for the library if multi-targeting).
  3. Sample (`Demo.csproj`) must be `net10.0`.
  4. Rebuild and resolve API changes (hosting model, EF Core, Razor).

### 2. Enable nullable reference types and warnings-as-errors
- **What:** Add `Nullable=enable` and `TreatWarningsAsErrors=true` to every project.
- **Why:** `R1` — both are non-negotiable.
- **Steps:**
  1. Set both in `Directory.Build.props` (Task 3), not per-project.
  2. Fix resulting warnings. Expect work in `PaginationTagHelper` — `PageFirst`, `PageLast`,
     `PageTarget` are `string` but conditionally defaulted; make them `string?` (and default
     in `InitDefaults`) or `required`.

### 3. Add `Directory.Build.props` at the repo root
- **What:** One MSBuild file holding every project-wide property.
- **Why:** Chapter 12 — constant settings live in one place; a `.csproj` repeating them is noise.
- **Steps:** Create with:
  - `TargetFramework`, `LangVersion=latest`, `ImplicitUsings=enable`
  - `Nullable=enable`, `TreatWarningsAsErrors=true`
  - `EnableNETAnalyzers=true`, `AnalysisLevel=latest`, `AnalysisMode=All`
  - `EnforceCodeStyleInBuild=true`
  - `PackageReference` (PrivateAssets="all") for `Roslynator.Analyzers` and `Meziantou.Analyzer`

### 4. Adopt Central Package Management (`Directory.Packages.props`)
- **What:** Move every package version into one root file; `.csproj` references carry no version.
- **Why:** Chapter 12 — single source of truth for versions.
- **Steps:**
  1. Create `Directory.Packages.props` with `<ManagePackageVersionsCentrally>true</…>`.
  2. Add `PackageVersion` entries for current deps (Razor, EF Core, analyzers, test packages).
  3. Strip `Version=` from all `<PackageReference>` elements.

### 5. Add `.editorconfig` at the repo root
- **What:** The canonical style + analyzer-severity file.
- **Why:** Chapter 12 — style enforced by build, not by reviewers.
- **Steps:** Use the handbook's canonical block: file-scoped namespaces (`warning`),
  `_camelCase` private fields, 4-space C# / 2-space csproj-json, analyzer severities (CA1062,
  CA1307, CA1309, CA1848, CA2007=none, MA0048, etc.).

### 6. Add `global.json` pinning the SDK
- **What:** Pin the .NET SDK version.
- **Why:** Chapter 13 — prevents silent SDK drift between dev machines and CI.
- **Steps:** `{ "sdk": { "version": "10.0.100" } }` (default `rollForward: latestPatch`).

---

## P1 — Project structure & packaging

> Chapters 13 and 21.

### 7. Adopt the `src/` + `tests/` solution layout
- **What:** Production projects under `src/`, test projects under `tests/`.
- **Why:** Chapter 13/21 — the one structural rule with *no* exception.
- **Steps:**
  1. Move `Codehaks.Pagination/` → `src/Codehaks.Pagination/`.
  2. Move the sample → `src/<sample-name>/` (see Task 8).
  3. Create `tests/` (see Task 17).

### 8. Unify the sample project's name
- **What:** Folder `PeopleApp`, project `Demo.csproj`, namespace `Demo.Pages`,
  `RootNamespace` `PeopleApp` — three different names.
- **Why:** `R20` (token-match naming) + MA0048 (file-name matches type). Folder = project = namespace.
- **Steps:** Pick one name (e.g. `Codehaks.Pagination.Sample`); rename folder, `.csproj`,
  namespaces, and root namespace to match.

### 9. Make the library a proper, publishable NuGet package
- **What:** Add full package metadata + XML docs to the library `.csproj`.
- **Why:** Core to "usable by other developers." Makes the package discoverable and documented.
- **Steps:** Add `PackageId`, `Version`, `Authors`, `Description`, `PackageLicenseExpression`,
  `RepositoryUrl`, `PackageReadmeFile`, `PackageTags`, `GenerateDocumentationFile=true`.
  Consider a versioning tool (e.g. MinVer / Nerdbank.GitVersioning) — **but** verify against the
  Library Catalog (`R4`) before adding any package.

### 10. Regenerate the solution file
- **What:** Rebuild `Pagination.sln` after the moves/renames.
- **Why:** Chapter 13 — the `.sln` is maintained via `dotnet sln`, never hand-edited.
- **Steps:** `dotnet sln add` each project at its new path.

---

## P2 — Library code: correctness + idioms

> Folds in the bugs found in review, plus C# idioms (chapter 10).

### 11. Fix the three confirmed bugs
- **What:**
  1. **Malformed closing tag** — `PaginationTagHelper.cs:51` appends `" </ul"` (missing `>`).
  2. **Missing attribute space** — lines 63/76/89 emit `class='page-link'href='…'` (no space).
  3. **Off-by-one page count** — `Index.cshtml.cs:39` `Count() / PageSize` integer-divides;
     a partial last page becomes unreachable. Use ceiling: `(count + PageSize - 1) / PageSize`.
- **Why:** Correctness. Lock these in with tests (Task 17) before refactoring.

### 12. Build HTML with `TagBuilder`/`IHtmlContent`, not interpolated `StringBuilder`
- **What:** Replace hand-built string interpolation with proper builders.
- **Why:** Raw interpolation of `PageTarget`/`PageFirst`/`PageLast` is fragile and unencoded;
  it caused bugs #1 and #2. Builders eliminate that class structurally.

### 13. Migrate to file-scoped namespaces
- **What:** Both `.cs` files use the old braced namespace form.
- **Why:** Chapter 10 — file-scoped is mandatory; the analyzer flags braced as `warning` →
  build error under `TreatWarningsAsErrors`.

### 14. Resolve `PaginationService<T>` — finish or delete
- **What:** `PaginationService<T>` computes skip/take but never sets `TotalPages`/`PageNumber`;
  it's dead and half-finished, and `Service` is a discouraged generic suffix.
- **Why:** `R20` (avoid uninformative `Service`/`Manager`/`Helper` suffixes); dead code is debt.
- **Steps:** Either delete it, or replace with the handbook's idiom (chapter 25):
  an `IQueryable<T>.Page(page, size)` extension method returning a `PagedResult<T>` record.

### 15. Add input guards
- **What:** Guard `pageSize <= 0` and `pageNumber < 1` instead of producing bad skip values.
- **Why:** Chapter 10 exception philosophy — throw at discovery rather than silently mis-paging.

### 16. Use records for data carriers
- **What:** Introduce `PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)`.
- **Why:** Chapter 10 — records are the idiomatic shape for data carriers / value equality.

---

## P3 — Tests (currently zero)

> `R18`, chapter 28.

### 17. Add a unit test project for the library
- **What:** `tests/Codehaks.Pagination.Tests` (xUnit), namespaces mirroring production.
- **Why:** `R18` (xUnit) + chapter 28 — the tag helper's range math (under/between/upper-range
  branches, off-by-one) is exactly the branchy logic that must be tested.
- **Steps:**
  1. `dotnet new xunit`, add to solution under `tests/`.
  2. Cover the page-count / page-number matrix with `[Theory]` / `[InlineData]`.
  3. Add regression tests for the three Task 11 bugs.
  4. Test method names in `snake_case` describing behavior (chapter 28).

### 18. Note the Testcontainers/SQLite tension (no action for the library)
- **What:** `R19` mandates real Postgres via Testcontainers and bans SQLite/in-memory *for tests*.
- **Why:** The tag helper has no DB dependency → its tests are pure in-process unit tests with
  no fixture (chapter 28 deviation clause). No Testcontainers needed here.
- **Steps:** None for the library. The sample's SQLite usage is handled in Task 22.

---

## P4 — The sample app

> Where the web rules mostly apply — with explicit tensions.

### 19. Remove MVC; use Razor Pages only
- **What:** `Startup.cs` calls `AddMvc()` + `UseMvcWithDefaultRoute()`; the app already uses
  Razor Pages.
- **Why:** `R6`/`R7` — server-rendered HTML uses Razor Pages, no MVC controllers.
- **Steps:** Replace with `AddRazorPages()` + `MapRazorPages()`; drop the MVC wiring.

### 20. Modernize hosting to net10 minimal hosting
- **What:** `Startup.cs` uses the obsolete `IHostingEnvironment` + Startup pattern.
- **Why:** Chapter 22 — canonical `Program.cs` with top-level statements.
- **Steps:** Collapse `Startup` into `Program.cs`; follow the chapter-22 builder→pipeline order.

### 21. Propagate `CancellationToken` on async paths
- **What:** Page handlers / EF calls take and pass a `CancellationToken`.
- **Why:** Chapter 10 async discipline — `ToListAsync(ct)`, token last in the signature.

### 22. Resolve the data-stack tension (Decision D2)
- **What:** Sample uses SQLite and commits `People.sqlite`. `R11` allows only Postgres+Npgsql
  and the handbook bans committing the DB.
- **Why:** Forcing Postgres on someone who just wants to see pagination hurts the "usable"
  goal; but SQLite violates `R11`.
- **Steps (choose one):**
  - **(a) Keep SQLite** for a self-contained demo → document the deviation in `CLAUDE.md`
    and `.gitignore` the `.sqlite` file (seed it at startup instead). *Recommended for a sample.*
  - **(b) Move to Postgres** + `compose.yml` (chapter 13), removing the committed DB.

### 23. Trim the `User` model
- **What:** `User` has ~45 properties; the sample uses 3 (Givenname, Maidenname, Age).
- **Why:** A sample should model only what it demonstrates.
- **Steps:** Reduce to the fields the demo renders (plus a key).

---

## P5 — CI/CD, docs, repo hygiene

### 24. Add a CI workflow
- **What:** `.github/workflows/ci.yml`.
- **Why:** Chapter 13 — restore → build → test → format, no skips.
- **Steps:** Steps in order: `dotnet restore` → `dotnet build -c Release --no-restore`
  (warnings-as-errors) → `dotnet test -c Release --no-build --collect:"XPlat Code Coverage"`
  → `dotnet format --verify-no-changes --severity warn`. Add `dotnet pack` (and publish-on-tag)
  for the library.

### 25. Enforce `dotnet format` in CI
- **What:** The format-verify step is a hard gate.
- **Why:** Chapter 12 — `--severity warn` matches the `.editorconfig` policy; non-optional.

### 26. Add `CLAUDE.md` at the repo root
- **What:** Project memory file (`R21`).
- **Why:** Lets any agent/dev work without prior context.
- **Steps:** State that it's a library + sample, the structure conventions, the banned packages
  (`R5`: Newtonsoft.Json, AutoMapper, Mapster, MediatR, FluentValidation, Autofac, Moq, NodaTime),
  and any documented deviations (e.g. the D2 SQLite decision).

### 27. Rewrite `README.md` for consumers
- **What:** Consumer-facing docs.
- **Why:** Single biggest "usable by others" win.
- **Steps:** Install command, `<Pagination>` attribute table (`page-count`, `page-target`,
  `page-number`, `page-range`, `page-first`, `page-last`), a minimal usage snippet, and the
  existing `pagination.JPG` screenshot.

### 28. Docker (conditional on D2)
- **What:** Dockerfile/compose.
- **Why:** Chapter 13 — only deployable web services ship containers. A NuGet library doesn't.
- **Steps:** Add `compose.yml` for the sample **only if** D2 chooses Postgres (Task 22b);
  otherwise N/A.

---

## Suggested order of execution

1. **P0** (substrate files) — Tasks 1–6
2. **Task 11** — fix the shipped bugs
3. **P3** — Tasks 17–18 (lock the fixes in with tests)
4. **P1** — Tasks 7–10 (structure + packaging)
5. **P5** — Tasks 24–27 (CI + README)
6. **P4** — Tasks 19–23 (sample modernization)
7. **P2 remainder** — Tasks 12–16 (idiomatic refactor)

## Task checklist

- [ ] 1. Retarget to net10 / C# 14
- [ ] 2. Enable nullable + warnings-as-errors
- [ ] 3. `Directory.Build.props`
- [ ] 4. Central Package Management
- [ ] 5. `.editorconfig`
- [ ] 6. `global.json`
- [ ] 7. `src/` + `tests/` layout
- [ ] 8. Unify sample project name
- [ ] 9. NuGet package metadata + XML docs
- [ ] 10. Regenerate solution
- [ ] 11. Fix the three confirmed bugs
- [ ] 12. `TagBuilder`/`IHtmlContent` for HTML
- [ ] 13. File-scoped namespaces
- [ ] 14. Finish or delete `PaginationService<T>`
- [ ] 15. Add input guards
- [ ] 16. `PagedResult<T>` record
- [ ] 17. Library unit test project
- [ ] 18. (Note) Testcontainers/SQLite tension — no action for library
- [ ] 19. Remove MVC; Razor Pages only
- [ ] 20. Modernize hosting (`Program.cs`)
- [ ] 21. Propagate `CancellationToken`
- [ ] 22. Resolve data-stack tension (D2)
- [ ] 23. Trim the `User` model
- [ ] 24. CI workflow
- [ ] 25. `dotnet format` gate
- [ ] 26. `CLAUDE.md`
- [ ] 27. Rewrite `README.md`
- [ ] 28. Docker (conditional on D2)
