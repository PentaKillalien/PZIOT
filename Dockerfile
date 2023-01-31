#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#这种模式是直接在构建镜像的内部编译发布dotnet项目。
#注意下容器内输出端口是9291
#如果你想先手动dotnet build成可执行的二进制文件，然后再构建镜像，请看.Api层下的dockerfile。


FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build
WORKDIR /src
COPY ["PZIOT.Api/PZIOT.Api.csproj", "PZIOT.Api/"]
COPY ["PZIOT.Extensions/PZIOT.Extensions.csproj", "PZIOT.Extensions/"]
COPY ["PZIOT.Tasks/PZIOT.Tasks.csproj", "PZIOT.Tasks/"]
COPY ["PZIOT.IServices/PZIOT.IServices.csproj", "PZIOT.IServices/"]
COPY ["PZIOT.Model/PZIOT.Model.csproj", "PZIOT.Model/"]
COPY ["PZIOT.Common/PZIOT.Common.csproj", "PZIOT.Common/"]
COPY ["PZIOT.Services/PZIOT.Services.csproj", "PZIOT.Services/"]
COPY ["PZIOT.Repository/PZIOT.Repository.csproj", "PZIOT.Repository/"]
COPY ["PZIOT.EventBus/PZIOT.EventBus.csproj", "PZIOT.EventBus/"]
RUN dotnet restore "PZIOT.Api/PZIOT.Api.csproj"
COPY . .
WORKDIR "/src/PZIOT.Api"
RUN dotnet build "PZIOT.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PZIOT.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 9291 
ENTRYPOINT ["dotnet", "PZIOT.Api.dll"]
