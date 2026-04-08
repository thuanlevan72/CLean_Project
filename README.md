# 🚀 Hệ Thống Quản Lý Todo (TODO Management System) - Tài Liệu Hướng Dẫn Chuyên Nghiệp

<div align="center">

[![.NET](https://img.shields.io/badge/.NET-8%20%7C%2010-blue?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-12%2B-336791?style=flat-square&logo=postgresql)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-6%2B-DC382D?style=flat-square&logo=redis)](https://redis.io/)
[![Kafka](https://img.shields.io/badge/Apache%20Kafka-Latest-231F20?style=flat-square&logo=apache-kafka)](https://kafka.apache.org/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)
[![Status](https://img.shields.io/badge/Status-Active%20Development-success?style=flat-square)](https://github.com/thuanlevan72/CLean_Project)

**Ứng dụng Todo API hiện đại được xây dựng với Clean Architecture, CQRS, Event-Driven Architecture**

[🌐 Website](#) • [📚 Documentation](#) • [🐛 Report Bug](#) • [✨ Request Feature](#) • [👥 Contributors](#)

</div>

---

## 📚 Mục Lục (Table of Contents)

### Core Documentation
- [🎯 Quick Start](#quick-start-5-phút)
- [1️⃣ Giới Thiệu](#1-giới-thiệu)
- [2️⃣ Kiến Trúc Hệ Thống](#2-kiến-trúc-hệ-thống)
- [3️⃣ Cấu Trúc Dự Án](#3-cấu-trúc-dự-án)
- [4️⃣ Công Nghệ & Dependency](#4-công-nghệ--dependency)

### Setup & Configuration
- [5️⃣ Hướng Dẫn Cài Đặt](#5-hướng-dẫn-cài-đặt)
- [6️⃣ Cấu Hình Chi Tiết](#6-cấu-hình-chi-tiết)
- [🐳 Docker Setup](#docker-setup)
- [🔑 Environment Variables](#environment-variables)

### Usage & Development
- [7️⃣ Sử Dụng API](#7-sử-dụng-api)
- [8️⃣ Phát Triển & Mở Rộng](#8-phát-triển--mở-rộng)
- [9️⃣ Kiểm Thử](#9-kiểm-thử)

### Advanced Topics
- [📊 Database Schema](#database-schema)
- [🔄 Event Flow Diagrams](#event-flow-diagrams)
- [⚙️ Performance Tuning](#performance-tuning)
- [📈 Monitoring & Logging](#monitoring--logging)
- [🔐 Security](#security)

### Troubleshooting & Reference
- [🚨 Troubleshooting](#troubleshooting)
- [❓ FAQ](#faq)
- [📖 Glossary](#glossary)
- [🛠️ Common Workflows](#common-workflows)
- [11️⃣ Best Practices](#11-best-practices)

---

## Quick Start (5 phút)

> ⏱️ **Đây là phần nhanh nhất để bắt đầu. Chi tiết đầy đủ xem phía dưới.**

### Prerequisites (Yêu cầu tối thiểu)
```bash
# Kiểm tra phiên bản
dotnet --version          # Phải >= 8.0
docker --version          # Tùy chọn (nếu dùng Docker)
git --version             # Phải có
```

### One-Command Setup (Setup 1 lệnh)

```bash
# Clone + Restore + Build
git clone https://github.com/thuanlevan72/CLean_Project.git
cd todo_project
dotnet restore
dotnet build

# Start Infrastructure (Docker)
docker-compose up -d

# Run migrations
dotnet ef database update -p Infrastructure/Infrastructure.Postgres.csproj

# Start API
dotnet run --project Api/Api.csproj
```

### Verify Installation
```bash
# API Health Check
curl http://localhost:5000/health

# Swagger UI
# Mở: http://localhost:5000/swagger

# Hangfire Dashboard
# Mở: http://localhost:5000/hangfire
```

✅ **Done!** Bây giờ bạn có thể:
- 📖 Xem API docs: http://localhost:5000/swagger
- 🎮 Thử API trực tiếp
- 💼 Monitor jobs: http://localhost:5000/hangfire

---

## 1. Giới Thiệu

### 1.1 Tổng Quan

**Hệ Thống Quản Lý Todo** là một ứng dụng API RESTful hiện đại được xây dựng với:

- ✅ **Clean Architecture** - Cấu trúc sạch, dễ bảo trì
- 📋 **CQRS Pattern** - Tách Command và Query
- 🎯 **Domain-Driven Design** - Thiết kế theo domain
- 🔄 **Event-Driven Architecture** - Kiến trúc hướng sự kiện
- 🔌 **Microservice-Ready** - Sẵn sàng cho kiến trúc microservice
- 🔐 **Enterprise-Grade Security** - Bảo mật cấp doanh nghiệp

### 1.2 Tính Năng Chính

#### 📝 Quản Lý Todo Items
- ✅ Tạo, cập nhật, xóa, hoàn thành công việc
- 🏷️ Phân loại theo Category và Tag (với support bulk create)
- 📅 Quản lý deadline và reminder
- 🔄 Tạo sub-tasks (công việc con)
- 📊 Ưu tiên 4 mức: Low, Medium, High, Critical
- 🎯 Trạng thái: Todo, In Progress, Done, Cancelled

#### 👤 Xác Thực & Phân Quyền
- 🔐 JWT Authentication + Refresh Token
- 👥 Role-based Authorization (RBAC)
- 🛡️ Secure password hashing (bcrypt)
- 📱 Multi-device support
- 🔒 Token expiration & blacklist

#### 💼 Quản Lý Background Jobs
- ⏰ Recurring jobs (email reminders, cleanup, etc.)
- 📧 Async task processing
- 🔄 Retry mechanism
- 📊 Job monitoring dashboard

#### 🚀 Advanced Features
- ⚡ Redis caching (2-tier cache strategy)
- 📤 Event publishing via Kafka
- 🔒 Distributed lock support
- 🛡️ Rate limiting (prevent API abuse)
- 📊 Pagination & filtering
- 🔍 Full-text search ready

### 1.3 Use Cases

```
┌─────────────────────────────────────────────────────────────┐
│              📱 Loại Ứng Dụng Sử Dụng                        │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ 🏢 Enterprise Task Management                               │
│    ├─ Team collaboration                                    │
│    ├─ Project tracking                                      │
│    └─ Resource allocation                                   │
│                                                              │
│ 👤 Personal Productivity                                    │
│    ├─ Daily planning                                        │
│    ├─ Goal tracking                                         │
│    └─ Habit formation                                       │
│                                                              │
│ 🏥 Healthcare/Education                                     │
│    ├─ Patient task management                               │
│    ├─ Student assignment tracking                           │
│    └─ Course progress monitoring                            │
│                                                              │
│ 🛒 E-Commerce Order Management                              │
│    ├─ Order processing                                      │
│    ├─ Delivery tracking                                     │
│    └─ Customer task automation                              │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## Docker Setup

### 3️⃣ Nếu không muốn install thủ công, dùng Docker:

**docker-compose.yml:**
```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15-alpine
    environment:
      POSTGRES_DB: todo_db
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  kafka:
    image: confluentinc/cp-kafka:7.5.0
    environment:
      KAFKA_BROKER_ID: 1
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:9092
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
    depends_on:
      - zookeeper
    ports:
      - "9092:9092"

  zookeeper:
    image: confluentinc/cp-zookeeper:7.5.0
    environment:
      ZOOKEEPER_CLIENT_PORT: 2181
    ports:
      - "2181:2181"

volumes:
  postgres_data:
```

**Chạy:**
```bash
docker-compose up -d

# Verify
docker-compose ps
```

---

## Environment Variables

### Development (.env.development)
```bash
# Database
DB_HOST=localhost
DB_PORT=5432
DB_NAME=todo_db
DB_USER=postgres
DB_PASSWORD=postgres

# Redis
REDIS_HOST=localhost
REDIS_PORT=6379

# Kafka
KAFKA_BOOTSTRAP_SERVERS=localhost:9092

# JWT
JWT_SECRET_KEY=your-super-secret-key-min-32-chars-long-!!!
JWT_ISSUER=todo-api
JWT_AUDIENCE=todo-client
JWT_EXPIRATION_MINUTES=60
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7

# API
API_PORT=5000
API_LOG_LEVEL=Information
ENABLE_SWAGGER=true
ENABLE_HANGFIRE_DASHBOARD=true
```

### Production (.env.production)
```bash
# Database (Managed Service)
DB_HOST=prod-db.azure.com
DB_PORT=5432
DB_NAME=todo_db_prod
DB_USER=admin
DB_PASSWORD=${SECRETS_DB_PASSWORD}

# Redis (Managed Service)
REDIS_HOST=prod-cache.redis.cache.windows.net
REDIS_PORT=6379
REDIS_PASSWORD=${SECRETS_REDIS_PASSWORD}
REDIS_SSL=true

# Kafka (Cloud)
KAFKA_BOOTSTRAP_SERVERS=prod-kafka.confluent.cloud:9092
KAFKA_SASL_USERNAME=${SECRETS_KAFKA_USER}
KAFKA_SASL_PASSWORD=${SECRETS_KAFKA_PASSWORD}

# JWT
JWT_SECRET_KEY=${SECRETS_JWT_KEY}
JWT_ISSUER=https://todo-api.yourdomain.com
JWT_AUDIENCE=https://todo-client.yourdomain.com
JWT_EXPIRATION_MINUTES=30
JWT_REFRESH_TOKEN_EXPIRATION_DAYS=7

# API
API_PORT=443
API_LOG_LEVEL=Warning
ENABLE_SWAGGER=false
ENABLE_HANGFIRE_DASHBOARD=false
ENABLE_CORS=true
CORS_ALLOWED_ORIGINS=https://todo-client.yourdomain.com

# Security
RATE_LIMIT_ENABLED=true
RATE_LIMIT_REQUESTS_PER_MINUTE=60
ENABLE_HTTPS_ONLY=true
```

---

## Database Schema

### 📊 Entity Relationship Diagram (ERD)

```
┌─────────────────────────────────────────────────────────────────┐
│                      DATABASE SCHEMA                            │
└─────────────────────────────────────────────────────────────────┘

┌──────────────────┐
│      Users       │
├──────────────────┤
│ Id (PK)          │ ◄─────────────┐
│ Email            │               │
│ UserName         │               │ 1:N (One User has Many Todos)
│ PasswordHash     │               │
│ CreatedAt        │               │
│ UpdatedAt        │               │
└──────────────────┘               │
                                   │
                    ┌──────────────────────────┐
                    │     TodoItems (Main)     │
                    ├──────────────────────────┤
                    │ Id (PK)                  │
                    │ Title                    │
                    │ Description              │
                    │ Priority (enum)          │
                    │ Status (enum)            │
                    │ DueDate                  │
                    │ CompletedAt              │
                    │ UserId (FK) ─────────────┘
                    │ CategoryId (FK) ───────┐
                    │ ParentTaskId (FK)      │ (Self-referencing for SubTasks)
                    │ CreatedAt              │
                    │ UpdatedAt              │
                    │ IsDeleted              │
                    └──────────────────────────┘
                             │
                             │ 1:N (One Todo has Many Tags)
                             │
        ┌────────────────────┴────────────────────┐
        │                                         │
┌───────▼──────────────────┐    ┌────────────────▼────────────┐
│    TodoItem_Tags (Join)  │    │     Categories              │
├──────────────────────────┤    ├─────────────────────────────┤
│ TodoItemId (FK)          │    │ Id (PK)                     │
│ TagId (FK)               │    │ Name                        │
│ CreatedAt                │    │ Description                 │
└──────────────────────────┘    │ Color                       │
                                │ UserId (FK)                 │
        ┌───────────────────────┤ CreatedAt                   │
        │                       │ UpdatedAt                   │
        │                       │ IsDeleted                   │
        │                       └─────────────────────────────┘
        │
┌───────▼──────────────────┐
│       Tags               │
├──────────────────────────┤
│ Id (PK)                  │
│ Name                     │
│ Description              │
│ UserId (FK)              │
│ CreatedAt                │
│ UpdatedAt                │
│ IsDeleted                │
└──────────────────────────┘

┌──────────────────────────┐
│   OutboxMessages         │
├──────────────────────────┤
│ Id (PK)                  │
│ AggregateId (FK)         │
│ EventType                │
│ EventData (JSON)         │
│ CreatedAt                │
│ ProcessedAt (nullable)   │
│ IsProcessed              │
└──────────────────────────┘

┌──────────────────────────┐
│   HangfireJobs           │
├──────────────────────────┤
│ Id (PK)                  │
│ JobName                  │
│ JobType                  │
│ CronExpression           │
│ NextExecution            │
│ LastExecution            │
│ IsActive                 │
└──────────────────────────┘
```

### SQL Schema (DDL)

**Bảng Users:**
```sql
CREATE TABLE "Users" (
  "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  "Email" varchar(255) NOT NULL UNIQUE,
  "UserName" varchar(100) NOT NULL UNIQUE,
  "PasswordHash" varchar(255) NOT NULL,
  "FullName" varchar(255),
  "ProfilePicture" text,
  "IsActive" boolean DEFAULT true,
  "CreatedAt" timestamp DEFAULT CURRENT_TIMESTAMP,
  "UpdatedAt" timestamp DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_email ON "Users"("Email");
CREATE INDEX idx_users_username ON "Users"("UserName");
```

**Bảng Categories:**
```sql
CREATE TABLE "Categories" (
  "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  "UserId" uuid NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
  "Name" varchar(255) NOT NULL,
  "Description" text,
  "Color" varchar(7),
  "Icon" varchar(50),
  "DisplayOrder" int DEFAULT 0,
  "IsDeleted" boolean DEFAULT false,
  "CreatedAt" timestamp DEFAULT CURRENT_TIMESTAMP,
  "UpdatedAt" timestamp DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_categories_userid ON "Categories"("UserId");
CREATE UNIQUE INDEX idx_categories_user_name ON "Categories"("UserId", "Name") WHERE "IsDeleted" = false;
```

**Bảng Tags:**
```sql
CREATE TABLE "Tags" (
  "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  "UserId" uuid NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
  "Name" varchar(100) NOT NULL,
  "Description" text,
  "Color" varchar(7),
  "IsDeleted" boolean DEFAULT false,
  "CreatedAt" timestamp DEFAULT CURRENT_TIMESTAMP,
  "UpdatedAt" timestamp DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_tags_userid ON "Tags"("UserId");
CREATE UNIQUE INDEX idx_tags_user_name ON "Tags"("UserId", "Name") WHERE "IsDeleted" = false;
```

**Bảng TodoItems (Main):**
```sql
CREATE TABLE "TodoItems" (
  "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  "UserId" uuid NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
  "Title" varchar(255) NOT NULL,
  "Description" text,
  "Priority" int DEFAULT 0,  -- 0: Low, 1: Medium, 2: High, 3: Critical
  "Status" int DEFAULT 0,    -- 0: Todo, 1: InProgress, 2: Done, 3: Cancelled
  "DueDate" timestamp,
  "CompletedAt" timestamp,
  "CategoryId" uuid REFERENCES "Categories"("Id") ON DELETE SET NULL,
  "ParentTaskId" uuid REFERENCES "TodoItems"("Id") ON DELETE CASCADE,
  "EstimatedHours" decimal(10,2),
  "ActualHours" decimal(10,2),
  "Attachment" text,
  "IsDeleted" boolean DEFAULT false,
  "CreatedAt" timestamp DEFAULT CURRENT_TIMESTAMP,
  "UpdatedAt" timestamp DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_todositems_userid_status ON "TodoItems"("UserId", "Status");
CREATE INDEX idx_todositems_userid_duedate ON "TodoItems"("UserId", "DueDate");
CREATE INDEX idx_todositems_parenttaskid ON "TodoItems"("ParentTaskId");
CREATE INDEX idx_todositems_categoryid ON "TodoItems"("CategoryId");
```

**Bảng Join TodoItem_Tags:**
```sql
CREATE TABLE "TodoItem_Tags" (
  "TodoItemId" uuid NOT NULL REFERENCES "TodoItems"("Id") ON DELETE CASCADE,
  "TagId" uuid NOT NULL REFERENCES "Tags"("Id") ON DELETE CASCADE,
  PRIMARY KEY ("TodoItemId", "TagId"),
  "CreatedAt" timestamp DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_todotags_tagid ON "TodoItem_Tags"("TagId");
```

**Bảng OutboxMessages (Event Sourcing):**
```sql
CREATE TABLE "OutboxMessages" (
  "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  "AggregateId" uuid NOT NULL,
  "EventType" varchar(255) NOT NULL,
  "EventData" jsonb NOT NULL,
  "IsProcessed" boolean DEFAULT false,
  "CreatedAt" timestamp DEFAULT CURRENT_TIMESTAMP,
  "ProcessedAt" timestamp
);

CREATE INDEX idx_outboxmessages_processed ON "OutboxMessages"("IsProcessed");
```

---

## Event Flow Diagrams

### Todo Creation Flow

```
┌─────────────────────────────────────────────────────────────────────┐
│                   🎯 TODO CREATION EVENT FLOW                       │
└─────────────────────────────────────────────────────────────────────┘

Client                    API                  App Layer           Infrastructure
  │                        │                      │                      │
  ├─ POST /todos ──────────┤                      │                      │
  │  (CreateTodoCommand)   │                      │                      │
  │                        │                      │                      │
  │                        ├─ Validate ──────────►│                      │
  │                        │                      │                      │
  │                        │                      ├─ Check Cache ────────┤ Redis
  │                        │                      │  (Get User)          │
  │                        │                      │◄─ Cache Hit ─────────┤
  │                        │                      │                      │
  │                        │                      ├─ Map DTO → Entity    │
  │                        │                      │                      │
  │                        │                      ├─ Add to UoW ─────────┤ DbContext
  │                        │                      │                      │
  │                        │                      ├─ SaveChanges ────────┤ PostgreSQL
  │                        │                      │  (INSERT)            │
  │                        │                      │◄─ Success ───────────┤
  │                        │                      │                      │
  │                        │                      ├─ Create Event ──────►│ OutboxTable
  │                        │                      │  (TodoCreatedEvent)  │
  │                        │                      │                      │
  │                        │                      ├─ Publish to Kafka ──►│ Kafka Topic
  │                        │                      │  (Background Job)    │
  │                        │                      │                      │
  │                        │                      ├─ Invalidate Cache ──►│ Redis
  │                        │                      │  (User's Todos)      │
  │                        │                      │                      │
  │◄─── 201 Created ───────┤◄─ Result ──────────┤                      │
  │    { id, ... }         │                      │                      │
  │                        │                      │                      │

📢 Events được publish:
   1️⃣ TodoCreatedEvent → Kafka → Email Service (Send reminder setup)
   2️⃣ TodoCreatedEvent → Kafka → Notification Service (Notify team)
   3️⃣ Cache Invalidation → Redis (Remove user's todo list cache)
```

### Todo Update Flow with Concurrency

```
┌─────────────────────────────────────────────────────────────────────┐
│                   🔄 TODO UPDATE WITH CONCURRENCY CHECK             │
└─────────────────────────────────────────────────────────────────────┘

User A                        User B                        System
  │                             │                            │
  ├─ Get Todo (v1) ─────────────┤────────────────────────────┤
  │                             │                            │
  │                             ├─ Get Todo (v1) ────────────┤
  │                             │                            │
  ├─ Modify & Submit ─────────────────────────────────────────┤
  │  (PUT /todos/123)           │                            │
  │                             │  ├─ Check Version (v1 vs v2)
  │                             │  │  ✅ PASS (v1 == v1)
  │                             │  │
  │                             │  ├─ Update (v1 → v2)
  │                             │  │
  │                             │  ├─ Publish Event
  │                             │  │
  │◄──── ✅ 200 OK ─────────────┤  ├─ Invalidate Cache
  │                             │  │
  │                             ├─ Modify & Submit ─────────┤
  │                             │  (PUT /todos/123)         │
  │                             │                           │
  │                             │                           ├─ Check Version
  │                             │                           │  ❌ FAIL (v1 != v2)
  │                             │◄──── ❌ 409 Conflict ────┤
  │                             │   "Version mismatch"       │

🔑 Key Features:
   ✅ Optimistic Locking (Version field)
   ✅ Conflict Detection
   ✅ Last-Write-Wins strategy option
```

---

## Performance Tuning

### 1️⃣ Database Optimization

**Index Strategy:**
```sql
-- Frequently Used Queries
CREATE INDEX idx_todositems_userid_priority_status 
ON "TodoItems"("UserId", "Priority", "Status") 
WHERE "IsDeleted" = false;

-- Range Queries
CREATE INDEX idx_todositems_duedate_range 
ON "TodoItems"("UserId", "DueDate") 
WHERE "DueDate" IS NOT NULL AND "IsDeleted" = false;

-- Text Search (if using PostgreSQL full-text search)
CREATE INDEX idx_todositems_title_tsearch 
ON "TodoItems" USING GIN(to_tsvector('english', "Title"));

-- Composite Index for Common Filters
CREATE INDEX idx_todositems_search 
ON "TodoItems"("UserId", "Status", "CategoryId") 
WHERE "IsDeleted" = false;
```

**Query Analysis:**
```csharp
// ❌ BAD: N+1 Problem
var todos = await _context.TodoItems
    .Where(x => x.UserId == userId)
    .ToListAsync();

foreach (var todo in todos)
{
    var category = await _context.Categories.FindAsync(todo.CategoryId);
    // 🔴 Database call for each Todo!
}

// ✅ GOOD: Eager Loading
var todos = await _context.TodoItems
    .Where(x => x.UserId == userId)
    .Include(x => x.Category)
    .Include(x => x.Tags)
    .ToListAsync();

// ✅ BETTER: Projection (Select only needed fields)
var todos = await _context.TodoItems
    .Where(x => x.UserId == userId && !x.IsDeleted)
    .Select(x => new TodoDto
    {
        Id = x.Id,
        Title = x.Title,
        Status = x.Status,
        Priority = x.Priority,
        CategoryName = x.Category.Name,
        TagNames = x.Tags.Select(t => t.Name).ToList()
    })
    .ToListAsync();
```

### 2️⃣ Caching Strategy

**3-Tier Caching:**
```csharp
// Tier 1: Memory Cache (Ultra-fast, single server)
private readonly IMemoryCache _memoryCache;

// Tier 2: Distributed Cache (Redis, shared across servers)
private readonly IDistributedCache _distributedCache;

// Tier 3: Database (Slow, authoritative source)
private readonly IRepository _repository;

public async Task<TodoDto> GetTodoAsync(Guid id, CancellationToken ct)
{
    var cacheKey = $"todo:{id}";

    // Tier 1: Check memory
    if (_memoryCache.TryGetValue(cacheKey, out TodoDto cached))
        return cached;

    // Tier 2: Check Redis
    var redisCached = await _distributedCache.GetStringAsync(cacheKey, ct);
    if (!string.IsNullOrEmpty(redisCached))
    {
        var todo = JsonSerializer.Deserialize<TodoDto>(redisCached);
        _memoryCache.Set(cacheKey, todo, TimeSpan.FromMinutes(5));
        return todo;
    }

    // Tier 3: Query database
    var todoItem = await _repository.GetByIdAsync(id, ct);
    var result = _mapper.Map<TodoDto>(todoItem);

    // Set caches
    var json = JsonSerializer.Serialize(result);
    await _distributedCache.SetStringAsync(cacheKey, json, 
        new DistributedCacheEntryOptions 
        { 
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) 
        }, ct);
    _memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

    return result;
}
```

**Cache Invalidation Pattern:**
```csharp
public async Task InvalidateTodoAsync(Guid id)
{
    var cacheKey = $"todo:{id}";

    // Remove from both caches
    _memoryCache.Remove(cacheKey);
    await _distributedCache.RemoveAsync(cacheKey);

    // Also invalidate related caches
    await InvalidateUserTodosCache(/* userId */);
    await InvalidateCategoryCache(/* categoryId */);
}

private async Task InvalidateUserTodosAsync(Guid userId)
{
    // Pattern-based invalidation
    var userTodosPattern = $"todos:user:{userId}:*";
    // Implement pattern matching in Redis
    var todos = await _redisConnection.SearchAsync(userTodosPattern);
    foreach (var key in todos)
        await _distributedCache.RemoveAsync(key);
}
```

### 3️⃣ Batch Operations

```csharp
// ✅ Batch Insert (instead of one-by-one)
public async Task CreateTagsBulkAsync(List<string> tagNames, Guid userId, CancellationToken ct)
{
    var tags = tagNames.Select(name => new Tag 
    { 
        Name = name, 
        UserId = userId 
    }).ToList();

    await _context.Tags.AddRangeAsync(tags, ct);
    await _context.SaveChangesAsync(ct);
}

// ✅ Batch Update
public async Task UpdateTodosStatusBulkAsync(List<Guid> todoIds, TodoStatus newStatus, CancellationToken ct)
{
    await _context.TodoItems
        .Where(x => todoIds.Contains(x.Id))
        .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, newStatus), ct);
}

// ✅ Batch Delete
public async Task DeleteTodosBulkAsync(List<Guid> todoIds, CancellationToken ct)
{
    await _context.TodoItems
        .Where(x => todoIds.Contains(x.Id))
        .ExecuteDeleteAsync(ct);
}
```

### 4️⃣ Connection Pooling

```csharp
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=todo_db;Username=postgres;Password=postgres;Pooling=true;Min Pool Size=10;Max Pool Size=100;Connection Idle Lifetime=300;"
  }
}

// Explain:
// Pooling=true           - Enable connection pooling
// Min Pool Size=10       - Maintain minimum 10 connections
// Max Pool Size=100      - Allow maximum 100 connections
// Connection Idle Lifetime=300 - Close idle connections after 5 minutes
```

### 5️⃣ Monitoring Query Performance

```csharp
// Add query logging for slow queries
builder.Services.AddDbContext<TodoDbContext>(options =>
{
    options.UseNpgsql(connectionString);

    // Log slow queries (> 1 second)
    options.LogTo(Console.WriteLine, 
        new[] { DbLoggerCategory.Query.Name }, 
        LogLevel.Warning);

    // Or use performance counters
    options.UseSnakeCaseNamingConvention();
});

// EF Core Query API
public async Task LogSlowQueriesAsync()
{
    var slowQueries = await _context.Database
        .SqlQueryRaw<SlowQueryLog>(
            @"SELECT query, calls, mean_time 
              FROM pg_stat_statements 
              WHERE mean_time > 1000 
              ORDER BY mean_time DESC")
        .ToListAsync();
}
```

---

## Monitoring & Logging

### 1️⃣ Structured Logging (Serilog)

```csharp
// Program.cs
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "TodoApi")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .WriteTo.Console(new CompactJsonFormatter())
        .WriteTo.File("logs/todo-api-.txt", 
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:G} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.Seq("http://seq-server:5341");
});

// Usage in code
_logger.LogInformation("Todo {TodoId} created by user {UserId}", todoId, userId);
_logger.LogError(exception, "Failed to create todo for user {UserId}", userId);
```

**Sample Log Output (JSON):**
```json
{
  "Timestamp": "2024-12-21T14:30:00.1234567Z",
  "Level": "Information",
  "MessageTemplate": "Todo {TodoId} created by user {UserId}",
  "Properties": {
    "TodoId": "550e8400-e29b-41d4-a716-446655440000",
    "UserId": "550e8400-e29b-41d4-a716-446655440001",
    "Application": "TodoApi",
    "Environment": "Production",
    "SourceContext": "Application.Services.Todo.Commands.CreateTodo.CreateTodoHandler"
  }
}
```

### 2️⃣ Application Insights (Azure Monitor)

```csharp
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.InstrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];
});

builder.Services.AddLogging(logging =>
{
    logging.AddApplicationInsights();
});
```

### 3️⃣ Metrics & Health Checks

```csharp
// Health Check Configuration
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<TodoDbContext>()
    .AddRedis(configuration["Redis:ConnectionString"], name: "redis")
    .AddKafka(configuration["Kafka:BootstrapServers"], name: "kafka")
    .AddCheck("database", () => CheckDatabaseHealth())
    .AddCheck("api", () => CheckApiHealth());

// Endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Response:
// {
//   "status": "Healthy",
//   "checks": {
//     "DbContext": { "status": "Healthy" },
//     "redis": { "status": "Healthy" },
//     "database": { "status": "Healthy", "duration": "150ms" }
//   },
//   "duration": "250ms"
// }
```

### 4️⃣ Distributed Tracing

```csharp
// Using Activity/OpenTelemetry
var activitySource = new ActivitySource("Todo.API");

using (var activity = activitySource.StartActivity("CreateTodo"))
{
    activity?.SetTag("userId", userId);
    activity?.SetTag("title", command.Title);

    var result = await _handler.Handle(command, ct);

    activity?.SetTag("todoId", result);
    activity?.SetStatus(ActivityStatusCode.Ok);
}
```

---

## Security

### 🔐 Security Checklist

```
✅ Authentication & Authorization
  ├─ JWT tokens with expiration
  ├─ Refresh token rotation
  ├─ Role-based access control (RBAC)
  └─ MFA support ready

✅ Data Protection
  ├─ Passwords hashed with bcrypt
  ├─ Sensitive data encrypted in transit (HTTPS)
  ├─ Sensitive data encrypted at rest (column-level encryption)
  └─ PII compliance (GDPR ready)

✅ API Security
  ├─ Rate limiting (60 requests/minute per user)
  ├─ CORS properly configured
  ├─ Input validation (FluentValidation)
  ├─ SQL injection prevention (EF Core parameterization)
  ├─ XSS protection (Content Security Policy)
  └─ CSRF protection

✅ Infrastructure Security
  ├─ Secrets stored in Key Vault (not in code)
  ├─ Connection strings encrypted
  ├─ Database encryption enabled
  ├─ Network isolation (VPC/VNet)
  └─ Firewall rules configured

✅ Monitoring & Logging
  ├─ Security events logged
  ├─ Failed login attempts tracked
  ├─ Unauthorized access attempts alert
  ├─ Audit logs maintained
  └─ Regular security scanning
```

### JWT Token Security

```csharp
// ✅ Secure JWT Configuration
var jwtSettings = configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]); // Min 32 chars!

var tokenHandler = new JwtSecurityTokenHandler();
var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Email, email),
        new Claim("role", role),
    }),
    Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpirationMinutes"])),
    Issuer = jwtSettings["Issuer"],
    Audience = jwtSettings["Audience"],
    SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(secretKey), 
        SecurityAlgorithms.HmacSha256Signature)
};

var token = tokenHandler.CreateToken(tokenDescriptor);
return tokenHandler.WriteToken(token);

// ✅ Refresh Token Strategy
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsValid => !IsExpired && !IsRevoked;
}
```

---

## Monitoring & Logging (continued)

### 2.1 Mô Hình Clean Architecture

```
┌─────────────────────────────────────────────────────┐
│                  Presentation Layer                 │
│         (Api / Controllers / Web Interface)         │
└──────────────────┬──────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────┐
│              Application Layer                      │
│  (Use Cases / Commands / Queries / Services)        │
└──────────────────┬──────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────┐
│              Domain Layer                           │
│   (Entities / Aggregates / Repository Interfaces)   │
└──────────────────┬──────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────┐
│          Infrastructure Layer                       │
│  (DB / Cache / Message Queue / External Services)   │
└─────────────────────────────────────────────────────┘
```

### 2.2 Các Layer Chi Tiết

#### **Presentation Layer (Api Project)**
- Controllers: TodoItemsController, CategoriesController, TagsController, AuthController, AdminController
- Middleware: GlobalExceptionHandler (xử lý lỗi toàn cục)
- Services: CurrentUserService (lấy thông tin người dùng hiện tại)
- Extensions: ApiServiceCollectionExtensions (DI configuration)

#### **Application Layer (Application Project)**
- **Features**: Chứa các Command và Query (CQRS pattern)
  - TodoItems: CreateTodoCommand, UpdateTodoCommand, DeleteTodoCommand, GetMyTodosQuery, GetTodoByIdQuery
  - Categories: CategoryCommands, CategoryQueries
  - Tags: TagCommands, TagQueries
  - Auth: LoginCommand, RegisterCommand
- **Services**: Xử lý business logic (CreateTodoHandler, UpdateTodoHandler, etc.)
- **Dtos**: Data Transfer Objects (TodoDto, CategoryDto, TagDto, AuthResultDto)
- **Interfaces**: ICacheService, IAuthService, ITodoServices, IKafkaPublisher, IPubSubService, IDistributedLockService, IRateLimitService
- **Mappers**: AutoMapper profiles (IMapFrom, IMapTo)
- **Events**: Domain events (TodoCreatedEvent)
- **DependencyInjection**: DependencyInjection.cs (cấu hình DI cho Application layer)

#### **Domain Layer (Domain Project)**
- **Entities**: Các thực thể chính
  - TodoItem: Công việc (Title, Description, Priority, Status, DueDate, CompletedAt, UserId, CategoryId, Tags, SubTasks)
  - Category: Danh mục
  - Tag: Nhãn
  - OutboxMessage: Lưu trữ sự kiện cho Outbox pattern
- **Base Classes**: 
  - BaseEntity<T>: Entity cơ sở với Id, CreatedAt, UpdatedAt
  - ISoftDelete: Interface cho soft delete
- **Enums**: 
  - Enum.cs: Chứa PriorityLevel (Low, Medium, High, Critical), TodoStatus (Todo, InProgress, Done, Cancelled)
- **Repositories**: Repository interfaces
  - IUnitOfWork: Pattern UnitOfWork
  - IGenericRepository: Generic repository interface
  - ITodoItemRepository, ICategoryRepository, ITagRepository
  - IOutboxRepository: Lưu trữ outbox messages

#### **Infrastructure Layer (Infrastructure Projects)**
1. **Infrastructure.Postgres**: Entity Framework Core + PostgreSQL
   - DbContext configuration
   - Repository implementations
   - Migrations

2. **Infrastructure.Redis**: Redis caching
   - ICacheService implementation
   - IPubSubService implementation
   - Distributed lock service

3. **Infrastructure.Kafka**: Message broker
   - IKafkaPublisher implementation
   - Event publishing

4. **Infrastructure.Hangfire**: Background jobs
   - Hangfire configuration
   - Job scheduling
   - Recurring jobs

---

## 3. Cấu Trúc Dự Án

```
solution/
├── Api/                              # ASP.NET Core Web API
│   ├── Controllers/
│   │   ├── Todo/
│   │   │   ├── TodoItemsController.cs
│   │   │   ├── CategoriesController.cs
│   │   │   └── TagsController.cs
│   │   └── User/
│   │       ├── AuthController.cs
│   │       └── AdminController.cs
│   ├── Middlewares/
│   │   └── GlobalExceptionHandler.cs
│   ├── Services/
│   │   └── CurrentUserService.cs
│   ├── Extensions/
│   │   └── ApiServiceCollectionExtensions.cs
│   ├── Program.cs                   # Startup configuration
│   └── appsettings.json
│
├── Application/                      # Business Logic Layer
│   ├── Features/
│   │   ├── TodoItems/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateTodoCommand.cs
│   │   │   │   ├── UpdateTodoCommand.cs
│   │   │   │   └── DeleteTodoCommand.cs
│   │   │   └── Queries/
│   │   │       ├── GetMyTodosQuery.cs
│   │   │       └── GetTodoByIdQuery.cs
│   │   ├── Categories/
│   │   │   ├── Commands/
│   │   │   │   └── CategoryCommands.cs
│   │   │   └── Queries/
│   │   │       └── CategoryQueries.cs
│   │   ├── Tags/
│   │   │   ├── Commands/
│   │   │   │   └── TagCommands.cs
│   │   │   └── Queries/
│   │   │       └── TagQueries.cs
│   │   └── Auth/
│   │       ├── LoginCommand.cs
│   │       └── RegisterCommand.cs
│   ├── Services/
│   │   ├── Todo/
│   │   │   ├── Commands/
│   │   │   │   └── CreateTodoHandler.cs
│   │   │   └── ...
│   │   └── ...
│   ├── Dtos/
│   │   ├── TodoDto.cs
│   │   ├── CategoryDto.cs
│   │   ├── TagDto.cs
│   │   └── AuthResultDto.cs
│   ├── Interfaces/
│   │   ├── ITodoServices.cs
│   │   ├── IAuthService.cs
│   │   ├── ICacheService.cs
│   │   ├── IKafkaPublisher.cs
│   │   ├── IPubSubService.cs
│   │   ├── IRateLimitService.cs
│   │   └── IDistributedLockService.cs
│   ├── Mappers/
│   │   ├── MappingProfile.cs
│   │   ├── TodoProfile.cs
│   │   ├── IMapFrom.cs
│   │   └── IMapTo.cs
│   ├── Events/
│   │   └── TodoCreatedEvent.cs
│   ├── DependencyInjections/
│   │   └── DependencyInjection.cs
│   └── Application.csproj
│
├── Domain/                           # Business Domain
│   ├── Entities/
│   │   ├── Base/
│   │   │   ├── BaseEntity.cs
│   │   │   └── ISoftDelete.cs
│   │   ├── TodoItem.cs
│   │   ├── Category.cs
│   │   ├── Tag.cs
│   │   └── OutboxMessage.cs
│   ├── Enums/
│   │   └── Enum.cs
│   ├── Repositories/
│   │   ├── Base/
│   │   │   ├── IGenericRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   ├── ITodoItemRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   ├── ITagRepository.cs
│   │   └── IOutboxRepository.cs
│   └── Domain.csproj
│
├── Infrastructure/                   # Data Access + EF Core
│   ├── DbContexts/
│   │   └── TodoDbContext.cs
│   ├── Repositories/
│   │   ├── Base/
│   │   │   └── GenericRepository.cs
│   │   ├── TodoItemRepository.cs
│   │   ├── CategoryRepository.cs
│   │   ├── TagRepository.cs
│   │   └── OutboxRepository.cs
│   ├── Migrations/
│   ├── DependencyInjections/
│   │   └── DependencyInjection.cs
│   ├── Configuration/
│   ├── appsettings.json
│   └── Infrastructure.Postgres.csproj
│
├── Infrastructure.Redis/             # Redis Caching & Pub/Sub
│   ├── Services/
│   │   ├── RedisCacheService.cs
│   │   ├── RedisPubSubService.cs
│   │   └── RedisDistributedLockService.cs
│   ├── DependencyInjections/
│   │   └── DependencyInjection.cs
│   └── Infrastructure.Redis.csproj
│
├── Infrastructure.Kafka/             # Kafka Message Broker
│   ├── Services/
│   │   └── KafkaPublisher.cs
│   ├── Consumers/
│   │   └── ...
│   ├── DependencyInjections/
│   │   └── DependencyInjection.cs
│   └── Infrastructure.Kafka.csproj
│
├── Infrastructure.Hangfire/          # Background Jobs
│   ├── Services/
│   │   └── HangfireJobService.cs
│   ├── Jobs/
│   │   ├── CleanupOutboxJob.cs
│   │   ├── SendEmailJob.cs
│   │   └── ...
│   ├── DependencyInjections/
│   │   └── DependencyInjection.cs
│   └── Infrastructure.Hangfire.csproj
│
├── Workers/                          # Background Worker Service
│   ├── Services/
│   ├── Program.cs
│   └── Workers.csproj
│
├── Common/                           # Utilities & Constants
│   ├── Constants/
│   ├── Helpers/
│   ├── Extensions/
│   └── Common.csproj
│
├── DatabaseFristPostgres/            # Database First Support
│   ├── Models/
│   └── DatabaseFristPostgres.csproj
│
└── todo_project.sln                  # Solution file
```

---

## 4. Công Nghệ & Dependency

### 4.1 Core Frameworks
| Công Nghệ | Phiên Bản | Mục Đích |
|-----------|---------|---------|
| .NET | 8, 10 | Runtime framework |
| ASP.NET Core | Latest | Web API framework |
| Entity Framework Core | 10.0.5 | ORM |
| MediatR | Latest | CQRS pattern |
| AutoMapper | Latest | Object mapping |

### 4.2 Infrastructure & Services
| Công Nghệ | Phiên Bản | Mục Đích |
|-----------|---------|---------|
| PostgreSQL | 12+ | Cơ sở dữ liệu relational |
| Redis | 6+ | Caching & Pub/Sub |
| Kafka | Latest | Message broker |
| Hangfire | Latest | Background jobs |
| Serilog | Latest | Logging |

### 4.3 Security & Validation
| Công Nghệ | Phiên Bản | Mục Đích |
|-----------|---------|---------|
| JWT (System.IdentityModel.Tokens.Jwt) | Latest | Authentication |
| FluentValidation | Latest | Data validation |
| Newtonsoft.Json | 13.0.3 | JSON serialization |

### 4.4 Cài Đặt Package

Các package đã được cấu hình trong từng project:

**Api.csproj:**
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
<PackageReference Include="Swashbuckle.AspNetCore" />
<PackageReference Include="Serilog.AspNetCore" />
```

**Application.csproj:**
```xml
<PackageReference Include="MediatR" />
<PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" />
```

**Infrastructure.Postgres.csproj:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.5" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
```

**Infrastructure.Redis.csproj:**
```xml
<PackageReference Include="StackExchange.Redis" />
```

**Infrastructure.Kafka.csproj:**
```xml
<PackageReference Include="Confluent.Kafka" />
```

**Infrastructure.Hangfire.csproj:**
```xml
<PackageReference Include="Hangfire.AspNetCore" Version="1.8.23" />
<PackageReference Include="Hangfire.PostgreSql" Version="1.21.1" />
```

---

## 5. Hướng Dẫn Cài Đặt

### 5.1 Yêu Cầu Hệ Thống

| Thành Phần | Yêu Cầu |
|-----------|--------|
| OS | Windows / Linux / macOS |
| .NET SDK | 8.0+ hoặc 10.0+ |
| PostgreSQL | 12+ |
| Redis | 6+ |
| Kafka | Latest |
| Git | Latest |
| Visual Studio / VS Code | 2022+ |

### 5.2 Bước Cài Đặt Chi Tiết

#### **Bước 1: Clone Repository**
```bash
git clone https://github.com/thuanlevan72/CLean_Project.git
cd todo_project
```

#### **Bước 2: Cài Đặt .NET SDK**

Kiểm tra phiên bản:
```bash
dotnet --version
```

Cài đặt (nếu chưa có):
- Tải từ https://dotnet.microsoft.com/download
- Hoặc dùng package manager (Windows):
  ```bash
  choco install dotnet-sdk-8.0
  choco install dotnet-sdk-10.0
  ```

#### **Bước 3: Cài Đặt PostgreSQL**

**Windows:**
```bash
choco install postgresql
```

**Linux (Ubuntu):**
```bash
sudo apt-get update
sudo apt-get install postgresql postgresql-contrib
```

Tạo database:
```bash
psql -U postgres
CREATE DATABASE todo_db;
\q
```

#### **Bước 4: Cài Đặt Redis**

**Windows (via WSL2 hoặc Chocolatey):**
```bash
choco install redis-64
```

**Linux:**
```bash
sudo apt-get install redis-server
sudo systemctl start redis-server
```

**macOS:**
```bash
brew install redis
brew services start redis
```

#### **Bước 5: Cài Đặt Kafka**

**Windows:**
- Tải từ https://kafka.apache.org/downloads
- Extract và cài đặt JAVA_HOME

**Linux/macOS:**
```bash
brew install kafka
```

#### **Bước 6: Cấu Hình Connection Strings**

Mở `Api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=todo_db;Username=postgres;Password=your_password;",
    "Redis": "localhost:6379",
    "Kafka": "localhost:9092"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-min-32-chars-long!!",
    "Issuer": "todo-api",
    "Audience": "todo-client",
    "ExpiredMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

#### **Bước 7: Restore & Build**

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Nếu có build error, clean trước:
dotnet clean
dotnet build
```

#### **Bước 8: Chạy Database Migrations**

```bash
cd Infrastructure
dotnet ef database update
cd ..
```

#### **Bước 9: Chạy Ứng Dụng**

**Cách 1: Dùng Visual Studio**
- Mở Api project làm Startup Project
- Nhấn F5 hoặc Ctrl+F5

**Cách 2: Dùng CLI**
```bash
dotnet run --project Api/Api.csproj
```

**Cách 3: Dùng PowerShell (WSL2)**
```powershell
# Start Kafka
wsl -d Ubuntu20.04 -e bash -c "cd /opt/kafka && ./bin/kafka-server-start.sh config/server.properties"

# Start Redis
redis-server

# Start API
dotnet run --project Api/Api.csproj
```

### 5.3 Xác Thực Cài Đặt Thành Công

✅ **Swagger UI:** http://localhost:5000/swagger
✅ **Hangfire Dashboard:** http://localhost:5000/hangfire
✅ **Health Check API:** `GET http://localhost:5000/health`

---

## 6. Cấu Hình Chi Tiết

### 6.1 Cấu Hình JWT Authentication

**Program.cs (Api project):**
```csharp
var jwtSettings = configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
```

### 6.2 Cấu Hình PostgreSQL

**Infrastructure/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=todo_db;Username=postgres;Password=postgres;Integrated Security=false;Pooling=true;Max Pool Size=20;"
  },
  "EntityFramework": {
    "CommandTimeout": 30,
    "EnableSensitiveDataLogging": false
  }
}
```

### 6.3 Cấu Hình Redis

**Infrastructure.Redis/appsettings.json:**
```json
{
  "Redis": {
    "Host": "localhost",
    "Port": 6379,
    "Password": null,
    "Database": 0,
    "CacheDurationMinutes": 60,
    "EnableCompression": true
  }
}
```

### 6.4 Cấu Hình Kafka

**Infrastructure.Kafka/appsettings.json:**
```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "todo-group",
    "TopicPrefix": "todo-",
    "ProducerConfig": {
      "Acks": "All",
      "CompressionType": "Snappy",
      "Retries": 3
    },
    "ConsumerConfig": {
      "AutoOffsetReset": "Earliest",
      "EnableAutoCommit": true,
      "MaxPollIntervalMs": 300000
    }
  }
}
```

### 6.5 Cấu Hình Hangfire

**Infrastructure.Hangfire/appsettings.json:**
```json
{
  "Hangfire": {
    "ConnectionString": "Host=localhost;Port=5432;Database=hangfire_db;Username=postgres;Password=postgres;",
    "DashboardPath": "/hangfire",
    "RecurringJobs": [
      {
        "JobId": "cleanup-outbox",
        "CronExpression": "0 2 * * *",
        "JobType": "CleanupOutboxJob"
      }
    ]
  }
}
```

---

## 7. Sử Dụng API

### 7.1 Authentication

#### **Đăng Ký (Register)**
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "userName": "john_doe"
}
```

**Response:**
```json
{
  "message": "Đăng ký thành công",
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "email": "user@example.com",
    "userName": "john_doe"
  }
}
```

#### **Đăng Nhập (Login)**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "message": "Đăng nhập thành công",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiredAt": "2024-12-31T23:59:59Z",
    "refreshToken": "...",
    "user": {
      "userId": "550e8400-e29b-41d4-a716-446655440000",
      "email": "user@example.com",
      "userName": "john_doe"
    }
  }
}
```

### 7.2 Todo Items - CRUD Operations

#### **Tạo Todo Item (Create)**
```http
POST /api/todos
Authorization: Bearer <your_jwt_token>
Content-Type: application/json

{
  "title": "Hoàn thành dự án",
  "description": "Cần hoàn thành dự án trước thứ 6",
  "priority": "High",
  "dueDate": "2024-12-31T23:59:59Z",
  "categoryId": "550e8400-e29b-41d4-a716-446655440001",
  "tags": ["work", "urgent"]
}
```

**Response (201 Created):**
```json
{
  "message": "Tạo công việc thành công",
  "id": "550e8400-e29b-41d4-a716-446655440002"
}
```

#### **Lấy Tất Cả Todo Items của User (Get All)**
```http
GET /api/todos
Authorization: Bearer <your_jwt_token>
```

**Response (200 OK):**
```json
{
  "message": "Lấy danh sách công việc thành công",
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440002",
      "title": "Hoàn thành dự án",
      "description": "Cần hoàn thành dự án trước thứ 6",
      "priority": "High",
      "status": "InProgress",
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
        },
        {
          "id": "550e8400-e29b-41d4-a716-446655440004",
          "name": "urgent"
        }
      ],
      "createdAt": "2024-12-20T10:00:00Z",
      "updatedAt": "2024-12-21T14:30:00Z"
    }
  ],
  "totalCount": 1
}
```

#### **Lấy Chi Tiết Todo Item (Get By ID)**
```http
GET /api/todos/{id}
Authorization: Bearer <your_jwt_token>
```

**Response:**
```json
{
  "message": "Lấy chi tiết công việc thành công",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "title": "Hoàn thành dự án",
    "description": "Cần hoàn thành dự án trước thứ 6",
    "priority": "High",
    "status": "InProgress",
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
      },
      {
        "id": "550e8400-e29b-41d4-a716-446655440004",
        "name": "urgent"
      }
    ],
    "subTasks": [],
    "createdAt": "2024-12-20T10:00:00Z",
    "updatedAt": "2024-12-21T14:30:00Z"
  }
}
```

#### **Cập Nhật Todo Item (Update)**
```http
PUT /api/todos/{id}
Authorization: Bearer <your_jwt_token>
Content-Type: application/json

{
  "title": "Hoàn thành dự án - Cập nhật",
  "description": "Cần hoàn thành dự án trước thứ 7",
  "priority": "Critical",
  "dueDate": "2025-01-05T23:59:59Z",
  "categoryId": "550e8400-e29b-41d4-a716-446655440001"
}
```

**Response (200 OK):**
```json
{
  "message": "Cập nhật công việc thành công"
}
```

#### **Cập Nhật Trạng Thái Todo Item**
```http
PUT /api/todos/{id}/status
Authorization: Bearer <your_jwt_token>
Content-Type: application/json

{
  "newStatus": "Done"
}
```

**Status có thể là:** `Todo`, `InProgress`, `Done`, `Cancelled`

#### **Xóa Todo Item (Delete)**
```http
DELETE /api/todos/{id}
Authorization: Bearer <your_jwt_token>
```

**Response (204 No Content hoặc 200 OK):**
```json
{
  "message": "Xóa công việc thành công"
}
```

### 7.3 Categories API

#### **Tạo Category**
```http
POST /api/categories
Authorization: Bearer <your_jwt_token>
Content-Type: application/json

{
  "name": "Work",
  "description": "Công việc liên quan đến công ty",
  "color": "#FF5733"
}
```

#### **Lấy Tất Cả Categories**
```http
GET /api/categories
Authorization: Bearer <your_jwt_token>
```

#### **Cập Nhật Category**
```http
PUT /api/categories/{id}
Authorization: Bearer <your_jwt_token>
Content-Type: application/json

{
  "name": "Work Updated",
  "description": "Cập nhật mô tả",
  "color": "#33FF57"
}
```

#### **Xóa Category**
```http
DELETE /api/categories/{id}
Authorization: Bearer <your_jwt_token>
```

### 7.4 Tags API

#### **Tạo Tag**
```http
POST /api/tags
Authorization: Bearer <your_jwt_token>
Content-Type: application/json

{
  "name": "urgent",
  "description": "Công việc cần làm ngay"
}
```

#### **Lấy Tất Cả Tags**
```http
GET /api/tags
Authorization: Bearer <your_jwt_token>
```

#### **Cập Nhật Tag**
```http
PUT /api/tags/{id}
Authorization: Bearer <your_jwt_token>
Content-Type: application/json

{
  "name": "urgent-updated",
  "description": "Cập nhật mô tả"
}
```

#### **Xóa Tag**
```http
DELETE /api/tags/{id}
Authorization: Bearer <your_jwt_token>
```

### 7.5 Admin API

#### **Lấy Thông Tin Admin**
```http
GET /api/admin/info
Authorization: Bearer <admin_jwt_token>
```

#### **Xem Tất Cả Todos (Admin)**
```http
GET /api/admin/todos
Authorization: Bearer <admin_jwt_token>
```

#### **Xem Tất Cả Users (Admin)**
```http
GET /api/admin/users
Authorization: Bearer <admin_jwt_token>
```

---

## 8. Phát Triển & Mở Rộng

### 8.1 Cấu Trúc CQRS + MediatR

Hệ thống sử dụng **CQRS (Command Query Responsibility Segregation)** với **MediatR** để tách biệt các lệnh và truy vấn.

#### **Tạo Mới Command**

**Bước 1:** Tạo file Command trong `Application/Features/YourFeature/Commands/`

```csharp
// Application/Features/TodoItems/Commands/CreateTodoCommand.cs
public class CreateTodoCommand : IRequest<Guid>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? CategoryId { get; set; }
    public List<string> Tags { get; set; } = new();
}
```

**Bước 2:** Tạo Handler trong `Application/Services/`

```csharp
// Application/Services/Todo/Commands/CreateTodo/CreateTodoHandler.cs
public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITodoItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly IKafkaPublisher _kafkaPublisher;

    public CreateTodoHandler(IUnitOfWork unitOfWork, ITodoItemRepository repository, 
        IMapper mapper, IKafkaPublisher kafkaPublisher)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _mapper = mapper;
        _kafkaPublisher = kafkaPublisher;
    }

    public async Task<Guid> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title cannot be empty");

        // Map DTO to Entity
        var todoItem = _mapper.Map<TodoItem>(request);

        // Add to DB
        await _repository.AddAsync(todoItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish event
        await _kafkaPublisher.PublishAsync("todo-created", new TodoCreatedEvent 
        { 
            TodoId = todoItem.Id, 
            Title = todoItem.Title 
        });

        return todoItem.Id;
    }
}
```

**Bước 3:** Đăng ký Validator (tùy chọn)

```csharp
// Application/Features/TodoItems/Commands/CreateTodoCommandValidator.cs
public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority is invalid");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("DueDate must be in the future")
            .When(x => x.DueDate.HasValue);
    }
}
```

**Bước 4:** Sử dụng trong Controller

```csharp
[HttpPost]
public async Task<IActionResult> CreateTodo([FromBody] CreateTodoCommand command)
{
    var newTodoId = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetTodoById), new { id = newTodoId }, 
        new { Message = "Tạo công việc thành công", Id = newTodoId });
}
```

#### **Tạo Mới Query**

```csharp
// Application/Features/TodoItems/Queries/GetMyTodosQuery.cs
public class GetMyTodosQuery : IRequest<List<TodoDto>>
{
    public TodoStatus? Status { get; set; }
    public PriorityLevel? Priority { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
}

// Handler
public class GetMyTodosQueryHandler : IRequestHandler<GetMyTodosQuery, List<TodoDto>>
{
    private readonly ITodoItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ICurrentUserService _currentUserService;

