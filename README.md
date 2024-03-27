# AuthDemo: .NET Web API Authorization and Authentication Demo & Tutorial

AuthDemo is a .NET Web API application designed to provide a practical learning demonstration of implementing authorization and authentication mechanisms in a .NET Web API application. Additionally, it showcases chore management functionalities, serving as a educational example of integrating these features into simulated and some real-world scenarios.


## Table of Contents

1. [Key Features](#key-features)
2. [Technical Highlights](#technical-highlights)
3. [Getting Started](#getting-started)

## Key Features

- **Secure Authentication**: Implement secure user login functionality.
- **Role-Based Authorization**: Control user access based on roles (Administrator, Manager, Employee).
- **Chore Management**: Manage chores efficiently with chore creation, updating, and deletion capabilities.
- **Task Assignment**: Assign chores to users.
- **Progress Tracking**: Monitor chore completion status to track task progress effectively.


## Technical Highlights

- **JWT Authentication**<br> Learn how to implement JSON Web Token (JWT) authentication in .NET Web API application.
- **Role-based Authorization**<br> Explore role-based access control (RBAC) to restrict access to specific endpoints based on user roles.
- **Bearer Token Authorization with Claims**<br> Understand how to use bearer tokens to authenticate and authorize requests and learn how to utilize claims to carry additional information about the user within JWTs.
- **Policies**<br> Define and enforce custom authorization policies to control access to resources based on various conditions and requirements.
- **Access and Refresh Tokens**<br> Implement access and refresh token functionality to manage user sessions securely and efficiently.
- **Redis Integration**<br> Utilize Redis for storing and managing access tokens, improving scalability and session management.
- **Entity Framework**<br> Utilize Entity Framework for data access and database management in your .NET Web API application.
- **PostgreSQL Integration**<br> Integrate PostgreSQL as the database backend for your .NET Web API application.
- **Audit Log**<br> Implement auditing functionality to track and log user actions such as additions, deletions, and modifications, enhancing transparency and accountability.
- **Docker Integration**<br> Utilize Docker for containerization to ensure consistent deployment across different environments.
- **Docker Compose**<br> Leverage Docker Compose for orchestrating multi-container Docker applications, simplifying the deployment process.
- **Demo Application**<br> Get hands-on experience with a fully functional .NET Web API application showcasing these concepts.

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

10. **Explore the Endpoints**: Once the application is running, you can explore the available API endpoints. Refer to the [API Endpoints](#api-endpoints) section below for a summary list of endpoints and their descriptions.

11. **Interact with Endpoints**: Use tools like [Postman](https://www.postman.com/downloads/), [curl](https://curl.se/download.html) or build-in Swagger UI to interact with the endpoints. You can send requests to endpoints such as login, logout, create user, manage chores, etc.

12. **Follow the Tutorial**: For a detailed understanding of how authorization and authentication are implemented within the application, refer to the tutorial provided in the repository. The tutorial offers step-by-step guidance and explanations on key concepts and functionalities.

13. **Explore Endpoint Details**: Each endpoint has detailed explanations provided in the repository. Refer to the [API Endpoints Details](#api-endpoints-details) to understand the purpose and usage of each endpoint effectively.

By following these steps, you'll be able to navigate through AuthDemo, understand its functionalities, and gain insights into implementing authorization and authentication mechanisms within your own .NET Web API applications.
<br>
<br>
<br>

## Architecture Overview

AuthDemo follows a structured architecture to ensure modularity, scalability, and maintainability. Here's a breakdown of the main components:

| Component         | Purpose                                                              | Functionality                                                                                                                                                             |
|-------------------|----------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| AuthDemo.Cache    | Handles Redis JWT token caching.                                     | Responsible for caching JWT tokens in Redis for efficient session management and token validation.                                                                       |
| AuthDemo.Contracts| Contains request and response objects used throughout the application.| Defines contracts and data transfer objects (DTOs) for communication between different layers of the application.                                                          |
| AuthDemo.Domain   | Houses all repositories and services responsible for interacting with the database. | Implements business logic and data access operations.                                                                                                                     |
| AuthDemo.Infrastructure | Centralizes entities, audits, entity type configurations, lookup data, and migration files. | Provides infrastructure-related functionalities such as database entity definitions, database migrations, and data seeding.                                           |
| AuthDemo.Security | Manages security-related functionalities such as JWT settings, token creation, and authorization policies. | Defines JWT settings, generates JWT tokens, and enforces authorization policies based on user roles and claims.                                                         |
| AuthDemo.Web      | Contains controllers and automapper configurations for handling HTTP requests and responses. | Exposes RESTful API endpoints, maps requests to appropriate actions, and transforms data between DTOs and domain models.                                                |


This architecture ensures separation of concerns, making the application modular and easy to maintain. Each component is responsible for a specific aspect of the application, promoting code organization and reusability.
<br>
<br>
<br>

## API Endpoints

| Endpoint                       | Description                           | Method |
|-----------------------------------------------|---------------------------------------|--------|
| /api/auth/login                               | Login                                 | POST   |
| /api/auth/logout                              | Logout                                | POST   |
| /api/auth/logoutAllSessions                   | Logout from all Sessions              | POST   |
| /api/auth/changePassword                      | Change User's password                | POST   |
| /api/auth/changeEmail                         | Change User's email                   | POST   |
| /api/authTokens/refreshTokens                 | Request a new Access Token            | POST   |
| /api/authTokens/invalidateUserTokens/{id}     | Invalidate all User's tokens          | POST   |
| /api/chores                                   | Get all chores                        | GET    | 
| /api/chores                                   | Create new chore                      | POST   | 
| /api/chores/{id}                              | Get specific chore                    | GET    | 
| /api/chores/{id}                              | Update specific chore                 | PUT    | 
| /api/chores/{id}                              | Delete specific chore                 | DELETE | 
| /api/chores/assignUser                        | Assing User to chore                  | PUT    |
| /api/chores/finish/{id}                       | Finish chore                          | PUT    | 
| /api/chores/approve/{id}                      | Approve chore                         | PUT    | 
| /api/users/toggleUserActivation/{id}          | Activate/Deactivate user              | POST   |
| /api/users                                    | Create a user                         | POST   | 
| /api/users                                    | Get all users                         | GET    | 
| /api/users/{id}                               | Get specific user                     | GET    | 

<br>
<br>
<br>


## API Endpoints Details

<!--BEGIN POST /api/auth/login ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **POST** **/api/Auth/Login**
<details>
  <summary> Click to expand!</summary>
<br>
<br>
  <table>
<tr>
<td> 
    
**POST** 

</td>
<td> 
    
**api/Auth/Login**

</td>
<td colspan="2">
    
**EndPoint Authorization: None/Anonymous** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_Login/Login123.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_Login/Login345.gif?raw=true) 
        
</td>
<td  colspan="2">
    
- **Step 1: User sends login request to Server** <br><br>

- **Step 2: Validation of user's email and password.** <br>
    Server sends a request to DB to check user's email and password.
    User can be locked out on 5 wrong login attempts for 5 min.
    Server gets response from DB and if user credentials are correct go to step 3 and 4.<br><br>
  
- **Step 3: Creation of Access Token**<br>
    Server sends request to Redis to create Access Token. Access Token is in format of JWT Token and is valid and
    stored in Redis for 10 minutes and deleted on expire. Server gets newly created Access Token from Redis.<br><br>
    
- **Step 4: Creation of Refresh Token**<br>
    Server sends request to DB to create Refresh Token. Refresh Token is in format of random string
    and is valid for 7 days and can only be used once.
    Refresh Token is stored indefinitely, it is persistent.
    On each use of Refresh Token user gets a new refresh Token
    that is valid for another 7 days meaning that
    if user has been inactive for 7 days user will  have to
    login by using email and password. Server gets newly created Refresh Token from DB.<br>

- **Step 5: Server sends Refresh and Access Tokens to the User.**<br><br>
  
</td>
</tr>
<tr>
<td colspan="2">
    
**Request**
    
```json
{
  "email": "admin@authdemo.com",
  "password": "12345678"
}
```
</td>
<td colspan="1">
    
**Response 200 OK:**
    
```json
{
  "accessToken": "****",
  "refreshToken": "****"
}
```

</td>
<td colspan="1">

**Response 400 Bad Request on Model Validation**
    
**Response 400 Bad Request:**<br>
"Invalid login attempt"

**Response 400 Bad Request:**<br>
"Too many unsuccessful login attempts, your account is temporarily locked. Please try again in 5 minutes."

</td>
</tr>
</table>
 

</details>
<br>
<!--END POST /api/auth/login ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>





<!--BEGIN POST api/Auth/Logout ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **POST** **api/Auth/Logout**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**POST**

</td>
<td> 
    
**api/Auth/Logout**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>NoPolicy, Claims contain current user to logout** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/TokenInvalidation23.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_Logout/Logout4.gif?raw=true) 

</td>
<td  colspan="2">
    
- **Step 1: User sends logout request** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2 and 3.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Invalidation of User's Current Access Token**<br>
    Server sends request to Redis to invalidate Access Token of a current logged-in user.
    Redis deletes Access Token and sends response to the Server. <br><br>
    
- **Step 3: Invalidation of User's Current Refresh Token from DB**<br>
    Server sends request to DB to invalidate Refresh Token.
    JWT Access Token contains RefreshToken Id that is used to found RefreshToken in DB.
    DB will update that Refresh Token to revoked and send response to the Server.<br><br>

- **Step 4: Server sends response to the User**<br>
      

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**
    
</td>
<td colspan="1">
    
**Response 200 OK:**<br>
"Logout successful"   

</td>
<td colspan="1">
    
**Response 400 Bad Request**<br>
"Unable to logout"

**Response 400 Bad Request**<br>
"Invalid logout attempt"

**Response 401 Unauthorized**

</td>
</tr>
</table>

</details>
 <br>
<!--END POST api/Auth/Logout ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


<!--BEGIN POST api/Auth/LogoutAllSessions ------------------------------------------------------------------------------------------------------------------------------->
<!----------------------------------------------------------------------------------------------------------------------------------------------------------------------->
### **POST** **api/Auth/LogoutAllSessions**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**POST**

</td>
<td> 
    
**api/Auth/LogoutAllSessions**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>NoPolicy, Claims contain current user to logout from all sessions** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/TokenInvalidation23.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_Logout/Logout4.gif?raw=true) 

</td>
<td  colspan="2">

- **Step 1: User sends logout all sessions request** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2 and 3.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>
    
- **Step 2: Invalidation of All User's Access Tokens**<br>
    Server sends request to Redis to invalidate all Access Tokens of a current logged-in user.
    Redis deletes all Access Tokens and sends response to the Server. <br><br>
    
- **Step 3: Invalidation of All User's Refresh Tokens**<br>
    Server sends request to DB to invalidate all Refresh Tokens of a current logged-in user.
    DB will update all Refresh Tokens to revoked and send response to the Server.<br><br>

- **Step 4: Server sends response to the User**<br>

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**
    
</td>
<td colspan="1">
    
**Response 200 OK:**<br>
"Logout successful"   
    
</td>
<td colspan="1">
    
**Response 400 Bad Request**<br>
"Unable to logout"

**Response 400 Bad Request**<br>
"Invalid logout attempt"

**Response 401 Unauthorized**

</td>
</tr>
</table>

</details>
 <br>
<!--END POST api/Auth/LogoutAllSessions ------------------------------------------------------------------------------------------------------------------------------->
<!--------------------------------------------------------------------------------------------------------------------------------------------------------------------->


<!--BEGIN POST /api/Auth/ChangePassword ------------------------------------------------------------------------------------------------------------------------------->
<!--------------------------------------------------------------------------------------------------------------------------------------------------------------------->
### **POST** **api/Auth/ChangePassword**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**POST**

</td>
<td> 
    
**api/Auth/ChangePassword**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>NoPolicy, Custom Code Validation: User is Admin or changing own password** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword3.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword456.gif?raw=true) 
       
</td>
<td  colspan="2">
    
- **Step 1: User sends change password request** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and user's Claims in the Access Token.
    If that token and claims are valid as well as request body go to 1b midstep and then to step 2 and 3.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>
    
- **Step 2: Check User and Request's Data**<br>
    Server sends request to DB to check and validate if User exists. DB sends response to the Server.<br><br>
- **Step 3: Server sends request to update user.** <br>
    Server sends request to DB to update User's password. DB updates the User and sends response to the Server.<br><br>
    
- **Step 4: Invalidation of All User's Access Tokens**<br>
    Server sends request to Redis to invalidate all Access Tokens of a current logged-in user.
    Redis deletes all Access Tokens and sends response to the Server. <br><br>
    
- **Step 5: Invalidation of All User's Refresh Tokens**<br>
    Server sends request to DB to invalidate all Refresh Tokens of a current logged-in user.
    DB will update all Refresh Tokens to revoked and send response to the Server.<br><br>

- **Step 6: Server sends response to the User**<br>

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

```json
{
  "userId": 0,
  "currentPassword": "string",
  "newPassword": "stringst",
  "confirmNewPassword": "string"
}
```
    
</td>
<td colspan="1">
    
**Response 200 OK**
    
</td>
<td colspan="1">
    
**Response 400 Bad Request on Model Validation**

**Response 404 Not Found:**<br>
"User does not exist"

**Response 403 Forbidden**

**Response 401 Unauthorized**

</td>
</tr>
</table>

</details>
 <br>
<!--END POST /api/Auth/ChangePassword ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------------->




<!--BEGIN POST /api/Auth/ChangeEmail ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **POST** **api/Auth/ChangeEmail**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**POST**

</td>
<td> 
    
**api/Auth/ChangeEmail**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>NoPolicy, Custom Code Validation: User is Admin or changing own email** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword3.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword456.gif?raw=true) 
        
</td>
<td  colspan="2">

- **Step 1: User sends change email request** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and user's Claims in the Access Token.
    If that token and claims are valid as well as request body go to 1b midstep and then to step 2 and 3.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Check User and Request's Data**<br>
    Server sends request to DB to check and validate if User exists, if new email is used and if current email in request body is correct. DB sends response to the Server.<br><br>

- **Step 3: Server sends request to update user.** <br>
    Server sends request to DB to update User's email. DB updates the User and sends response to the Server.<br><br>
    
- **Step 4: Invalidation of All User's Access Tokens**<br>
    Server sends request to Redis to invalidate all Access Tokens of a current logged-in user.
    Redis deletes all Access Tokens and sends response to the Server. <br><br>
    
- **Step 5: Invalidation of All User's Refresh Tokens**<br>
    Server sends request to DB to invalidate all Refresh Tokens of a current logged-in user.
    DB will update all Refresh Tokens to revoked and send response to the Server.<br><br>

- **Step 6: Server sends response to the User**<br>
    

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

```json
{
  "userId": 0,
  "currentEmail": "user@example.com",
  "newEmail": "user@example.com"
}
```
    
</td>
<td colspan="1">
    
**Response 200 OK**
    
</td>
<td colspan="1">
    
**Response 400 Bad Request on Model Validation**
    
**Response 404 Not Found:**<br>
"User does not exist"

**Response 400 Bad Request:**<br>
"Wrong current user email"

**Response 400 Bad Request:**<br>
"Choose another new email"

**Response 403 Forbidden**

**Response 401 Unauthorized**

</td>
</tr>
</table>

</details>
 <br>
<!--END POST /api/Auth/ChangeEmail ------------------------------------------------------------------------------------------------------------------------------->
<!---------------------------------------------------------------------------------------------------------------------------------------------------------------->


<!--BEGIN POST /api/AuthTokens/RefreshToken ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------------------->
### **POST** **api/AuthTokens/RefreshToken**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**POST**

</td>
<td> 
    
**api/AuthTokens/RefreshToken**

</td>
<td colspan="2">
    
**EndPoint Authorization: None/Anonymous** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)     

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_RefreshToken/RefreshToken2.gif?raw=true)     

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_RefreshToken/RefreshToken3.gif?raw=true)     

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_RefreshToken/RefreshToken4.gif?raw=true)    

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_RefreshToken/RefreshToken567.gif?raw=true)    
        
