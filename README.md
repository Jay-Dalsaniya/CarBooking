CAR BOOKING MANAGEMENT SYSTEM
=============================

A complete Car Booking Management System built using
ASP.NET Core MVC, Entity Framework Core, and SQL Server.

This application allows users to book cars online while admins
manage cars, bookings, users, and view booking analytics.

------------------------------------------------------------
FEATURES
------------------------------------------------------------

USER FEATURES
-------------
- User Registration with Email Verification
- Secure Login and Logout
- Forgot Password using OTP (Email)
- Profile Management
- Two-Factor Authentication (Google Authenticator)
- View Available Cars
- Book Cars with Date Selection
- View Booking History
- View Booking Details

ADMIN FEATURES
--------------
- Admin Dashboard
- Add, Edit, Delete Cars
- Upload RC, PUC, Insurance and Multiple Car Images
- Manage All Car Bookings
- Booking Analytics (Bar Chart & Pie Chart)
- User Management
- Soft Delete System (No permanent delete)

SECURITY
--------
- Role-Based Authorization (Admin / User)
- Cookie Authentication
- Email Verification
- OTP-based Password Reset
- Two-Factor Authentication (2FA)

------------------------------------------------------------
TECHNOLOGY STACK
------------------------------------------------------------
- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- Bootstrap
- jQuery
- Chart.js
- Google Authenticator
- SMTP Email Service

------------------------------------------------------------
PROJECT STRUCTURE
------------------------------------------------------------

CarBooking
|
|-- Controllers
|   |-- AdminCarController.cs
|   |-- CarBookingController.cs
|   |-- UserController.cs
|
|-- Models
|   |-- CarDetail.cs
|   |-- CarBookingDetail.cs
|   |-- CarImage.cs
|   |-- User.cs
|
|-- ViewModels
|
|-- Views
|
|-- wwwroot
|   |-- CarImages
|
|-- Program.cs
|-- README.txt

------------------------------------------------------------
CONFIGURATION
------------------------------------------------------------

DATABASE CONNECTION
-------------------
Update appsettings.json with your SQL Server details:

ConnectionStrings:
- DefaultConnection = Server=YOUR_SERVER;Database=CarBooking;Trusted_Connection=True;

EMAIL CONFIGURATION
-------------------
- Sender Email
- Sender Password (App Password)

ADMIN 2FA MASTER SECRET (OPTIONAL)
----------------------------------
Used for admin login override in 2FA.

------------------------------------------------------------
HOW TO RUN THE PROJECT
------------------------------------------------------------

1. Clone the repository
   git clone https://github.com/Jay-Dalsaniya/CarBooking.git

2. Open the solution in Visual Studio

3. Restore NuGet packages

4. Run database migration
   Update-Database

5. Run the project
   dotnet run

------------------------------------------------------------
DATABASE
------------------------------------------------------------
- SQL Server
- Entity Framework Core (Code First)
- Soft Delete implemented using IsDelete flag

------------------------------------------------------------
AUTHOR
------------------------------------------------------------
Name  : Jay Dalsaniya
Role  : ASP.NET Core MVC Developer
GitHub: https://github.com/Jay-Dalsaniya

------------------------------------------------------------
LICENSE
------------------------------------------------------------
This project is developed for learning and portfolio purposes.


CAR BOOKING DATABASE SCRIPT
===========================

--------------------------------------------------
CREATE DATABASE
--------------------------------------------------

CREATE DATABASE CarBooking;
GO

USE CarBooking;
GO

--------------------------------------------------
USER TABLE
--------------------------------------------------

CREATE TABLE [User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(50) NOT NULL UNIQUE,
    PhoneNo NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(50) NOT NULL,
    Role NVARCHAR(20) NOT NULL,
    IsDelete BIT NOT NULL DEFAULT 0,
    IsValid BIT DEFAULT 0,
    CreatedDate DATETIME NULL,
    UpdatedDate DATETIME NULL,
    ActivetionCode UNIQUEIDENTIFIER NULL,
    ResetOtp NVARCHAR(10) NULL,
    OtpExpireTime DATETIME NULL,
    IsTwoFactorEnabled BIT DEFAULT 0,
    TwoFactorSecret NVARCHAR(100) NULL
);

--------------------------------------------------
CAR DETAIL TABLE
--------------------------------------------------

CREATE TABLE CarDetail (
    CarId INT IDENTITY(1,1) PRIMARY KEY,
    CarName NVARCHAR(50) NOT NULL,
    CarNumber NVARCHAR(50) NOT NULL,
    RegisterDate DATETIME NOT NULL,
    PucexpireDate DATETIME NOT NULL,
    InsuranceDate DATETIME NOT NULL,
    PricePerDay DECIMAL(18,2) NOT NULL,
    RcimageOrignalFileName NVARCHAR(255) NULL,
    RcimagePath NVARCHAR(255) NULL,
    PucimageOrignalFileName NVARCHAR(255) NULL,
    PucimagePath NVARCHAR(255) NULL,
    InsuranceImageOrignalFileName NVARCHAR(255) NULL,
    InsuranceImagePath NVARCHAR(255) NULL,
    CarImageOrignalFileName NVARCHAR(255) NULL,
    CarImagePath NVARCHAR(255) NULL,
    CreatedDate DATETIME NULL,
    UpdatedDate DATETIME NULL,
    IsDelete BIT NOT NULL DEFAULT 0
);

--------------------------------------------------
CAR IMAGE TABLE
--------------------------------------------------

CREATE TABLE CarImages (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,
    CarId INT NOT NULL,
    ImagePath NVARCHAR(255) NOT NULL,
    OriginalFileName NVARCHAR(255) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0,
    CONSTRAINT FK_CarImages_CarDetail
        FOREIGN KEY (CarId) REFERENCES CarDetail(CarId)
);

--------------------------------------------------
CAR BOOKING DETAIL TABLE
--------------------------------------------------

CREATE TABLE CarBookingDetail (
    CarBookingId INT IDENTITY(1,1) PRIMARY KEY,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    TotalDays INT NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    UserId INT NOT NULL,
    CarId INT NOT NULL,
    CreatedDate DATETIME NULL,
    UpdatedDate DATETIME NULL,
    IsDelete BIT NOT NULL DEFAULT 0,
    UpdateBy INT NULL,
    CONSTRAINT FK_CarBookingDetail_User
        FOREIGN KEY (UserId) REFERENCES [User](UserId),
    CONSTRAINT FK_CarBookingDetail_CarDetail
        FOREIGN KEY (CarId) REFERENCES CarDetail(CarId)
);

--------------------------------------------------
END OF SCRIPT
--------------------------------------------------
