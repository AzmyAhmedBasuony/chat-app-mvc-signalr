# Chat Application Upgrade - Complete Implementation Guide

## ? What Has Been Implemented

### 1. **Database Models**
- ? **User** - User authentication and profile
- ? **PrivateMessage** - One-to-one chat messages
- ? **UserConnection** - Track active SignalR connections
- ? **ChatMessage** - Legacy group chat messages (kept for backward compatibility)

### 2. **DTOs (Data Transfer Objects)**
- ? LoginDto, LoginResponseDto
- ? UserDto, MessageDto, SendMessageDto
- ? ChatHistoryDto

### 3. **Services**
- ? **AuthService** - JWT token generation and validation
- ? **ChatService** - Chat operations (send messages, get history, mark as read)

### 4. **Repository Pattern**
- ? Generic Repository
- ? Unit of Work with all entities

### 5. **API Controllers**
- ? **AuthController** - Login endpoint
- ? **MessagesController** - Chat operations (protected with JWT)

### 6. **SignalR Hub**
- ? **ChatHub** - Real-time messaging with authentication
  - Online/Offline status tracking
  - Private messaging
  - Typing indicators
  - Message read receipts

### 7. **Authentication**
- ? JWT Bearer authentication
- ? SignalR JWT integration
- ? 5 seed users with password: "Password123"

### 8. **Seed Data (5 Demo Users)**
| Username | Password | Full Name | Email |
|----------|----------|-----------|-------|
| alice | Password123 | Alice Johnson | alice@chat.com |
| bob | Password123 | Bob Smith | bob@chat.com |
| charlie | Password123 | Charlie Brown | charlie@chat.com |
| diana | Password123 | Diana Prince | diana@chat.com |
| eve | Password123 | Eve Davis | eve@chat.com |

## ?? Next Steps to Complete Setup

### Step 1: Stop the Running Application
**You need to stop the currently running SignalrMvc application** before updating the database.

In Visual Studio:
- Press `Shift + F5` or click Stop Debugging button
- Or close the browser and wait for the app to stop

### Step 2: Update Database
After stopping the app, run:

```powershell
dotnet ef database update
```

This will create all the new tables:
- Users
- PrivateMessages
- UserConnections
- And seed the 5 demo users

### Step 3: Create Login View
Create `SignalrMvc\Views\Auth\Login.cshtml`:

```html
@{
    ViewData["Title"] = "Login";
    Layout = null;
}

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Chat App</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <style>
        body {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .login-container {
            max-width: 400px;
            width: 100%;
            padding: 20px;
        }
        .login-card {
            background: white;
            border-radius: 15px;
            padding: 40px;
            box-shadow: 0 10px 40px rgba(0,0,0,0.2);
        }
        .login-header {
            text-align: center;
            margin-bottom: 30px;
        }
        .login-header h2 {
            color: #667eea;
            font-weight: 700;
        }
        .btn-login {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border: none;
            color: white;
            padding: 12px;
            border-radius: 25px;
            font-weight: 600;
            width: 100%;
        }
        .btn-login:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }
        .form-control {
            border-radius: 25px;
            padding: 12px 20px;
            border: 2px solid #e5e7eb;
        }
        .form-control:focus {
            border-color: #667eea;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
        }
        .demo-users {
            margin-top: 20px;
            padding: 15px;
            background: #f8f9fa;
            border-radius: 10px;
            font-size: 0.85rem;
        }
        .alert {
            border-radius: 10px;
        }
    </style>
</head>
<body>
    <div class="login-container">
        <div class="login-card">
            <div class="login-header">
                <h2>?? Chat App</h2>
                <p class="text-muted">Login to start chatting</p>
            </div>

            <div id="errorMessage" class="alert alert-danger" style="display:none;"></div>

            <form id="loginForm">
                <div class="mb-3">
                    <input type="text" class="form-control" id="username" placeholder="Username" required />
                </div>
                <div class="mb-3">
                    <input type="password" class="form-control" id="password" placeholder="Password" required />
                </div>
                <button type="submit" class="btn btn-login">Login</button>
            </form>

            <div class="demo-users">
                <strong>Demo Users:</strong><br />
                alice, bob, charlie, diana, eve<br />
                <strong>Password:</strong> Password123
            </div>
        </div>
    </div>

    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script>
        $(document).ready(function() {
            $('#loginForm').on('submit', async function(e) {
                e.preventDefault();
                
                const username = $('#username').val();
                const password = $('#password').val();

                try {
                    const response = await fetch('/api/auth/login', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({ username, password })
                    });

                    if (response.ok) {
                        const data = await response.json();
                        
                        // Store token and user info
                        localStorage.setItem('token', data.token);
                        localStorage.setItem('userId', data.userId);
                        localStorage.setItem('username', data.username);
                        localStorage.setItem('fullName', data.fullName || data.username);
                        localStorage.setItem('profileImage', data.profileImage || '');

                        // Redirect to chat
                        window.location.href = '/Chat/Index';
                    } else {
                        const error = await response.json();
                        $('#errorMessage').text(error.message || 'Invalid username or password').show();
                    }
                } catch (error) {
                    $('#errorMessage').text('Login failed. Please try again.').show();
                }
            });
        });
    </script>
</body>
</html>
```

