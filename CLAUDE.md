# CLAUDE.md

## What this repo is

A reusable NuGet **library** (`Codehaks.Pagination`) — a Bootstrap pagination Razor Tag Helper
plus `IQueryable<T>` paging helpers — and a runnable **sample** Razor Pages app
(`Codehaks.Pagination.Sample`).

## Layout

```
Directory.Build.props        # shared MSBuild props (net10, nullable, warnings-as-errors, analyzers)
Directory.Packages.props     # Central Package Management — all versions live here
.editorconfig                # style + analyzer severities (enforced in build)
global.json                  # SDK pin
src/
  Codehaks.Pagination/        # the library (Razor class library, packable)
  Codehaks.Pagination.Sample/ # sample Razor Pages app (SQLite)
tests/
  Codehaks.Pagination.Tests/  # xUnit tests
```

## Conventions

- **Target:** net10.0 / C# 14. `Nullable=enable` and `TreatWarningsAsErrors=true` everywhere.
- **Packages:** Central Package Management — add a `PackageVersion` in `Directory.Packages.props`;
  `PackageReference` entries carry no `Version`.
- **Style:** file-scoped namespaces; `_camelCase` private fields. Run `dotnet format` before pushing;
  CI gates on `dotnet format --verify-no-changes --severity warn`.
- **Tests:** xUnit, `snake_case` test method names. The tag helper and paging helpers are
  pure/in-process, so no database fixture is needed.

## Documented deviations from the handbook

- **SQLite in the sample** (vs `R11` Postgres-only): the sample deliberately uses a throwaway
  SQLite file (created + seeded on startup, git-ignored) to stay self-contained. This is a
  sample-only decision; the library has no data dependency.
- **`NU1903` kept as a warning in the sample only**: a transitive `SQLitePCLRaw.lib.e_sqlite3`
  advisory has no patched release yet. Scoped via `WarningsNotAsErrors` in the sample csproj;
  revisit when a fix ships.

## Banned packages (do not add)

Newtonsoft.Json, AutoMapper, Mapster, MediatR, FluentValidation, Autofac, Moq, NodaTime.

## Build & test

```sh
dotnet build
dotnet test
dotnet run --project src/Codehaks.Pagination.Sample
```
