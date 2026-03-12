# Advance Requests API

API para gerenciamento de **solicitações de adiantamento financeiro**, desenvolvida como solução para um desafio técnico backend.

O projeto foi construído utilizando **.NET 10**, **Clean Architecture**, **Domain Driven Design**, **mensageria com RabbitMQ** e o padrão **Transactional Outbox** para garantir consistência entre persistência e eventos.

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

Essa camada não possui dependências externas.

---

### Application

Contém:

* Casos de uso
* Commands e Queries
* Handlers
* DTOs
* Interfaces de repositórios
* Serviços de aplicação

Responsável por orquestrar o domínio.

---

### Infrastructure

Implementa:

* Persistência com **Entity Framework Core**
* Repositórios
* Integração com **RabbitMQ**
* **Outbox Pattern**
* Background Worker responsável por publicar eventos

---

### API

Responsável por:

* Controllers
* Validação de entrada
* Autenticação JWT
* Middlewares
* Swagger / OpenAPI

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

1. Cliente envia requisição para API
2. API salva dados no banco
3. Evento é salvo na tabela **Outbox**
4. Worker lê a tabela Outbox
5. Evento é publicado no RabbitMQ

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
* xUnit
* FluentAssertions

---

# Decisões de Arquitetura

## Clean Architecture

A separação em camadas permite:

* isolamento do domínio
* facilidade de testes
* independência de frameworks
* evolução do sistema sem acoplamento

---

## Transactional Outbox

Evita inconsistência entre banco e eventos.

Sem esse padrão, poderiam ocorrer cenários como:

1. salvar no banco
2. falhar ao publicar evento

ou

1. publicar evento
2. falhar ao salvar no banco

O Outbox resolve esse problema garantindo consistência.

---

## JWT simplificado

Para o desafio foi implementado um mecanismo de autenticação JWT
sem persistência de usuários.

Isso permite simular diferentes papéis:

* **Admin**
* **Creator**

Durante execução local e testes.

---

# Autenticação

A API utiliza **JWT Bearer Authentication**.

Não existe persistência de usuários.
O endpoint de autenticação apenas gera tokens para simulação de papéis.

---

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

---

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

Requer autenticação **Creator**.

```
POST /api/v1/advance-requests
```

Body:

```json
{
  "grossAmount": 1000
}
```

O `CreatorId` é obtido a partir do **JWT**.

---

## Listar solicitações

```
GET /api/v1/advance-requests
```

Requer autenticação.

### Regras

Creator:

* lista apenas suas solicitações

Admin:

* lista todas as solicitações

---

### Filtros suportados

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

Apenas **Admin**.

```
POST /api/v1/advance-requests/{id}/approve
```

Resposta:

```
204 NoContent
```

---

## Rejeitar solicitação

Apenas **Admin**.

```
POST /api/v1/advance-requests/{id}/reject
```

Resposta:

```
204 NoContent
```

---

# Fluxo de uso da API

1. Gerar token

```
POST /api/v1/auth/login
```

Creator:

```json
{
  "role": "Creator",
  "creatorId": "11111111-1111-1111-1111-111111111111"
}
```

---

2. Criar solicitação

```
POST /api/v1/advance-requests
```

Header:

```
Authorization: Bearer TOKEN
```

Body:

```json
{
  "grossAmount": 1000
}
```

---

3. Listar solicitações

```
GET /api/v1/advance-requests
```

Header:

```
Authorization: Bearer TOKEN
```

---

4. Aprovar solicitação (Admin)

```
POST /api/v1/advance-requests/{id}/approve
```

Header:

```
Authorization: Bearer TOKEN_ADMIN
```

---

# Regras de negócio

* Valor mínimo da solicitação deve ser **maior que 100**
* Taxa de adiantamento: **5%**
* Apenas **uma solicitação pendente por creator**
* Estados possíveis:

```
Pending
Approved
Rejected
```

---

# Validação

A API utiliza **FluentValidation** para validação de entrada.

Exemplos:

* valor maior que zero
* campos obrigatórios

As regras de negócio permanecem no **Domain**.

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

### RabbitMQ Management

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

Validam o comportamento da aplicação completa:

* autenticação
* autorização
* regras de negócio
* persistência

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
* Dead Letter Queues
* Idempotência de consumidores
* Rate limiting
* Autenticação real com Identity
* Circuit breaker para integrações externas

---

# Autor

Projeto desenvolvido como solução para desafio técnico backend.
