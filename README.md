# Music Master Shop API

## Descrição

A **Music Master Shop API** é uma API REST para gerenciamento de uma loja de instrumentos musicais.

Ela permite:

- cadastrar e autenticar usuários;
- consultar categorias de instrumentos;
- cadastrar, atualizar, consultar, listar e excluir produtos;
- controlar a quantidade disponível em estoque;
- criar e atualizar o carrinho de um vendedor;
- converter um carrinho válido em pedido;
- consultar os dados de um pedido.

A aplicação utiliza **.NET 10**, ASP.NET Core, Entity Framework Core, MediatR, FluentValidation e autenticação JWT.
Podendo ser executada via Docker.

> Atualmente, a API utiliza um banco de dados **InMemory** chamado `test`. Os dados são descartados quando a aplicação é encerrada.

## Arquitetura

O projeto segue os princípios da **Clean Architecture**, separando responsabilidades em camadas:

```text
src/MusicMasterShop/
├── Domain/
│   └── MusicMasterShop.Domain
├── Application/
│   └── MusicMasterShop.Application
├── Infrastructure/
│   └── MusicMasterShop.InfraData
├── Presentation/
│   └── MusicMasterShop.WebApi
└── Test/
    └── MusicMasterShop.Test
```

### Domain

Contém as regras e os modelos centrais do negócio:

- entidades;
- objetos de valor;
- enums;
- contratos dos repositórios;
- resultados e paginação.

### Application

Coordena os casos de uso da aplicação:

- commands e queries;
- handlers;
- validadores;
- geração de JWT;
- acesso às informações do usuário autenticado;
- abstrações de resposta.

### Infrastructure

Implementa os contratos definidos pelo domínio:

- contexto do Entity Framework Core;
- repositórios;
- persistência;
- Unit of Work.

### Presentation

Expõe os recursos por HTTP:

- controllers;
- autenticação e autorização;
- versionamento;
- Swagger;
- CORS;
- configuração da aplicação.

### Test

Contém testes unitários com:

- xUnit;
- Moq;
- FluentValidation TestHelper;
- Builder Pattern para criação dos objetos utilizados nos testes.

## Design Patterns

### CQRS

As operações são separadas entre:

- **Commands**, responsáveis por alterar o estado da aplicação;
- **Queries**, responsáveis por consultar dados.

### Mediator

Os controllers enviam requests por meio do MediatR. O handler correspondente executa a regra de negócio, reduzindo o acoplamento entre a camada HTTP e a aplicação.

### Repository

O acesso aos dados é abstraído por interfaces como:

- `IUsuarioRepository`;
- `IProdutoRepository`;
- `ICategoriaRepository`;
- `ICarrinhoRepository`;
- `IPedidoRepository`.

### Unit of Work

O `IUnitOfWork` centraliza a confirmação das alterações por meio de `CommitAsync`.

### Result Pattern

Os handlers retornam `BaseResponse<T>`, contendo o resultado de sucesso ou os erros de negócio. O `ApiControllerBase` converte esse resultado para o status HTTP correspondente.

### Middleware

O `CorrelationIdMiddleware` trata o identificador de rastreamento de cada requisição.

### Dependency Injection

Handlers, serviços, repositórios, validadores e demais dependências são registrados no container nativo do ASP.NET Core.

### Builder Pattern

Os testes utilizam builders para gerar requests, entidades e respostas de maneira legível e reutilizável.

## Autenticação JWT

A API utiliza **JSON Web Token (JWT)** com assinatura HMAC SHA-256.

O token contém informações como:

- identificador do usuário;
- e-mail;
- nome;
- perfil do usuário;
- identificador único do token;
- data de expiração.

Por padrão, o token expira após **60 minutos**.

### Perfis de acesso

| Valor | Perfil | Permissões principais |
|---:|---|---|
| `1` | `Administrador` | Categorias e gerenciamento de produtos |
| `2` | `Vendedor` | Carrinho e pedidos |

