# Publishing Guide

This document explains how to publish the Laneful C# SDK to NuGet using .NET CLI.

## Prerequisites

- .NET 9.0 SDK installed
- NuGet API key (get from [NuGet.org](https://www.nuget.org/account/apikeys))

## Quick Start

### 1. Setup Environment

Copy the environment template and configure your NuGet API key:

```bash
cp env.example .env
# Edit .env and add your NUGET_API_KEY
```

### 2. Build and Publish

Use the provided build script:

```bash
# Make the script executable (if not already)
chmod +x build.sh

# Publish to NuGet
./build.sh publish
```

Or use .NET CLI directly:

```bash
# Build the project
dotnet build --configuration Release

# Create NuGet package
dotnet pack --configuration Release --output ./packages

# Publish to NuGet
dotnet nuget push ./packages/*.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

## Available Commands

### Build Script Commands

The `build.sh` script provides several convenient commands:

```bash
./build.sh build      # Build locally
./build.sh test       # Run tests
./build.sh package    # Create NuGet package locally
./build.sh publish    # Build and publish to NuGet
./build.sh clean      # Clean build artifacts
./build.sh help       # Show help
```

## Manual Publishing Steps

### 1. Build the Project

```bash
dotnet build --configuration Release
```

This will:
- Restore dependencies
- Build the project in Release configuration
- Generate optimized binaries

### 2. Create NuGet Package

```bash
dotnet pack --configuration Release --output ./packages
```

This will:
- Create a NuGet package (.nupkg)
- Create a symbols package (.snupkg)
- Output packages to `./packages/` directory

### 3. Publish to NuGet

```bash
dotnet nuget push ./packages/*.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate
```

This will:
- Push the package to NuGet.org
- Skip duplicate packages if they already exist

## Environment Configuration

### Required Environment Variables

- `NUGET_API_KEY`: Your NuGet API key (required for publishing)

### Optional Environment Variables

- `NUGET_SOURCE`: NuGet source URL (defaults to NuGet.org)
- `PACKAGE_VERSION`: Override package version
- `BUILD_CONFIGURATION`: Build configuration (defaults to Release)

### Example .env File

```bash
# Required
NUGET_API_KEY=oy2abc123def456ghi789jkl012mno345pqr678stu901vwx234yz

# Optional
NUGET_SOURCE=https://api.nuget.org/v3/index.json
PACKAGE_VERSION=1.0.1
BUILD_CONFIGURATION=Release
```

## Package Metadata

The package includes comprehensive metadata:

- **Package ID**: `Laneful.CSharp`
- **Description**: A C# client library for the Laneful email API
- **Tags**: email, api, laneful, csharp, dotnet
- **License**: MIT
- **Repository**: GitHub repository URL
- **Documentation**: Auto-generated XML documentation
- **Symbols**: Debug symbols package (.snupkg)

## Troubleshooting

### Common Issues

#### 1. .NET SDK Not Found

```
Error: .NET SDK is not installed
```

**Solution**: Install .NET 9.0 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)

#### 2. Missing API Key

```
Error: NUGET_API_KEY environment variable is required
```

**Solution**: Add your NuGet API key to the `.env` file

#### 3. Package Already Exists

```
Error: Package already exists
```

**Solution**: 
- Update the version in `Laneful.CSharp.csproj`
- Or use `--skip-duplicate` flag (already included in the publish script)

#### 4. Build Failures

```
Error: Build failed
```

**Solution**:
- Check that all dependencies are properly referenced
- Ensure the project builds locally first
- Check for compilation errors

### Debugging

#### Test Locally

```bash
# Build and test locally
./build.sh build
./build.sh test
./build.sh package

# Check the generated package
ls -la packages/
```

#### Manual Commands

```bash
# Check .NET version
dotnet --version

# Restore dependencies
dotnet restore

# Build with verbose output
dotnet build --configuration Release --verbosity normal

# Pack with verbose output
dotnet pack --configuration Release --output ./packages --verbosity normal
```

## Version Management

### Semantic Versioning

The project follows [Semantic Versioning](https://semver.org/):

- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

### Updating Versions

Update the version in `Laneful.CSharp.csproj`:

```xml
<Version>1.0.1</Version>
```

## Security Considerations

### API Key Security

- **Never commit** your `.env` file to version control
- **Rotate API keys** regularly
- **Use least privilege** - create API keys with minimal required permissions

### Package Signing

Consider adding package signing for enhanced security:

```xml
<PropertyGroup>
  <SignAssembly>true</SignAssembly>
  <AssemblyOriginatorKeyFile>key.snk</AssemblyOriginatorKeyFile>
</PropertyGroup>
```

## Additional Resources

- [NuGet Package Creation Guide](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package)
- [.NET CLI Documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/)
- [Semantic Versioning](https://semver.org/)