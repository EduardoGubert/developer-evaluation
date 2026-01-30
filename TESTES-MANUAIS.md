
# Guia de Testes Manuais - Developer Evaluation

Este documento contém todos os testes manuais necessarios para validar que a API atende aos requisitos do teste tecnico.

**Base URL (local):** `http://localhost:5119` ou `https://localhost:7181`
**Swagger:** `http://localhost:5119/swagger` ou `https://localhost:7181/swagger`

---

## Pre-requisitos

1. **Docker Desktop** instalado e rodando
2. **.NET 8 SDK** instalado
3. **Subir as dependencias** (PostgreSQL, MongoDB, Redis):
   ```bash
   cd template/backend
   docker-compose up -d ambev.developerevaluation.database ambev.developerevaluation.nosql ambev.developerevaluation.cache
   ```
4. **Rodar as migrations** (aplica schema no PostgreSQL):
   ```bash
   cd template/backend/src/Ambev.DeveloperEvaluation.WebApi
   dotnet ef database update --project ../Ambev.DeveloperEvaluation.ORM
   ```
5. **Iniciar a API**:
   ```bash
   cd template/backend/src/Ambev.DeveloperEvaluation.WebApi
   dotnet run
   ```
6. **Rodar testes automatizados**:
   ```bash
   cd template/backend
   dotnet test
   ```
7. Acesse o **Swagger** em `http://localhost:5119/swagger`

> **Nota:** As portas do PostgreSQL, Redis e MongoDB no `appsettings.Development.json` podem diferir das portas padroes do docker-compose. Verifique se as portas mapeadas pelo Docker correspondem as configuradas no appsettings.

---

## 1. TESTES DA AUTH API

### 1.1 Criar usuario para autenticacao

Antes de testar login, crie um usuario:

**Endpoint:** `POST /api/users`
**Headers:** `Content-Type: application/json`
**Autenticacao:** Nenhuma (AllowAnonymous)

```json
{
  "email": "admin@teste.com",
  "username": "admin",
  "password": "Admin@123",
  "name": {
    "firstname": "Admin",
    "lastname": "Teste"
  },
  "address": {
    "city": "Sao Paulo",
    "street": "Rua Teste",
    "number": 100,
    "zipcode": "01000-000",
    "geolocation": {
      "lat": "-23.5505",
      "long": "-46.6333"
    }
  },
  "phone": "+55 11 99999-0000",
  "status": 1,
  "role": 3
}
```

**Verificar:**
- [ ] Status 201 Created
- [ ] Resposta contem `id`, `email`, `username`
- [ ] Objeto `name` com `firstname` e `lastname`
- [ ] Objeto `address` com campos aninhados
- [ ] Anotar o `id` retornado para uso posterior

---

### 1.2 Login (autenticacao)

**Endpoint:** `POST /api/auth`
**Headers:** `Content-Type: application/json`

```json
{
  "email": "admin@teste.com",
  "password": "Admin@123"
}
```

**Verificar:**
- [ ] Status 200 OK
- [ ] Resposta contem `token` (JWT)
- [ ] Copiar o token para usar nos proximos requests autenticados

---

### 1.3 Login com credenciais invalidas

**Endpoint:** `POST /api/auth`

```json
{
  "email": "admin@teste.com",
  "password": "SenhaErrada"
}
```

**Verificar:**
- [ ] Status 401 Unauthorized

---

## 2. TESTES DA USERS API

> **Importante:** Para todos os endpoints abaixo (exceto POST e GET), inclua o header:
> `Authorization: Bearer {token_jwt}`

### 2.1 Listar usuarios (paginacao)

**Endpoint:** `GET /api/users?_page=1&_size=10`

**Verificar:**
- [ ] Status 200 OK
- [ ] Resposta contem lista de usuarios
- [ ] Campos de paginacao presentes (totalItems/totalCount, currentPage, totalPages)

---

### 2.2 Listar usuarios (ordenacao)

**Endpoint:** `GET /api/users?_page=1&_size=10&_order=username asc`

**Verificar:**
- [ ] Status 200 OK
- [ ] Usuarios ordenados por username ascendente

