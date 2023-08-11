# Simple E-Commerce Web API with ASP.NET Core 6.0

This project is for learning purposes only. It is not intended to be used in production. It implements API endpoints for a simple e-commerce application, including authentication and authorization, product management, and order management.

## Getting Started

1. Clone the repository:

```
git clone https://github.com/khoa288/ecommerce-web-api.git
```

2. Open the `EcommerceWebApi.sln` solution file in Visual Studio. Make sure you have installed the [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0).

3. Run the application:

```
dotnet run
```

## Features

- Authentication and authorization using JWT and refresh tokens
- Time-based OTP for two-factor authentication
- Using JSON flat file as database
- Support transaction for database operations
- Pagination, sorting and filtering for API endpoints
- Real-time notification using SignalR
- Logging to file using Serilog
- Using Dependency Injection for services, Repositories and Unit of Work patterns for data access

## API Endpoints

### Authentication

- POST `/register`
- POST `/login`
- POST `/loginSecondFactor`
- DELETE `/revokeToken`
- GET `/isAuthenticated`
- GET `/getQrCode`
- POST `/validateTotp`

### Users

- GET `/getUsers`
- GET `/getUserInfo`
- POST `/updateUser`
- POST `/changeUserPassword`
- POST `/changeUserTwoFactorStatus`
- DELETE `/deleteUser`

### Products

- GET `/getProducts`
- GET `/getProductById/{id}`
- POST `/insertProduct`
- PUT `/updateProduct`
- DELETE `/deleteProduct/{id}`

### Orders

- GET `/getOrders`
- GET `/getOrderById/{id}`
- POST `/insertOrder`
- PUT `/updateOrderStatus/{id}`
- DELETE `/deleteOrder/{id}`
