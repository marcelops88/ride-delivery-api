# ride-delivery-api

## Descrição

MotoHub é uma aplicação desenvolvida para gerenciar o aluguel de motos e entregadores, permitindo que entregadores registrados com uma locação ativa possam efetuar entregas de pedidos disponíveis na plataforma.

## Tecnologias Utilizadas

- **C#** / **.NET**
- **PostgreSQL** ou **MongoDB**
- Algum Sistema de Mensageria (RabbitMQ, SQS/SNS, Kafka, Google Pub/Sub)
- **Swagger** para documentação e testes de APIs
- **xUnit**, **FluentAssertions**, **NSubstitute** para testes

## Funcionalidades

- Gerenciamento de entregadores
  - Registro de entregadores
  - Atualização de status de locação
- Gerenciamento de motos
  - Registro de motos disponíveis para locação
  - Aluguel de motos para entregadores
- Gerenciamento de pedidos
  - Listagem de pedidos disponíveis para entrega
  - Atribuição de entregas a entregadores com locação ativa
- Sistema de Mensageria para comunicação entre os serviços

## Estrutura de API

A aplicação segue o padrão **RESTful** e os endpoints estão documentados conforme as especificações do Swagger fornecido. Abaixo estão as principais rotas:

### Endpoints:

- **Entregadores:**
  - `POST /entregadores`: Registrar novo entregador
  - `GET /entregadores`: Listar todos os entregadores
  - `PUT /entregadores/{id}`: Atualizar dados do entregador
  - `DELETE /entregadores/{id}`: Remover entregador

- **Motos:**
  - `POST /motos`: Registrar nova moto para locação
  - `GET /motos`: Listar todas as motos
  - `PUT /motos/{id}`: Atualizar status da moto (alugada/disponível)
  - `DELETE /motos/{id}`: Remover moto

- **Pedidos:**
  - `GET /pedidos`: Listar pedidos disponíveis para entrega
  - `PUT /pedidos/{id}`: Atribuir pedido a entregador com locação ativa

### Exemplo de Request/Response

```json
POST /entregadores
{
  "nome": "João Silva",
  "cpf": "123.456.789-00",
  "email": "joao.silva@example.com",
  "telefone": "(11) 99999-9999"
}