---

### 2.3 Buscar usuario por ID

**Endpoint:** `GET /api/users/{id}`
(usar o ID retornado no teste 1.1)

**Verificar:**
- [ ] Status 200 OK
- [ ] Dados do usuario corretos
- [ ] Objetos aninhados `name` e `address` presentes

---

### 2.4 Atualizar usuario

**Endpoint:** `PUT /api/users/{id}`

```json
{
  "email": "admin@teste.com",
  "username": "admin_updated",
  "password": "Admin@456",
  "name": {
    "firstname": "Admin",
    "lastname": "Atualizado"
  },
  "address": {
    "city": "Rio de Janeiro",
    "street": "Rua Nova",
    "number": 200,
    "zipcode": "20000-000",
    "geolocation": {
      "lat": "-22.9068",
      "long": "-43.1729"
    }
  },
  "phone": "+55 21 88888-0000",
  "status": 1,
  "role": 3
}
```

**Verificar:**
- [ ] Status 200 OK
- [ ] Dados atualizados na resposta

---

### 2.5 Deletar usuario

> **Cuidado:** Nao delete o usuario que voce vai usar para autenticacao!

Crie um usuario extra para deletar:

**Endpoint:** `POST /api/users` (criar usuario temporario)

```json
{
  "email": "temp@teste.com",
  "username": "temp_user",
  "password": "Temp@123",
  "name": { "firstname": "Temp", "lastname": "User" },
  "address": {
    "city": "Teste",
    "street": "Rua X",
    "number": 1,
    "zipcode": "00000-000",
    "geolocation": { "lat": "0", "long": "0" }
  },
  "phone": "+55 11 00000-0000",
  "status": 1,
  "role": 1
}
```

Depois delete com: `DELETE /api/users/{id_do_temp}`

**Verificar:**
- [ ] Status 200 OK
- [ ] GET /api/users/{id_do_temp} retorna 404

---

### 2.6 Validacao de campos obrigatorios

**Endpoint:** `POST /api/users` com body vazio:

```json
{}
```

**Verificar:**
- [ ] Status 400 Bad Request
- [ ] Mensagem de erro com detalhes de validacao

---

## 3. TESTES DA PRODUCTS API

### 3.1 Criar produto

**Endpoint:** `POST /api/products`
**Headers:** `Authorization: Bearer {token}`

```json
{
  "title": "Notebook Dell Inspiron 15",
  "price": 3500.00,
  "description": "Notebook com processador Intel Core i7",
  "category": "electronics",
  "image": "https://example.com/notebook.jpg",
  "rating": {
    "rate": 4.5,
    "count": 150
  }
}
```

**Verificar:**
- [ ] Status 201 Created
- [ ] Resposta contem `id`, `title`, `price`
- [ ] Objeto `rating` com `rate` e `count`
- [ ] Anotar o `id` para uso posterior

---

### 3.2 Criar mais produtos (para testar paginacao e categorias)

Criar pelo menos mais 2 produtos com categorias diferentes:

```json
{
  "title": "Camiseta Nike Dry Fit",
  "price": 149.90,
  "description": "Camiseta esportiva de alta performance",
  "category": "clothing",
  "image": "https://example.com/camiseta.jpg",
  "rating": { "rate": 4.0, "count": 80 }
}
```

```json
{
  "title": "Teclado Mecanico Redragon",
  "price": 250.00,
  "description": "Teclado mecanico com switches blue",
  "category": "electronics",
  "image": "https://example.com/teclado.jpg",
  "rating": { "rate": 4.8, "count": 200 }
}
```

---

### 3.3 Listar produtos (paginacao)

**Endpoint:** `GET /api/products?_page=1&_size=2`

**Verificar:**
- [ ] Status 200 OK
- [ ] Retorna no maximo 2 produtos
- [ ] Campos de paginacao: `totalItems`, `currentPage`, `totalPages`

---

### 3.4 Listar produtos (ordenacao)

**Endpoint:** `GET /api/products?_page=1&_size=10&_order=price desc`

**Verificar:**
- [ ] Status 200 OK
- [ ] Produtos ordenados por preco decrescente

