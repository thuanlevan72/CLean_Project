# 📚 API Reference - Todo Management System

## 📖 Mục Lục

- [Authentication API](#authentication-api)
- [Todo Items API](#todo-items-api)
- [Categories API](#categories-api)
- [Tags API](#tags-api)
- [Admin API](#admin-api)
- [Status Codes & Errors](#status-codes--errors)
- [Rate Limiting](#rate-limiting)

---

## Authentication API

### 1. Register (Đăng Ký)

**Endpoint:**
```
POST /api/auth/register
Content-Type: application/json
```

**Request Body:**
```json
{
  "email": "user@example.com",
  "userName": "john_doe",
  "password": "SecurePassword123!"
}
```

**Validation Rules:**
- Email: Required, valid email format, unique
- UserName: Required, 3-50 characters, alphanumeric + underscore
- Password: Required, min 8 characters, must contain uppercase, lowercase, number, special char

**Response (201 Created):**
```json
{
  "statusCode": 201,
  "message": "User registered successfully",
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "email": "user@example.com",
    "userName": "john_doe",
    "createdAt": "2024-12-21T14:30:00Z"
  }
}
```

**Error Responses:**
```json
// 400 - Validation Error
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": {
    "email": ["Email is already in use"],
    "password": ["Password must be at least 8 characters"]
  }
}

// 409 - Conflict
{
  "statusCode": 409,
  "message": "Email already registered"
}
```

---

### 2. Login (Đăng Nhập)

**Endpoint:**
```
POST /api/auth/login
Content-Type: application/json
```

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1NTBlODQwMC1lMjliLTQxZDQtYTcxNi00NDY2NTU0NDAwMDAiLCJlbWFpbCI6InVzZXJAZXhhbXBsZS5jb20iLCJleHAiOjE3MDMxNzcwMDB9.signature",
    "expiresAt": "2024-12-21T15:30:00Z",
    "tokenType": "Bearer",
    "refreshToken": "550e8400-e29b-41d4-a716-446655440111",
    "refreshTokenExpiresAt": "2024-12-28T14:30:00Z",
    "user": {
      "userId": "550e8400-e29b-41d4-a716-446655440000",
      "email": "user@example.com",
      "userName": "john_doe"
    }
  }
}
```

**Error Responses:**
```json
// 401 - Invalid Credentials
{
  "statusCode": 401,
  "message": "Invalid email or password"
}

// 404 - User Not Found
{
  "statusCode": 404,
  "message": "User not found"
}
```

---

### 3. Refresh Token

**Endpoint:**
```
POST /api/auth/refresh-token
Content-Type: application/json
Authorization: Bearer <refresh_token>
```

**Request Body:**
```json
{
  "refreshToken": "550e8400-e29b-41d4-a716-446655440111"
}
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Token refreshed successfully",
  "data": {
    "token": "new_jwt_token_here",
    "expiresAt": "2024-12-22T15:30:00Z",
    "refreshToken": "new_refresh_token_here",
    "refreshTokenExpiresAt": "2024-12-29T14:30:00Z"
  }
}
```

---

### 4. Logout (Đăng Xuất)

**Endpoint:**
```
POST /api/auth/logout
Authorization: Bearer <your_jwt_token>
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Logged out successfully"
}
```

---

## Todo Items API

### 1. Create Todo (Tạo Todo)

**Endpoint:**
```
POST /api/todos
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "title": "Hoàn thành dự án",
  "description": "Cần hoàn thành dự án trước thứ 6",
  "priority": "High",
  "dueDate": "2024-12-31T23:59:59Z",
  "categoryId": "550e8400-e29b-41d4-a716-446655440001",
  "tagIds": [
    "550e8400-e29b-41d4-a716-446655440003",
    "550e8400-e29b-41d4-a716-446655440004"
  ],
  "parentTaskId": null,
  "estimatedHours": 8.5
}
```

**Fields Explanation:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| title | string | ✓ | Title (1-255 chars) |
| description | string | ✗ | Detailed description |
| priority | enum | ✓ | Low, Medium, High, Critical |
| dueDate | datetime | ✗ | ISO 8601 format |
| categoryId | guid | ✗ | Category reference |
| tagIds | array | ✗ | Tag IDs |
| parentTaskId | guid | ✗ | Parent task (for subtasks) |
| estimatedHours | decimal | ✗ | Estimated effort |

**Response (201 Created):**
```json
{
  "statusCode": 201,
  "message": "Todo created successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "title": "Hoàn thành dự án",
    "description": "Cần hoàn thành dự án trước thứ 6",
    "priority": 2,
    "priorityName": "High",
    "status": 0,
    "statusName": "Todo",
    "dueDate": "2024-12-31T23:59:59Z",
    "completedAt": null,
    "categoryId": "550e8400-e29b-41d4-a716-446655440001",
    "category": {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "name": "Work"
    },
    "tags": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440003",
        "name": "work"
      },
      {
        "id": "550e8400-e29b-41d4-a716-446655440004",
        "name": "urgent"
      }
    ],
    "subTasks": [],
    "estimatedHours": 8.5,
    "actualHours": null,
    "createdAt": "2024-12-21T14:30:00Z",
    "updatedAt": "2024-12-21T14:30:00Z"
  }
}
```

---

### 2. Get All Todos

**Endpoint:**
```
GET /api/todos?status=InProgress&priority=High&categoryId=...&skip=0&take=20&sortBy=dueDate&sortOrder=asc
Authorization: Bearer <your_jwt_token>
```

**Query Parameters:**
| Parameter | Type | Optional | Default | Description |
|-----------|------|----------|---------|-------------|
| status | enum | ✓ | All | Filter by status |
| priority | enum | ✓ | All | Filter by priority |
| categoryId | guid | ✓ | All | Filter by category |
| tagIds | array | ✓ | All | Filter by tags (OR logic) |
| search | string | ✓ | - | Search in title/description |
| skip | int | ✓ | 0 | Pagination offset |
| take | int | ✓ | 20 | Pagination limit (max 100) |
| sortBy | enum | ✓ | createdAt | dueDate, priority, status, createdAt, updatedAt |
| sortOrder | enum | ✓ | desc | asc, desc |
| includeCompleted | bool | ✓ | false | Include completed todos |

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Todos retrieved successfully",
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440002",
      "title": "Hoàn thành dự án",
      "priority": 2,
      "priorityName": "High",
      "status": 1,
      "statusName": "InProgress",
      "dueDate": "2024-12-31T23:59:59Z",
      "completedAt": null,
      "category": {
        "id": "550e8400-e29b-41d4-a716-446655440001",
        "name": "Work"
      },
      "tags": [
        {
          "id": "550e8400-e29b-41d4-a716-446655440003",
          "name": "work"
        }
      ],
      "createdAt": "2024-12-21T14:30:00Z",
      "updatedAt": "2024-12-21T15:00:00Z"
    }
  ],
  "pagination": {
    "skip": 0,
    "take": 20,
    "totalCount": 42,
    "pageCount": 3,
    "currentPage": 1
  }
}
```

