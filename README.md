# ride-delivery-api

## Descrição

RideAndDeliver é uma aplicação desenvolvida para gerenciar o aluguel de motos e entregadores, permitindo que entregadores registrados com uma locação ativa possam efetuar entregas de pedidos disponíveis na plataforma.

## Tecnologias Utilizadas

- **C#** / **.NET**
- **PostgreSQL** ou **MongoDB**
- Algum Sistema de Mensageria (RabbitMQ, SQS/SNS, Kafka, Google Pub/Sub)
- **Swagger** para documentação e testes de APIs
- **xUnit**, **FluentAssertions**, **NSubstitute** para testes

## Funcionalidades

- **Motos**
  - Cadastro de novas motos
  - Modificação da placa de motos
  - Remoção de motos
  - Consulta de motos por ID
- **Entregadores**
  - Registro de novos entregadores
  - Envio da CNH dos entregadores
- **Locação**
  - Aluguel de motos
  - Consulta de locações por ID
  - Devolução de motos e cálculo de valor

## Estrutura de API

A aplicação segue o padrão **RESTful** e os endpoints estão documentados conforme as especificações do Swagger fornecido. Abaixo estão as principais rotas:

### Endpoints:

#### **Motos**
- `POST /motos`: Cadastrar uma nova moto
- `GET /motos`: Consultar motos existentes
- `PUT /motos/{id}/placa`: Modificar a placa de uma moto
- `GET /motos/{id}`: Consultar moto existente por ID
- `DELETE /motos/{id}`: Remover uma moto

#### **Entregadores**
- `POST /entregadores`: Cadastrar entregador
- `POST /entregadores/{id}/cnh`: Enviar foto da CNH do entregador

#### **Locação**
- `POST /locacao`: Alugar uma moto
- `GET /locacao/{id}`: Consultar locação por ID
- `PUT /locacao/{id}/devolucao`: Informar data de devolução e calcular valor de aluguel

### Exemplo de Request/Response

```json
POST /motos
{
  "modelo": "Honda CB500",
  "placa": "ABC-1234",
  "cor": "Vermelho"
}