O fluxo completo requer a criação de **dois usuários**:

1. um administrador para gerenciar produtos;
2. um vendedor para criar carrinhos e pedidos.

Depois do login, envie o token nas rotas protegidas:

```http
Authorization: Bearer SEU_TOKEN_JWT
```

### Respostas relacionadas à autenticação

- `401 Unauthorized`: token ausente, inválido ou expirado; credenciais incorretas; ou categoria inexistente durante a criação de produto, conforme a regra atualmente implementada.
- `403 Forbidden`: usuário autenticado, mas sem o perfil exigido, ou tentativa de acessar carrinho/pedido pertencente a outro vendedor.

## Correlation ID

Cada requisição processada pelo middleware recebe um identificador de correlação no header:

```http
X-Correlation-Id: 9f13e931-1785-4a65-b5b2-8631ecadfcfe
```

O cliente pode enviar esse header. Caso ele não seja informado ou esteja vazio, a API gera um novo GUID.

O mesmo valor é:

- adicionado ao header da resposta;
- incluído no corpo das respostas normais de sucesso ou falha;
- utilizado para relacionar requisições, respostas e registros de diagnóstico.

Exemplo de sucesso:

```json
{
  "correlationId": "9f13e931-1785-4a65-b5b2-8631ecadfcfe",
  "data": {
    "id": "48de05a2-134d-4c50-af11-d0d91d64f356"
  }
}
```

Exemplo de falha:

```json
{
  "correlationId": "9f13e931-1785-4a65-b5b2-8631ecadfcfe",
  "errors": [
    {
      "detail": "Produto não encontrado"
    }
  ]
}
```
## Execução local

### Pré-requisitos

- .NET SDK 10;
- cliente HTTP, como Swagger, Postman, Insomnia ou cURL.

### Configuração JWT

A chave de teste está configurada em:

```text
src/MusicMasterShop/Presentation/MusicMasterShop.WebApi/appsettings.Development.json
```

### Executar sem Docker

```bash
dotnet restore src/MusicMasterShop/MusicMasterShop.slnx
dotnet build src/MusicMasterShop/MusicMasterShop.slnx
dotnet run --project src/MusicMasterShop/Presentation/MusicMasterShop.WebApi
```

### Executar com Docker

```bash
docker run src/MusicMasterShop/Presentation/MusicMasterShop.WebApi
```

## Formato e códigos de resposta

| Status | Significado |
|---:|---|
| `200 OK` | Operação concluída com sucesso |
| `400 Bad Request` | Request inválido ou regra de negócio não atendida |
| `401 Unauthorized` | Falha de autenticação ou credenciais inválidas |
| `403 Forbidden` | Perfil sem permissão ou recurso pertencente a outro usuário |
| `404 Not Found` | Recurso não encontrado |
| `500 Internal Server Error` | Erro interno não tratado |

As operações de criação retornam `200 OK`, e não `201 Created`, conforme o comportamento atual da API.

## Workflow da API

### 1. Criar usuário administrador

```http
POST /mmshop/v1/Usuario
Content-Type: application/json
```

```json
{
  "email": "admin@musicmaster.com",
  "nome": "Administrador",
  "tipoUsuario": 1,
  "senha": "admin123"
}
```

Regras para sucesso:

- `email` é obrigatório;
- o e-mail deve ser válido;
- o e-mail deve possuir no máximo 100 caracteres;
- `nome` é obrigatório;
- `tipoUsuario` é obrigatório;
- `senha` é obrigatória;
- a senha deve possuir pelo menos 6 caracteres.

Status possíveis:

- `200 OK`: usuário criado;
- `400 Bad Request`: dados inválidos;
- `500 Internal Server Error`: falha não tratada.

### 2. Criar usuário vendedor

```http
POST /mmshop/v1/Usuario
Content-Type: application/json
```

```json
{
  "email": "vendedor@musicmaster.com",
  "nome": "Vendedor",
  "tipoUsuario": 2,
  "senha": "venda123"
}
```

