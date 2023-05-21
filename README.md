# Simple Login Form with ASP.NET 6.0 and ReactJS

This project is a simple login form application that uses ASP.NET 6.0 for backend and ReactJS for frontend. The backend handles authentication using JWT and refresh tokens, and it also implements time-based OTP for two-factor authentication. The frontend is responsive, using Bootstrap, and it follows the Dependency Injection/Service pattern inspired by Angular.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js](https://nodejs.org/en/download/) (version 14 or higher)

### Installation

1. Clone the repository

```
git clone https://github.com/MKhoaaa/login-form.git
```

2. Install frontend dependencies

```
cd client
npm install
```

## Running the Application

### Backend

1. Make sure you are in the root folder of the project

```
cd ..
```

2. Run the backend on port 3000

```
dotnet run
```

### Frontend

1. Make sure you are in the `client` folder

```
cd client
```

2. Run the frontend on port 3001

```
npm start
```

Now, you can access the application in your browser at `http://localhost:3001`.

## Features

### Backend

- ASP.NET 6.0 for building the API
- JWT and refresh tokens stored in HTTP only cookie for authentication
- Time-based OTP for two-factor authentication
- Custom API Filters for handling user's authentication status

### Frontend

- ReactJS for building the UI
- Responsive design using Bootstrap
- Dependency Injection/Service pattern inspired by Angular
- Pages are accessible based on user's authentication status
- Two-factor authentication handling:
  - Displaying QR code for scanning with an authenticator app
  - Requiring OTP input when two-factor authentication is enabled

## License

This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/your_username_/simple-login-form/blob/main/LICENSE.md) file for details.
