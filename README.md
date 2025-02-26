# Workplace Collaboration

## Overview
Workplace Collaboration is an ASP.NET Core web application designed to facilitate team communication and collaboration within a workplace. It allows users to create and manage channels, post messages, categorize discussions, and handle user roles (Admin, Moderator, User). The application supports user authentication, authorization, and a robust database structure to ensure secure and efficient interaction.

## Features
- **User Management**:
  - Roles: Admin, Moderator, User with distinct permissions.
  - User profiles with additional fields (FirstName, LastName).
  - CRUD operations for users (Admin only) with role assignment.
- **Channels**:
  - Create, edit, and delete channels with name, description, and category.
  - Join/leave channels with approval workflow (pending/accepted status).
  - Pagination (3 channels per page) and search functionality.
  - Access control based on roles and ownership.
- **Messages**:
  - Post, edit, and delete messages within channels.
  - Timestamps and user attribution for each message.
  - Restricted editing/deletion to message owners, Moderators, and Admins.
- **Categories**:
  - Manage categories (Admin only) to organize channels.
  - Associate channels with categories via dropdown selection.
- **Authentication & Authorization**:
  - Built-in Identity system with pre-seeded roles and users (e.g., `admin@test.com`, `moderator@test.com`).
  - Role-based access control for actions (e.g., only Admins can manage categories).
- **UI Enhancements**:
  - Alerts for success/error messages using TempData.
  - Responsive views with category dropdowns and channel listings.

## Technologies Used
- **ASP.NET Core**: Framework for building the web application.
- **Entity Framework Core**: ORM for database interactions with `ApplicationDbContext`.
- **Identity**: Authentication and authorization system with roles and users.
- **C#**: Primary programming language for backend logic.
- **SQL Server**: Database for storing users, channels, messages, and categories (assumed via EF Core).
- **Razor Views**: Frontend templating for dynamic HTML generation.

## Purpose
This project was developed to create a collaborative platform for workplace communication, demonstrating proficiency in ASP.NET Core, Entity Framework, and Identity-based authentication. It provides a practical example of role-based access control, data modeling, and CRUD operations in a modern web application.
