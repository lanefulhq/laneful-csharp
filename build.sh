#!/bin/bash

# Laneful C# SDK Build and Publish Script
# This script provides various commands for building, testing, and publishing the SDK

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to show usage
show_usage() {
    echo "Usage: $0 [COMMAND]"
    echo ""
    echo "Commands:"
    echo "  build       Build the project locally"
    echo "  test        Run tests"
    echo "  package     Create NuGet package locally"
    echo "  publish     Build and publish to NuGet"
    echo "  clean       Clean build artifacts"
    echo "  help        Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 build                    # Build locally"
    echo "  $0 publish                  # Publish to NuGet"
    echo "  $0 package                  # Create package locally"
}

# Function to check if .NET is installed
check_dotnet() {
    if ! command -v dotnet > /dev/null 2>&1; then
        print_error ".NET SDK is not installed. Please install .NET 9.0 SDK and try again."
        exit 1
    fi
    
    # Check .NET version
    DOTNET_VERSION=$(dotnet --version)
    print_status "Using .NET version: $DOTNET_VERSION"
}

# Function to check if .env file exists
check_env() {
    if [ ! -f ".env" ]; then
        print_warning ".env file not found. Creating from template..."
        if [ -f "env.example" ]; then
            cp env.example .env
            print_warning "Please edit .env file with your NuGet API key before publishing."
        else
            print_error "env.example file not found. Cannot create .env file."
            exit 1
        fi
    fi
}

# Function to load environment variables
load_env() {
    if [ -f ".env" ]; then
        export $(cat .env | grep -v '^#' | xargs)
    fi
}

# Function to build locally
build_local() {
    check_dotnet
    print_status "Building project locally..."
    dotnet build --configuration Release
    print_success "Build completed successfully!"
}

# Function to run tests
run_tests() {
    check_dotnet
    print_status "Running tests..."
    dotnet test --configuration Release
    print_success "Tests completed successfully!"
}

# Function to create package locally
create_package() {
    check_dotnet
    print_status "Creating NuGet package locally..."
    
    # Create packages directory if it doesn't exist
    mkdir -p packages
    
    dotnet pack --configuration Release --output ./packages --no-build
    print_success "Package created successfully in ./packages directory!"
}

# Function to publish to NuGet
publish_nuget() {
    check_dotnet
    check_env
    load_env
    
    print_status "Building and publishing to NuGet..."
    
    # Check if API key is provided
    if [ -z "$NUGET_API_KEY" ]; then
        print_error "NUGET_API_KEY environment variable is required"
        print_error "Please set it in your .env file or export it directly"
        exit 1
    fi
    
    # Set default source if not provided
    if [ -z "$NUGET_SOURCE" ]; then
        NUGET_SOURCE="https://api.nuget.org/v3/index.json"
    fi
    
    print_status "Using NuGet source: $NUGET_SOURCE"
    
    # Build the project
    print_status "Building project..."
    dotnet build --configuration Release
    
    # Create the package
    print_status "Creating NuGet package..."
    mkdir -p packages
    dotnet pack --configuration Release --output ./packages --no-build
    
    # Find the .nupkg file
    PACKAGE_FILE=$(find ./packages -name "*.nupkg" | head -1)
    
    if [ -z "$PACKAGE_FILE" ]; then
        print_error "No .nupkg file found in packages directory"
        exit 1
    fi
    
    print_status "Found package: $PACKAGE_FILE"
    
    # Publish the package
    print_status "Publishing package to NuGet..."
    dotnet nuget push "$PACKAGE_FILE" --api-key "$NUGET_API_KEY" --source "$NUGET_SOURCE" --skip-duplicate
    
    print_success "Package published successfully!"
}

# Function to clean build artifacts
clean_artifacts() {
    print_status "Cleaning build artifacts..."
    
    # Clean local artifacts
    dotnet clean
    rm -rf bin/ obj/ packages/
    
    print_success "Cleanup completed!"
}

# Main script logic
case "${1:-help}" in
    "build")
        build_local
        ;;
    "test")
        run_tests
        ;;
    "package")
        create_package
        ;;
    "publish")
        publish_nuget
        ;;
    "clean")
        clean_artifacts
        ;;
    "help"|"--help"|"-h")
        show_usage
        ;;
    *)
        print_error "Unknown command: $1"
        echo ""
        show_usage
        exit 1
        ;;
esac

