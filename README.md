# web-api-net-core-3
<br>
API simples em .NET Core 3 utilizando JWT Authentication
<br><br>

## Requisitos/Ferramentas

Para executar a API localmente, será necessário baixar e instalar os itens abaixo

- **.NET Core SDK** versão .NET Core 3.1 - [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)
    - Incluíndo o .NET Core Runtime e as ferramentas de linha de comando (command line tools)
- **Visual Studio Code (VS Code)** - [https://code.visualstudio.com/](https://code.visualstudio.com/)
- **C# Extension para VS Code** - [https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
- **MySQL Server** 5.7/8.0 - [https://dev.mysql.com/downloads](https://dev.mysql.com/downloads)
- **Git** - [https://git-scm.com/downloads](https://git-scm.com/downloads)
- **Postman** - [https://www.postman.com/downloads](https://www.postman.com/downloads)
<br><br>

## Configurações Gerais

- Baixar ou clonar este projeto

    ```csharp
    git clone https://github.com/jgivago/web-api-net-core-3.git
    ```

- Abrir o arquivo `WebApi\Data\DatabaseContext.cs` e alterar a configuração de conexão com o banco de dados local (MySQL)

    ```csharp
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL(@"server=localhost;user=root;password=root;database=apidb");
    }
    ```
- Abrir o banco de dados local e criar um novo database com o nome da configuração `database` descrita no passo acima

- Executar no terminal o comando `dotnet restore` para baixar todas as dependências do projeto

- Executar no terminal o comando `dotnet ef database update --project WebApi/WebApi.csproj` para a criação da estrutura do banco de dados (Migrations)

    ```csharp
    $ dotnet ef database update --project WebApi/WebApi.csproj
    Build started...
    Build succeeded.
    info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
        Entity Framework Core 3.1.10 initialized 'DatabaseContext' using provider 'MySql.EntityFrameworkCore' with options: None
    info: Microsoft.EntityFrameworkCore.Database.Command[20101]
        Executed DbCommand (2ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
        SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='apidb' AND TABLE_NAME='__EFMigrationsHistory';
    info: Microsoft.EntityFrameworkCore.Database.Command[20101]
        Executed DbCommand (38ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
        CREATE TABLE `__EFMigrationsHistory` (
            `MigrationId` varchar(150) NOT NULL,
            `ProductVersion` varchar(32) NOT NULL,
            PRIMARY KEY (`MigrationId`)
        );
    info: Microsoft.EntityFrameworkCore.Database.Command[20101]
        Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
        SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='apidb' AND TABLE_NAME='__EFMigrationsHistory';
    info: Microsoft.EntityFrameworkCore.Database.Command[20101]
        Executed DbCommand (2ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
        SELECT `MigrationId`, `ProductVersion`
        FROM `__EFMigrationsHistory`
        ORDER BY `MigrationId`;
    info: Microsoft.EntityFrameworkCore.Migrations[20402]
        Applying migration '20210520183221_InitialCreate'.
    Applying migration '20210520183221_InitialCreate'.
    info: Microsoft.EntityFrameworkCore.Database.Command[20101]
        Executed DbCommand (35ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
        CREATE TABLE `Users` (
            `Id` int NOT NULL AUTO_INCREMENT,
            `FirstName` varchar(100) NOT NULL,
            `LastName` varchar(100) NOT NULL,
            `Username` varchar(50) NOT NULL,
            `Password` varchar(50) NOT NULL,
            PRIMARY KEY (`Id`)
        );
    info: Microsoft.EntityFrameworkCore.Database.Command[20101]
        Executed DbCommand (2ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
        INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
        VALUES ('20210520183221_InitialCreate', '3.1.10');
    Done.
    ```

- Executar o comando `dotnet run --project WebApi/WebApi.csproj` para levantar um servidor local

    ```csharp
    $ dotnet run --project WebApi/WebApi.csproj
    info: Microsoft.Hosting.Lifetime[0]
        Now listening on: http://localhost:4000        
    info: Microsoft.Hosting.Lifetime[0]
        Application started. Press Ctrl+C to shut down.
    info: Microsoft.Hosting.Lifetime[0]
        Hosting environment: Production
    info: Microsoft.Hosting.Lifetime[0]
        Content root path: C:\Code\web-api-net-core-3
    ```
<br>

## Utilização da API

- Importar o arquivo `WebApi\web-api-aspnet-core-3.postman_collection.json` no Postman (Todos os endpoints já estão configurados/documentados)
    - Criar um novo usuário executando o método `Create New User` (não precisa de autenticação)
    - Autenticar o usuário criado executando o método `Authorization`
    - As demais operações com o usuário precisarão utilizar o token retornado na execução do `Authorization`
    ```csharp
    {
        "id": 1,
        "firstName": "Lionel (Editado)",
        "lastName": "Messi",
        "username": "lionel",
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE2MjE1NTk5MDYsImV4cCI6MTYyMjE2NDcwNiwiaWF0IjoxNjIxNTU5OTA2fQ.9yCC2Z3NDswnwf2GPxKy964moBCH1eD-ZZsLKJ6IXzg"
    }
    ```
    - Utilizar a opção `Bearer Token` para autenticação
<br><br>

## Testes
- Para executar os testes via linha de comando:
    - Executar o comando `dotnet run --project WebApi/WebApi.csproj` para levantar a API (caso ainda não tenha sido executado anteriormente)
    - Executar no terminal o comando `dotnet test`
    - Aguardar o resultado dos testes
    ```csharp
    $ dotnet test
    Determinando os projetos a serem restaurados...
    Todos os projetos estão atualizados para restauração.
    WebApi -> C:\Code\web-api-net-core-3-v2\web-api-net-core-3\WebApi\bin\Debug\netcoreapp3.1\WebApi.dll
    WebApi.Tests -> C:\Code\web-api-net-core-3-v2\web-api-net-core-3\WebApi.Tests\bin\Debug\net5.0\WebApi.Tests.dll
    Execução de teste para C:\Code\web-api-net-core-3-v2\web-api-net-core-3\WebApi.Tests\bin\Debug\net5.0\WebApi.Tests.dll (.NETCoreApp,Version=v5.0)
    Ferramenta de Linha de Comando de Execução de Teste da Microsoft (R) Versão 16.9.4
    Copyright (c) Microsoft Corporation. Todos os direitos reservados.

    Iniciando execução de teste, espere...
    1 arquivos de teste no total corresponderam ao padrão especificado.

    Aprovado!  - Com falha:     0, Aprovado:     1, Ignorado:     0, Total:     1, Duração: 1 ms - WebApi.Tests.dll (net5.0)
    ```
<br>

## Alguma dúvida sobre o .NET Core?
A documentação do .NET Core pode ser encontrada em [https://docs.microsoft.com/pt-br/dotnet/core/introduction](https://docs.microsoft.com/pt-br/dotnet/core/introduction)
<br><br>

## Referências
[https://github.com/cornflourblue/aspnet-core-3-jwt-authentication-api](https://github.com/cornflourblue/aspnet-core-3-jwt-authentication-api)