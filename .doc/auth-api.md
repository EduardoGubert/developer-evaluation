[Back to README](../README.md)


### Authentication

#### POST /api/auth
- Description: Authenticate a user
- Request Body:
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```
- Response: 
  ```json
  {
    "success": true,
    "message": "Authentication successful",
    "data": {
      "token": "string",
      "email": "string",
      "name": "string",
      "role": "string"
    }
  }
  ```

<br/>
<div style="display: flex; justify-content: space-between;">
  <a href="./users-api.md">Previous: Users API</a>
  <a href="./project-structure.md">Next: Project Structure</a>
</div>
