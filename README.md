# Advance Requests API

API para gerenciamento de **solicitações de adiantamento financeiro**, desenvolvida como solução para um desafio técnico backend.

O projeto foi implementado utilizando **.NET 10**, **Clean Architecture**, **CQRS (Command/Query Dispatcher)**, **mensageria com RabbitMQ** e o padrão **Transactional Outbox** para garantir consistência entre persistência e publicação de eventos.

---

# Arquitetura

O projeto segue **Clean Architecture**, separando responsabilidades em camadas independentes.

```
src/
 ├─ AdvanceRequests.Api
 ├─ AdvanceRequests.Application
 ├─ AdvanceRequests.Domain
 └─ AdvanceRequests.Infrastructure

tests/
 ├─ AdvanceRequests.UnitTests
 └─ AdvanceRequests.IntegrationTests
```

## Camadas

### Domain

Contém:

* Entidades
* Enums
* Eventos de domínio
* Regras de negócio
* Exceptions de domínio

Essa camada não possui dependência de frameworks.

---

### Application

Contém:

* Commands
* Queries
* Handlers
* DTOs
* Abstrações de repositórios
* Dispatcher CQRS (CommandDispatcher / QueryDispatcher)

Responsável por orquestrar o domínio.

---

### Infrastructure

Implementa:

* Persistência com **Entity Framework Core**
* Repositórios
* Integração com **RabbitMQ**
* Implementação do **Outbox Pattern**
* Worker responsável por publicar eventos

---

### API

Responsável por:

* Controllers
* Validação com **FluentValidation**
* Autenticação JWT
* Versionamento de API
* Swagger
* Middlewares

---

# Diagrama de Arquitetura

```
Client
   │
   ▼
AdvanceRequests API
   │
   ├── PostgreSQL
   │      ├── AdvanceRequests
   │      └── OutboxMessages
   │
   └── Outbox Worker
           │
           ▼
        RabbitMQ
           │
           ▼
     Event Consumers
```

Fluxo:

1. Cliente envia requisição
2. API executa regras de negócio
3. Dados são persistidos no banco
4. Evento é salvo na tabela **Outbox**
5. Worker lê a Outbox
6. Evento é publicado no RabbitMQ

---

# Tecnologias utilizadas

* .NET 10
* ASP.NET Core
* Entity Framework Core
* PostgreSQL
* RabbitMQ
* FluentValidation
* JWT Authentication
* Docker
* Docker Compose
* Swagger
* xUnit
* FluentAssertions

---

# Decisões de Arquitetura

## Clean Architecture

Separação clara de responsabilidades entre camadas, permitindo:

* baixo acoplamento
* facilidade de testes
* independência de frameworks

---

## CQRS com Dispatcher

Foi implementado um **CommandDispatcher** e **QueryDispatcher**, permitindo que os controllers não dependam diretamente dos handlers da camada de aplicação.

Isso mantém os controllers mais limpos e reduz acoplamento.

---

## Transactional Outbox

Evita inconsistência entre banco e eventos.

Sem esse padrão poderiam ocorrer cenários como:

1. salvar no banco
2. falhar ao publicar evento

ou

1. publicar evento
2. falhar ao salvar no banco

O Outbox garante consistência.

---

## Autenticação simplificada

Para facilitar execução local e testes, foi implementado um endpoint de login que gera tokens JWT simulando usuários:

* Admin
* Creator

Não há persistência de usuários.

---

# Versionamento da API

A API utiliza **versionamento por segmento de URL**.

Exemplo:

```
/api/v1/advance-requests
```

---

# Autenticação

A API utiliza **JWT Bearer Authentication**.

## Endpoint de login

```
POST /api/v1/auth/login
```

### Login como Admin

```json
{
  "role": "Admin"
}
```

---

### Login como Creator

```json
{
  "role": "Creator",
  "creatorId": "11111111-1111-1111-1111-111111111111"
}
```

Resposta:

```json
{
  "accessToken": "JWT_TOKEN",
  "expiresAtUtc": "2026-01-01T00:00:00Z"
}
```

---

# Autorização

## Creator

Pode:

* criar solicitação
* listar apenas suas solicitações

Não pode:

* aprovar
* rejeitar

---

## Admin

Pode:

* listar todas as solicitações
* aprovar solicitações
* rejeitar solicitações

---

# Endpoints

## Simular adiantamento

Endpoint público.

```
GET /api/v1/advance-requests/simulate?amount=1000
```

Resposta:

```json
{
  "grossAmount": 1000,
  "feeAmount": 50,
  "netAmount": 950
}
```

---

## Criar solicitação

```
POST /api/v1/advance-requests
```

Header:

```
Authorization: Bearer TOKEN_CREATOR
```

Body:

```json
{
  "grossAmount": 1000
}
```

---

## Listar solicitações

```
GET /api/v1/advance-requests
```

Regras:

Creator:

* vê apenas suas solicitações

Admin:

* vê todas

---

### Filtros

```
status
creatorId
fromDate
toDate
```

Exemplo:

```
GET /api/v1/advance-requests?status=Pending
```

---

## Aprovar solicitação

Admin:

```
POST /api/v1/advance-requests/{id}/approve
```

---

## Rejeitar solicitação

Admin:

```
POST /api/v1/advance-requests/{id}/reject
```

---

# Fluxo de uso da API

1. Gerar token

```
POST /api/v1/auth/login
```

2. Criar solicitação

```
POST /api/v1/advance-requests
```

3. Listar solicitações

```
GET /api/v1/advance-requests
```

4. Aprovar ou rejeitar

```
POST /api/v1/advance-requests/{id}/approve
```

---

# Postman

Para facilitar testes da API, o projeto inclui:

* Collection do Postman
* Variáveis de ambiente

Arquivos incluídos:

```
postman/
 ├─ AdvanceRequests.postman_collection.json
 └─ AdvanceRequests.postman_environment.json
```

---

## Importar no Postman

1. Abrir Postman
2. Clique em **Import**
3. Importar:

```
AdvanceRequests.postman_collection.json
```

4. Importar também:

```
AdvanceRequests.postman_environment.json
```

5. Selecionar o environment.

---

## Variáveis do ambiente

```
base_url = http://localhost:8080
creator_token
admin_token
request_id
creator_id
```

O login gera automaticamente os tokens.

---

# Executando o projeto

## Pré-requisitos

* Docker
* Docker Compose
* .NET SDK 10

---

## Subir aplicação

```
docker compose up --build
```

---

## Serviços disponíveis

### API

```
http://localhost:8080/swagger
```

---

### RabbitMQ

```
http://localhost:15672
```

Login:

```
guest
guest
```

---

# Banco de dados

PostgreSQL executado via Docker.

As migrations são aplicadas automaticamente ao iniciar a aplicação.

---

# Testes

O projeto possui dois tipos de testes.

---

## Testes unitários

Validam regras de negócio do domínio.

Executar:

```
dotnet test tests/AdvanceRequests.UnitTests
```

---

## Testes de integração

Validam comportamento completo da API.

Executar:

```
dotnet test tests/AdvanceRequests.IntegrationTests
```

---

# Melhorias futuras

Possíveis evoluções:

* Observabilidade (OpenTelemetry)
* Métricas (Prometheus)
* Retry policy no Outbox Worker
* Dead Letter Queue
* Idempotência de consumidores
* Rate limiting
* Autenticação com Identity

---

# Autor

Projeto desenvolvido como solução para desafio técnico backend.