---

### 3.5 Buscar produto por ID

**Endpoint:** `GET /api/products/{id}`

**Verificar:**
- [ ] Status 200 OK
- [ ] Dados corretos com rating aninhado

---

### 3.6 Listar categorias

**Endpoint:** `GET /api/products/categories`

**Verificar:**
- [ ] Status 200 OK
- [ ] Retorna lista de categorias (ex: ["electronics", "clothing"])

---

### 3.7 Buscar produtos por categoria

**Endpoint:** `GET /api/products/category/electronics?_page=1&_size=10`

**Verificar:**
- [ ] Status 200 OK
- [ ] Todos os produtos retornados sao da categoria "electronics"

---

### 3.8 Atualizar produto

**Endpoint:** `PUT /api/products/{id}`

```json
{
  "title": "Notebook Dell Inspiron 15 (Atualizado)",
  "price": 3200.00,
  "description": "Notebook com desconto",
  "category": "electronics",
  "image": "https://example.com/notebook-v2.jpg",
  "rating": { "rate": 4.7, "count": 200 }
}
```

**Verificar:**
- [ ] Status 200 OK
- [ ] Dados atualizados na resposta

---

### 3.9 Deletar produto

**Endpoint:** `DELETE /api/products/{id}` (usar um produto que nao sera necessario)

**Verificar:**
- [ ] Status 200 OK
- [ ] GET /api/products/{id} retorna 404

---

## 4. TESTES DA CARTS API

### 4.1 Criar carrinho

> **Prerequisito:** Ter o `userId` (do usuario criado) e `productId` (dos produtos criados)

**Endpoint:** `POST /api/carts`
**Headers:** `Authorization: Bearer {token}`

```json
{
  "userId": "{id_do_usuario}",
  "date": "2026-01-30T10:00:00",
  "products": [
    {
      "productId": "{id_produto_1}",
      "quantity": 5
    },
    {
      "productId": "{id_produto_2}",
      "quantity": 2
    }
  ]
}
```

**Verificar:**
- [ ] Status 201 Created
- [ ] Resposta contem `id`, `userId`, `date`, `products`
- [ ] Products tem `productId` e `quantity`
- [ ] Anotar o `id` do carrinho

---

### 4.2 Listar carrinhos (paginacao)

**Endpoint:** `GET /api/carts?_page=1&_size=10`

**Verificar:**
- [ ] Status 200 OK
- [ ] Paginacao presente

---

### 4.3 Buscar carrinho por ID

**Endpoint:** `GET /api/carts/{id}`

**Verificar:**
- [ ] Status 200 OK
- [ ] Dados do carrinho com produtos

---

### 4.4 Atualizar carrinho

**Endpoint:** `PUT /api/carts/{id}`

```json
{
  "userId": "{id_do_usuario}",
  "date": "2026-01-30T12:00:00",
  "products": [
    {
      "productId": "{id_produto_1}",
      "quantity": 10
    }
  ]
}
```

**Verificar:**
- [ ] Status 200 OK
- [ ] Quantidade atualizada

---

### 4.5 Remover item do carrinho

**Endpoint:** `DELETE /api/carts/{cartId}/products/{productId}`

**Verificar:**
- [ ] Status 200 OK
- [ ] Produto removido do carrinho

---

### 4.6 Deletar carrinho

Crie um carrinho extra e delete:

**Endpoint:** `DELETE /api/carts/{id}`

**Verificar:**
- [ ] Status 200 OK

---

### 4.7 Carrinho com mais de 20 itens identicos (DEVE FALHAR)

**Endpoint:** `POST /api/carts`

```json
{
  "userId": "{id_do_usuario}",
  "date": "2026-01-30T10:00:00",
  "products": [
    {
      "productId": "{id_produto_1}",
      "quantity": 21
    }
  ]
}
```

**Verificar:**
- [ ] Status 400 Bad Request
- [ ] Mensagem de erro indicando limite de 20 itens

---

## 5. TESTES DA SALES API (REQUISITO PRINCIPAL)

