# Segfy — API de Apólices de Seguro Automóvel

API REST em **ASP.NET Core MVC (.NET 8)** para o cadastro e consulta de apólices de seguro automóvel, desenvolvida para o desafio hands-on de Desenvolvedor(a) Jr.

## Stack

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core + SQLite
- xUnit, Moq e Microsoft.Data.Sqlite (testes)
- Swagger

## Como executar

Pré-requisito: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
git clone <url-do-repositorio>
cd SegfyDesafio
dotnet restore
dotnet run --project src/Segfy.Api
```

A API sobe em http://localhost:5080 e o Swagger fica disponível em http://localhost:5080/swagger, onde é possível visualizar os endpoints, consultar a estrutura das requisições e testar a API.

O banco de dados SQLite (`segfy.db`) é criado automaticamente na primeira execução, na pasta do projeto `src/Segfy.Api`.

## Como rodar os testes

```bash
dotnet test
```

## Estrutura do projeto

```
SegfyDesafio/
├── src/
│   └── Segfy.Api/
│       ├── Controllers/     -> endpoints REST
│       ├── Dtos/             -> contratos de entrada e saída
│       ├── Models/           -> entidades de domínio
│       ├── Data/             -> DbContext
│       ├── Repositories/     -> acesso a dados (EF Core)
│       └── Services/         -> regras de negócio
└── tests/
    └── Segfy.Api.Tests/
        ├── Services/         -> testes unitários da camada de serviço
        └── Repositories/     -> testes de integração da camada de dados
```

A aplicação segue separação em camadas (Controller → Service → Repository), com as dependências injetadas por interface para facilitar testes e manutenção.


## Endpoints

| Método | Rota                                | Descrição                                    |
|--------|--------------------------------------|-----------------------------------------------|
| GET    | `/api/apolices`                     | Lista todas as apólices                       |
| GET    | `/api/apolices/{id}`                | Consulta uma apólice por id                   |
| GET    | `/api/apolices/vencendo-em-30-dias` | Lista apólices ativas com vigência vencendo nos próximos 30 dias |
| POST   | `/api/apolices`                     | Cria uma nova apólice                         |
| PUT    | `/api/apolices/{id}`                | Atualiza uma apólice existente                |
| DELETE | `/api/apolices/{id}`                | Remove uma apólice                            |