[Back to README](../README.md)

### Sales

#### GET /sales
- Description: Retrieve a paginated list of sales
- Query Parameters:
  - `_page` (optional): Page number for pagination (default: 1)
  - `_size` (optional): Number of items per page (default: 10)
  - `_order` (optional): Ordering of results (e.g., "totalAmount desc")
  - Filters: any additional query parameters are treated as filters (see General API)
- Response:
  ```json
  {
    "success": true,
    "message": "Sales retrieved successfully",
    "data": {
      "data": [
        {
          "id": "guid",
          "saleNumber": "string",
          "saleDate": "string (date-time)",
          "customerId": "guid",
          "customerName": "string",
          "branchId": "guid",
          "branchName": "string",
          "totalAmount": "number",
          "isCancelled": "boolean"
        }
      ],
      "totalItems": "integer",
      "currentPage": "integer",
      "totalPages": "integer"
    }
  }
  ```

#### GET /sales/{id}
- Description: Retrieve a specific sale by ID
- Path Parameters:
  - `id`: Sale ID
- Response:
  ```json
  {
    "success": true,
    "message": "Sale retrieved successfully",
    "data": {
      "id": "guid",
      "saleNumber": "string",
      "saleDate": "string (date-time)",
      "customerId": "guid",
      "customerName": "string",
      "branchId": "guid",
      "branchName": "string",
      "totalAmount": "number",
      "isCancelled": "boolean",
      "items": [
        {
          "id": "guid",
          "productId": "guid",
          "productName": "string",
          "quantity": "integer",
          "unitPrice": "number",
          "discount": "number",
          "totalAmount": "number",
          "isCancelled": "boolean"
        }
      ],
      "createdAt": "string (date-time)",
      "updatedAt": "string (date-time)"
    }
  }
  ```

#### POST /sales
- Description: Create a new sale
- Request Body:
  ```json
  {
    "saleNumber": "string",
    "saleDate": "string (date-time)",
    "customerId": "guid",
    "customerName": "string",
    "branchId": "guid",
    "branchName": "string",
    "items": [
      {
        "productId": "guid",
        "productName": "string",
        "quantity": "integer",
        "unitPrice": "number"
      }
    ]
  }
  ```
- Response:
  ```json
  {
    "success": true,
    "message": "Sale created successfully",
    "data": {
      "id": "guid",
      "saleNumber": "string",
      "saleDate": "string (date-time)",
      "customerId": "guid",
      "customerName": "string",
      "branchId": "guid",
      "branchName": "string",
      "totalAmount": "number",
      "items": [
        {
          "id": "guid",
          "productId": "guid",
          "productName": "string",
          "quantity": "integer",
          "unitPrice": "number",
          "discount": "number",
          "totalAmount": "number"
        }
      ]
    }
  }
  ```

#### PUT /sales/{id}
- Description: Update an existing sale
- Path Parameters:
  - `id`: Sale ID
- Request Body:
  ```json
  {
    "customerId": "guid",
    "customerName": "string",
    "branchId": "guid",
    "branchName": "string",
    "items": [
      {
        "id": "guid (optional)",
        "productId": "guid",
        "productName": "string",
        "quantity": "integer",
        "unitPrice": "number"
      }
    ]
  }
  ```
- Response:
  ```json
  {
    "success": true,
    "message": "Sale updated successfully",
    "data": {
      "id": "guid",
      "saleNumber": "string",
      "saleDate": "string (date-time)",
      "customerId": "guid",
      "customerName": "string",
      "branchId": "guid",
      "branchName": "string",
      "totalAmount": "number",
      "items": [
        {
          "id": "guid",
          "productId": "guid",
          "productName": "string",
          "quantity": "integer",
          "unitPrice": "number",
          "discount": "number",
          "totalAmount": "number"
        }
      ]
    }
  }
  ```

#### DELETE /sales/{id}
- Description: Delete a specific sale by ID
- Path Parameters:
  - `id`: Sale ID
- Response:
  ```json
  {
    "success": true,
    "message": "Sale deleted successfully"
  }
  ```

#### PATCH /sales/{id}/cancel
- Description: Cancel an entire sale
- Path Parameters:
  - `id`: Sale ID
- Response:
  ```json
  {
    "success": true,
    "message": "Sale cancelled successfully",
    "data": {
      "success": true,
      "saleId": "guid",
      "saleNumber": "string"
    }
  }
  ```

#### PATCH /sales/{id}/items/{itemId}/cancel
- Description: Cancel a specific item in a sale
- Path Parameters:
  - `id`: Sale ID
  - `itemId`: Sale item ID
- Response:
  ```json
  {
    "success": true,
    "message": "Sale item cancelled successfully",
    "data": {
      "success": true,
      "saleId": "guid",
      "itemId": "guid",
      "productName": "string"
    }
  }
  ```

<br>
<div style="display: flex; justify-content: space-between;">
  <a href="./users-api.md">Previous: Users API</a>
  <a href="./auth-api.md">Next: Auth API</a>
</div>
