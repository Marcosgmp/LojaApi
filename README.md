# Loja API — Web API com EF Core (.NET 8 + SQLite)

API REST para gerenciamento de uma loja simples, demonstrando uso de **ASP.NET Core Web API** com **Entity Framework Core** e banco de dados **SQLite**.

## 📦 Entidades e Relacionamentos

| Entidade  | Relacionamento                          |
|-----------|------------------------------------------|
| `Pessoa`  | **1:1** com `Endereco`                   |
| `Endereco`| **1:1** com `Pessoa` (dependente, possui FK `PessoaId`) |
| `Categoria`| **1:N** com `Produto`                   |
| `Produto` | **N:1** com `Categoria` e **N:M** com `Tag` (via `ProdutoTag`) |
| `Tag`     | **N:M** com `Produto`                    |
| `ProdutoTag` | Tabela associativa explícita (chave composta `ProdutoId` + `TagId`) |

Resumo visual:

```
Pessoa (1) ───── (1) Endereco

Categoria (1) ───── (N) Produto (N) ───── (N) Tag
                                via ProdutoTag
```

## 🛠️ Tecnologias

- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core 8 (`Microsoft.EntityFrameworkCore.Sqlite`)
- SQLite (arquivo local `loja.db`)
- Swagger / Swashbuckle (documentação e testes dos endpoints)

## ✅ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado (`dotnet --version` deve mostrar `8.x`)
- Ferramenta `dotnet-ef` (instruções abaixo)

## 🚀 Como rodar o projeto

### 1. Clonar o repositório e entrar na pasta

```bash
git clone https://github.com/Marcosgmp/LojaApi.git
cd LojaApi
```

### 2. Restaurar pacotes

```bash
dotnet restore
```

### 3. Instalar a ferramenta `dotnet-ef` (caso não tenha)

```bash
dotnet tool install --global dotnet-ef
```

> Se já tiver instalada e a versão for diferente, pode atualizar com:
> `dotnet tool update --global dotnet-ef`

### 4. Criar as migrations (primeira vez)

Este repositório **não** inclui a pasta `Migrations/` versionada — gere localmente para garantir compatibilidade com a versão do SDK/pacotes na sua máquina:

```bash
dotnet ef migrations add InitialCreate
```

### 5. Aplicar a migration e criar o banco SQLite

```bash
dotnet ef database update
```

Isso vai criar o arquivo `loja.db` na raiz do projeto, já com as 6 tabelas (`Pessoas`, `Enderecos`, `Categorias`, `Produtos`, `Tags`, `ProdutoTags`) e suas chaves estrangeiras/relacionamentos.

### 6. Executar a aplicação

```bash
dotnet run
```

A API estará disponível em:

```
http://localhost:5099
```

O **Swagger UI** abre automaticamente na raiz (`http://localhost:5099/`), com todos os endpoints documentados e testáveis diretamente no navegador.

## 🔌 Connection String

Definida em `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=loja.db"
}
```

Para usar outro banco (Postgres/MySQL), basta:
1. Trocar o pacote `Microsoft.EntityFrameworkCore.Sqlite` pelo provider correspondente (ex: `Npgsql.EntityFrameworkCore.PostgreSQL`).
2. Alterar `UseSqlite(...)` para `UseNpgsql(...)` em `Program.cs`.
3. Atualizar a connection string.
4. Apagar a pasta `Migrations/` antiga e gerar novamente (`dotnet ef migrations add InitialCreate`).

## 📋 Endpoints disponíveis

### Pessoas (`/api/pessoas`) — inclui Endereço (1:1)
| Método | Rota              | Descrição              |
|--------|-------------------|-------------------------|
| GET    | /api/pessoas      | Lista todas as pessoas |
| GET    | /api/pessoas/{id} | Busca por id            |
| POST   | /api/pessoas      | Cria pessoa (+ endereço opcional) |
| PUT    | /api/pessoas/{id} | Atualiza pessoa/endereço |
| DELETE | /api/pessoas/{id} | Remove pessoa (cascade no endereço) |

Exemplo de payload (POST/PUT):
```json
{
  "nome": "Maria Silva",
  "email": "maria@email.com",
  "dataNascimento": "1995-04-12",
  "endereco": {
    "logradouro": "Rua das Flores",
    "numero": "123",
    "cidade": "Fortaleza",
    "estado": "CE",
    "cep": "60000-000"
  }
}
```

### Categorias (`/api/categorias`)
| Método | Rota                 | Descrição |
|--------|----------------------|-----------|
| GET    | /api/categorias      | Lista categorias (com contagem de produtos) |
| GET    | /api/categorias/{id} | Busca por id |
| POST   | /api/categorias      | Cria categoria |
| PUT    | /api/categorias/{id} | Atualiza categoria |
| DELETE | /api/categorias/{id} | Remove (bloqueado se houver produtos vinculados) |

```json
{ "nome": "Eletrônicos", "descricao": "Produtos eletrônicos em geral" }
```

### Tags (`/api/tags`)
| Método | Rota           | Descrição |
|--------|----------------|-----------|
| GET    | /api/tags      | Lista tags |
| GET    | /api/tags/{id} | Busca por id |
| POST   | /api/tags      | Cria tag |
| PUT    | /api/tags/{id} | Atualiza tag |
| DELETE | /api/tags/{id} | Remove tag |

```json
{ "nome": "Promoção" }
```

### Produtos (`/api/produtos`) — N:1 Categoria, N:M Tags
| Método | Rota               | Descrição |
|--------|--------------------|-----------|
| GET    | /api/produtos      | Lista produtos (com categoria e tags) |
| GET    | /api/produtos/{id} | Busca por id |
| POST   | /api/produtos      | Cria produto |
| PUT    | /api/produtos/{id} | Atualiza produto |
| DELETE | /api/produtos/{id} | Remove produto |

```json
{
  "nome": "Notebook Gamer",
  "preco": 4599.90,
  "estoque": 10,
  "categoriaId": 1,
  "tagIds": [1, 2]
}
```

## 🧪 Sugestão de roteiro de teste (via Swagger)

1. `POST /api/categorias` → criar "Eletrônicos"
2. `POST /api/tags` → criar "Promoção" e "Novidade"
3. `POST /api/produtos` → criar produto vinculando `categoriaId` e `tagIds`
4. `POST /api/pessoas` → criar pessoa com endereço embutido
5. `GET` em cada recurso para validar os relacionamentos retornados
6. `PUT`/`DELETE` para validar atualização e exclusão (testar a regra que impede excluir categoria com produtos)

## 📁 Estrutura do projeto

```
LojaApi/
├── Controllers/        # Controllers (CRUD de cada entidade)
├── Models/              # Entidades do domínio (Pessoa, Endereco, Categoria, Produto, Tag, ProdutoTag)
├── DTOs/                # Objetos de entrada/saída usados pelos controllers
├── Data/                # LojaContext (DbContext + configuração dos relacionamentos)
├── Migrations/          # Gerado por você via dotnet ef (não versionado)
├── appsettings.json     # Connection string
└── Program.cs           # Configuração da aplicação (EF Core, Swagger, CORS)
```

## 👥 Equipe

|       Nome        |       GitHub      |     
|-------------------|-------------------|
| Yan Carlos        | [Yan-CarlosIF](https://github.com/Yan-CarlosIF)      |
| Marcos Gustavo    | [Marcosgmp](https://github.com/Marcosgmp) |
| Artur Soares      | [arturuzi1](https://github.com/arturuzi1) |
