#!/bin/bash

# Laneful C# SDK Examples Docker Runner
# Usage: ./run-docker.sh [example-name|all]

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🚀 Laneful C# SDK Examples Docker Runner${NC}"
echo "=============================================="

# Check if .env file exists
if [ ! -f ".env" ]; then
    echo -e "${RED}❌ Error: .env file not found${NC}"
    echo "Please copy env.example to .env and fill in your credentials:"
    echo "  cp env.example .env"
    echo "  # Edit .env with your actual values"
    exit 1
fi

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo -e "${RED}❌ Error: Docker is not running${NC}"
    echo "Please start Docker and try again"
    exit 1
fi

# Build the Docker image
echo -e "${YELLOW}🔨 Building Docker image...${NC}"
docker build -f examples/Dockerfile -t laneful-csharp-examples . || {
    echo -e "${RED}❌ Failed to build Docker image${NC}"
    exit 1
}

echo -e "${GREEN}✅ Docker image built successfully${NC}"

# Determine what to run
EXAMPLE=${1:-"all"}

if [ "$EXAMPLE" = "all" ]; then
    echo -e "${BLUE}📦 Running all examples...${NC}"
    docker run --env-file examples/.env laneful-csharp-examples all
else
    echo -e "${BLUE}🎯 Running example: $EXAMPLE${NC}"
    docker run --env-file examples/.env laneful-csharp-examples "$EXAMPLE"
fi

echo -e "${GREEN}✅ Examples completed!${NC}"
