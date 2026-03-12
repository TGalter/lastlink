# Advance Requests API

API para gerenciamento de solicitações de adiantamento financeiro.

O projeto foi desenvolvido seguindo princípios de **Clean Architecture**, com foco em **regras de negócio claras**, **eventos de domínio** e **comunicação assíncrona via RabbitMQ** utilizando **Transactional Outbox Pattern**.

---

# Arquitetura

O projeto segue uma arquitetura em camadas:

* **Domain**
  Entidades, enums, eventos de domínio e regras de negócio.

* **Application**
  Casos de uso e orquestração das operações.

* **Infrastructure**
  Persistência, integração com RabbitMQ, outbox e worker.

* **Api**
  Endpoints HTTP, configuração da aplicação e middleware.

Estrutura de diretórios:

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

---

# Tecnologias utilizadas

* .NET 10
* ASP.NET Core
* Entity Framework Core
* PostgreSQL
* RabbitMQ
* Docker / Docker Compose
* Testcontainers
* xUnit
* FluentAssertions
* FluentValidation

---

# Fluxo da aplicação

```
Client
   ↓
API (.NET)
   ↓
PostgreSQL
   ↓
Outbox Table
   ↓
Outbox Worker
   ↓
RabbitMQ Exchange
```

### Explicação

1. A API recebe uma solicitação de adiantamento.
2. A solicitação é salva no banco.
3. Um evento de domínio é salvo na tabela **Outbox**.
4. Um **Background Worker** lê a Outbox.
5. O evento é publicado no **RabbitMQ**.
6. A mensagem é marcada como processada.

Esse padrão garante **consistência entre banco e eventos**.

---

# Regras de negócio

* Valor mínimo da solicitação: **maior que 100**
* Taxa de adiantamento: **5%**
* Apenas **uma solicitação pendente por creator**
* Estados possíveis:

  * Pending
  * Approved
  * Rejected

---

# Endpoints principais

## Criar solicitação

POST `/api/v1/advance-requests`

Exemplo:

```json
{
  "creatorId": "11111111-1111-1111-1111-111111111111",
  "grossAmount": 1000
}
```

Resposta esperada:

```json
{
  "grossAmount": 1000,
  "feeAmount": 50,
  "netAmount": 950
}
```

---

## Simular cálculo

GET

```
/api/v1/advance-requests/simulate?amount=1000
```

Retorna:

```
grossAmount
feeAmount
netAmount
```

---

## Aprovar solicitação

POST

```
/api/v1/advance-requests/{id}/approve
```

---

## Rejeitar solicitação

POST

```
/api/v1/advance-requests/{id}/reject
```

---

# Executando o projeto

Pré-requisitos:

* Docker
* Docker Compose

Execute:

```
docker compose up --build
```

---

# Serviços disponíveis

API (Swagger)

```
http://localhost:8080/swagger
```

RabbitMQ Management

```
http://localhost:15672
```

Login:

```
guest
guest
```

---

# Testes

O projeto possui dois tipos de testes.
Os testes de integração também validam cenários de entrada inválida, como GUID vazio e valores menores ou iguais a zero.

## Testes unitários

Validam regras de negócio do domínio.

Executar:

```
dotnet test tests/AdvanceRequests.UnitTests
```

---

## Testes de integração

Validam a aplicação completa:

* API
* banco
* regras de negócio

Utilizam **Testcontainers** para subir:

* PostgreSQL
* RabbitMQ

Executar:

```
dotnet test tests/AdvanceRequests.IntegrationTests
```

---

# Decisões de arquitetura

### Clean Architecture

Separação clara entre domínio, aplicação e infraestrutura.

### Domain Events

Eventos disparados pelas entidades para registrar mudanças importantes.

### Transactional Outbox

Garante consistência entre gravação no banco e publicação de eventos.

### Background Worker

Responsável por publicar eventos da Outbox no RabbitMQ.

### Input validation

Foi utilizado FluentValidation na camada de API para validação de entrada, mantendo regras de negócio na camada de domínio.

---

# Melhorias futuras

Possíveis evoluções do projeto:

* Idempotência para consumidores de eventos
* Retry policy no Outbox Worker
* Observabilidade (OpenTelemetry)
* Métricas (Prometheus)
* Autenticação e autorização
* Rate limiting
* Dead Letter Queues
* Circuit breaker para integrações externas

---

# Autor

Projeto desenvolvido como solução para desafio técnico backend.