</td>
<td  colspan="2">

 - **Step 1: User sends request to get new access token** <br>
    Server validates the request body.
    Access Token has to be expired and generated by the Server.
    Go to step 2<br><br>
- **Step 2: Validation of Refresh Token from request body**<br>
    Server sends request to DB. Refresh Token must exist in DB, must not be used, expired or revoked as well as it's AccessTokenId property  has to be the same as  AccessToken Id from body's Access Token.
    If Accesss Token is not expired go to optional Step 3 otherwise go to Step 4.<br><br>
- **Step 3: Invalidation of User's Access Token**<br>
    Server sends request to Redis to invalidate Access Token from request body.
    Redis deletes Access Token and sends response to the Server. Go to step 4. <br><br>
- **Step 4: Validation of the User from Access Token**<br>
    Server sends request to DB to check and validate if User from expired Access Token exists or is active. DB sends response to the Server.<br><br>
- **Step 5: Creation of Access Token**<br>
    Server sends request to Redis to create Access Token.
    Server gets newly created Access Token from Redis.<br><br>  
- **Step 6: Creation of Refresh Token**<br>
    Server sends request to DB to create Refresh Token. Server gets newly created Refresh Token from DB.<br><br>
- **Step 7: Server sends Refresh and Access Tokens to the User.**<br><br>
    
     

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

