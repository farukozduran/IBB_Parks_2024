# 🏞️ IBB Parks 2024

IBB Parks 2024 is a modular, layered .NET Core web application designed to manage and monitor public park information across Istanbul. It integrates caching, background job processing, external API consumption, and queue-based asynchronous operations.

---

## 🚀 Features

- ✅ RESTful API for accessing park data
- 🌍 District-based filtering of parks
- 🔄 Background update jobs via RabbitMQ
- 🧠 Redis-based caching system
- 🔐 JWT authentication support for protected endpoints
- 🗂️ Layered architecture with dependency injection

---

## 🏗️ Project Structure

```
📁 IBB.Nesine
├── 📁 IBB.Nesine.API → Web API controllers
├── 📁 IBB.Nesine.Services → Business logic layer
├── 📁 IBB.Nesine.Data → DB provider (Dapper-based)
├── 📁 IBB.Nesine.Caching → Redis/Memory cache abstractions
└── 📁 IBB.Nesine.UI → MVC or frontend layer
```


---

## 📡 API Endpoints

### 🏞️ ParksController

| Method | Route                              | Auth     | Description                                |
|--------|------------------------------------|----------|--------------------------------------------|
| POST   | `/api/parks/UpdateParksInfo`       | ❌       | Fetches and queues new park data from API  |
| GET    | `/api/parks/GetParkByDistrict`     | ❌       | Lists parks by district                    |
| GET    | `/api/parks/GetParkAvailabilityByParkId` | ✅ | Checks availability status of a park       |
| GET    | `/api/parks/GetAllParks`           | ❌       | Returns cached list of all parks           |

### 👤 UserController

| Method | Route           | Auth | Description              |
|--------|------------------|------|--------------------------|
| POST   | `/api/user/register` | ❌ | User registration         |
| POST   | `/api/user/login`    | ❌ | User login (returns JWT) |

---

## 🧰 Technologies Used

- **ASP.NET Core Web API**
- **C#**
- **Redis / MemoryCache**
- **RabbitMQ** – for queue-based updates
- **Dapper** – lightweight data access
- **SQL Server** – stored procedures
- **JWT** – token-based authentication
- **Newtonsoft.Json** – serialization
- **DataTable Conversion Helpers**

---

## 🛠️ How It Works

### Park Data Update Flow:
1. `/api/parks/UpdateParksInfo` triggers an async request to an external API.
2. Data is batched into groups of 50.
3. Each batch is serialized and published to a RabbitMQ queue.
4. `UpdateAvailableParksInfoJobConsumer` listens to this queue.
5. On message arrival, data is deserialized and sent to the database via `usp_SetIsAvailable`.

👨‍💻 Developed By

Faruk Furkan Özduran
Backend Developer (.NET)