---

### 3. Get Todo By ID

**Endpoint:**
```
GET /api/todos/{id}
Authorization: Bearer <your_jwt_token>
```

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | guid | Todo ID |

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Todo retrieved successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "title": "Hoàn thành dự án",
    "description": "Cần hoàn thành dự án trước thứ 6",
    "priority": 2,
    "priorityName": "High",
    "status": 1,
    "statusName": "InProgress",
    "dueDate": "2024-12-31T23:59:59Z",
    "completedAt": null,
    "categoryId": "550e8400-e29b-41d4-a716-446655440001",
    "category": {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "name": "Work"
    },
    "tags": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440003",
        "name": "work"
      }
    ],
    "subTasks": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440005",
        "title": "Subtask 1",
        "status": 0,
        "statusName": "Todo"
      }
    ],
    "estimatedHours": 8.5,
    "actualHours": 6.25,
    "attachments": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440006",
        "fileName": "project_spec.pdf",
        "fileSize": 1048576,
        "uploadedAt": "2024-12-21T14:30:00Z"
      }
    ],
    "createdAt": "2024-12-21T14:30:00Z",
    "updatedAt": "2024-12-21T15:00:00Z"
  }
}
```

**Error Responses:**
```json
// 404 - Not Found
{
  "statusCode": 404,
  "message": "Todo not found"
}

