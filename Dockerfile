FROM microsoft/dotnet:1.0.5-sdk
WORKDIR /app

# copy csproj and restore as distinct layers
COPY src/StegoCoreLib/StegoCoreLib.csproj .
COPY src/StegoCoreLib/NuGet.config .
RUN dotnet restore

# copy everything else and build
COPY src/StegoCoreLib/ .
RUN dotnet publish -c Release -o out

# test
COPY src/ ./t/src
COPY tests/ ./t/tests
RUN dotnet restore ./t/tests/StegoCoreTests/StegoCoreTests.csproj
RUN dotnet test ./t//tests/StegoCoreTests/StegoCoreTests.csproj