    public async Task<List<TodoDto>> Handle(GetMyTodosQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetUserId();

        // Check cache first
        var cacheKey = $"todos:{userId}:{request.Status}:{request.Priority}";
        var cachedResult = await _cacheService.GetAsync<List<TodoDto>>(cacheKey);
        if (cachedResult != null)
            return cachedResult;

        // Query from DB
        var todos = await _repository.GetUserTodosAsync(userId, request.Status, 
            request.Priority, request.Skip, request.Take, cancellationToken);

        var result = _mapper.Map<List<TodoDto>>(todos);

        // Cache result
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(1));

        return result;
    }
}
```

### 8.2 Thêm Infrastructure Mới

Ví dụ: Thêm **Elasticsearch** cho full-text search

**Bước 1:** Tạo project `Infrastructure.Elasticsearch`

```bash
dotnet new classlib -n Infrastructure.Elasticsearch
```

**Bước 2:** Thêm NuGet packages

```xml
<ItemGroup>
    <PackageReference Include="Elasticsearch.Net" Version="8.0.0" />
    <PackageReference Include="NEST" Version="8.0.0" />
</ItemGroup>
```

**Bước 3:** Tạo Service

```csharp
// Infrastructure.Elasticsearch/Services/ElasticsearchService.cs
public interface IElasticsearchService
{
    Task<List<TodoDto>> SearchTodosAsync(string query);
}

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _client;

    public ElasticsearchService(IElasticClient client)
    {
        _client = client;
    }

    public async Task<List<TodoDto>> SearchTodosAsync(string query)
    {
        var response = await _client.SearchAsync<TodoItem>(s => s
            .Query(q => q
                .MultiMatch(m => m
                    .Query(query)
                    .Fields(f => f
                        .Field(p => p.Title)
                        .Field(p => p.Description)
                    )
                )
            )
        );

        return response.Documents.Select(d => new TodoDto { /* ... */ }).ToList();
    }
}
```

**Bước 4:** Đăng ký DI

```csharp
// Infrastructure.Elasticsearch/DependencyInjections/DependencyInjection.cs
public static IServiceCollection AddInfrastructureElasticsearch(
    this IServiceCollection services, IConfiguration configuration)
{
    var settings = new ConnectionSettings(new Uri(configuration["Elasticsearch:Uri"]))
        .DefaultIndex("todos");

    var client = new ElasticClient(settings);
    services.AddSingleton(client);
    services.AddScoped<IElasticsearchService, ElasticsearchService>();

    return services;
}
```

**Bước 5:** Sử dụng trong Api

```csharp
// Program.cs
builder.Services.AddInfrastructureElasticsearch(builder.Configuration);