```json
{
  "accessToken": "string",
  "refreshToken": "string"
}
```
    
</td>
<td colspan="1">
    
**Response 200 OK**

```json
{
  "accessToken": "****",
  "refreshToken": "****"
}
```
    
</td>
<td colspan="1">
    
**Response 400 Bad Request on Model Validation**

**Response 401 Unauthorized:**<br>
"Invalid tokens"

</td>
</tr>
</table>

</details>
 <br>
<!--END POST /api/AuthTokens/RefreshToken ------------------------------------------------------------------------------------------------------------------------------->
<!----------------------------------------------------------------------------------------------------------------------------------------------------------------------->




<!--BEGIN PUT /api/AuthTokens/InvalidateUserTokens/{id}------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **PUT** **api/AuthTokens/InvalidateUserTokens/{id}**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**PUT**

</td>
<td> 
    
**api/AuthTokens/InvalidateUserTokens/{id}**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>Policy=Admin** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_RefreshToken/RefreshToken2.gif?raw=true)     

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_InvalidateTokens/InvalidateTokens345.gif?raw=true)          

</td>
<td  colspan="2">
    
- **Step 1: User send request to invalidate all tokens of a specific user** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Server sends request to DB** <br>
    Server checks if specifc user exists in DB. If user exists go to Step 3 and Step 4.<br><br>
    