Este e o requisito principal do teste. Todos os testes abaixo devem passar.

### 5.1 Criar venda - sem desconto (quantidade < 4)

**Endpoint:** `POST /api/sales`
**Headers:** `Authorization: Bearer {token}`, `Content-Type: application/json`

```json
{
  "saleNumber": "SALE-001",
  "saleDate": "2026-01-30T10:00:00",
  "customerId": "{id_do_usuario}",
  "customerName": "Admin Teste",
  "branchId": "11111111-1111-1111-1111-111111111111",
  "branchName": "Filial Centro SP",
  "items": [
    {
      "productId": "{id_produto}",
      "productName": "Notebook Dell Inspiron 15",
      "quantity": 3,
      "unitPrice": 100.00
    }
  ]
}
```

**Verificar:**
- [ ] Status 201 Created
- [ ] `success: true`
- [ ] Item com `discount: 0` (sem desconto para qty < 4)
- [ ] Item com `totalAmount: 300.00` (3 x 100 - 0% desconto)
- [ ] Sale `totalAmount: 300.00`
- [ ] Anotar o `id` da venda e o `id` do item

---

### 5.2 Criar venda - desconto 10% (quantidade 4-9)

**Endpoint:** `POST /api/sales`

```json
{
  "saleNumber": "SALE-002",
  "saleDate": "2026-01-30T11:00:00",
  "customerId": "{id_do_usuario}",
  "customerName": "Admin Teste",
  "branchId": "11111111-1111-1111-1111-111111111111",
  "branchName": "Filial Centro SP",
  "items": [
    {
      "productId": "{id_produto}",
      "productName": "Notebook Dell Inspiron 15",
      "quantity": 5,
      "unitPrice": 100.00
    }
  ]
}
```

**Verificar:**
- [ ] Status 201 Created
- [ ] Item com `discount: 0.10` (10%)
- [ ] Item com `totalAmount: 450.00` (5 x 100 = 500 - 10% = 450)
- [ ] Sale `totalAmount: 450.00`

---

### 5.3 Criar venda - desconto 20% (quantidade 10-20)

**Endpoint:** `POST /api/sales`

```json
{
  "saleNumber": "SALE-003",
  "saleDate": "2026-01-30T12:00:00",
  "customerId": "{id_do_usuario}",
  "customerName": "Admin Teste",
  "branchId": "11111111-1111-1111-1111-111111111111",
  "branchName": "Filial Centro SP",
  "items": [
    {
      "productId": "{id_produto}",
      "productName": "Notebook Dell Inspiron 15",
      "quantity": 15,
      "unitPrice": 100.00
    }
  ]
}
```

**Verificar:**
- [ ] Status 201 Created
- [ ] Item com `discount: 0.20` (20%)
- [ ] Item com `totalAmount: 1200.00` (15 x 100 = 1500 - 20% = 1200)
- [ ] Sale `totalAmount: 1200.00`

---

### 5.4 Criar venda - limite de 20 itens (quantidade = 20)

**Endpoint:** `POST /api/sales`

```json
{
  "saleNumber": "SALE-004",
  "saleDate": "2026-01-30T13:00:00",
  "customerId": "{id_do_usuario}",
  "customerName": "Admin Teste",
  "branchId": "11111111-1111-1111-1111-111111111111",
  "branchName": "Filial Centro SP",
  "items": [
    {
      "productId": "{id_produto}",
      "productName": "Notebook Dell Inspiron 15",
      "quantity": 20,
      "unitPrice": 100.00
    }
  ]
}
```

**Verificar:**
- [ ] Status 201 Created
- [ ] Item com `discount: 0.20` (20%)
- [ ] Item com `totalAmount: 1600.00` (20 x 100 = 2000 - 20% = 1600)

---

### 5.5 Criar venda - acima de 20 itens (DEVE FALHAR)

**Endpoint:** `POST /api/sales`

