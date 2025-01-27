# Integração com Mensageria - Cartão Seguro

![image](https://github.com/user-attachments/assets/0fb68047-06d3-4f70-bea7-daf607e546fa)

## Descrição do Projeto

Este projeto é uma aplicação de consumo e publicação de mensagens desenvolvida em .NET, utilizando Docker e Docker Compose para gerenciamento de recursos e orquestração da API e dos serviços. Ele é composto por dois serviços principais:

- **API de Usuários e Cartões**, que utiliza MongoDB como banco de dados não relacional para armazenamento flexível e eficiente.  
- **Notifier**, que utiliza o Kafka para consumo de mensagens, garantindo o envio de email com as informações necessárias.  

Este projeto demonstra uma arquitetura moderna e escalável, ideal para aplicações distribuídas, aproveitando o poder do Docker para simplificar a implantação, escalabilidade e manutenção dos serviços.

## Objetivo

O objetivo principal da aplicação é fornecer aos usuários a capacidade de bloquear ou desbloquear seus cartões de maneira segura e eficiente. Isso é feito por meio do envio de um token de confirmação para o email cadastrado do usuário. Após a validação do token, a ação de bloqueio ou desbloqueio do cartão será concluída, garantindo maior segurança e praticidade na gestão dos cartões.

## Estrutura do Projeto

A solução foi estruturada em 6 projetos principais, cada um com uma responsabilidade clara:

- **Domain**: Contém as entidades do domínio, interfaces de repositório e validações. Não depende de nenhuma outra camada, garantindo a independência de frameworks.
- **Application**: Implementa a lógica de negócios, regras de aplicação e publicador de mensagens. Foi utilizado os bons padrões de criação de requests, responses e o uso do AutoMapper para mapeamento entre as entidades
- **Infrastructure**: Camada responsável pelo acesso a dados e comunicação externa do banco de dados MongoDb. Implementa os repositórios e os contratos definidos na camada de domínio.
- **API**: Um módulo adicional que expõe os serviços por meio de uma API RESTful, utilizando **Swagger** para documentação, **JWT** para autenticação, e **Entity Framework** para gerenciamento de usuários.
- **Notifiers**: Serviço de consumidor de mensagens, que ao tratar a mensagem recebida envia um email para o usuário com o token solicitado que faz a confirmação do bloqueio ou desbloqueio do cartão.
- **Tests**: Testes unitários para meus serviços e controlladores.

## Tecnologias Utilizadas

- **.NET 8**: Framework principal utilizado para o desenvolvimento da aplicação.
- **ASP.NET Core Web API**: Camada de API para requisições via HTTP.
- **MongoDB**: Banco de dados não relacional.
- **AutoMapper**: Para mapeamento automático entre requests, responses e entidades do domínio.
- **Injeção de Dependência**: Configurada para garantir um código modular e testável.
- **Domain-Driven Design (DDD)**: Padrão para modelar o sistema baseado no domínio da aplicação.
- **Repository Pattern**: Para centralizar a lógica de acesso a dados e promover o encapsulamento.
- **Swagger**: Para documentar a API e facilitar a interação com os endpoints.
- **JWT (JSON Web Token)**: Para autenticação segura nas requisições da API.
- **Kafka**: Utilização da mensageria com pub/sub para tráfego de mensagens.
- **XUnit**: Para testes unitários dos serviços e controllers da aplicação.

## Instruções para Execução

1. **Clone o repositório**:
   ```bash
   git clone https://github.com/gustavorenedev/CartaoSeguro
   cd CartaoSeguro
   cd src
   ```

2. **Execute o Docker Compose**:
   ```bash
   docker-compose up --build
   ```
3. **Execute o projeto Notifier como uma nova instância em Debug para ver o fluxo com as mensagens**

4. **Acesse os serviços**:
   - **CartaoSeguroAPI**: `http://localhost:8080`  
   - **KafDrop**: `http://localhost:9007`  

5. **Documentação Swagger**: Utilize a interface Swagger para testar os endpoints disponíveis, acessando o endereço de cada API.

# Rotas da API

## 1. Auth

| Método | Rota                    | Descrição                               |
|--------|-------------------------|-----------------------------------------|
| POST   | `/api/Auth/Register`   | Registra um novo usuário.               |
| POST   | `/api/Auth/Login`      | Autentica um usuário e retorna um token.|

# Utilize a auth bearer do token gerado ao logar na aplicação

## 2. Card

| Método | Rota                            | Descrição                                           |
|--------|---------------------------------|---------------------------------------------------|
| POST   | `/api/Card/CreateCard`          | Registra um novo cartão.                          |
| GET    | `/api/Card/FindCardsByUser`     | Busca os cartões de um usuário pelo email.        |
| GET    | `/api/Card/FindCardById`        | Busca um cartão pelo ID.                          |
| POST   | `/api/Card/BlockOrActiveUserCard` | Solicita o bloqueio ou ativação de um cartão.    |
| POST   | `/api/Card/ConfirmedToken`      | Confirma o token recebido para bloqueio/desbloqueio. |

## 3. User

| Método | Rota                        | Descrição                               |
|--------|-----------------------------|-----------------------------------------|
| GET    | `/api/User/FindUserByEmail` | Busca informações de um usuário pelo email. |


## Conclusão

Este projeto exemplifica como uma clean architecture, modular e escalável ao utilizar tecnologias modernas como Docker, MongoDB, Kafka, API Rest. A separação de responsabilidades entre os serviços permite fácil manutenção e implementação de novas funcionalidades, enquanto o uso de containers garante uma implantação ágil e consistente.
