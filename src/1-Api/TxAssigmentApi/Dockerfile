#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/1-Api/TxAssigmentApi/TxAssigmentApi.csproj", "src/1-Api/TxAssigmentApi/"]
COPY ["src/3-Services/TxAssignmentServices/TxAssignmentServices.csproj", "src/3-Services/TxAssignmentServices/"]
COPY ["src/4-Infra/TxAssignmentInfra/TxAssignmentInfra.csproj", "src/4-Infra/TxAssignmentInfra/"]
RUN dotnet restore "src/1-Api/TxAssigmentApi/TxAssigmentApi.csproj"
COPY . .
WORKDIR "/src/src/1-Api/TxAssigmentApi"
RUN dotnet build "TxAssigmentApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TxAssigmentApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TxAssigmentApi.dll"]