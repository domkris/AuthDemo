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
<br>
<br>
<br>

## AuthDemo API Endpoints

### Auth

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
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/AuthDemo_LoginRequestA.gif?raw=true) 
        
</td>
<td  colspan="2">
    
- **Step 1: User sends login request** <br>
    Server checks user's email and password.
    User can be locked out on 5 wrong login attempts for 5 min.
    If user credentials are correct go to step 2 and 3.<br><br>
- **Step 2: Server Creates JWT Access Token, sends it to Redis**<br>
    Access Token is in format of JWT Token and is valid and
    stored in Redis for 10 minutes and deleted on expire.
    To get a new Access Token without using log-in user will have to
    send expired Access Token and current valid unexpired
    Refresh Token to refreshToken api endpoint.<br><br>
- **Step 3: Server creates Refresh token, sends it to DB**<br>
    Refresh Token is in format of random string
    and is valid for 7 days and can only be used once.
    Refresh Token is stored indefinitely, it is persistent.
    On each use of Refresh Token user gets a new refresh Token
    that is valid for another 7 days meaning that
    if user has been inactive for 7 days user will  have to
    login by using email and password.
    After all steps are successfull, Server sends
    Refresh and Access Tokens to the user.<br><br>
  
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
    
**Response 400 Bad Request:**<br>
"Invalid login attempt"

**Response 400 Bad Request:**<br>
"Too many unsuccessful login attempts, your account is temporarily locked. Please try again in 5 minutes."

</td>
</tr>
</table>
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
    

        
</td>
<td  colspan="2">
    
- **Step 1: User sends logout request** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and user's Claims in the Access Token.
    If that token and claims are valid go to step 2.<br><br>
- **Step 2: Server sends request to Redis to check if Access Token exists (Auth Handler)**<br>
    We do this step to make sure that user's Access Token was not invalidated, if all is good go to Step 3,
    if not then user has to use Refresh Token to get new Access Token.<br><br>
- **Step 3: Server sends request to Redis to get Access Token**<br>
    Server sends request to Redis to Access Token of a current logged in user. Go to Step 4 and Step 5.<br><br>
- **Step 4: Servers gets Refresh Token from DB**<br>
    JWT Access Token contains RefreshToken Id.
    If Refresh Token is found in DB from that Id Server will update Refresh Token to revoked (Step 6).<br><br>
- **Step 5: Server sends request to Redis**<br>
    Server sends request to Redis to expire/delete fetched Access Token of a current logged in user.<br><br>
- **Step 6: Server sends request to DB**<br>
    Server sends request to DB to revoke fetched Refresh Token of current logged in user.<br><br>    
      

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
    
**EndPoint Authorization: Authorized<br>NoPolicy, Claims contain current user to logout** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 
    

        
</td>
<td  colspan="2">

- **Step 1: User sends logout all sessions request** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and user's Claims in the Access Token.
    If that token and claims are valid go to step 2.<br><br>
- **Step 2: Server sends request to Redis to check if Access Token exists (Auth Handler)**<br>
    We do this step to make sure that user's Access Token was not invalidated, if all is good go to Step 3 and Step 4,
    if not then user has to use Refresh Token to get new Access Token.<br><br>
- **Step 3: Server sends request to Redis**<br>
    Server sends request to Redis to expire/delete all Access Tokens of a current logged in user.<br><br>
- **Step 4: Server sends request to DB**<br>
    Server sends request to DB to revoke all unrevoked and unexpired Refresh Tokens of current logged in user.<br><br>    

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
    

        
</td>
<td  colspan="2">
    
- **Step 1: User sends change password request** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and user's Claims in the Access Token.
    If that token and claims are valid as well as request body go to step 2.<br><br>
- **Step 2: Server sends request to Redis to check if Access Token exists (Auth Handler)**<br>
    We do this step to make sure that user's Access Token was not invalidated, if all is good go to Step 3.<br><br>
- **Step 3: Server sends request to update user.** <br>
    Server sends request to DB to update user. Go to Step 4 and 5.<br><br>
- **Step 4: Server sends request to Redis**<br>
    Server sends request to Redis to expire/delete all Access Tokens of a current logged in user.<br><br>
- **Step 5: Server sends request to DB**<br>
    Server sends request to DB to revoke all unrevoked and unexpired Refresh Tokens of current logged in user.<br><br>      

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

**Response 400 Bad Request:**<br>
"User does not exist"

**Response 403 Forbidden**

**Response 401 Unauthorized**

