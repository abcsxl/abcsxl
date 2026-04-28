# abcsxl Blog Website

A simple blog website built with ASP.NET Core MVC, using PostgreSQL database.

![Build Status](https://travis-ci.org/user/repo.svg)
![Coverage](https://codecov.io/gh/user/repo/branch/main/graph/badge.svg)
![License](https://img.shields.io/badge/license-Six%20Labs%20Split-blue)

## Features

- Homepage displaying list of blog posts
- Click to view details of each post
- Admin panel for managing blog posts
- Markdown support for blog content
- Vditor editor for creating/editing posts
- Client-side Markdown rendering using Marked.js

## Prerequisites

- .NET 10.0 SDK
- PostgreSQL16 database

## Setup

1. Clone the repository
2. Update the connection string in `appsettings.json` to match your PostgreSQL database
3. Run the application: `dotnet run`
4. Navigate to `/Admin` to manage blog posts

## Database

The application uses Entity Framework Core with PostgreSQL.

```bash
docker run -d --name db_abcsxl -p 5432:5432 -e POSTGRES_DB=db_abcsxl -e POSTGRES_USER=u_abcsxl -e POSTGRES_PASSWORD=password -v pgdata_abcsxl:/var/lib/postgresql/data postgres:16
```

Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=db_abcsxl;Username=u_abcsxl;Password=password"
}
```

To create and apply migrations:

```bash
dotnet ef migrations add InitialCreate -o Data/Migrations
dotnet ef database update
```
or

```bash
Add-Migration InitialCreate -o Data/Migrations
Update-Database
```

## Usage

- Visit the homepage to see all blog posts
- Click "Read More" to view the full post with rendered Markdown
- Go to `/Admin` to create new posts or edit existing ones
- Use the Vditor editor to write Markdown content