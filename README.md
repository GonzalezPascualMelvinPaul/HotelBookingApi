

📄 **README.md**

````md
# 🏨 API de Reservas de Hotel - .NET 5

API REST para gestionar habitaciones y reservas de un hotel, desarrollada en **.NET 5** con **Entity Framework Core**, **JWT Authentication**, **Serilog**, y **xUnit** para pruebas unitarias.

---

## 🚀 Tecnologías Utilizadas

- **.NET 5**
- **Entity Framework Core** (SQL Server)
- **Autenticación con JWT (Roles & Policies)**
- **Serilog** (Logging centralizado)
- **xUnit + Moq** (Pruebas unitarias)
- **Swagger** (Documentación API)

---

## 🛠 Instalación y Configuración

### 1️⃣ **Clonar el repositorio**

```sh
git clone https://github.com/GonzalezPascualMelvinPaul/HotelBookingApi.git
cd HotelBookingApi
```
````

### 2️⃣ **Instalar Dependencias**

```sh
dotnet restore
```

### 3️⃣ **Configurar la Base de Datos (SQL Server)**

- Modifica `appsettings.json` en `HotelBooking.Api` con tu conexión:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HotelBookingDB;User Id=sa;Password=TuPassword123;"
}
```

- **Ejecutar migraciones y actualizar la BD**:

```sh
dotnet ef migrations add InitialCreate --project HotelBooking.Infrastructure --startup-project HotelBooking.Api
dotnet ef database update --project HotelBooking.Infrastructure --startup-project HotelBooking.Api
```

### 4️⃣ **Ejecutar la API**

```sh
dotnet run --project HotelBooking.Api
```

- **Abrir en el navegador**:  
  👉 [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## 🔑 **Autenticación JWT**

### **📌 Crear Usuario (Register)**

```sh
POST /api/auth/register
{
  "username": "admin",
  "email": "admin@email.com",
  "password": "Admin123!",
  "role": "Admin"
}
```

### **📌 Obtener Token (Login)**

```sh
POST /api/auth/login
{
  "email": "admin@email.com",
  "password": "Admin123!"
}
```

📌 **Respuesta**:

```json
{
  "token": "eyJhbGciOiJIUzI1..."
}
```

### **📌 Usar Token en Peticiones Protegidas**

Agregar el token en los **headers**:

```
Authorization: Bearer TU_TOKEN_AQUI
```

---

## 📌 **Endpoints Principales**

### 🔹 **Habitaciones**

| Método   | Ruta              | Acceso                         | Descripción                    |
| -------- | ----------------- | ------------------------------ | ------------------------------ |
| `GET`    | `/api/rooms`      | Usuarios autenticados          | Obtener todas las habitaciones |
| `GET`    | `/api/rooms/vip`  | Usuarios con email corporativo | Obtener habitaciones VIP       |
| `POST`   | `/api/rooms`      | Admins                         | Crear una habitación           |
| `PUT`    | `/api/rooms/{id}` | Admins / Gerentes              | Editar una habitación          |
| `DELETE` | `/api/rooms/{id}` | Admins                         | Eliminar una habitación        |

### 🔹 **Reservaciones**

| Método   | Ruta                     | Acceso                 | Descripción                     |
| -------- | ------------------------ | ---------------------- | ------------------------------- |
| `GET`    | `/api/reservations`      | Usuarios autenticados  | Obtener todas las reservaciones |
| `POST`   | `/api/reservations`      | Usuarios autenticados  | Crear una reservación           |
| `DELETE` | `/api/reservations/{id}` | Admins / Usuario dueño | Cancelar una reservación        |

---

## ✅ **Pruebas Unitarias con xUnit**

### **📌 Instalar Dependencias**

```sh
dotnet add HotelBooking.Tests package xunit
dotnet add HotelBooking.Tests package Moq
dotnet add HotelBooking.Tests package Microsoft.EntityFrameworkCore.InMemory
dotnet add HotelBooking.Tests package Microsoft.AspNetCore.Mvc.Testing
```

### **📌 Ejecutar Pruebas**

```sh
dotnet test
```

### **📌 Ejemplo de Prueba para `RoomsController`**

📄 **HotelBooking.Tests/RoomsControllerTests.cs**

```csharp
[Fact]
public async Task GetVipRooms_Should_Return_Only_VIP_Rooms()
{
    // Arrange
    var mockRepo = new Mock<IRoomRepository>();
    var vipRooms = new List<Room>
    {
        new Room { Id = 2, RoomNumber = "102", Type = "VIP", Price = 200, IsAvailable = true },
        new Room { Id = 3, RoomNumber = "103", Type = "VIP", Price = 250, IsAvailable = false }
    };

    mockRepo.Setup(repo => repo.GetVipRoomsAsync()).ReturnsAsync(vipRooms);
    var controller = new RoomsController(mockRepo.Object);

    // Act
    var result = await controller.GetVipRooms();

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = okResult.Value as Dictionary<string, object>;

    Assert.NotNull(response);
    Assert.True(response.ContainsKey("rooms"));
}
```

---

## 📌 **Logging con Serilog**

📄 **Configurar en `Program.cs`**

```csharp
using Serilog;
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console();
    config.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});
```

📄 **Ejemplo de Registro en Controladores**

```csharp
private readonly ILogger<RoomsController> _logger;

public RoomsController(IRoomRepository roomRepository, ILogger<RoomsController> logger)
{
    _roomRepository = roomRepository;
    _logger = logger;
}

[HttpGet]
public async Task<IActionResult> GetRooms()
{
    _logger.LogInformation("Obteniendo todas las habitaciones");
    var rooms = await _roomRepository.GetAllRoomsAsync();
    return Ok(new { status = 200, rooms });
}
```

📌 **Los logs se guardarán en `logs/log-YYYYMMDD.txt`.**

---

## 📩 **Contacto**

🔹 Desarrollado por: **Melvin Paul Gonzalez Pascual**  
🔹 Email: [gonzalezpascualmelvinpaul@gmail.com](mailto:gonzalezpascualmelvinpaul@gmail.com)  
🔹 GitHub: [https://github.com/GonzalezPascualMelvinPaul](https://github.com/GonzalezPascualMelvinPaul)