```json
{
  "saleNumber": "SALE-005-FAIL",
  "saleDate": "2026-01-30T14:00:00",
  "customerId": "{id_do_usuario}",
  "customerName": "Admin Teste",
  "branchId": "11111111-1111-1111-1111-111111111111",
  "branchName": "Filial Centro SP",
  "items": [
    {
      "productId": "{id_produto}",
      "productName": "Notebook Dell Inspiron 15",
      "quantity": 21,
      "unitPrice": 100.00
    }
  ]
}
```

**Verificar:**
- [ ] Status 400 Bad Request
- [ ] Mensagem de erro indicando que nao e possivel vender mais de 20 itens

---

### 5.6 Criar venda - multiplos itens com descontos diferentes

**Endpoint:** `POST /api/sales`

```json
{
  "saleNumber": "SALE-006",
  "saleDate": "2026-01-30T15:00:00",
  "customerId": "{id_do_usuario}",
  "customerName": "Admin Teste",
  "branchId": "11111111-1111-1111-1111-111111111111",
  "branchName": "Filial Centro SP",
  "items": [
    {
      "productId": "{id_produto_1}",
      "productName": "Notebook Dell",
      "quantity": 2,
      "unitPrice": 100.00
    },
    {
      "productId": "{id_produto_2}",
      "productName": "Teclado Mecanico",
      "quantity": 5,
      "unitPrice": 50.00
    },
    {
      "productId": "{id_produto_3}",
      "productName": "Mouse Gamer",
      "quantity": 12,
      "unitPrice": 30.00
    }
  ]
}
```

**Verificar:**
- [ ] Status 201 Created
- [ ] Item 1 (qty=2): discount=0, totalAmount=200.00
- [ ] Item 2 (qty=5): discount=0.10, totalAmount=225.00 (5x50=250 - 10%)
- [ ] Item 3 (qty=12): discount=0.20, totalAmount=288.00 (12x30=360 - 20%)
- [ ] Sale totalAmount = 200 + 225 + 288 = 713.00

---

### 5.7 Criar venda - SaleNumber duplicado (DEVE FALHAR)

**Endpoint:** `POST /api/sales`

```json
{
  "saleNumber": "SALE-001",
  "saleDate": "2026-01-30T16:00:00",
  "customerId": "{id_do_usuario}",
  "customerName": "Admin Teste",
  "branchId": "11111111-1111-1111-1111-111111111111",
  "branchName": "Filial Centro SP",
  "items": [
    {
      "productId": "{id_produto}",
      "productName": "Notebook Dell",
      "quantity": 1,
      "unitPrice": 100.00
    }
  ]
}
```

**Verificar:**
- [ ] Status 400 Bad Request (SaleNumber ja existe)

---

### 5.8 Buscar venda por ID

**Endpoint:** `GET /api/sales/{id}` (usar ID da SALE-001)

**Verificar:**
- [ ] Status 200 OK
- [ ] Todos os campos presentes: saleNumber, saleDate, customerId, customerName, branchId, branchName, totalAmount, isCancelled
- [ ] Items com: productId, productName, quantity, unitPrice, discount, totalAmount, isCancelled

---

### 5.9 Listar vendas (paginacao)

**Endpoint:** `GET /api/sales?_page=1&_size=3`

**Verificar:**
- [ ] Status 200 OK
- [ ] Retorna no maximo 3 vendas
- [ ] Campos de paginacao: totalItems, currentPage, totalPages

---

### 5.10 Listar vendas (ordenacao)

**Endpoint:** `GET /api/sales?_page=1&_size=10&_order=totalAmount desc`

**Verificar:**
- [ ] Status 200 OK
- [ ] Vendas ordenadas por totalAmount decrescente

---

### 5.11 Atualizar venda

**Endpoint:** `PUT /api/sales/{id}` (usar ID da SALE-001)

```json
{
  "customerId": "{id_do_usuario}",
  "customerName": "Admin Teste Atualizado",
  "branchId": "22222222-2222-2222-2222-222222222222",
  "branchName": "Filial Zona Sul",
  "items": [
    {
      "productId": "{id_produto}",
      "productName": "Notebook Dell Inspiron 15",
      "quantity": 4,
      "unitPrice": 120.00
    }
  ]
}
```

