# Employee Management System

## Overview

This is a CRUD-based Employee Management System built using ASP.NET MVC and SQL Server. The system allows users to add, delete, update, and view employee details. It is designed to streamline employee record management efficiently.

## Developer

**Abnet Ayele**

## Repository Link

GitHub Repository: [Employee Management System](https://github.com/Abnetayel/EmployeeMS.git)

## Technologies Used

- **ASP.NET MVC** (Framework for building the web application)
- **SQL Server** (Database for storing employee data)
- **Entity Framework** (ORM for database interactions)
- **Bootstrap** (Front-end framework for styling and responsiveness)
- **Visual Studio 2022** (Development environment)

## Features

- **Add Employee**: Register a new employee with relevant details such as name, phone Number, Email, and Country.
- **Update Employee**: Modify existing employee records with new information.
- **Delete Employee**: Remove an employee from the system.
- **Employee Details**: View specific employee information, including job title and contact details.
- **Responsive UI**: Optimized layout for different screen sizes.

## Setup Instructions

1. **Clone the Repository**
   ```sh
   git clone https://github.com/Abnetayel/EmployeeMS.git
   ```
2. **Open in Visual Studio 2022**
   - Open the solution file (`.sln`).
3. **Database Configuration**
   - Ensure SQL Server is installed and running.
   - Update the `connection string` in `appsettings.json` or `Web.config` to match your SQL Server instance.
   - Run database migrations or execute the SQL script provided to set up tables and seed initial data.
4. **Run the Application**
   - Press `F5` or select `Start` in Visual Studio.
   - The application will launch in your default web browser.

## Project Structure

- **Models/**: Contains entity classes representing the database tables.
- **Controllers/**: Handles HTTP requests and business logic.
- **Views/**: Contains Razor views for the front-end interface.
- **Migrations/**: Stores Entity Framework migration scripts.
- **wwwroot/**: Holds static files such as CSS, JavaScript, and images.

## Future Enhancements

- **User Authentication**: Implement login and role-based access.
- **Export Data**: Generate reports in PDF or Excel format.
- **Email Notifications**: Send automated emails for employee updates.
- **REST API Integration**: Provide API endpoints for external applications.

## License

This project is for educational purposes and can be modified as needed.

## Contact

For any inquiries, feel free to reach out to **Abnet Ayele** at [abnetayele694@gmail.com].

