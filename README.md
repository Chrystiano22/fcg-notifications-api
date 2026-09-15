# FCG Notifications

Repositorio responsavel por notificacoes simuladas por log no console.

Na Fase 3, o fluxo principal foi migrado para uma funcao serverless acionada por mensagens RabbitMQ. A API HTTP foi mantida apenas como modo legado de validacao local.

## Responsabilidades

- Processar `UserCreatedEvent` por funcao serverless.
- Log de e-mail de boas-vindas.
- Processar `PaymentProcessedEvent` por funcao serverless.
- Log de e-mail de confirmacao de compra aprovada.
- Ignorar pagamento rejeitado com resposta clara.

## Funcao serverless

Projeto:

```text
src/Fcg.Notifications.Function
```

Handler:

```text
Fcg.Notifications.Function::Fcg.Notifications.Function.NotificationFunction::HandleAsync
```

Infraestrutura como codigo:

```text
serverless/template.yaml
```

Simular evento de usuario criado:

```powershell
dotnet run --project src\Fcg.Notifications.Function -- --event samples\user-created-function-event.json
```

Simular evento de pagamento processado:

```powershell
dotnet run --project src\Fcg.Notifications.Function -- --event samples\payment-processed-function-event.json
```

## API HTTP legada para validacao local

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

Validar explicitamente a funcao serverless:

```powershell
dotnet build src\Fcg.Notifications.Function\Fcg.Notifications.Function.csproj
dotnet run --project src\Fcg.Notifications.Function -- --event samples\user-created-function-event.json
dotnet run --project src\Fcg.Notifications.Function -- --event samples\payment-processed-function-event.json
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
 
Na funcao serverless, `RabbitMq__Enabled` deve ficar `false`, pois o trigger do provedor cloud entrega a mensagem diretamente para o handler.

## Mensageria

- Fila esperada para usuario criado: `notifications-user-created`
- Fila esperada para pagamento processado: `notifications-payment-processed`
- Serializacao com broker: raw JSON, para interoperar com contratos duplicados entre repositorios.
- Retry e escala passam a ser responsabilidade do trigger serverless configurado em `serverless/template.yaml`.