**Verificar:**
- [ ] Status 200 OK
- [ ] CustomerName atualizado
- [ ] BranchName atualizado
- [ ] Item recalculado: qty=4, discount=0.10 (10%), totalAmount=432.00 (4x120=480 - 10%)
- [ ] TotalAmount da venda recalculado

---

### 5.12 Cancelar item da venda

**Endpoint:** `PATCH /api/sales/{saleId}/items/{itemId}/cancel`

(Usar o ID de um item de uma venda com multiplos itens - SALE-006)

**Verificar:**
- [ ] Status 200 OK
- [ ] Resposta contem `success: true`, `saleId`, `itemId`, `productName`
- [ ] GET /api/sales/{saleId} mostra o item com `isCancelled: true`
- [ ] O totalAmount da venda foi recalculado (sem o item cancelado)

---

### 5.13 Cancelar venda inteira

**Endpoint:** `PATCH /api/sales/{id}/cancel` (usar ID da SALE-004)

**Verificar:**
- [ ] Status 200 OK
- [ ] Resposta contem `success: true`, `saleId`, `saleNumber`
- [ ] GET /api/sales/{id} mostra `isCancelled: true`

---

### 5.14 Cancelar venda ja cancelada (DEVE FALHAR)

**Endpoint:** `PATCH /api/sales/{id}/cancel` (mesmo ID da SALE-004, ja cancelada)

**Verificar:**
- [ ] Status 400 Bad Request
- [ ] Mensagem indicando que a venda ja esta cancelada

---

### 5.15 Atualizar venda cancelada (DEVE FALHAR)

**Endpoint:** `PUT /api/sales/{id}` (ID da SALE-004, ja cancelada)

```json
{
  "customerId": "{id_do_usuario}",
  "customerName": "Tentativa",
  "branchId": "11111111-1111-1111-1111-111111111111",
  "branchName": "Filial",
  "items": [
    {
      "productId": "{id_produto}",
      "productName": "Produto",
      "quantity": 1,
      "unitPrice": 10.00
    }
  ]
}
```

**Verificar:**
- [ ] Status 400 Bad Request
- [ ] Mensagem indicando que venda cancelada nao pode ser atualizada

---

### 5.16 Deletar venda

Crie uma venda temporaria (SALE-TEMP) e delete:

**Endpoint:** `DELETE /api/sales/{id}`

**Verificar:**
- [ ] Status 200 OK
- [ ] GET /api/sales/{id} retorna 404

---

### 5.17 Buscar venda inexistente

**Endpoint:** `GET /api/sales/00000000-0000-0000-0000-000000000000`

**Verificar:**
- [ ] Status 404 Not Found

---

### 5.18 Criar venda com campos obrigatorios faltando

**Endpoint:** `POST /api/sales`

```json
{
  "saleNumber": "",
  "saleDate": "2026-01-30T10:00:00",
  "customerId": "00000000-0000-0000-0000-000000000000",
  "customerName": "",
  "branchId": "00000000-0000-0000-0000-000000000000",
  "branchName": "",
  "items": []
}
```

**Verificar:**
- [ ] Status 400 Bad Request
- [ ] Erros de validacao para: SaleNumber, CustomerName, BranchName, Items

---

## 6. TESTES DE CHECKOUT (CARTS -> SALES)

### 6.1 Checkout do carrinho (cria venda automaticamente)

1. Crie um carrinho com produtos:

**Endpoint:** `POST /api/carts`

```json
{
  "userId": "{id_do_usuario}",
  "date": "2026-01-30T10:00:00",
  "products": [
    {
      "productId": "{id_produto_1}",
      "quantity": 6
    }
  ]
}
```

2. Faca o checkout:

**Endpoint:** `POST /api/carts/{cartId}/checkout`

```json
{
  "branchId": "33333333-3333-3333-3333-333333333333",
  "branchName": "Filial Online"
}
```

**Verificar:**
- [ ] Status 200 OK
- [ ] Resposta contem: `saleId`, `saleNumber`, `totalAmount`
- [ ] Items com desconto calculado (qty=6 -> 10% desconto)
- [ ] GET /api/carts/{cartId} retorna 404 (carrinho foi deletado apos checkout)
- [ ] GET /api/sales/{saleId} retorna a venda criada