// Controller
[HttpGet("search")]
public async Task<IActionResult> SearchTodos(string query)
{
    var results = await _elasticsearchService.SearchTodosAsync(query);
    return Ok(results);
}
```

### 8.3 Thêm Background Job Mới

**Bước 1:** Tạo Job trong `Infrastructure.Hangfire/Jobs/`

```csharp
// Infrastructure.Hangfire/Jobs/SendEmailNotificationJob.cs
public class SendEmailNotificationJob
{
    private readonly IEmailService _emailService;
    private readonly ITodoItemRepository _repository;

    public SendEmailNotificationJob(IEmailService emailService, ITodoItemRepository repository)
    {
        _emailService = emailService;
        _repository = repository;
    }

    public async Task Execute()
    {
        // Lấy todos sắp hết hạn (trong 24h)
        var dueSoonTodos = await _repository.GetDueSoonTodosAsync(DateTime.UtcNow.AddHours(24));

        foreach (var todo in dueSoonTodos)
        {
            await _emailService.SendDueDateReminderAsync(todo.UserId, todo);
        }
    }
}
```

**Bước 2:** Đăng ký Job trong Hangfire DI

```csharp
// Infrastructure.Hangfire/DependencyInjections/DependencyInjection.cs
public static void RegisterRecurringJobs(IRecurringJobManager recurringJobManager)
{
    recurringJobManager.AddOrUpdate<SendEmailNotificationJob>(
        "send-email-notifications",
        job => job.Execute(),
        Cron.Daily(2, 0)  // Chạy lúc 2h sáng mỗi ngày
    );
}
```

**Bước 3:** Gọi hàm đăng ký

```csharp
// Program.cs
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    Infrastructure.Hangfire.DependencyInjection.RegisterRecurringJobs(recurringJobManager);
}
```

### 8.4 Event-Driven Architecture

#### **Publish Event (Kafka)**

```csharp
// Trong Handler
var @event = new TodoCreatedEvent
{
    TodoId = todo.Id,
    Title = todo.Title,
    UserId = todo.UserId,
    CreatedAt = DateTime.UtcNow
};

