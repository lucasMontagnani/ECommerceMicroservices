# ECommerceMicroservices

This is a Cloud-Native E-Commerce Microservices Application built with .NET 8, with modern software architecture patterns and best practices for distributed systems.

The system is composed of several modular microservices — Catalog, Basket, Discount, Ordering, YARP API Gateway, and a Shopping Web Client — each responsible for a specific business capability. These services interact through both gRPC (synchronous communication) and RabbitMQ (event-driven communication), providing a robust and scalable architecture.

Data is persisted using a mix of NoSQL (e.g., PostgreSQL Document DB with Marten, Redis) and relational databases (e.g., SQLite, SQL Server), showcasing flexibility in data storage strategies. All client requests are routed through the YARP Gateway, enabling centralized routing, transformation, and security policies.

This project is ideal for learning and applying key principles of microservices, such as domain-driven design (DDD), CQRS, clean architecture, and container orchestration with Docker.

## ✅ Technologies & Concepts Covered
- ASP.NET Core 8 - Minimal APIs, Web APIs, Razor Pages
- Microservices Architecture - Product, Basket, Discount, Ordering
- Event-Driven Communication - RabbitMQ + MassTransit
- Sync Communication - gRPC
- YARP API Gateway - Reverse Proxy with Rate Limiting
- Data Stores - PostgreSQL (Marten), Redis, SQLite, SQL Server
- CQRS & DDD - With MediatR, FluentValidation, Mapster
- Vertical Slice & Clean Architecture - Feature folders, SOLID principles
- Containerization - Docker & Docker Compose
- Refit HTTP Clients - Typed clients for API communication

## 🧩 Microservices Breakdown
### 📦 Catalog Microservice
- **Minimal APIs** with **Carter** and **Vertical Slice Architecture**
- **Marten** for Document DB on **PostgreSQL**
- **CQRS** with **MediatR** & **FluentValidation**
- Docker support, health checks, global exception handling

### 🛒 Basket Microservice
- RESTful Web API with **Redis caching**
- **Decorator**, **Proxy** & **Cache-aside** Patterns
- **gRPC client** to Discount service
- Publishes BasketCheckout event via **RabbitMQ** and **MassTransit**

### 🎁 Discount Microservice
- **gRPC server** with Protobuf messages
- **SQLite** + **EF Core** with Docker support
- High-performance **inter-service communication** (with basket)

### 📑 Ordering Microservice
- Implements **DDD**, **CQRS**, and **Clean Architecture**
- Uses **RabbitMQ **to consume BasketCheckout event
- **SQL Serve**r with **EF Core** and automatic migrations

### 🌐 YARP API Gateway
- Central **API Gateway** with Reverse Proxy using **YARP**
- Configurable routes, clusters, and transformations
- Rate limiting and gateway routing pattern

### 🛍️ ShoppingApp WebUI
- ASP.NET Core **Razor Pages** + **Bootstrap 4**
- Communicates via **Refit-generated HTTP clients**
- **Razor components**, validation, and clean UX patterns

## 🙌 Acknowledgments
This project was built following the Microservices on .NET 8 course by Mehmet Ozkaya.