---

## 7. TESTES DE EVENTOS (BONUS)

Os eventos sao armazenados no MongoDB. Para verificar:

### 7.1 Verificar evento SaleCreated

Apos criar uma venda (teste 5.1), verificar nos logs da aplicacao ou no MongoDB:

```bash
# Conectar ao MongoDB
docker exec -it ambev_developer_evaluation_nosql mongosh -u developer -p "ev@luAt10n" --authenticationDatabase admin

# No shell do mongo:
use developer_evaluation
db.audit_logs.find({ EventType: "SaleCreated" }).sort({ Timestamp: -1 }).limit(1).pretty()
```

**Verificar:**
- [ ] Evento registrado com EventType "SaleCreated"
- [ ] Data do evento (Timestamp) presente
- [ ] AggregateId corresponde ao ID da venda
- [ ] Data contem: SaleNumber, TotalAmount, CustomerId, CustomerName

---

### 7.2 Verificar evento SaleModified

Apos atualizar uma venda (teste 5.11):

```bash
db.audit_logs.find({ EventType: "SaleModified" }).sort({ Timestamp: -1 }).limit(1).pretty()
```

**Verificar:**
- [ ] Evento registrado com EventType "SaleModified"
- [ ] Data contem: PreviousTotalAmount, NewTotalAmount

---

### 7.3 Verificar evento SaleCancelled

Apos cancelar uma venda (teste 5.13):

```bash
db.audit_logs.find({ EventType: "SaleCancelled" }).sort({ Timestamp: -1 }).limit(1).pretty()
```

**Verificar:**
- [ ] Evento registrado com EventType "SaleCancelled"
- [ ] Data contem: SaleNumber, CancelledAt

---

### 7.4 Verificar evento ItemCancelled

Apos cancelar um item (teste 5.12):

```bash
db.audit_logs.find({ EventType: "ItemCancelled" }).sort({ Timestamp: -1 }).limit(1).pretty()
```

**Verificar:**
- [ ] Evento registrado com EventType "ItemCancelled"
- [ ] Data contem: SaleNumber, ItemId, ProductName

---

## 8. TESTES AUTOMATIZADOS

### 8.1 Rodar testes unitarios

```bash
cd template/backend
dotnet test --filter "FullyQualifiedName~Unit" --verbosity normal
```

**Verificar:**
- [ ] Todos os testes passam
- [ ] Testes de desconto (SaleItemTests):
  - Quantidade < 4: 0% desconto
  - Quantidade 4-9: 10% desconto
  - Quantidade 10-20: 20% desconto
- [ ] Testes de handler (CreateSale, UpdateSale, DeleteSale, GetSale, GetSales, CancelSale, CancelSaleItem)

---

### 8.2 Rodar testes de integracao

```bash
cd template/backend
dotnet test --filter "FullyQualifiedName~Integration" --verbosity normal
```

**Verificar:**
- [ ] Todos os testes passam
- [ ] Testes de integracao cobrem todas as regras de negocio

---

### 8.3 Rodar todos os testes

```bash
cd template/backend
dotnet test --verbosity normal
```

**Verificar:**
- [ ] Todos os testes passam sem erro

---

## 9. CHECKLIST FINAL - REQUISITOS DO TESTE

### Requisitos obrigatorios (Use Case):

| # | Requisito | Como verificar | Status |
|---|-----------|----------------|--------|
| 1 | Numero da venda (SaleNumber) | Testes 5.1-5.6: campo `saleNumber` na resposta | [ ] |
| 2 | Data da venda (SaleDate) | Testes 5.1-5.6: campo `saleDate` na resposta | [ ] |
| 3 | Cliente (Customer) | Testes 5.1-5.6: campos `customerId`, `customerName` | [ ] |
| 4 | Valor total (TotalAmount) | Testes 5.1-5.6: campo `totalAmount` calculado | [ ] |
| 5 | Filial (Branch) | Testes 5.1-5.6: campos `branchId`, `branchName` | [ ] |
| 6 | Produtos | Testes 5.1-5.6: `items[].productId`, `productName` | [ ] |
| 7 | Quantidades | Testes 5.1-5.6: `items[].quantity` | [ ] |
| 8 | Precos unitarios | Testes 5.1-5.6: `items[].unitPrice` | [ ] |
| 9 | Descontos | Testes 5.1-5.4: `items[].discount` | [ ] |
| 10 | Total por item | Testes 5.1-5.4: `items[].totalAmount` | [ ] |
| 11 | Cancelado/Nao cancelado | Testes 5.13: `isCancelled` | [ ] |

