# Logging

Logging is the repository for the Atya.Diagnostics.Logging NuGet package.

| | |
| --- | --- |
| Repository | [https://github.com/aasulyan/Diagnostics](https://github.com/aasulyan/Diagnostics) |
| NuGet | Atya.Diagnostics.Logging |
| License | MIT |

Provider-agnostic structured logging helpers for .NET applications.

## Layout

```text
.
|-- src/Logging/
|-- tests/Logging.UnitTests/
|-- samples/Logging.Samples.Console/
|-- benchmarks/Logging.Benchmarks/
|-- build/
\-- .github/
```

## Build and test

```bash
./build/build.ps1 -Configuration Release
./build/pack.ps1 -Configuration Release
```

Artifacts land in artifacts/packages/.

CI runs restore, vulnerability audit, format verification, Release build, Release tests with coverage, and package validation.

## Consumer guidance

Package-specific usage guidance lives in src/Logging/README.md.
