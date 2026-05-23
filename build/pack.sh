#!/usr/bin/env bash
set -euo pipefail

CONFIGURATION="${CONFIGURATION:-Release}"
VERSION_SUFFIX="${VERSION_SUFFIX:-}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
PACKAGE_PROJECT="./src/Logging/Logging.csproj"
TEST_PROJECT="./tests/Logging.UnitTests/Logging.UnitTests.csproj"

cd "$ROOT_DIR"

dotnet restore "$PACKAGE_PROJECT"
dotnet restore "$TEST_PROJECT"
dotnet test "$TEST_PROJECT" --configuration "$CONFIGURATION" --no-restore

pack_args=(
  pack
  "$PACKAGE_PROJECT"
  --configuration "$CONFIGURATION"
  --no-restore
  --output "./artifacts/packages"
  --verbosity "minimal"
  -p:EnablePackageValidation=true
)

if [ -n "$VERSION_SUFFIX" ]; then
  pack_args+=("/p:VersionSuffix=$VERSION_SUFFIX")
fi

dotnet "${pack_args[@]}"