- **Step 3: Invalidation of All User's Access Tokens**<br>
    Server sends request to Redis to invalidate all Access Tokens of a current logged-in user.
    Redis deletes all Access Tokens and sends response to the Server. <br><br>
    
- **Step 4: Invalidation of All User's Refresh Tokens**<br>
    Server sends request to DB to invalidate all Refresh Tokens of a current logged-in user.
    DB will update all Refresh Tokens to revoked and send response to the Server.<br><br>

- **Step 5: Server sends response to the User**<br>    

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

</td>
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">
    
**Response 404 Not Found:**<br>
"User does not exist"

**Response 403 Forbidden**

**Response 401 Unauthorized**

</td>
</tr>
</table>

</details>
 <br>
<!--END PUT /api/AuthTokens/InvalidateUserTokens/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------->


<!--BEGIN GET /api/Chores------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **GET** **api/Chores**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**GET**

</td>
<td> 
    
**api/Chores**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>NoPolicy** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Response3.gif?raw=true)     
        
</td>
<td  colspan="2">

- **Step 1: User send request to get all chores** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Fetching all Chores**<br>
    Server sends request to DB to get all Chores. DB sends data to the Server.<br><br>

- **Step 3: Server sends response to the User**<br>    

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

</td>
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">
    
**Response 401 Unauthorized**

