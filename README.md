# FCG NotificationsAPI

Microsservico responsavel por notificacoes simuladas por log no console.

## Responsabilidades

- Receber `UserCreatedEvent` por endpoint local ou RabbitMQ/MassTransit.
- Log de e-mail de boas-vindas.
- Receber `PaymentProcessedEvent` por endpoint local ou RabbitMQ/MassTransit.
- Log de e-mail de confirmacao de compra aprovada.
- Ignorar pagamento rejeitado com resposta clara.

## Execucao local

```powershell
dotnet restore
dotnet run --project src/Fcg.Notifications.Api --launch-profile http
```

Swagger:

- `http://localhost:5104/swagger`

Health check:

- `http://localhost:5104/health`

Endpoints de validacao:

- `POST http://localhost:5104/notificacoes/usuario-criado`
- `POST http://localhost:5104/notificacoes/pagamento-processado`

Exemplo de usuario criado:

```json
{
  "usuarioId": "11111111-1111-1111-1111-111111111111",
  "nome": "Chrys",
  "email": "chrys@fcg.local",
  "criadoEm": "2026-07-13T00:00:00Z"
}
```

Exemplo de pagamento processado:

```json
{
  "pedidoId": "22222222-2222-2222-2222-222222222222",
  "usuarioId": "11111111-1111-1111-1111-111111111111",
  "jogoId": "33333333-3333-3333-3333-333333333333",
  "preco": 49.9,
  "status": "Approved",
  "processadoEm": "2026-07-13T00:00:00Z"
}
```

## Validacao

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build --no-restore
```

## Variaveis de ambiente

- `ASPNETCORE_ENVIRONMENT`
- `ASPNETCORE_URLS`
- `Notifications__FromEmail`
- `RabbitMq__Enabled`
- `RabbitMq__Host`
- `RabbitMq__VirtualHost`
- `RabbitMq__Username`
- `RabbitMq__Password`

## Mensageria

- Exchange consumido: `fcg.user-created`
- Fila consumidora: `notifications-user-created`
- Exchange consumido: `fcg.payment-processed`
- Fila consumidora: `notifications-payment-processed`
- Serializacao com broker: raw JSON, para interoperar com contratos duplicados entre repositorios.
- Retry dos consumers: `3` tentativas com intervalo de `5` segundos.
- Para usar RabbitMQ, configure `RabbitMq__Enabled=true`.