### Regras de negocio:

| # | Regra | Como verificar | Status |
|---|-------|----------------|--------|
| 1 | 4+ itens = 10% desconto | Teste 5.2: qty=5, discount=0.10 | [ ] |
| 2 | 10-20 itens = 20% desconto | Teste 5.3: qty=15, discount=0.20 | [ ] |
| 3 | >20 itens = ERRO | Teste 5.5: status 400 | [ ] |
| 4 | <4 itens = sem desconto | Teste 5.1: qty=3, discount=0 | [ ] |

### Bonus (eventos):

| # | Evento | Como verificar | Status |
|---|--------|----------------|--------|
| 1 | SaleCreated | Teste 7.1: MongoDB audit_logs | [ ] |
| 2 | SaleModified | Teste 7.2: MongoDB audit_logs | [ ] |
| 3 | SaleCancelled | Teste 7.3: MongoDB audit_logs | [ ] |
| 4 | ItemCancelled | Teste 7.4: MongoDB audit_logs | [ ] |

### Competencias tecnicas avaliadas:

| # | Competencia | Evidencia no projeto | Status |
|---|-------------|---------------------|--------|
| 1 | C# e .NET 8.0 | Todo o backend | [ ] |
| 2 | Separacao de camadas | Domain, Application, ORM, WebApi, Common, IoC | [ ] |
| 3 | PostgreSQL e MongoDB | DefaultContext + MongoEventStore | [ ] |
| 4 | Design Patterns (Mediator) | MediatR em todos os handlers | [ ] |
| 5 | EF Core | Repositories, Migrations, Configurations | [ ] |
| 6 | xUnit (testes) | Unit tests e Integration tests | [ ] |
| 7 | NSubstitute (mocking) | Todos os handler tests | [ ] |
| 8 | AutoMapper | Profiles em Application e WebApi | [ ] |
| 9 | RESTful API | Controllers com CRUD completo | [ ] |
| 10 | Git | Repositorio versionado, feature branch | [ ] |
| 11 | DB relacional + nao relacional | PostgreSQL + MongoDB | [ ] |
| 12 | Faker (Bogus) | TestData classes nos testes | [ ] |
| 13 | Organizacao de codigo | Estrutura limpa e organizada | [ ] |
| 14 | Paginacao, ordenacao | _page, _size, _order em todos os GETs | [ ] |
| 15 | Error handling | ValidationExceptionMiddleware | [ ] |
| 16 | Git Flow | Branch feature/bonus-improvements | [ ] |
| 17 | Performance | Redis cache nos produtos | [ ] |
| 18 | Async/await | Consistente em todo o codigo | [ ] |
| 19 | Code quality | Clean architecture, SOLID | [ ] |
| 20 | Business logic | Regras de desconto, validacoes | [ ] |

---

## 10. ORDEM RECOMENDADA DE EXECUCAO

1. Subir infraestrutura (docker-compose)
2. Aplicar migrations
3. Iniciar a API
4. **Rodar testes automatizados** (secao 8)
5. Criar usuario (teste 1.1)
6. Fazer login e obter token (teste 1.2)
7. Criar produtos (testes 3.1, 3.2)
8. Testar CRUD de Users (secao 2)
9. Testar CRUD de Products (secao 3)
10. Testar CRUD de Carts (secao 4)
11. **Testar CRUD de Sales** (secao 5 - MAIS IMPORTANTE)
12. Testar Checkout (secao 6)
13. Verificar eventos no MongoDB (secao 7)
14. Preencher checklist final (secao 9)
