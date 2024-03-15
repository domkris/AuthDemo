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


![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/POST_XS.png?raw=true)

<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>

<td>
### api/Auth/Login

```json
{
  "email": "user@example.com",
  "password": "stringst"
}
```

</td>
  
<td>
  
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthDemo_LoginRequestA.gif?raw=true)
  
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


<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>

<td>

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/POST_XS.png?raw=true)

### api/Auth/Logout


</td>
  
<td>
</td>

<td>
</td>

</tr>
</table>


<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/POST_XS.png?raw=true)

### api/Auth/LogoutAllSessions
  
</td>

<td>
</td>

<td>
</td>

</tr>
</table>

<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/POST_XS.png?raw=true)

### api/Auth/ChangePassword

```json
{
  "userId": 0,
  "currentPassword": "string",
  "newPassword": "stringst",
  "confirmNewPassword": "string"
}
```
  
</td>

<td>


</td>
<td>
</td>

</tr>
</table>

<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/POST_XS.png?raw=true)

### api/Auth/ChangeEmail

```json
{
  "userId": 0,
  "currentEmail": "user@example.com",
  "newEmail": "user@example.com"
}
```
  
</td>

<td>
</td>

<td>
</td>

</tr>
</table>

<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/POST_XS.png?raw=true)

### api/AuthTokens/RefreshToken

```json
{
  "accessToken": "string",
  "refreshToken": "string"
}
```
  
</td>

<td>
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

<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/PUT_XS.png?raw=true)

### api/AuthTokens/InvalidateUserTokens/{id}


  
</td>

<td>
</td>

<td>
</td>

</tr>
</table>

<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/GET_XS.png?raw=true)

### api/Chores/Chores

</td>

<td>
</td>

<td>
</td>

</tr>
</table>


<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/POST_XS.png?raw=true)

### api/Chores/Chores

```json
{
  "title": "string",
  "description": "string"
}
```

</td>

<td>
</td>

<td>
</td>

</tr>
</table>

<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/GET_XS.png?raw=true)

### api/Chores/Chores/{id}

</td>

<td>
</td>

<td>
</td>

</tr>
</table>

<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/PUT_XS.png?raw=true)

### api/Chores/Chores/{id}

```json
{
  "title": "string",
  "description": "string"
}
```

</td>

<td>
</td>

<td>
</td>

</tr>
</table>


<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/DELETE_XS.png?raw=true)

### api/Chores/Chores/{id}

</td>

<td>
</td>

<td>
</td>

</tr>
</table>


<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/PUT_XS.png?raw=true)

### api/Chores/AssignUser

```json
{
  "choreId": 0,
  "userId": 0
}
```

</td>

<td>
</td>

<td>
</td>

</tr>
</table>


<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/PUT_XS.png?raw=true)

### api/Chores/Finish/{id}

```json
{
  "choreId": 0,
  "userId": 0
}
```

</td>

<td>
</td>

<td>
</td>

</tr>
</table>

### (Chores): PUT api/Chores/Approve/{id}

<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/PUT_XS.png?raw=true)

### api/Chores/Approve/{id}

</td>

<td>
</td>

<td>
</td>

</tr>
</table>


<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/PUT_XS.png?raw=true)

### api/Users/ToggleUserActivation/{id}

</td>

<td>
</td>

<td>
</td>

</tr>
</table>


<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/GET_XS.png?raw=true)

### api/Users

</td>

<td>
</td>

<td>
</td>

</tr>
</table>


<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/POST_XS.png?raw=true)

### api/Users

```json
{
  "firstName": "string",
  "lastName": "string",
  "email": "user@example.com",
  "password": "stringst",
  "confirmPassword": "string",
  "role": 1
}
```

```csharp

// not a part of request
public enum Role
{
    Administrator = 1,
    Manager = 2,
    Employee = 3
}
```

</td>


<td>
</td>

<td>
</td>

</tr>
</table>

### (Users): GET api/Users/{id}

<table>
<tr>
<th> Request </th>
<th> Visualisation </th>
<th> Response (SUCCESS 200 Ok) </th>
</tr>
<tr>
  
<td>
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/GET_XS.png?raw=true)

### api/Users

</td>

<td>
</td>

<td>
</td>

</tr>
</table>


<hr>

## Contributions

Contributions to this repository are welcome!

## Acknowledgments

- Special thanks to the .NET community for their invaluable contributions and support.
- Inspiration drawn from various tutorials and resources on authorization and authentication in .NET Web API.

## Contact

For any inquiries or feedback, please contact me at [domagojkk@gmail.com](mailto:domagojkk@gmail.com).