// 403 - Forbidden (Not owner)
{
  "statusCode": 403,
  "message": "You do not have permission to access this todo"
}
```

---

### 4. Update Todo

**Endpoint:**
```
PUT /api/todos/{id}
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "title": "Hoàn thành dự án - Updated",
  "description": "Cập nhật mô tả",
  "priority": "Critical",
  "dueDate": "2025-01-05T23:59:59Z",
  "categoryId": "550e8400-e29b-41d4-a716-446655440001",
  "tagIds": [
    "550e8400-e29b-41d4-a716-446655440003"
  ],
  "estimatedHours": 10
}
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Todo updated successfully"
}
```

**Error Responses:**
```json
// 409 - Conflict (Version mismatch)
{
  "statusCode": 409,
  "message": "Todo has been modified by another user"
}
```

---

### 5. Update Todo Status

**Endpoint:**
```
PATCH /api/todos/{id}/status
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "newStatus": "Done"
}
```

**Valid Statuses:**
- `Todo` (0)
- `InProgress` (1)
- `Done` (2)
- `Cancelled` (3)

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Todo status updated successfully",
  "data": {
    "previousStatus": "InProgress",
    "newStatus": "Done",
    "completedAt": "2024-12-21T15:30:00Z"
  }
}
```

---

### 6. Delete Todo

**Endpoint:**
```
DELETE /api/todos/{id}
Authorization: Bearer <your_jwt_token>
```

**Response (204 No Content):**
```
No body returned
```

**Or (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Todo deleted successfully"
}
```

---

### 7. Bulk Operations

#### Bulk Update Status
```
PATCH /api/todos/bulk/status
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "todoIds": [
    "550e8400-e29b-41d4-a716-446655440002",
    "550e8400-e29b-41d4-a716-446655440003"
  ],
  "newStatus": "Done"
}
```

#### Bulk Delete
```
DELETE /api/todos/bulk
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "todoIds": [
    "550e8400-e29b-41d4-a716-446655440002",
    "550e8400-e29b-41d4-a716-446655440003"
  ]
}
```

---

## Categories API

### 1. Create Category

**Endpoint:**
```
POST /api/categories
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Work",
  "description": "Công việc liên quan đến công ty",
  "color": "#FF5733",
  "icon": "briefcase"
}
```

**Response (201 Created):**
```json
{
  "statusCode": 201,
  "message": "Category created successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "name": "Work",
    "description": "Công việc liên quan đến công ty",
    "color": "#FF5733",
    "icon": "briefcase",
    "displayOrder": 0,
    "createdAt": "2024-12-21T14:30:00Z"
  }
}
```

---

### 2. Get All Categories

**Endpoint:**
```
GET /api/categories
Authorization: Bearer <your_jwt_token>
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Categories retrieved successfully",
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440001",
      "name": "Work",
      "description": "Công việc liên quan đến công ty",
      "color": "#FF5733",
      "icon": "briefcase",
      "displayOrder": 0,
      "todoCount": 15
    }
  ]
}
```

---

### 3. Update Category

**Endpoint:**
```
PUT /api/categories/{id}
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Work Updated",
  "description": "Cập nhật mô tả",
  "color": "#33FF57",
  "icon": "work"
}
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Category updated successfully"
}
```

---

### 4. Delete Category

**Endpoint:**
```
DELETE /api/categories/{id}
Authorization: Bearer <your_jwt_token>
```

**Query Parameters:**
| Parameter | Type | Optional | Default | Description |
|-----------|------|----------|---------|-------------|
| reassignToCategory | guid | ✓ | null | Category ID to reassign todos |

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Category deleted successfully",
  "data": {
    "deletedCategoryId": "550e8400-e29b-41d4-a716-446655440001",
    "todosReassignedCount": 5
  }
}
```

---

## Tags API

### 1. Create Tag

**Endpoint:**
```
POST /api/tags
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "urgent",
  "description": "Công việc cần làm ngay",
  "color": "#FF0000"
}
```

**Response (201 Created):**
```json
{
  "statusCode": 201,
  "message": "Tag created successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440003",
    "name": "urgent",
    "description": "Công việc cần làm ngay",
    "color": "#FF0000"
  }
}
```

---

### 2. Get All Tags

