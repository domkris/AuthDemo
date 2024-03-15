# .NET Web API Authorization and Authentication Demo & Tutorial

Welcome to the .NET Web API Authorization and Authentication Demo & Tutorial! This repository serves as a guide and demonstration of implementing authorization and authentication mechanisms in a .NET Web API application.

## Introduction

This repository aims to provide an understanding of how to integrate authorization and authentication features into .NET Web API projects.

## Features

- **JWT Authentication**: Learn how to implement JSON Web Token (JWT) authentication in .NET Web API application.
- **Role-based Authorization**: Explore role-based access control (RBAC) to restrict access to specific endpoints based on user roles.
- **Bearer Token Authorization**: Understand how to use bearer tokens to authenticate and authorize requests.
- **Claims**: Learn how to utilize claims to carry additional information about the user within JWTs, enabling fine-grained authorization and personalization.
- **Policies**: Define and enforce custom authorization policies to control access to resources based on various conditions and requirements.
- **Access and Refresh Tokens**: Implement access and refresh token functionality to manage user sessions securely and efficiently.
- **Redis Integration**: Utilize Redis for storing and managing access tokens, improving scalability and session management.
- **Entity Framework**: Utilize Entity Framework for data access and database management in your .NET Web API application.
- **PostgreSQL Integration**: Integrate PostgreSQL as the database backend for your .NET Web API application.
- **Audit Log**: Implement auditing functionality to track and log user actions such as additions, deletions, and modifications, enhancing transparency and accountability.
- **Docker Integration**: Utilize Docker for containerization to ensure consistent deployment across different environments.
- **Docker Compose**: Leverage Docker Compose for orchestrating multi-container Docker applications, simplifying the deployment process.
- **Demo Application**: Get hands-on experience with a fully functional .NET Web API application showcasing these concepts.

## Getting Started

To get started with the tutorial, follow these steps:

1. **Clone the Repository**: Clone this repository to your local machine using the following command:

    ```powershell
    git clone https://github.com/domkris/AuthDemo.git

4. **Navigate to the Project Directory**: Change your directory to the cloned repository:

5. **Install Dependencies**: Install the required dependencies using NuGet Package Manager:

   ```powershell
    dotnet restore
   
7. **Run the Application**: Start the AuthDemo.Web API application:
   
   ```powershell
    dotnet run

9. **Or Run Using Docker Compose**: Start the application using Docker Compose:
    
    ```powershell
    docker-compose up

11. **Explore the Tutorial**: Follow along with the tutorial provided in the repository to understand how authorization and authentication are implemented.

## AuthDemo API Endpoints

### POST api/Auth/Login

<table>
<tr>
<th> Visualisation </th>
<th> Request body </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
  
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthDemo_LoginRequestA.gif?raw=true)
  
</td>

<td>

```json
{
  "email": "user@example.com",
  "password": "stringst"
}
```

</td>
<td>

```json
{
  "accessToken": "****",
  "refreshToken": "****"
}
```

</td>
</tr>
</table>

### (Auth): POST api/Auth/Logout
### (Auth): POST api/Auth/LogoutAllSessions
### (Auth): POST api/Auth/ChangePassword
### (Auth): POST api/Auth/ChangeEmail


### (AuthTokens): POST api/AuthTokens/RefreshToken
### (AuthTokens): PUT api/AuthTokens/InvalidateUserTokens/{id}


### (Chores): GET api/Chores/Chores
### (Chores): POST api/Chores/Chores
### (Chores): GET api/Chores/Chores/{id}
### (Chores): PUT api/Chores/Chores/{id}
### (Chores): DELETE api/Chores/Chores/{id}
### (Chores): PUT api/Chores/AssignUser
### (Chores): PUT api/Chores/Finish/{id}
### (Chores): PUT api/Chores/Approve/{id}

### (Users): POST api/Users/ToggleUserActivation/{id}
### (Users): GET api/Users
### (Users): POST api/Users
### (Users): GET api/Users/{id}
<hr>

## Contributions

Contributions to this repository are welcome!

## Acknowledgments

- Special thanks to the .NET community for their invaluable contributions and support.
- Inspiration drawn from various tutorials and resources on authorization and authentication in .NET Web API.

## Contact

For any inquiries or feedback, please contact me at [domagojkk@gmail.com](mailto:domagojkk@gmail.com).




