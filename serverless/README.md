# Notifications serverless

Esta pasta contem a configuracao de infraestrutura como codigo da funcao serverless da Fase 3.

## Objetivo

Substituir o container continuo da NotificationsAPI por uma funcao acionada por mensagens RabbitMQ.

## Funcao

Projeto:

```text
src/Fcg.Notifications.Function
```

Handler:

```text
Fcg.Notifications.Function::Fcg.Notifications.Function.NotificationFunction::HandleAsync
```

Eventos processados:

- `UserCreatedEvent`
- `PaymentProcessedEvent`

Filas esperadas:

- `notifications-user-created`
- `notifications-payment-processed`

## Execucao local simulada

Usuario criado:

```powershell
dotnet run --project src\Fcg.Notifications.Function -- --event samples\user-created-function-event.json
```

Pagamento processado:

```powershell
dotnet run --project src\Fcg.Notifications.Function -- --event samples\payment-processed-function-event.json
```

## Deploy com AWS SAM

Exemplo:

```powershell
sam build --template-file serverless\template.yaml
sam deploy --guided --template-file serverless\template.yaml
```

Parametros necessarios:

- `RabbitMqBrokerArn`: ARN do broker Amazon MQ for RabbitMQ.
- `RabbitMqCredentialsSecretArn`: ARN do secret no AWS Secrets Manager com usuario e senha do RabbitMQ.
- `RabbitMqVirtualHost`: virtual host do RabbitMQ.
- `NotificationsFromEmail`: e-mail remetente usado nos logs simulados.

## Observacao

No ambiente Docker Compose local, a API HTTP antiga pode ser usada apenas como modo legado de validacao manual. Na Fase 3, o caminho principal de notificacoes e a funcao serverless.