</td>
</tr>
</table>
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
    

        
</td>
<td  colspan="2">

    
- **Step 1: User sends change email request** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and user's Claims in the Access Token.
    If that token and claims are valid as well as request body go to Step 2.<br><br>
- **Step 2: Server sends request to Redis to check if Access Token exists (Auth Handler)**<br>
    We do this step to make sure that user's Access Token was not invalidated, if all is good go to Step 3.<br><br>
- **Step 3: Server does validation of user's data from DB** <br>
    Server sends request to DB to validate if new email is used and if current email is correct. Go to Step 4.<br><br>
- **Step 4: Server sends request to DB**<br>
    Server sends request to DB to update user's email. Go to Step 5 and Step 6.<br><br>
- **Step 5: Server sends request to Redis**<br>
    Server sends request to Redis to expire/delete all Access Tokens of a current logged in user.<br><br>
- **Step 6: Server sends request to DB**<br>
    Server sends request to DB to revoke all unrevoked and unexpired Refresh Tokens of current logged in user.<br><br>     
    

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
    
**Response 400 Bad Request:**<br>
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
<br>
<br>
<br>

### AuthTokens

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
    
        
</td>
<td  colspan="2">

 - **Step 1: User sends request to get new access token** <br>
    Server validates the request body and AccessToken.
    Access Token has to be expired and generated by Server.
    Go to step 2<br><br>
- **Step 2: Server sends request to DB to check Refresh Token from body**<br>
    Refresh Token must exist in DB, must not be used, expired or revoked as well as it's AccessTokenId property  has to be the same as  AccessToken Id from body's Access Token.
    If Accesss Token is not expired go to Step 3.Go To Step 4 and Step 5.<br><br>
- **Step 3: Server sends request to Redis**<br>
    Server sends request to Redis to expire/delete Access Token.<br><br>
- **Step 4: Server Creates JWT Access Token, sends it to Redis**<br><br>
- **Step 5: Server creates Refresh token, sends it to DB**<br><br>
     

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

 
        
</td>
<td  colspan="2">

 - **Step 1: User send request to invalidate all tokens of a specific user** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and user's Claims in the Access Token.
    If that token and claims are valid go to Step 2.<br><br>
- **Step 2: Server sends request to Redis to check if Access Token exists (Auth Handler)**<br>
    We do this step to make sure that user's Access Token was not invalidated, if all is good go to Step 3.<br><br>
- **Step 3: Server sends request to DB** <br>
    Server checks if specifc user exists in DB. If user exists go to Step 4 ad Step 5.<br><br>
- **Step 4: Server sends request to Redis to expire/delete all Access Tokens of a specific user**<br><br>
- **Step 5: Server sends request to DB to revoke all unrevoked and unexpired Refresh Tokens of a specific user**<br><br>       

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
    
**Response 400 Bad Request:**<br>
"User does not exist"

**Response 403 Forbidden**

**Response 401 Unauthorized**

</td>
</tr>
</table>
<br>
<br>
<br>

### Chores


<table>
<tr>
<td> 
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/GET_XS.png?raw=true)

</td>
<td> 
    
**api/Chores/Chores**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>NoPolicy** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

  
        
</td>
<td  colspan="2">

 - **Step 1: User send request to get all chores** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and user's Claims, if all is good go to step 2.<br><br>
- **Step 2: Server sends request to Redis to check if Access Token exists**<br>
    We do this step to make sure that user's Access Token was not invalidated, if all is good go to Step 3,
    if not then user has to use Refresh Token to get new Access Token..<br><br>
- **Step 3: Server sends request to DB to fecth all chores**<br><br>       

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
    
**Response 400 Bad Request:**<br>
"User does not exist"

**Response 401 Unauthorized**

</td>
</tr>
</table>
<br>
<br>


<table>
<tr>
<td> 
    
![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/POST_XS.png?raw=true)

</td>
<td> 
    
**api/Chores/Chores**

</td>
<td colspan="2">
    
**EndPoint Authorization: Authorized<br>Policy=AdminOrManager** 

</td>
</tr>
<tr>
<td rowspan="1" colspan="2"> 

  
        
</td>
<td  colspan="2">

 - **Step 1: User send request to create a chore** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and user's Claims, if all is good go to step 2.<br><br>
- **Step 2: Server sends request to Redis to check if Access Token exists**<br>
    We do this step to make sure that user's Access Token was not invalidated, if all is good go to Step 3,
    if not then user has to use Refresh Token to get new Access Token.<br><br>
- **Step 3: Server sends request to DB to create a chore**<br><br>       

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
<br>
<br>




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