</td>
</tr>
</table>

</details>
 <br>
<!--END GET /api/Chores--------------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


<!--BEGIN POST /api/Chores------------------------------------------------------------------------------------------------------------------------------------>
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **POST** **api/Chores**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**POST**

</td>
<td> 
    
**api/Chores**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>Policy=AdminOrManager** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Response3.gif?raw=true)       
        
</td>
<td  colspan="2">


- **Step 1: User send request to create a chore** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Creation of Chore**<br>
    Server sends request to DB to create a Chore. DB creates a Chore and sends data to the Server.<br><br>
- **Step 3: Server sends response to the User**<br>
   

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

```json
{
  "title": "Chore 1",
  "description": "Chore 1 Desc"
}
```

</td>
<td colspan="1">

**Response 201 Created**

```json
{
  "id": "3"
}
```
 
</td>
<td colspan="1">
    
**Response 400 Bad Request on Model Validation**

**Response 401 Unauthorized**

**Response 403 Forbidden**

</td>
</tr>
</table>

</details>
 <br>
<!--END POST /api/Chores ------------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>



<!--BEGIN GET /api/Chores/{id}------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **GET** **api/Chores/{id}**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**GET**

</td>
<td> 
    
**api/Chores/{id}**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>NoPolicy** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Response3.gif?raw=true)  
        