### Step 4: Create New Chat View with User List

Create `SignalrMvc\Views\Chat\Index.cshtml` (This will be a complete new chat interface)

### Step 5: Create AuthController View

Create controller action in `SignalrMvc\Controllers\AuthController.cs`:

```csharp
[HttpGet]
public IActionResult Login()
{
    return View();
}
```

## ?? API Endpoints

### Authentication
- `POST /api/auth/login` - Login and get JWT token
- `GET /api/auth/users` - Get all users (for testing)

### Messages (Requires JWT)
- `GET /api/messages/users` - Get all users except current user
- `GET /api/messages/history/{otherUserId}` - Get chat history with specific user
- `POST /api/messages/send` - Send a message
- `POST /api/messages/mark-read/{senderId}` - Mark messages as read
- `GET /api/messages/unread-count` - Get total unread count
- `GET /api/messages/unread-counts-per-user` - Get unread counts per user

### SignalR Hub Methods

#### Client -> Server
- `SendPrivateMessage(receiverId, message)` - Send private message
- `MarkMessagesAsRead(senderId)` - Mark messages as read
- `SendTypingIndicator(receiverId, isTyping)` - Show typing indicator

#### Server -> Client
- `ReceivePrivateMessage(messageDto)` - Receive new message
- `MessageSent(messageDto)` - Confirmation message was sent
- `UserStatusChanged(userId, isOnline)` - User online/offline status
- `UpdateUserCount(count)` - Online users count
- `UserTyping(userId, isTyping)` - Someone is typing
- `MessagesRead(userId)` - Someone read your messages
- `Error(message)` - Error occurred

## ?? Database Schema

### Users Table
- Id (PK)
- Username (Unique)
- PasswordHash
- FullName
- Email (Unique)
- ProfileImage
- IsOnline
- LastSeen
- CreatedAt

### PrivateMessages Table
- Id (PK)
- SenderId (FK)
- ReceiverId (FK)
- Message
- IsRead
- SentAt
- ReadAt

### UserConnections Table
- Id (PK)
- UserId (FK)
- ConnectionId (Unique)
- ConnectedAt

## ?? How to Use

1. **Login**: Navigate to `/` (redirects to `/Auth/Login`)
2. **Select User**: Choose from alice, bob, charlie, diana, or eve
3. **Password**: Use "Password123" for all users
4. **Chat**: Select a user from the list to start chatting
5. **Real-time**: See online/offline status, typing indicators, read receipts

## ?? Configuration

### appsettings.json
- `ConnectionStrings:DefaultConnection` - SQL Server connection
- `Jwt:Key` - Secret key for JWT (already configured)
- `Jwt:Issuer` - Token issuer (already configured)
- `Jwt:Audience` - Token audience (already configured)

## ?? Features Implemented

? JWT Authentication
? User Login System
? Online/Offline Status
? Private One-to-One Messaging
? Message History
? Unread Message Counts
? Typing Indicators
? Read Receipts
? Real-time Updates via SignalR
? Repository Pattern & Unit of Work
? Entity Framework Code First
? 5 Seed Demo Users
? Password Hashing with BCrypt
? Clean Architecture

## ?? Testing Flow

1. Open two different browsers
2. Login as "alice" in first browser
3. Login as "bob" in second browser
4. See both users show as online
5. Send messages between alice and bob
6. See real-time delivery and read receipts
7. Close bob's browser
8. See bob go offline in alice's view

## ?? NuGet Packages Added

- Microsoft.AspNetCore.Authentication.JwtBearer 8.0.0
- BCrypt.Net-Next 4.0.3
- (Already had: Entity Framework Core, SignalR, System.IdentityModel.Tokens.Jwt)

## ?? Important Notes

1. **Stop the app** before running `dotnet ef database update`
2. The migration is already created: `AddUserAuthAndPrivateMessaging`
3. JWT tokens expire after 24 hours
4. Default route is now `/Auth/Login`
5. Old group chat functionality is preserved for backward compatibility

---

**Created by**: GitHub Copilot
**Date**: 2025
**Version**: 1.0