await _kafkaPublisher.PublishAsync("todo-created", @event);
```

#### **Subscribe Event (Kafka Consumer)**

```csharp
// Infrastructure.Kafka/Consumers/TodoEventConsumer.cs
public class TodoEventConsumer : IKafkaConsumer
{
    private readonly INotificationService _notificationService;

    public TodoEventConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(string topic, string message)
    {
        if (topic == "todo-created")
        {
            var @event = JsonConvert.DeserializeObject<TodoCreatedEvent>(message);
            await _notificationService.SendNotificationAsync($"New todo created: {@event.Title}");
        }
    }
}
```

### 8.5 Caching Strategy

#### **Cache-Aside Pattern**

```csharp
public async Task<TodoDto> GetTodoAsync(Guid id)
{
    var cacheKey = $"todo:{id}";

    // Try get from cache
    var cached = await _cacheService.GetAsync<TodoDto>(cacheKey);
    if (cached != null)
        return cached;

    // Get from DB
    var todo = await _repository.GetByIdAsync(id);
    if (todo == null)
        throw new NotFoundException("Todo not found");

    var result = _mapper.Map<TodoDto>(todo);

    // Store in cache
    await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(1));

    return result;
}
```

#### **Invalidate Cache**

```csharp
public async Task UpdateTodoAsync(UpdateTodoCommand command)
{
    var todo = await _repository.GetByIdAsync(command.Id);

    // Update
    todo.Title = command.Title;
    todo.Description = command.Description;
    await _repository.UpdateAsync(todo);

    // Invalidate cache
    var cacheKey = $"todo:{command.Id}";
    await _cacheService.RemoveAsync(cacheKey);

    // Also invalidate user's todo list cache
    var userCacheKey = $"todos:{todo.UserId}:*";
    await _cacheService.RemoveByPatternAsync(userCacheKey);
}
```

---

## 9. Kiểm Thử

### 9.1 Unit Testing

**Cấu trúc thư mục test:**
```
Tests/
├── Application.Tests/
│   ├── Features/
│   │   ├── TodoItems/
│   │   │   ├── CreateTodoHandlerTests.cs
│   │   │   ├── UpdateTodoHandlerTests.cs
│   │   │   └── GetMyTodosQueryHandlerTests.cs
│   │   └── Auth/
│   │       └── LoginCommandHandlerTests.cs
│   └── Mappers/
│       └── TodoProfileTests.cs
├── Domain.Tests/
│   └── Entities/
│       └── TodoItemTests.cs
└── Api.Tests/
    └── Controllers/
        └── TodoItemsControllerTests.cs
