# Simple Login Form with ASP.NET 6.0 and ReactJS

This project is a simple login form application that uses ASP.NET 6.0 for backend and ReactJS for frontend. This login form handles authentication using JWT and refresh tokens, and it also implements time-based OTP for two-factor authentication.

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js](https://nodejs.org/en/download/) (version 14 or higher)

### Installation

1. Clone the repository:

```
git clone https://github.com/MKhoaaa/login-form.git
```

2. Set up the required environment variables `SECRET_ENV_VARIABLE` used for encrypting and decrypting JWT:

#### Windows (Command Prompt)
```
setx SECRET_ENV_VARIABLE "your_secret_value"
```

#### Windows (PowerShell)
```powershell
[Environment]::SetEnvironmentVariable("SECRET_ENV_VARIABLE", "your_secret_value", "User")
```

#### macOS and Linux
```bash
export SECRET_ENV_VARIABLE="your_secret_value"
```

3. Install frontend dependencies:

```
cd client
npm install
```

## Running the Application

### Backend

1. Make sure you are in the root folder of the project:

```
cd ..
```

2. Run the backend on port 3000:

```
dotnet run
```

### Frontend

1. Make sure you are in the `client` folder:

```
cd client
```

2. Run the frontend on port 3001:

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
- Dependency Injection pattern for API calls
- Pages are accessible based on user's authentication status
- Two-factor authentication handling:
  - Display QR code for scanning with an authenticator app
  - Require OTP input when two-factor authentication is enabled