As mesmas validações da criação do administrador são aplicadas.

Guarde o `id` retornado caso seja necessário consultar o usuário posteriormente.

### 3. Login do administrador

```http
POST /mmshop/v1/Auth/login
Content-Type: application/json
```

```json
{
  "email": "admin@musicmaster.com",
  "senha": "admin123"
}
```

Regras para sucesso:

- e-mail obrigatório e válido;
- senha obrigatória;
- usuário existente e ativo;
- senha correta.

Status possíveis:

- `200 OK`: token gerado;
- `400 Bad Request`: request inválido;
- `401 Unauthorized`: e-mail ou senha inválidos;
- `500 Internal Server Error`: falha não tratada.

Guarde `data.token` como `ADMIN_TOKEN`.

### 4. Login do vendedor

```http
POST /mmshop/v1/Auth/login
Content-Type: application/json
```

```json
{
  "email": "vendedor@musicmaster.com",
  "senha": "venda123"
}
```

Guarde `data.token` como `VENDEDOR_TOKEN`.

### 5. Listar categorias

```http
GET /mmshop/v1/Produto/categorias
Authorization: Bearer ADMIN_TOKEN
```

As categorias são carregadas automaticamente na inicialização:

| `tipoCategoriaId` | Categoria |
|---:|---|
| `1` | Instrumento de Corda |
| `2` | Instrumento de Sopro |
| `3` | Instrumento de Percussão |
| `4` | Instrumento de Tecla |

Regras para sucesso:

- token JWT válido;
- usuário com perfil `Administrador`.

Status possíveis:

- `200 OK`: categorias retornadas;
- `401 Unauthorized`: token ausente, inválido ou expirado;
- `403 Forbidden`: usuário não administrador;
- `500 Internal Server Error`: falha não tratada.

Use o valor de `tipoCategoriaId` na criação do produto.

### 6. Criar produto

```http
POST /mmshop/v1/Produto
Authorization: Bearer ADMIN_TOKEN
Content-Type: application/json
```

```json
{
  "nome": "Guitarra Elétrica",
  "descricao": "Guitarra de seis cordas",
  "preco": 2499.90,
  "tipoCategoriaId": 1
}
```

Regras para sucesso:

- usuário administrador;
- `nome` obrigatório;
- `descricao` obrigatória;
- `preco` maior que zero;
- `tipoCategoriaId` obrigatório;
- categoria existente.

Status possíveis:

- `200 OK`: produto criado;
- `400 Bad Request`: campos inválidos;
- `401 Unauthorized`: token inválido ou categoria inexistente, conforme a implementação atual;
- `403 Forbidden`: usuário não administrador;
- `500 Internal Server Error`: falha não tratada.

Guarde `id` retornado como `PRODUTO_ID`.

### 7. Atualizar produto

```http
PUT /mmshop/v1/Produto/PRODUTO_ID
Authorization: Bearer ADMIN_TOKEN
Content-Type: application/json
```

```json
{
  "nome": "Guitarra Elétrica Premium",
  "descricao": "Guitarra atualizada",
  "preco": 2799.90,
  "tipoCategoriaId": 1
}
```

Regras para sucesso:

- usuário administrador;
- `PRODUTO_ID` deve ser um GUID válido e existente;
- `nome` e `descricao`, quando informados, não podem ser vazios;
- `tipoCategoriaId`, quando informado, deve ser válido e existente;
- valores nulos preservam os dados atuais;
- `preco` igual a zero preserva o preço atual;
- preço positivo substitui o valor atual.

Status possíveis:

- `200 OK`: produto atualizado;
- `400 Bad Request`: request inválido;
- `401 Unauthorized`: token ausente, inválido ou expirado;
- `403 Forbidden`: usuário não administrador;
- `404 Not Found`: produto ou categoria não encontrado;
- `500 Internal Server Error`: falha não tratada.

### 8. Atualizar estoque