```

**Ví dụ Unit Test (xUnit + Moq):**

```csharp
// Application.Tests/Features/TodoItems/CreateTodoHandlerTests.cs
public class CreateTodoHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITodoItemRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IKafkaPublisher> _kafkaPublisherMock;
    private readonly CreateTodoHandler _handler;

    public CreateTodoHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<ITodoItemRepository>();
        _mapperMock = new Mock<IMapper>();
        _kafkaPublisherMock = new Mock<IKafkaPublisher>();

        _handler = new CreateTodoHandler(
            _unitOfWorkMock.Object,
            _repositoryMock.Object,
            _mapperMock.Object,
            _kafkaPublisherMock.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateTodoAndPublishEvent()
    {
        // Arrange
        var command = new CreateTodoCommand
        {
            Title = "Test Todo",
            Description = "Test Description",
            Priority = PriorityLevel.High,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var todoItem = new TodoItem { Id = Guid.NewGuid(), Title = command.Title };

        _mapperMock.Setup(m => m.Map<TodoItem>(command)).Returns(todoItem);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _kafkaPublisherMock.Setup(k => k.PublishAsync(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(todoItem.Id, result);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
        _kafkaPublisherMock.Verify(k => k.PublishAsync(It.IsAny<string>(), It.IsAny<object>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateTodoCommand { Title = "" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
```

### 9.2 Integration Testing

```csharp
// Api.Tests/Controllers/TodoItemsControllerTests.cs
[Collection("Integration Tests")]
public class TodoItemsControllerIntegrationTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public TodoItemsControllerIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Setup test data
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetMyTodos_WithValidToken_ShouldReturnTodos()
    {
        // Arrange
        var token = GenerateTestJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task CreateTodo_WithValidCommand_ShouldReturn201Created()
    {
        // Arrange
        var token = GenerateTestJwtToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new CreateTodoCommand
        {
            Title = "Test Todo",
            Priority = PriorityLevel.High
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(command),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await _client.PostAsync("/api/todos", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private string GenerateTestJwtToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-min-32-chars-long!!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "test@example.com")
        };

        var token = new JwtSecurityToken(
            issuer: "todo-api",
            audience: "todo-client",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### 9.3 Chạy Tests

```bash
# Run all tests
dotnet test

# Run specific project
dotnet test Application.Tests/Application.Tests.csproj

# Run with verbose output
dotnet test --verbosity detailed

# Run with code coverage
dotnet test /p:CollectCoverage=true

# Run specific test class
dotnet test --filter FullyQualifiedName~CreateTodoHandlerTests

# Run specific test method
dotnet test --filter Name~Handle_WithValidCommand_ShouldCreateTodoAndPublishEvent
```

---

## Troubleshooting

### 10.1 Lỗi Connection

**Lỗi:** `Connection to PostgreSQL failed`

**Giải pháp:**
```bash
# Kiểm tra PostgreSQL đang chạy
psql --version
pg_isready -h localhost -p 5432

# Restart PostgreSQL
sudo systemctl restart postgresql  # Linux
# hoặc
brew services restart postgresql   # macOS

# Kiểm tra connection string
Connection String: "Host=localhost;Port=5432;Database=todo_db;Username=postgres;Password=postgres;"
```

### 10.2 Lỗi Migration

**Lỗi:** `The migration '...' has already been applied to the database`

**Giải pháp:**
```bash
# Xem migrations đã được apply
dotnet ef migrations list

# Rollback migration cuối cùng
dotnet ef database update <previous_migration>

# Xóa migration
dotnet ef migrations remove

# Tạo lại migration
dotnet ef migrations add Initial
dotnet ef database update
```

### 10.3 Lỗi JWT Token

**Lỗi:** `401 Unauthorized`

**Giải pháp:**
```bash
# Kiểm tra token format
# Header: Authorization: Bearer <token>

# Kiểm tra secret key dài >= 32 characters
# Kiểm tra token expiration time

# Invalidate cache để lấy token mới
```

### 10.4 Lỗi Redis Connection

**Lỗi:** `StackExchange.Redis.RedisConnectionException`

**Giải pháp:**
```bash
# Kiểm tra Redis chạy
redis-cli ping
# Output: PONG

# Kiểm tra connection string
Redis: "localhost:6379"

# Restart Redis
redis-server
```

### 10.5 Lỗi Kafka Connection

**Lỗi:** `Confluent.Kafka.KafkaException: Local: Broker transport failure`

**Giải pháp:**
```bash
# Kiểm tra Kafka chạy
# Start Zookeeper (Windows)
bin/windows/zookeeper-server-start.bat config/zookeeper.properties

# Start Kafka Broker
bin/windows/kafka-server-start.bat config/server.properties

# Kiểm tra topic
kafka-topics.sh --list --bootstrap-server localhost:9092
```

### 10.6 Lỗi Build

**Lỗi:** `The type or namespace name 'X' does not exist`

**Giải pháp:**
```bash
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Build lại
dotnet build

# Nếu vẫn lỗi, check file references (.csproj)
```

---

## 11. Best Practices

### 11.1 Coding Conventions

✅ **DO:**
- Sử dụng meaningful names cho variables, methods, classes
- Viết async/await thay vì blocking calls
- Sử dụng DTOs để transfer data giữa layers
- Implement proper error handling
- Use dependency injection
- Write unit tests cho business logic

❌ **DON'T:**
- Không dùng magic strings/numbers
- Không bắt tất cả exceptions với `catch (Exception ex)`
- Không để connection strings cứng trong code
- Không bỏ qua input validation
- Không lạm dụng try-catch

### 11.2 Security Best Practices

1. **Authentication & Authorization**
   - Luôn validate JWT token
   - Sử dụng HTTPS in production
   - Implement rate limiting
   - Sanitize user inputs

2. **Database Security**
   - Parameterize queries (EF Core làm sẵn)
   - Mã hóa sensitive data
   - Use connection pooling
   - Regular backups

3. **API Security**
   - CORS policy phù hợp
   - Remove sensitive info từ error messages
   - Implement API versioning
   - Use API keys cho external services

### 11.3 Performance Best Practices

1. **Caching**
   - Cache frequently accessed data
   - Use Redis for distributed cache
   - Implement cache invalidation strategy
   - Monitor cache hit ratio

2. **Database**
   - Use indexes cho frequently queried columns
   - Implement pagination
   - Use projection (`.Select()`) để giảm data transferred
   - Monitor slow queries

3. **Async Programming**
   - Use async/await consistently
   - Avoid blocking calls (`.Result`, `.Wait()`)
   - Use `Task.WhenAll()` cho parallel operations
   - Implement proper timeout handling

### 11.4 Code Organization

```
✅ Organized Structure:
- Features grouped by domain (TodoItems, Categories, etc.)
- Clear separation of concerns (Commands, Queries, Handlers)
- Reusable mappers and profiles
- Common utilities in shared project

❌ Avoid:
- God classes
- Circular dependencies
- Mixed business logic with presentation
- Hardcoded values in code
```

### 11.5 Documentation

- Thêm XML comments cho public APIs
- Maintain architecture documentation
- Update README khi có changes
- Document configuration options
- Keep API documentation in Swagger

```csharp
/// <summary>
/// Retrieves a list of todos for the current user
/// </summary>
/// <param name="query">Query parameters for filtering and pagination</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>List of todos</returns>
/// <exception cref="UnauthorizedAccessException">Thrown when user is not authenticated</exception>
public async Task<List<TodoDto>> Handle(GetMyTodosQuery query, CancellationToken cancellationToken)
{
    // Implementation
}
```

### 11.6 Deployment Best Practices

1. **Environment Configuration**
   - Use different appsettings per environment
   - Store secrets in Secret Manager / Key Vault
   - Configure logging appropriately
   - Use environment variables

2. **Database Migration**
   - Always backup before migration
   - Test migrations in staging first
   - Keep rollback plan
   - Document schema changes

3. **Monitoring & Logging**
   - Implement structured logging (Serilog)
   - Monitor application health
   - Set up alerts for errors
   - Track performance metrics

4. **Release Process**
   - Use CI/CD pipeline
   - Automated testing
   - Stage deployment
   - Gradual rollout (canary deployment)

---

## 📞 Liên Hệ & Hỗ Trợ

- **Repository:** https://github.com/thuanlevan72/CLean_Project
- **Tác Giả:** thuanlevan72
- **Issues:** https://github.com/thuanlevan72/CLean_Project/issues
- **Discussions:** https://github.com/thuanlevan72/CLean_Project/discussions

## 📝 License

Dự án này được cấp phép dưới MIT License. Xem file [LICENSE](LICENSE) để biết thêm chi tiết.

---

**Cập nhật lần cuối:** 2024-12-21
**Phiên bản:** 1.0.0
**Status:** Active Development ✨