**Endpoint:**
```
GET /api/tags
Authorization: Bearer <your_jwt_token>
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Tags retrieved successfully",
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440003",
      "name": "urgent",
      "description": "Công việc cần làm ngay",
      "color": "#FF0000",
      "todoCount": 8
    }
  ]
}
```

---

### 3. Bulk Create Tags

**Endpoint:**
```
POST /api/tags/bulk
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "tags": [
    {
      "name": "frontend",
      "description": "Frontend tasks"
    },
    {
      "name": "backend",
      "description": "Backend tasks"
    },
    {
      "name": "documentation",
      "description": "Documentation tasks"
    }
  ]
}
```

**Response (201 Created):**
```json
{
  "statusCode": 201,
  "message": "Tags created successfully",
  "data": {
    "createdCount": 3,
    "tags": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440010",
        "name": "frontend"
      },
      {
        "id": "550e8400-e29b-41d4-a716-446655440011",
        "name": "backend"
      },
      {
        "id": "550e8400-e29b-41d4-a716-446655440012",
        "name": "documentation"
      }
    ]
  }
}
```

---

## Admin API

### 1. Get System Stats

**Endpoint:**
```
GET /api/admin/stats
Authorization: Bearer <admin_jwt_token>
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Statistics retrieved successfully",
  "data": {
    "totalUsers": 1250,
    "activeUsersToday": 420,
    "totalTodos": 45830,
    "completedTodos": 38420,
    "completionRate": 83.8,
    "averageTodosPerUser": 36.7,
    "lastUpdate": "2024-12-21T14:30:00Z"
  }
}
```

---

## Status Codes & Errors

### HTTP Status Codes

| Code | Name | Description |
|------|------|-------------|
| 200 | OK | Successful request |
| 201 | Created | Resource successfully created |
| 204 | No Content | Successful request with no response body |
| 400 | Bad Request | Invalid request parameters |
| 401 | Unauthorized | Authentication required |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource not found |
| 409 | Conflict | Resource conflict (e.g., version mismatch) |
| 422 | Unprocessable Entity | Validation failed |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Server error |
| 503 | Service Unavailable | Service temporarily unavailable |

### Error Response Format

```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "title": [
      "Title is required",
      "Title must not exceed 255 characters"
    ],
    "priority": [
      "Invalid priority value"
    ]
  },
  "timestamp": "2024-12-21T14:30:00Z",
  "path": "/api/todos",
  "traceId": "0HN1GK8D49BSF:00000001"
}
```

### Common Error Codes

| Code | Message | Resolution |
|------|---------|-----------|
| VALIDATION_ERROR | Validation failed | Check request body for invalid fields |
| UNAUTHORIZED | Authentication required | Provide valid JWT token |
| FORBIDDEN | Insufficient permissions | Check user role and permissions |
| NOT_FOUND | Resource not found | Verify resource ID |
| CONFLICT_ERROR | Resource conflict | Refresh data and try again |
| DUPLICATE_ERROR | Resource already exists | Use unique identifier |
| INVALID_TOKEN | Invalid or expired token | Login again to get new token |
| RATE_LIMIT_EXCEEDED | Too many requests | Wait before making new requests |

---

## Rate Limiting

### Rate Limit Headers

All API responses include rate limit information:

```
X-RateLimit-Limit: 60
X-RateLimit-Remaining: 58
X-RateLimit-Reset: 1703174400
```

### Rate Limits by Endpoint Type

| Endpoint Type | Limit | Window | Notes |
|---------------|-------|--------|-------|
| Read (GET) | 100 req | 1 minute | Per user |
| Write (POST/PUT/PATCH) | 60 req | 1 minute | Per user |
| Delete (DELETE) | 30 req | 1 minute | Per user |
| Auth (Login) | 10 attempts | 15 minutes | Per IP |
| Export | 5 req | 1 hour | Per user |

### Retry Strategy

**Exponential Backoff:**
```csharp
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => (int)r.StatusCode == 429)
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => 
            TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        onRetry: (outcome, timeSpan, retryCount, context) =>
        {
            Console.WriteLine($"Retrying in {timeSpan.TotalSeconds}s (Attempt {retryCount})");
        });
```

---

**Version**: 1.0.0  
**Last Updated**: 2024-12-21  
**API Base URL**: https://api.yourdomain.com/api/v1