```http
PATCH /mmshop/v1/Produto/PRODUTO_ID/atualizar-estoque
Authorization: Bearer ADMIN_TOKEN
Content-Type: application/json
```

```json
{
  "quantidade": 10,
  "numeroNotaFiscal": "NF-2026-0001"
}
```

Regras para sucesso:

- usuário administrador;
- produto existente;
- `quantidade` maior ou igual a zero;
- `numeroNotaFiscal` obrigatório.

Status possíveis:

- `200 OK`: estoque e nota fiscal atualizados;
- `400 Bad Request`: quantidade negativa ou nota fiscal vazia;
- `401 Unauthorized`: token ausente, inválido ou expirado;
- `403 Forbidden`: usuário não administrador;
- `404 Not Found`: produto não encontrado;
- `500 Internal Server Error`: falha não tratada.

O estoque deve ser maior ou igual à quantidade que será adicionada ao carrinho para que o pedido possa ser criado.

### 9. Consultar produto por ID

```http
GET /mmshop/v1/Produto/PRODUTO_ID
Authorization: Bearer ADMIN_TOKEN
```

Regras para sucesso:

- usuário administrador;
- produto existente.

Status possíveis:

- `200 OK`: produto retornado;
- `401 Unauthorized`: token ausente, inválido ou expirado;
- `403 Forbidden`: usuário não administrador;
- `404 Not Found`: produto não encontrado;
- `500 Internal Server Error`: falha não tratada.

### 10. Listar produtos

```http
GET /mmshop/v1/Produto?pageNumber=1&pageSize=10
Authorization: Bearer ADMIN_TOKEN
```

Regras para sucesso:

- usuário administrador;
- `pageNumber` maior que zero;
- `pageSize` entre 1 e 100.

Status possíveis:

- `200 OK`: página de produtos retornada;
- `400 Bad Request`: paginação inválida;
- `401 Unauthorized`: token ausente, inválido ou expirado;
- `403 Forbidden`: usuário não administrador;
- `500 Internal Server Error`: falha não tratada.

A resposta contém:

- itens;
- página atual;
- tamanho da página;
- total de registros;
- total de páginas;
- indicação de página anterior e próxima.

### 11. Excluir produto

Execute esta operação apenas depois de concluir os exemplos de carrinho e pedido, caso pretenda usar o mesmo produto.

```http
DELETE /mmshop/v1/Produto/PRODUTO_ID
Authorization: Bearer ADMIN_TOKEN
```

Regras para sucesso:

- usuário administrador;
- produto existente.

Status possíveis:

- `200 OK`: produto excluído;
- `401 Unauthorized`: token ausente, inválido ou expirado;
- `403 Forbidden`: usuário não administrador;
- `404 Not Found`: produto não encontrado;
- `500 Internal Server Error`: falha não tratada.

> No workflow atual, faça a exclusão ao final. Pois carrinho exige que o produto ainda exista.

### 12. Criar carrinho

Use o token do vendedor.

```http
POST /mmshop/v1/Pedido/criar-carrinho
Authorization: Bearer VENDEDOR_TOKEN
Content-Type: application/json
```

```json
{
  "quantidade": 2,
  "produtoId": "PRODUTO_ID"
}
```

Regras para sucesso:

- usuário autenticado com perfil `Vendedor`;
- vendedor existente e ativo;
- `quantidade` maior que zero;
- `produtoId` obrigatório;
- produto existente.

Se o vendedor não possuir carrinho ativo, um novo carrinho será criado. Se já existir um carrinho ativo, esse carrinho retornará e o produto selecionado será incluído ou terá sua quantidade atualizada.

Status possíveis:

- `200 OK`: carrinho criado ou atualizado;
- `400 Bad Request`: request inválido;
- `401 Unauthorized`: token ausente, inválido ou expirado;
- `403 Forbidden`: usuário não vendedor;
- `404 Not Found`: produto ou vendedor não encontrado;
- `500 Internal Server Error`: falha não tratada.

