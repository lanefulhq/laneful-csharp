# Use the official .NET 9.0 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory
WORKDIR /app

# Copy the main SDK project file
COPY Laneful.CSharp.csproj ./
COPY src/ ./src/

# Build the SDK
RUN dotnet build Laneful.CSharp.csproj -c Release

# Copy examples project file and source
COPY examples/Examples.csproj ./examples/
COPY examples/*.cs ./examples/

# Build the examples project
WORKDIR /app/examples
RUN dotnet build Examples.csproj -c Release

# Create a script to run examples
RUN echo '#!/bin/bash\n\
if [ "$1" = "all" ]; then\n\
  echo "Running all examples..."\n\
  dotnet Examples.dll all\n\
else\n\
  echo "Running example: $1"\n\
  dotnet Examples.dll $1\n\
fi' > /app/run-example.sh && chmod +x /app/run-example.sh

# Final stage
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime

WORKDIR /app

# Copy the built application
COPY --from=build /app/examples/bin/Release/net9.0/ ./
COPY --from=build /app/run-example.sh ./

# Set the entrypoint
ENTRYPOINT ["/app/run-example.sh"]