</td>
<td  colspan="2">

- **Step 1: User send request to get specific chore**<br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Fetching specific Chore**<br>
    Server sends request to DB to get specific Chore. DB sends data to the Server.<br><br>

- **Step 3: Server sends response to the User**<br>       

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**


</td>
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">
    
**Response 401 Unauthorized**

**Response 404 Not Found**

</td>
</tr>
</table>

</details>
 <br>
<!--END GET /api/Chores/{id}------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


<!--BEGIN PUT /api/Chores/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **PUT** **api/Chores/{id}**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**PUT**

</td>
<td> 
    
**api/Chores/{id}**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>Policy=AdminOrManager** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword3.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_Logout/Logout4.gif?raw=true)   
        
</td>
<td  colspan="2">

- **Step 1: User send request to update a chore** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Validation of Chore**<br>
    Server sends request to DB to check if Chore exists. DB sends respond to the Server.<br><br>

- **Step 3: Update of Chore**<br>
    Server sends request to DB to update a Chore. DB updates a Chore and sends data to the Server.<br><br>
    
- **Step 4: Server sends response to the User**<br> 

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

```json
{
  "title": "Chore 1A",
  "description": "Chore 1A Desc"
}
```

</td>
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">
    
**Response 400 Bad Request on Model Validation**

**Response 404 Not Found**

**Response 401 Unauthorized**

**Response 403 Forbidden**

</td>
</tr>
</table>

</details>
 <br>
<!--END PUT /api/Chores/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


<!--BEGIN DELETE api/Chores/{id}*------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **DELETE** **api/Chores/{id}**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**DELETE**

</td>
<td> 
    
**api/Chores/{id}**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>Policy=AdminOrManager** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword3.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_Logout/Logout4.gif?raw=true)      
        
</td>
<td  colspan="2">


- **Step 1: User send request to delete a chore** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Validation of Chore**<br>
    Server sends request to DB to check if Chore exists. DB sends respond to the Server.<br><br>

- **Step 3: Deleteion of Chore**<br>
    Server sends request to DB to delete a Chore. DB deletes a Chore and sends data to the Server.<br><br>
    
- **Step 4: Server sends response to the User**<br> 

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**


</td>
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">

**Response 404 Not Found**
    
**Response 401 Unauthorized**

**Response 403 Forbidden**

</td>
</tr>
</table>

</details>
 <br>
<!--END DELETE /api/Chores/{id}*------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


<!--BEGIN PUT /api/Chores/AssignUser* ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **PUT** **api/Chores/AssignUser**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**PUT**

</td>
<td> 
    