Guarde `id` como `CARRINHO_ID`.

### 13. Atualizar carrinho

```http
PUT /mmshop/v1/Pedido/atualizar-carrinho/CARRINHO_ID
Authorization: Bearer VENDEDOR_TOKEN
Content-Type: application/json
```

```json
{
  "produtos": [
    {
      "produtoId": "PRODUTO_ID",
      "quantidade": 3
    }
  ]
}
```

Regras para sucesso:

- usuário vendedor;
- carrinho existente;
- carrinho pertencente ao vendedor autenticado;
- carrinho ativo;
- lista `produtos` obrigatória e não vazia;
- cada `produtoId` deve ser um GUID não vazio;
- cada quantidade deve ser maior ou igual a zero;
- produtos com quantidade maior que zero devem existir.

Comportamento das quantidades:

- `0`: remove o produto do carrinho;
- maior que `0`: adiciona o produto ou substitui sua quantidade no carrinho.

Status possíveis:

- `200 OK`: carrinho atualizado;
- `400 Bad Request`: request inválido ou carrinho inativo;
- `401 Unauthorized`: token ausente, inválido ou expirado;
- `403 Forbidden`: usuário não vendedor ou carrinho pertencente a outro vendedor;
- `404 Not Found`: carrinho ou produto não encontrado;
- `500 Internal Server Error`: falha não tratada.

### 14. Consultar carrinho ativo

```http
GET /mmshop/v1/Pedido/obter-carrinho
Authorization: Bearer VENDEDOR_TOKEN
```

Status possíveis:

- `200 OK`: carrinho ativo retornado;
- `401 Unauthorized`: token ausente, inválido ou expirado;
- `403 Forbidden`: usuário não vendedor;
- `404 Not Found`: carrinho ativo não encontrado;
- `500 Internal Server Error`: falha não tratada.

### 15. Criar pedido

```http
POST /mmshop/v1/Pedido
Authorization: Bearer VENDEDOR_TOKEN
Content-Type: application/json
```

```json
{
  "carrinhoId": "CARRINHO_ID",
  "documentoCliente": "12345678901"
}
```

Regras para sucesso:

- usuário vendedor;
- `carrinhoId` obrigatório;
- `documentoCliente` obrigatório;
- carrinho existente;
- carrinho pertencente ao vendedor autenticado;
- carrinho ativo;
- carrinho com pelo menos um produto;
- todas as quantidades maiores que zero;
- estoque suficiente para todos os itens.

Ao criar o pedido:

- o estoque dos produtos é reduzido;
- o carrinho é finalizado e fica inativo;
- o pedido é criado com status `Validado`.

Status possíveis:

- `200 OK`: pedido criado;
- `400 Bad Request`: request inválido, carrinho inativo ou vazio, quantidade inválida ou estoque insuficiente;
- `401 Unauthorized`: token ausente, inválido ou expirado;
- `403 Forbidden`: usuário não vendedor ou carrinho pertencente a outro vendedor;
- `404 Not Found`: carrinho não encontrado;
- `500 Internal Server Error`: falha não tratada.

Guarde `id` como `PEDIDO_ID`.

### 16. Consultar pedido

```http
GET /mmshop/v1/Pedido/PEDIDO_ID
Authorization: Bearer VENDEDOR_TOKEN
```

Regras para sucesso:

- usuário vendedor;
- pedido existente;
- pedido associado a um carrinho pertencente ao vendedor autenticado.

Status possíveis:

- `200 OK`: pedido retornado;
- `401 Unauthorized`: token ausente, inválido ou expirado;
- `403 Forbidden`: usuário não vendedor ou pedido pertencente a outro vendedor;
- `404 Not Found`: pedido não encontrado;
- `500 Internal Server Error`: falha não tratada.

## Testes

Para executar todos os testes:

```bash
dotnet test src/MusicMasterShop/MusicMasterShop.slnx
```

