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


CAR BOOKING MANAGEMENT SYSTEM
=============================

------------------------------------------------------------
DATABASE SQL SCRIPT
------------------------------------------------------------

NOTE:
This SQL script creates all required tables for the
Car Booking Management System.
Execute this script in SQL Server Management Studio (SSMS).

------------------------------------------------------------
CREATE DATABASE
------------------------------------------------------------

CREATE DATABASE CarBooking;
GO

USE CarBooking;
GO

------------------------------------------------------------
USER TABLE
------------------------------------------------------------

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

------------------------------------------------------------
CAR DETAIL TABLE
------------------------------------------------------------

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

------------------------------------------------------------
CAR IMAGE TABLE
------------------------------------------------------------

CREATE TABLE CarImages (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,
    CarId INT NOT NULL,
    ImagePath NVARCHAR(255) NOT NULL,
    OriginalFileName NVARCHAR(255) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (CarId) REFERENCES CarDetail(CarId)
);

------------------------------------------------------------
CAR BOOKING DETAIL TABLE
------------------------------------------------------------

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
    FOREIGN KEY (UserId) REFERENCES [User](UserId),
    FOREIGN KEY (CarId) REFERENCES CarDetail(CarId)
);

------------------------------------------------------------
END OF SQL SCRIPT
------------------------------------------------------------


CAR BOOKING MANAGEMENT SYSTEM
=============================

------------------------------------------------------------
DATABASE SQL SCRIPT
------------------------------------------------------------

NOTE:
This SQL script creates all required tables for the
Car Booking Management System.
Execute this script in SQL Server Management Studio (SSMS).

------------------------------------------------------------
CREATE DATABASE
------------------------------------------------------------

CREATE DATABASE CarBooking;
GO

USE CarBooking;
GO

------------------------------------------------------------
USER TABLE
------------------------------------------------------------

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

------------------------------------------------------------
CAR DETAIL TABLE
------------------------------------------------------------

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

------------------------------------------------------------
CAR IMAGE TABLE
------------------------------------------------------------

CREATE TABLE CarImages (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,
    CarId INT NOT NULL,
    ImagePath NVARCHAR(255) NOT NULL,
    OriginalFileName NVARCHAR(255) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (CarId) REFERENCES CarDetail(CarId)
);

------------------------------------------------------------
CAR BOOKING DETAIL TABLE
------------------------------------------------------------

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
    FOREIGN KEY (UserId) REFERENCES [User](UserId),
    FOREIGN KEY (CarId) REFERENCES CarDetail(CarId)
);

------------------------------------------------------------
END OF SQL SCRIPT
------------------------------------------------------------
CAR BOOKING MANAGEMENT SYSTEM
=============================

------------------------------------------------------------
DATABASE SQL SCRIPT
------------------------------------------------------------

NOTE:
This SQL script creates all required tables for the
Car Booking Management System.
Execute this script in SQL Server Management Studio (SSMS).

------------------------------------------------------------
CREATE DATABASE
------------------------------------------------------------

CREATE DATABASE CarBooking;
GO

USE CarBooking;
GO

------------------------------------------------------------
USER TABLE
------------------------------------------------------------

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

------------------------------------------------------------
CAR DETAIL TABLE
------------------------------------------------------------

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

------------------------------------------------------------
CAR IMAGE TABLE
------------------------------------------------------------

CREATE TABLE CarImages (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,
    CarId INT NOT NULL,
    ImagePath NVARCHAR(255) NOT NULL,
    OriginalFileName NVARCHAR(255) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (CarId) REFERENCES CarDetail(CarId)
);

------------------------------------------------------------
CAR BOOKING DETAIL TABLE
------------------------------------------------------------

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
    FOREIGN KEY (UserId) REFERENCES [User](UserId),
    FOREIGN KEY (CarId) REFERENCES CarDetail(CarId)
);

------------------------------------------------------------
END OF SQL SCRIPT
------------------------------------------------------------
<img width="1888" height="909" alt="image" src="https://github.com/user-attachments/assets/ed1c7f8c-30d6-4e61-96bb-ab1e0dbb74bc" />
<img width="1881" height="919" alt="image" src="https://github.com/user-attachments/assets/2adb0a3e-011f-403e-a1d2-55240a1a4476" />


admin portal UI
<img width="1882" height="912" alt="image" src="https://github.com/user-attachments/assets/00fc352c-76d3-4e2e-9dbc-71dfaa48e4c6" />
<img width="1866" height="915" alt="image" src="https://github.com/user-attachments/assets/7d803b87-50e7-4bc2-8fa7-161bb138dacf" />
<img width="1896" height="935" alt="image" src="https://github.com/user-attachments/assets/9c3b16e7-139a-4c04-99f0-6b093b282ef3" />
<img width="1881" height="904" alt="image" src="https://github.com/user-attachments/assets/647369c5-e24e-4ebb-a790-a4f57b1439f1" />

<img width="1905" height="908" alt="image" src="https://github.com/user-attachments/assets/11d4970c-6d11-4ef8-a56c-1d5d1707a0c5" />
<img width="1838" height="896" alt="image" src="https://github.com/user-attachments/assets/6cf72411-3a05-4338-a0d4-66852632d176" />

<img width="1874" height="915" alt="image" src="https://github.com/user-attachments/assets/f10cfcfe-c24f-42fa-a17c-eb6d34f71b12" />
<img width="1880" height="909" alt="image" src="https://github.com/user-attachments/assets/116c426e-710c-43ae-bdce-a25cb7a1d04a" />

<img width="1865" height="906" alt="image" src="https://github.com/user-attachments/assets/bf5fa938-88d2-41ab-a5be-3fb2834a214e" 

   
   <img width="1879" height="914" alt="image" src="https://github.com/user-attachments/assets/6ea26887-5336-4205-861e-db5f02d04277" />

<img width="1832" height="906" alt="image" src="https://github.com/user-attachments/assets/94214d4b-23a9-45b8-ba3d-b5b6f12730ed" />


-------------------------
User Portal UI

<img width="1870" height="904" alt="image" src="https://github.com/user-attachments/assets/72ca0d7d-538f-410a-b192-90e58b2d644f" />
<img width="1875" height="895" alt="image" src="https://github.com/user-attachments/assets/65580dc5-20d5-4c74-bfbd-5daf8b185f99" />
<img width="1495" height="895" alt="image" src="https://github.com/user-attachments/assets/90b01f7d-851d-4f5a-84b6-5448d75c1cd3" />
<img width="1517" height="893" alt="image" src="https://github.com/user-attachments/assets/af2d10e0-a62f-4884-9763-68e8ce5f207e" />
<img width="1871" height="915" alt="image" src="https://github.com/user-attachments/assets/631816ba-2ba3-46f8-af9d-85fa0540d266" />
<img width="1859" height="903" alt="image" src="https://github.com/user-attachments/assets/eab3119d-0320-4ad1-9e46-c2ef7a36425c" />
<img width="1882" height="853" alt="image" src="https://github.com/user-attachments/assets/b67c0782-6d95-4ede-9a11-31a189851cf1" />
<img width="1866" height="905" alt="image" src="https://github.com/user-attachments/assets/bedd3242-faf4-4dcb-89a6-ef760d865148" />
<img width="1848" height="908" alt="image" src="https://github.com/user-attachments/assets/55c6d86d-dc89-481e-b886-b5598daad433" />