**api/Chores/AssignUser**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>Policy=AdminOrManager** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword3.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_Logout/Logout4.gif?raw=true)  
        
</td>
<td  colspan="2">

- **Step 1: User send request to assing specific User to the Chore** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>
- **Step 2: Validation of User and Chore** <br>
   Server sends requests to DB to check for User and Chore. Db sends response to the Server. <br><br>
- **Step 3: Update of Chore**<br>
    Server sends request to DB to update a Chore. DB updates a Chore and sends data to the Server.<br><br>
- **Step 4: Server sends response to the User**<br>   

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

```json
{
  "choreId": 0,
  "userId": 0
}
```

</td>
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">
    
**Response 400 Bad Request on Model Validation**

**Response 404 Not Found**<br>
"User does not exists"

**Response 404 Not Found**<br>
"Chore does not exists"
    
**Response 401 Unauthorized**

**Response 403 Forbidden**

</td>
</tr>
</table>

</details>
 <br>
<!--END PUT /api/Chores/AssignUser*------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


<!--BEGIN PUT /api/Chores/Finish/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **PUT** **api/Chores/Finish/{id}**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**PUT**

</td>
<td> 
    
**api/Chores/Finish/{id}**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>NoPolicy, Custom Code Validation, User must be Admin or Manager otherwise can only finish Chore they were assigned to** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword3.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_Logout/Logout4.gif?raw=true)   
        
</td>
<td  colspan="2">

- **Step 1: User send request to finish the Chore** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>
- **Step 2: Validation of Chore** <br>
   Server sends requests to DB to check for Chore. Db sends response to the Server. <br><br>
- **Step 3: Update of Chore**<br>
    Server sends request to DB to update a Chore. DB updates a Chore and sends data to the Server.<br><br>
- **Step 4: Server sends response to the User**<br>   

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

</td>
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">
    
**Response 404 Not Found**<br>
"Chore does not exists"
    
**Response 401 Unauthorized**

**Response 403 Forbidden**

</td>
</tr>
</table>

</details>
 <br>
<!--END PUT /api/Chores/Finish/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>



<!--BEGIN PUT /api/Chores/Approve/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **PUT** **api/Chores/Approve/{id}**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**PUT**

</td>
<td> 
    
**api/Chores/Approve/{id}**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>Policy=AdminOrManager** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword3.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_Logout/Logout4.gif?raw=true)     
        
</td>
<td  colspan="2">

- **Step 1: User send request to approve the Chore** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>
- **Step 2: Validation of Chore** <br>
   Server sends requests to DB to check for Chore. Db sends response to the Server. <br><br>
- **Step 3: Update of Chore**<br>
    Server sends request to DB to update a Chore. DB updates a Chore and sends data to the Server.<br><br>
- **Step 4: Server sends response to the User**<br>   

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

</td>
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">
    
**Response 404 Not Found**<br>
"Chore does not exists"
    
**Response 401 Unauthorized**

**Response 403 Forbidden**

</td>
</tr>
</table>

</details>
 <br>
<!--END PUT /api/Chores/Approve/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>





<!--BEGIN POST /api/Users/ToggleUserActivation/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **POST** **api/Users/ToggleUserActivation/{id}**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**POST**

</td>
<td> 
    
**api/Users/ToggleUserActivation/{id}**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>Policy=Admin** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword3.gif?raw=true) 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword456.gif?raw=true)   
        
</td>
<td  colspan="2">

- **Step 1: User send request to activate/deactivate a specific User** <br>
   Server checks user's HTTP Authorization header for JWT bearer token and user's Claims in the Access Token.
    If that token and claims are valid as well as request body go to 1b midstep and then to step 2.<br><br>
- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Check User and Request's Data**<br>
    Server sends request to DB to check and validate if User exists. DB sends response to the Server.<br><br>
    
- **Step 3: Server sends request to update user.** <br>
    Server sends request to DB to update User. DB updates the User and sends response to the Server. Go to steps 4 and 5 is user is deactivated and then to Step 6.<br><br>
    
