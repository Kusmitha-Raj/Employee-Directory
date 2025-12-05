# Employee Directory App

A full-stack Employee Directory Management System with secure authentication, employee details management, and department organization. Built using ASP.NET Core Web API,React + TypeScript and SQL Server.

**********
## Key Features

1.  JWT Authentication (Login & secure routes)
2.  Employee CRUDfunctionality
3.  Department Management
4.  Dashboard with counts of employees & departments
5.  SQL Server Database using EF Core
6.  RESTful API architecture
   
**********

##  Tech Stack

| Layer | Technology |
| Frontend | React, TypeScript, Axios, Context API |
| Backend | ASP.NET Core Web API, EF Core |
| Database | SQL Server |
| Testing | xUnit |

***********
##  Installation & Setup

### Prerequisites

1.  .NET SDK 7/8
2.  Node.js 16+
3.  SQL Server

**********

### Setup Backend

```bash
cd Backend
dotnet restore
dotnet build
dotnet ef migrations add Message
dotnet ef database update
dotnet run

***********

###  Setup Frontend

```bash
cd Frontend
npm install
npm run dev
