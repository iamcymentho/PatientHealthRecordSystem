# Patient Health Records (PHR) System

A comprehensive Patient Health Records management system built with .NET 9.0, featuring enterprise-level authentication, authorization, and access control mechanisms.

---

## Table of Contents

- [Technical Overview](#technical-overview)
- [Features Implemented](#features-implemented)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [User Stories](#user-stories)
- [API Endpoints](#api-endpoints)
- [Test Data](#test-data)
- [Database Schema](#database-schema)

---

## Technical Overview

### Technology Stack

- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API development
- **Entity Framework Core** - ORM with SQLite database
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Request validation
- **JWT (JSON Web Tokens)** - Authentication and authorization
- **Repository Pattern** - Data access abstraction
- **Decorator Pattern** - Caching implementation
- **Clean Architecture** - Separation of concerns

### Project Structure

```
PHR/
├── PHR.Domain/              # Domain entities and enums
├── PHR.Application/         # Business logic, commands, queries
├── PHR.Infrastructure/      # Data access, external services
├── PHR.Persistence/         # Database context and repositories
└── PHR.Api/                 # Web API controllers and services
```

---

## Features Implemented

### 1. Authentication & Authorization

#### Core Authentication
- **User Registration** - Self-service user registration with email validation
- **User Login** - Email/password authentication with JWT token generation
- **Token Refresh** - Automatic token renewal without re-login
- **Logout** - Secure session termination with token invalidation

#### Admin User Management
- **Admin User Creation** - Administrators can create users with default passwords
- **Force Password Change** - Users created by admin must change password on first login
- **Role Assignment** - Assign multiple roles to users during creation

#### Password Management (Enterprise-Level)
- **Change Password** - Authenticated users can change their own passwords
- **Reset Password Request** - Generate password reset tokens for users
- **Reset Password** - Reset password using secure tokens (1-hour expiry)
- **Password History Tracking** - Track last password change timestamp

**Technical Highlights:**
- 8-character alphanumeric reset tokens
- Token expiry validation (1 hour)
- Single-use token enforcement
- Password hash storage (no plaintext)
- RequirePasswordChange flag for first-time login enforcement

### 2. Patient Records Management

- **Create Patient Records** - Add new patient health records
- **View Patient Records** - Retrieve patient information
- **Update Patient Records** - Modify existing records
- **Delete Patient Records** - Soft delete with audit trail
- **Search & Filter** - Advanced querying capabilities

### 3. Access Request System

#### Access Control Workflow
- **Request Access** - Users can request access to patient records
- **Pending Requests** - View all pending access requests
- **Approve Access** - Authorize access with time-bound permissions
- **Decline Access** - Reject access requests with reasons
- **Time-Bound Access** - Set start and end dates for access

**Access Request States:**
- Pending (0)
- Approved (1)
- Declined (2)

### 4. Security Features

- **JWT-Based Authentication** - Secure stateless authentication
- **Role-Based Access Control (RBAC)** - Fine-grained permissions
- **Policy-Based Authorization** - Custom authorization policies
- **Password Hashing** - Secure password storage
- **Token Expiration** - Automatic token invalidation
- **Cache Invalidation** - Automatic cache clearing on data updates

### 5. Performance Optimizations

- **Redis Caching** - In-memory caching for frequently accessed data
- **Repository Caching Decorator** - Transparent caching layer
- **Cache Invalidation Patterns** - Automatic cache clearing on updates
- **Optimized Database Queries** - EF Core query optimization

---

## Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────┐
│          PHR.Api (Presentation)         │
│  Controllers, Middleware, Filters       │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│      PHR.Application (Use Cases)        │
│  Commands, Queries, Handlers, DTOs      │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│    PHR.Infrastructure (External)        │
│  Repositories, Services, Configurations │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│       PHR.Domain (Core Business)        │
│  Entities, Enums, Domain Logic          │
└─────────────────────────────────────────┘
```

### Key Design Patterns

1. **CQRS (Command Query Responsibility Segregation)**
   - Separate read and write operations
   - MediatR for command/query handling

2. **Repository Pattern**
   - Abstract data access logic
   - Testable data layer

3. **Decorator Pattern**
   - Caching decorator for repositories
   - Transparent caching layer

4. **Factory Pattern**
   - Object creation abstraction
   - Notification factory, metrics factory

---

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- SQLite (included with .NET)
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd interswitch
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Apply database migrations:
```bash
cd PHR.Infrastructure
dotnet ef database update --startup-project "../PHR.Api/PHR.Api.csproj"
```

4. Run the application:
```bash
cd ../PHR.Api
dotnet run
```

The API will be available at `http://localhost:5271`

### Initial Setup

The system automatically seeds:
- Default permissions
- Default roles (Admin, Doctor, Nurse, Patient)
- Default admin user (check seeding configuration)

---

## User Stories

### Story 1: New User Registration

**As a** healthcare professional
**I want to** register for a PHR account
**So that** I can access the patient records system

**Steps:**
1. Navigate to `/api/Auth/register`
2. Provide email, password, full name, gender, and phone number
3. System creates account and returns user ID
4. Login with email and password to get access token

### Story 2: Administrator Creates User Account

**As an** administrator
**I want to** create user accounts for new staff members
**So that** they can access the system quickly

**Steps:**
1. Login as administrator to get access token
2. Navigate to `/api/Auth/admin/users`
3. Provide user details and default password
4. Assign appropriate roles (Doctor, Nurse, etc.)
5. User receives credentials and must change password on first login

### Story 3: User Resets Forgotten Password

**As a** user who forgot their password
**I want to** reset my password securely
**So that** I can regain access to my account

**Steps:**
1. Navigate to `/api/Auth/reset-password-request`
2. Provide email address
3. Receive password reset token (via response in dev, email in production)
4. Navigate to `/api/Auth/reset-password`
5. Provide reset token and new password
6. Login with new password

### Story 4: Doctor Requests Access to Patient Record

**As a** doctor
**I want to** request access to a patient's medical records
**So that** I can provide appropriate medical care

**Steps:**
1. Login as doctor to get access token
2. Navigate to `/api/AccessRequests`
3. Provide patient record ID and reason for access
4. System creates pending access request
5. Wait for administrator/authorized user to approve request

### Story 5: Administrator Approves Access Request

**As an** administrator with approval permissions
**I want to** review and approve access requests
**So that** authorized personnel can access patient records

**Steps:**
1. Login as administrator with ApproveAccessRequests permission
2. Navigate to `/api/AccessRequests/pending` to view pending requests
3. Review access request details
4. Navigate to `/api/AccessRequests/{id}/approve`
5. Set access start and end dates
6. Approve the request

### Story 6: User Changes Password After First Login

**As a** newly created user
**I want to** change my default password
**So that** I have a secure, personalized password

**Steps:**
1. Login with default password provided by administrator
2. System sets `RequirePasswordChange = true`
3. Navigate to `/api/Auth/change-password`
4. Provide current password and new password
5. System updates password and sets `RequirePasswordChange = false`

---

## API Endpoints

### Authentication Endpoints

#### 1. Register User
```http
POST /api/Auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "fullName": "John Doe",
  "gender": 0,
  "phoneNumber": "+1234567890"
}
```

**Response:**
```json
{
  "code": "00",
  "message": "Registration successful",
  "data": {
    "userId": "guid-value",
    "email": "user@example.com"
  },
  "success": true
}
```

---

#### 2. Login
```http
POST /api/Auth/login
Content-Type: application/json

{
  "email": "admin@phr.com",
  "password": "Admin@123"
}
```

**Response:**
```json
{
  "code": "00",
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh-token-value",
    "user": {
      "id": "user-guid",
      "email": "admin@phr.com",
      "fullName": "Admin User",
      "roles": ["Admin"]
    }
  },
  "success": true
}
```

---

#### 3. Refresh Token
```http
POST /api/Auth/refresh
Content-Type: application/json

{
  "refreshToken": "your-refresh-token"
}
```

**Response:**
```json
{
  "code": "00",
  "message": "Token refreshed successfully",
  "data": {
    "accessToken": "new-access-token",
    "refreshToken": "new-refresh-token"
  },
  "success": true
}
```

---

#### 4. Admin Create User
```http
POST /api/Auth/admin/users
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "email": "doctor@phr.com",
  "defaultPassword": "TempPass123!",
  "fullName": "Dr. Jane Smith",
  "gender": 1,
  "phoneNumber": "+9876543210",
  "isActive": true,
  "roles": ["Doctor"]
}
```

**Response:**
```json
{
  "code": "00",
  "message": "User created successfully",
  "data": {
    "userId": "user-guid"
  },
  "success": true
}
```

---

#### 5. Change Password
```http
POST /api/Auth/change-password
Authorization: Bearer {user-token}
Content-Type: application/json

{
  "currentPassword": "OldPass123!",
  "newPassword": "NewPass456!"
}
```

**Response:**
```json
{
  "code": "00",
  "message": "Password changed successfully",
  "data": null,
  "success": true
}
```

---

#### 6. Reset Password Request
```http
POST /api/Auth/reset-password-request
Content-Type: application/json

{
  "email": "user@example.com"
}
```

**Response (User Exists):**
```json
{
  "code": "00",
  "message": "Password reset token generated successfully",
  "data": {
    "token": "A1B2C3D4",
    "expiresAt": "2025-11-01T00:23:08Z",
    "message": "Password reset token generated successfully. In production, this would be sent via email."
  },
  "success": true
}
```

**Response (User Not Found):**
```json
{
  "code": "04",
  "message": "User not found",
  "data": null,
  "success": false
}
```

---

#### 7. Reset Password
```http
POST /api/Auth/reset-password
Content-Type: application/json

{
  "token": "A1B2C3D4",
  "newPassword": "NewSecurePass123!"
}
```

**Response:**
```json
{
  "code": "00",
  "message": "Password reset successfully",
  "data": null,
  "success": true
}
```

---

#### 8. Logout
```http
POST /api/Auth/logout
Authorization: Bearer {user-token}
Content-Type: application/json

{
  "refreshToken": "your-refresh-token"
}
```

**Response:**
```json
{
  "code": "00",
  "message": "Logout successful",
  "data": null,
  "success": true
}
```

---

### Access Request Endpoints

#### 1. Create Access Request
```http
POST /api/AccessRequests
Authorization: Bearer {user-token}
Content-Type: application/json

{
  "patientRecordId": "patient-record-guid",
  "reason": "Need to review medical history for consultation"
}
```

**Response:**
```json
{
  "code": "00",
  "message": "Access request created successfully",
  "data": {
    "accessRequestId": "access-request-guid"
  },
  "success": true
}
```

---

#### 2. Get Pending Access Requests
```http
GET /api/AccessRequests/pending
Authorization: Bearer {user-token}
```

**Response:**
```json
{
  "code": "00",
  "message": "Pending access requests retrieved",
  "data": [
    {
      "id": "request-guid",
      "patientRecordId": "patient-guid",
      "requestorUserId": "user-guid",
      "reason": "Medical consultation",
      "requestDateUtc": "2025-10-31T23:00:00Z",
      "status": 0
    }
  ],
  "success": true
}
```

---

#### 3. Get Access Request by ID
```http
GET /api/AccessRequests/{id}
Authorization: Bearer {user-token}
```

**Response:**
```json
{
  "code": "00",
  "message": "Access request retrieved",
  "data": {
    "id": "request-guid",
    "patientRecordId": "patient-guid",
    "requestorUserId": "user-guid",
    "reason": "Medical consultation",
    "requestDateUtc": "2025-10-31T23:00:00Z",
    "status": 0
  },
  "success": true
}
```

---

#### 4. Approve Access Request
```http
POST /api/AccessRequests/{id}/approve
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "approvedStartUtc": "2025-10-31T00:00:00Z",
  "approvedEndUtc": "2025-11-30T23:59:59Z"
}
```

**Response:**
```json
{
  "code": "00",
  "message": "Access request approved",
  "data": true,
  "success": true
}
```

---

#### 5. Decline Access Request
```http
POST /api/AccessRequests/{id}/decline
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "declineReason": "Insufficient justification for access"
}
```

**Response:**
```json
{
  "code": "00",
  "message": "Access request declined",
  "data": true,
  "success": true
}
```

---

## Test Data

### Pre-seeded Users

Check your database seeding configuration for default users. Typical setup:

```
Email: admin@phr.com
Password: Admin@123
Roles: Admin
```

### Test Scenarios

#### Scenario 1: Complete Authentication Flow

**Step 1: Register New User**
```json
POST /api/Auth/register
{
  "email": "testuser@phr.com",
  "password": "Test@12345",
  "fullName": "Test User",
  "gender": 0,
  "phoneNumber": "+1234567890"
}
```

**Step 2: Login**
```json
POST /api/Auth/login
{
  "email": "testuser@phr.com",
  "password": "Test@12345"
}
```

**Step 3: Change Password**
```json
POST /api/Auth/change-password
Authorization: Bearer {token-from-login}
{
  "currentPassword": "Test@12345",
  "newPassword": "NewTest@12345"
}
```

---

#### Scenario 2: Password Reset Flow

**Step 1: Request Password Reset**
```json
POST /api/Auth/reset-password-request
{
  "email": "testuser@phr.com"
}
```

**Response:**
```json
{
  "data": {
    "token": "A1B2C3D4",
    "expiresAt": "2025-11-01T01:00:00Z"
  }
}
```

**Step 2: Reset Password with Token**
```json
POST /api/Auth/reset-password
{
  "token": "A1B2C3D4",
  "newPassword": "ResetPass@123"
}
```

**Step 3: Login with New Password**
```json
POST /api/Auth/login
{
  "email": "testuser@phr.com",
  "password": "ResetPass@123"
}
```

---

#### Scenario 3: Admin Creates User & User Changes Password

**Step 1: Admin Login**
```json
POST /api/Auth/login
{
  "email": "admin@phr.com",
  "password": "Admin@123"
}
```

**Step 2: Admin Creates Doctor Account**
```json
POST /api/Auth/admin/users
Authorization: Bearer {admin-token}
{
  "email": "doctor1@phr.com",
  "defaultPassword": "Doctor@123",
  "fullName": "Dr. John Smith",
  "gender": 0,
  "phoneNumber": "+1122334455",
  "isActive": true,
  "roles": ["Doctor"]
}
```

**Step 3: Doctor First Login**
```json
POST /api/Auth/login
{
  "email": "doctor1@phr.com",
  "password": "Doctor@123"
}
```

**Step 4: Doctor Must Change Password**
```json
POST /api/Auth/change-password
Authorization: Bearer {doctor-token}
{
  "currentPassword": "Doctor@123",
  "newPassword": "MyNewPass@456"
}
```

---

#### Scenario 4: Access Request Workflow

**Step 1: Create Patient Record** (Prerequisite)
```json
POST /api/PatientRecords
Authorization: Bearer {admin-token}
{
  "firstName": "Jane",
  "lastName": "Doe",
  "dateOfBirth": "1990-05-15",
  "gender": 1,
  "bloodType": "A+"
}
```

**Step 2: Doctor Requests Access**
```json
POST /api/AccessRequests
Authorization: Bearer {doctor-token}
{
  "patientRecordId": "{patient-record-id-from-step-1}",
  "reason": "Scheduled consultation for annual checkup"
}
```

**Step 3: Admin Views Pending Requests**
```json
GET /api/AccessRequests/pending
Authorization: Bearer {admin-token}
```

**Step 4: Admin Approves Request**
```json
POST /api/AccessRequests/{access-request-id}/approve
Authorization: Bearer {admin-token}
{
  "approvedStartUtc": "2025-10-31T00:00:00Z",
  "approvedEndUtc": "2025-11-30T23:59:59Z"
}
```

---

### Sample Test Data

#### Users
```json
[
  {
    "email": "admin@phr.com",
    "password": "Admin@123",
    "role": "Admin"
  },
  {
    "email": "doctor1@phr.com",
    "password": "Doctor@123",
    "role": "Doctor"
  },
  {
    "email": "nurse1@phr.com",
    "password": "Nurse@123",
    "role": "Nurse"
  },
  {
    "email": "patient1@phr.com",
    "password": "Patient@123",
    "role": "Patient"
  }
]
```

#### Patient Records (Sample IDs)
```json
[
  {
    "id": "11111111-1111-1111-1111-111111111111",
    "firstName": "John",
    "lastName": "Doe",
    "dateOfBirth": "1985-03-15"
  },
  {
    "id": "22222222-2222-2222-2222-222222222222",
    "firstName": "Jane",
    "lastName": "Smith",
    "dateOfBirth": "1990-07-22"
  }
]
```

---

## Database Schema

### Key Entities

#### Users Table
```sql
CREATE TABLE Users (
    Id TEXT PRIMARY KEY,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    FullName TEXT NOT NULL,
    Gender INTEGER NOT NULL,
    PhoneNumber TEXT,
    IsActive INTEGER NOT NULL,
    RequirePasswordChange INTEGER NOT NULL DEFAULT 0,
    LastPasswordChangeUtc TEXT,
    CreatedAtUtc TEXT NOT NULL
);
```

#### PasswordResetTokens Table
```sql
CREATE TABLE PasswordResetTokens (
    Id TEXT PRIMARY KEY,
    UserId TEXT NOT NULL,
    Token TEXT NOT NULL UNIQUE,
    ExpiryDateUtc TEXT NOT NULL,
    IsUsed INTEGER NOT NULL DEFAULT 0,
    CreatedAtUtc TEXT NOT NULL,
    UsedAtUtc TEXT,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE INDEX IX_PasswordResetTokens_Token ON PasswordResetTokens(Token);
CREATE INDEX IX_PasswordResetTokens_UserId_ExpiryDateUtc_IsUsed ON PasswordResetTokens(UserId, ExpiryDateUtc, IsUsed);
```

#### AccessRequests Table
```sql
CREATE TABLE AccessRequests (
    Id TEXT PRIMARY KEY,
    PatientRecordId TEXT NOT NULL,
    RequestorUserId TEXT NOT NULL,
    Reason TEXT NOT NULL,
    RequestDateUtc TEXT NOT NULL,
    Status INTEGER NOT NULL DEFAULT 0,
    ApprovedByUserId TEXT,
    ApprovedStartUtc TEXT,
    ApprovedEndUtc TEXT,
    DecisionDateUtc TEXT,
    DeclineReason TEXT,
    FOREIGN KEY (PatientRecordId) REFERENCES PatientRecords(Id),
    FOREIGN KEY (RequestorUserId) REFERENCES Users(Id),
    FOREIGN KEY (ApprovedByUserId) REFERENCES Users(Id)
);
```

---

## Error Codes

| Code | Description |
|------|-------------|
| 00   | Success |
| 01   | Bad Request |
| 02   | Unauthorized |
| 03   | Forbidden |
| 04   | Not Found |
| 05   | Validation Error |
| 99   | Internal Server Error |

---

## Security Best Practices

1. **Password Requirements**
   - Minimum 6 characters (configurable)
   - Requires uppercase, lowercase, numbers, and special characters (recommended)

2. **Token Security**
   - Access tokens expire after configurable time
   - Refresh tokens have longer expiration
   - Password reset tokens expire after 1 hour
   - Single-use tokens prevent reuse attacks

3. **Authorization**
   - Role-based access control (RBAC)
   - Policy-based authorization for sensitive operations
   - JWT claims-based authentication

4. **Data Protection**
   - Password hashing (no plaintext storage)
   - Secure token generation
   - User enumeration prevention (generic error messages)

---

## TODO:: if time permits me

- [ ] Dockerization
- [ ] Email service integration for password reset (Smtp Implementation)
- [ ] Two-factor authentication (2FA)
- [ ] Audit logging for all operations
- [ ] Advanced search and filtering
- [ ] File upload for medical documents
- [ ] Real-time  event- driven notifications (pub-sub/ Kafka)
- [ ] Multi-tenant support

---

## License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## Acknowledgments

- Built with .NET 9.0
- Uses Entity Framework Core for data access
- Implements Clean Architecture principles
- Follows SOLID design principles
