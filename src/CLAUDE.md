# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build Gedcom.sln

# Run all tests
dotnet test Gedcom.sln

# Run a specific test project
dotnet test Stravaig.Gedcom.UnitTests/
dotnet test Stravaig.FamilyTreeGenerator.Tests/

# Run tests matching a filter
dotnet test Stravaig.Gedcom.UnitTests/ --filter "ClassName=GedcomDatabaseTests"

# Run the generator
dotnet run --project Stravaig.FamilyTreeGenerator -- --source-folder /path/to/gedcom --source-file family.ged --destination-folder /output
```

## Architecture

This solution processes GEDCOM genealogy files and generates family tree documentation.

### Projects

- **Stravaig.Gedcom** — Core GEDCOM parsing library (.NET Standard 2.0). Parses `.ged` files into a `GedcomDatabase` containing typed records (`GedcomIndividualRecord`, `GedcomFamilyRecord`, `GedcomSourceRecord`, `GedcomObjectRecord`, etc.).
- **Stravaig.FamilyTree.Common** — Shared utilities: Humanizer extensions, footnote interfaces, date/name formatting abstractions.
- **Stravaig.FamilyTreeGenerator** — Console app (.NET 8) that renders Markdown and JSON output from a `GedcomDatabase`.
- **Stravaig.FamilyTree.Standardiser** — Utility to normalise GEDCOM files.
- **Stravaig.FamilyTree.Blazor** — WebAssembly Blazor UI for viewing family trees.

### FamilyTreeGenerator: Command Bus Pattern

The generator uses **Paramore.Brighter** as a command bus. `Program.cs` wires up all handlers in DI and fires `new Application()` to start processing.

Request types and their handlers (registered in `Program.cs`):

| Request | Handlers |
|---|---|
| `Application` | `ApplicationHandler` — orchestrates all sub-requests |
| `InitFileSystem` | `InitFileSystemForMarkdownHandler` |
| `RenderIndividual` | Markdown person page, ancestors JSON, descendants JSON, family tree JSON |
| `RenderPersonIndex` | Multiple indexes: by name, all known names, DOB, birth/death/residence location, occupation, marriage date/name, on-this-day JSON, person index JSON |
| `RenderSourceIndex` | Markdown source index |
| `RenderSource` | Markdown source page |
| `RenderObject` | Media object handler |

Each request may have **multiple handlers** — all run for each dispatch. To add a new output format, implement a new handler and register it in `Program.cs`.

### Key Services (injected via DI)

- `IFileNamer` / `FileNamer` — determines output file paths
- `IDateRenderer` / `DateRenderer` — formats GEDCOM dates
- `IStaticFootnoteOrganiser` / `MarkdownFootnoteOrganiser` — manages footnote references in Markdown output
- `IOccupationDescriptionService` / `OccupationDescriptionService` — resolves occupation descriptions
- `IIndividualNameRenderer`, `ITimelineRenderer`, `IRelationshipRenderer`, `IResidenceRenderer`, `IOccupationRenderer` — render specific aspects of a person record

### Testing

Tests use **NUnit** with **Shouldly** assertions. Test resources (sample `.ged` files) live in `_resources/` folders as embedded resources within test projects.
