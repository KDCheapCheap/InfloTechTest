# User Management Technical Exercise

The exercise is an ASP.NET Core web application backed by Entity Framework Core, which faciliates management of some fictional users.
We recommend that you use [Visual Studio (Community Edition)](https://visualstudio.microsoft.com/downloads) or [Visual Studio Code](https://code.visualstudio.com/Download) to run and modify the application. 

**The application uses MySQL by default.** Its local connection is configured in `UserManagement.Web/appsettings.json`; database files persist in the Docker volume between application runs.

In the task list below, I have removed anything I have not completed.

## Getting Started
1. Clone the repository
2. Start the database

    If you have a MySQL database already, change the connection string in `appsettings.json` to what is required for your server then you will need to create a new database called "KD_InfloTechTest". Migrations will not create the database.

    If you do not, I have supplied a Docker startup script that creates a MySQL server and the database running in a Docker container.
    
    Steps to run:
    * Navigate to `/Supporting Files/mysql`
    * Run `start-db.bat`
    * This creates the container, downloads the mysql image and creates a volume that stores the data 

3. Restore dependencies and build
4. Apply database migrations
Open a terminal in `UserManagement.Web` and run `dotnet ef database update` to run migrations.
5. Run the application

## Once Finished
### Remove Docker MySQL
In the Docker MySQL set up I supplied, you can run `teardown-db.bat` to remove everything that was created. This will only delete what was made and won't affect any other containers.

## Packages / Libraries
### Heroicons.AspNetCore - Web
*[Package Link](https://www.nuget.org/packages/Heroicons.AspNetCore.Mvc.TagHelpers/)*

I added this to improve the look of the frontend a little. This package allows easy usage of [Heroicons](https://heroicons.com/) in an MVC application.

### Pomelo.EntityFrameworkCore.MySql - Web
*[Package Link](https://www.nuget.org/packages/pomelo.entityFrameworkcore.mysql)*

My local database uses MySql, this package adds MySql functionality to Entity Framework Core using the setup you can see in `Program,cs`. My implementation of this is simply from the nuget page linked above.

### Microsoft.EntityFrameworkCore.Design - Web
*[Package Link](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Design)*

This package is required for EF Core migrations due to them running at design time.

### Local MySQL Server Docker Scripts
*Found in `Supporting Files/mysql`*

The local database runs in a Docker container built from the MySQL 8.4 image. Run `start-db.bat` from `Supporting Files/mysql` to start it; `teardown-db.bat` is also provided to remove the container and, if requested, its data volume. See [the MySQL README](Supporting%20Files/mysql/README.md) for the connection settings and database setup steps. 

Once the database is created, run `dotnet ef database update` from the Web project to create the tables

When connecting a local MySQL client, you may encounter `Public Key Retrieval is not allowed`. For this local development connection, set the client's `allowPublicKeyRetrieval` option to `true`. Do not use this workaround for untrusted remote connections; configure TLS instead.

**Requirements**
Docker Desktop and Docker Compose are required.

## The Exercise
Complete as many of the tasks below as you feel comfortable with. These are split into 4 levels of difficulty 
* **Standard** - Functionality that is common when working as a web developer
* **Advanced** - Slightly more technical tasks and problem solving
* **Expert** - Tasks with a higher level of problem solving and architecture needed
* **Platform** - Tasks with a focus on infrastructure and scaleability, rather than application development.

### 1. Filters Section (Standard)

The users page contains 3 buttons below the user listing - **Show All**, **Active Only** and **Non Active**. Show All has already been implemented. Implement the remaining buttons using the following logic:
* Active Only – This should show only users where their `IsActive` property is set to `true`
* Non Active – This should show only users where their `IsActive` property is set to `false`

### 2. User Model Properties (Standard)

Add a new property to the `User` class in the system called `DateOfBirth` which is to be used and displayed in relevant sections of the app.

### 3. Actions Section (Standard)

Create the code and UI flows for the following actions
* **Add** – A screen that allows you to create a new user and return to the list
* **View** - A screen that displays the information about a user
* **Edit** – A screen that allows you to edit a selected user from the list  
* **Delete** – A screen that allows you to delete a selected user from the list

Each of these screens should contain appropriate data validation, which is communicated to the end user.

### 4. Data Logging (Advanced)

Extend the system to capture log information regarding primary actions performed on each user in the app.
* In the **View** screen there should be a list of all actions that have been performed against that user. 
* There should be a new **Logs** page, containing a list of log entries across the application.
* In the Logs page, the user should be able to click into each entry to see more detail about it.
* In the Logs page, think about how you can provide a good user experience - even when there are many log entries.

### 5. Extend the Application (Expert)

Make a significant architectural change that improves the application.
Structurally, the user management application is very simple, and there are many ways it can be made more maintainable, scalable or testable.
Some ideas are:
* Update the data access layer to support asynchronous operations.
* Update the data access layer to use a real database, and implement database schema migrations.

## Additional Notes

* Please feel free to change or refactor any code that has been supplied within the solution and think about clean maintainable code and architecture when extending the project.
* If any additional packages, tools or setup are required to run your completed version, please document these thoroughly.