- **Step 4: Invalidation of All User's Access Tokens**<br>
    Server sends request to Redis to invalidate all Access Tokens of a current logged-in user.
    Redis deletes all Access Tokens and sends response to the Server. Go to step 6. <br><br>
    
- **Step 5: Invalidation of All User's Refresh Tokens**<br>
    Server sends request to DB to invalidate all Refresh Tokens of a current logged-in user.
    DB will update all Refresh Tokens to revoked and send response to the Server. Go to step 6. <br><br>

- **Step 6: Server sends response to the User**<br>

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

</td>
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">
    
**Response 404 Not Found**<br>
"User does not exists"
    
**Response 401 Unauthorized**

**Response 403 Forbidden**

</td>
</tr>
</table>

</details>
 <br>
<!--END POST /api/Users/ToggleUserActivation/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>



<!--BEGIN GET /api/Users ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **GET** **api/Users**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**GET**

</td>
<td> 
    
**api/Users**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>NoPolicy** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Response3.gif?raw=true)   
        
</td>
<td  colspan="2">

- **Step 1: User send request to get all Users** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Fetching all Users**<br>
    Server sends request to DB to get all Users. DB sends data to the Server.<br><br>

- **Step 3: Server sends response to the User**<br>   

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

</td>
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">
    
**Response 401 Unauthorized**

</td>
</tr>
</table>

</details>
 <br>
<!--END GET /api/Users ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>



<!--BEGIN POST /api/Users ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **POST** **api/Users**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**POST**

</td>
<td> 
    
**api/Users**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>Policy=Admin** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword3.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_Logout/Logout4.gif?raw=true)   
        
</td>
<td  colspan="2">

- **Step 1: User send request to create a User** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Validation of New user email**<br>
    Server sends request to DB to check if email is already used. DB sends respond to the Server.<br><br>

- **Step 3: Creation of User**<br>
    Server sends request to DB to create a User. DB sends data to the Server.<br><br>
    
- **Step 4: Server sends response to the User**<br>   

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

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
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">

**Response 400 Bad Request on Model Validation**

**Response 400 Bad Request**<br>
"User already exists"

**Response 401 Unauthorized**

**Response 403 Forbidden**

</td>
</tr>
</table>

</details>
 <br>
<!--END POST /api/Users ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>



<!--BEGIN GET /api/Users/{id}------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **GET** **api/Users/{id}**
<details>
  <summary>Click to expand!</summary>
  <br>
  <br>
<table>
<tr>
<td> 
    
**GET**

</td>
<td> 
    
**api/Users/{id}**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>NoPolicy** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthHandler1b.gif?raw=true)    

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Auth_ChangePassword/ChangePassword2.gif?raw=true) 

 ![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Response3.gif?raw=true)   
        
</td>
<td  colspan="2">

- **Step 1: User send request to get specific User** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Fetching specific User**<br>
    Server sends request to DB to get specific User. DB sends data to the Server.<br><br>

- **Step 3: Server sends response to the User**<br>    

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

</td>
<td colspan="1">
    
**Response 200 OK**
 
</td>
<td colspan="1">
    
**Response 401 Unauthorized**

**Response 404 Not Found**<br>
"User does not exists"

</td>
</tr>
</table>

</details>
 <br>
<!--END GET /api/Users/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>

## To Implement in the Future

| Feature               | Description                                                                                               |
|-----------------------|-----------------------------------------------------------------------------------------------------------|
| Global Error Handler  | Centralized error handling for improved error logging, standardized error responses, and user feedback.  |
| Logging               | Incorporation of logging mechanisms for monitoring, debugging, and better visibility into application behavior. |
| Database Concurrency  | Implementation of concurrency control mechanisms to prevent data inconsistency issues during concurrent access. |

<br>
<hr>

## Contributions

Contributions to this repository are welcome!

## Acknowledgments

- Special thanks to the .NET community for their invaluable contributions and support.
- Inspiration drawn from various tutorials and resources on authorization and authentication in .NET Web API.

## Contact

For any inquiries or feedback, please contact me at [domagojkk@gmail.com](mailto:domagojkk@gmail.com).




