# AuthDemo: .NET Web API Authorization and Authentication Showcase

AuthDemo is a .NET Web API application designed to provide a practical learning demonstration of implementing authorization and authentication mechanisms in a .NET Web API application using JWT. Additionally, it showcases chore management functionalities, serving as an educational example of integrating these features into simulated and some real-world scenarios.
<br><br>
## Table of Contents

1. [App Overview](#app-overview)
2. [ASP.NET Authentication and Authorization](#asp.net-authentication-and-authorization)
3. [ASP.NET Core Identity](#asp.net-core-identity)
4. [Getting Started](#getting-started)
5. [API Endpoints](#api-endpoints)
6. [API Endpoints Details](#api-endpoints-details)
7. [Data Objects](#data-objects)
8. [Architecture Overview](#architecture-overview)
9. [Technical Highlights](#technical-highlights)
0. [Future Enhancements](#future-enhancements)
10. [Contributions](#contributions)
11. [Acknowledgments](#acknowledgments)
12. [Contact](#contact)

<br><br>
## App Overview

AuthDemo application provides secure authentication, allowing users to log in safely with assigned roles such as administrators, managers, or employees.
Users can efficiently manage chores, user accounts, and access permissions, while audit tracking ensures transparency across all actions within the system.<br><br>

### AuthDemo Capabilities

![promisechains](https://github.com/domkris/files/blob/master/AuthDemo/Roles.svg?raw=true) 


| Capability            | Description                                                                                               |
|--------------------|-----------------------------------------------------------------------------------------------------------|
| Secure Authentication | Users can securely log in to our application using JWT tokens. Upon successful authentication, a JWT token is issued, allowing users access to protected resources throughout their session.   |
| Role-Based Authorization | Users can be assigned one of three roles: Administrator, Manager, or Employee, each with different levels of access and permissions within the application.   |
| Chore Management      | All users can view all chores and mark them as finished. Managers and Admins can approve chores, assign others or themselves to specific chores. Additionally, they have the authority to create, edit, and delete chores. |
| User Management    | Users can view all users or specific user profiles. Admins can create new user accounts and activate or deactivate existing ones. Users can also modify their own email addresses and passwords, as well as those of other users, if granted appropriate permissions. |
| Token Management   | Users have the ability to invalidate their specific token, all of their tokens, or the tokens of other users if they have administrative privileges. This feature enhances security measures, allowing users to maintain control over access to their accounts. |
| Audit tracking | Audit tracking of entities is implemented to track changes made withing AuthDemo application.   |

<br><br>

## ASP.NET Authentication and Authorization

**Authentication** is confirming a user's identity, a process in which a user provides credentials that are compared to those stored in an operating system, database, app or resource.<br><br> 
**Authorization** means checking if an authenticated user is allowed to access a resource.<br><br>

### Claims and Identity

On each request, we get the user from **HttpContext** as a *ClaimsPrincipal*.<br> 
**HttpContext** is like the environment in which the app is running, and it contains info about the current request and response.<br>

**ClaimsPrincipal** is a collection of user's identities (*ClaimsIdentity*), each identity is containing user's claims.<br>
**Identity** is like a document that proves who you are, identity is represented by set of information about the user (age, name, role), each of those infos is a **Claim**.<br>

Sometimes a user can have more identities, for example:<br>
Let's say that you are an employee but also have a gym membership. One identity (*employee card*) is a set of claims that represent your employee identity which contains claims like: emplyee Id, role, status etc.<br> 
Other identity (*gym card*) represent your gym identity and contains claims like membership Id, membership type etc. In real life scenario, a passport would be an identity that proves your nationality.

<br>

### Authentication configuration 

In ASP.NET Core, authentication is handled by authentication service (*IAuthenticationService*) and authentication middleware. Authentication service uses authentication handlers, called "schemes", to perform tasks like user authentication and handling unauthorized access attempts.

Authentication services are registered in Program.cs (*builder.Services.AddAuthentication()*).<br>
Schemes are registered by calling specific methods like *AddJwtBearer* or *AddCookie* after AddAuthentication in Program.cs.<br>
Example that defines using schemes that allows us to use both Cookies and JWT tokens for authentication from [official Microsoft documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-8.0)):

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("JwtSettings", options))
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("CookieSettings", options));
```

The Authentication middleware is added to Program.cs by calling *UseAuthentication*.<br>
Calling *UseAuthentication* registers the middleware that uses the previously registered authentication schemes.<br> 
Call *UseAuthentication* before any middleware that depends on users being authenticated.


### Authentication scheme configuration

It is a setup of how authentication will work in the application. It involves defining mechanisms used to verify a user's identity. Examples:

- Setting up JWT (JSON Web Token) authentication.
	- Configuring thg JWT bearer auth scheme
    - Specify options like token validation parameters, token expiration, issuer, audience, signing key.
- Setting up Cookie based authentication
	- Configuring Cookie name, expiration time, login path, etc.
- Setting up external authentication providers like Oauth 2.0, OIDC (Facebook, Twitter/X, Google, Microsoft or other)
	- Configuring connections using industry-defined protocols between our application and other unrelated applications to share user profile information.

Configuration of AuthDemo JWT  bearer scheme:

```csharp
 TokenValidationParameters tokenValidationParameters = new TokenValidationParameters() 
 {
     ValidateIssuer = true,
     ValidIssuer = configuration.GetSection(nameof(JwtSettings))["Issuer"],
     ValidateAudience = true,
     ValidAudience = configuration.GetSection(nameof(JwtSettings))["Audience"],
     ValidateIssuerSigningKey = true,
     IssuerSigningKey = new SymmetricSecurityKey(secretKey),
     ValidateLifetime = true,
     RequireExpirationTime = true,
     ClockSkew = TimeSpan.Zero
 };

services.AddAuthentication(configureOptions =>
{
    configureOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    configureOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwtBearerOptions =>
{

    //...
    jwtBearerOptions.Events = new JwtBearerEvents
    {
       OnTokenValidated = context => {
           long.TryParse(context.Principal.FindFirstValue(ClaimTypes.NameIdentifier), out long userId);
           string? tokenId = context.Principal.FindFirstValue(JwtRegisteredClaimNames.Jti);

           // ...
           var result = tokenService.IsAccessTokenCached(tokenId, userId).Result;
           if (!result)
           {
               context.Fail("Access token expired. Use the refresh token to obtain a new access token.");
           }
           return Task.CompletedTask;
       }
    };
});
```
<br>
Authentication scheme actions:

| Action            | Description                                                                                               |
|--------------------|-----------------------------------------------------------------------------------------------------------|
| Authenticate | Responsible for constructing user's identity. Cookie authentication scheme constructs user's identity from cookies. JWT bearer scheme deserializes and validates JWT token to construct user's identity.|
| Challenge | Invoked when an unauthenticated user requests resource from enpoint that requires authentication. Cookie auth scheme will redirect the user to login page. JWT bearer scheme will return a 401 result with a www-authenticate: bearer header.|
| Forbid | Invoked when user is authenticated but not permitted to access. Cookie auth scheme will redirect the user to a page indicating that access was forbidden. JWT bearer scheme will return 403 result. |

<br><br>

### Authorization

You can use a combination of:
- Policy *(a rule or a requirement that has to be satisfied)* based authorization.
- Claim *(a part of identity)* based authorization.
- Role *(user's role, like admin)* based authorization.
  
Registering the policy takes place as part of the Authorization service configuration, typically in the Program.cs.

Role-based example from [official Microsoft documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-8.0):

```csharp
builder.Services.AddAuthorization(options => { 
    options.AddPolicy("RequireAdministratorRole", 
        policy => policy.RequireRole("Administrator")); 
});

//...

app.UseAuthorization();

```

Policy-based example from [official Microsoft documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-8.0):

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AtLeast21", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(21)));
});
```

Claim-based authorization in AuthDemo *(based on a user's role in Claim that are part of the JWT bearer token)*:

```csharp
options.AddPolicy(AuthDemoPolicies.Roles.Admin, policy =>
{
    policy.RequireClaim(ClaimTypes.Role, Roles.Administrator.GetValue());
});

options.AddPolicy(AuthDemoPolicies.Roles.Manager, policy =>
{
    policy.RequireClaim(ClaimTypes.Role, Roles.Manager.GetValue());
});

options.AddPolicy(AuthDemoPolicies.Roles.AdminOrManager, policy =>
{
    policy.RequireClaim(
        ClaimTypes.Role,
        Roles.Administrator.GetValue(),
        Roles.Manager.GetValue());
});
```

To apply the policy, use the Policy property on the [Authorize] attribute:
```csharp
[Authorize(Policy = Policies.Roles.AdminOrManager)]
```

<br><br>
## ASP.NET Core Identity

It is a ASP.NET CORE built-in authentication provider.<br>
It is an API that supports user interface login functionality, manages users, user registration, passwords, profile data, roles, claims, tokens, email configuration and more.
You do not need to use it, you can create your custom User and Role classes and use dbContext to do DB operations.<br><br>

By default, it creates tables in your DB using EntityFramework. Running your first migrations, you can see these tables:

| Table            |
|--------------------|
| dbo.AspNetRoleClaims | 
| dbo.AspNetRoles | 
| dbo.AspNetUserClaims| 
| dbo.AspNetUserLogins| 
| dbo.AspNetUserRoles|
| dbo.AspNetUsers| 
| dbo.AspNetUserTokens| 

The most important for AuthDemo are *AspNetUsers* and *AspNetRoles*. You can customize those tables and add new fields, for example in AuthDemo, a user can have only one role, but ASP.NET Identity by default defines multiple roles per user.<br> 
More info about the tables and fields they contain can be found [here](https://dotnettutorials.net/lesson/asp-net-core-identity-tables/).

Functionalities:
- **UserManager** class in ASP.NET Core Identity interacts with the AspNetUsers table to handle user-related operations like creation, deletion, updating profiles, etc.
- **SignInManager** class in ASP.NET Core Identity handles sign-in operations, including password checks, cookie management, etc.
- **RoleManager** class in ASP.NET Core Identity provides the RoleManager class to manage roles. This class is used to create, delete, and update roles and to assign roles to users.

For more info about ASP.NET Identity, check out [this link](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio).
You are free to not use ASP.NET Core Identity and use your custom users, roles, etc. I used it because it simplifies the validation of user registration, login, configuring password rules and other security functionality.
<br><br>

**ASP.NET Core Identity vs IdentityServer** <br>

ASP.NET Core Identity is not related to [Duende IdentityServer](https://duendesoftware.com/products/identityserver) or [Microsoft Entra ID](https://www.microsoft.com/en-us/security/business/identity-access/microsoft-entra-id) *(formerly Azure Active Directory)*.<br>
These  are frameworks that act as an authentication and authorization server and provide centralized authentication and single sign-on (SSO) capabilities across multiple applications and services.

Possible implementation for demonstration of a separate Authentication and Authorization OIDC Server:
- Authentication and Authorization is integrated into AuthDemo as part of *Security project/ class library*.<br>
We could  customize and exclude Security project from AuthDemo and move it to *separate  Web API application* that will act as a centralized auth server for AuthDemo and any other app.
AuthDemo would communicate with that IdentityServer to authorize and authenticate user.
That new IdentityServer could use ASP.NET Core Identity to manage users, tokens, roles etc.


<br><br>
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

11. **Interact with Endpoints**: Use tools like [Postman](https://www.postman.com/downloads/), [curl](https://curl.se/download.html) or the built-in Swagger UI to interact with the endpoints. You can send requests to endpoints such as login, logout, create users, manage chores, etc.

12. **Follow the Tutorial**: For a detailed understanding of how authorization and authentication are implemented within the application, refer to the tutorial provided in the repository. The tutorial offers step-by-step guidance and explanations on key concepts and functionalities.

13. **Explore Endpoint Details**: Each endpoint has detailed explanations provided in the repository. Refer to the [API Endpoints Details](#api-endpoints-details) to understand the purpose and usage of each endpoint effectively.

By following these steps, you'll be able to navigate through AuthDemo, understand its functionalities, and gain insights into implementing authorization and authentication mechanisms within your own .NET Web API applications.
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
| /api/auth/toggleUserActivation/{id}           | Activate/Deactivate user              | POST   |
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
    
- **Step 1: User sends a login request to Server** <br><br>

- **Step 2: Validation of the user's email and password.** <br>
    Server sends a request to DB to check the user's email and password.
    The U=user can be locked out on five wrong login attempts for 5 minutes.
    The server gets a response from DB, and if the user's credentials are correct, go to step 3 and 4.<br><br>
  
- **Step 3: Creation of the Access Token**<br>
    Server sends request to Redis to create the Access Token. Access Token is in the format of JWT Token and is valid and
    stored in Redis for 10 minutes and deleted on expire. Server gets newly created Access Token from Redis.<br><br>
    
- **Step 4: Creation of the Refresh Token**<br>
    Server sends request to DB to create the Refresh Token. Refresh Token is in format of a random string
    and is valid for 7 days and can only be used once.
    The Refresh Token is stored indefinitely, it is persistent.
    On each use of Refresh Token, the user gets a new refresh Token
    that is valid for another 7 days, meaning that
    if user has been inactive for 7 days user will  have to
    login by using an email and password. Server gets newly created Refresh Token from DB.<br>

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
    
- **Step 1: User sends a logout request** <br>
    Server checks user's HTTP Authorization header for the JWT bearer token and the User's Claims in the Access Token.
    If the Access Token and User's Claims are valid, go to 1b midstep and then to step 2 and 3.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that the User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Invalidation of the User's Current Access Token**<br>
    Server sends request to Redis to invalidate the Access Token of a current logged-in user.
    Redis deletes Access Token and sends response to the Server. <br><br>
    
- **Step 3: Invalidation of the User's Current Refresh Token from DB**<br>
    Server sends a request to DB to invalidate the Refresh Token.
    JWT Access Token contains RefreshToken's Id that is used to found the RefreshToken in DB.
    DB will update that Refresh Token to be revoked and send a response to the Server.<br><br>

- **Step 4: Server sends a response to the User**<br>
      

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
    Server checks user's HTTP Authorization header for the JWT bearer token and the User's Claims in the Access Token.
    If the Access Token and User's Claims are valid, go to 1b midstep and then to step 2 and 3.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that the User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>
    
- **Step 2: Invalidation of All User's Access Tokens**<br>
    Server sends request to Redis to invalidate all Access Tokens for a current logged-in user.
    Redis deletes all Access Tokens and sends response to the Server. <br><br>
    
- **Step 3: Invalidation of All User's Refresh Tokens**<br>
    Server sends request to DB to invalidate all Refresh Tokens for a current logged-in user.
    DB will update all Refresh Tokens to be revoked and send a response to the Server.<br><br>

- **Step 4: Server sends a response to the User**<br>

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
    
- **Step 1: User sends a change password request** <br>
    Server checks user's HTTP Authorization header for the JWT bearer token and the user's Claims in the Access Token.
    If that token and claims are valid, as well as the request body, go to 1b midstep and then to step 2 and 3.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that the User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>
    
- **Step 2: Check User and Request's Data**<br>
    Server sends request to the DB to check and validate if User exists. DB sends response to the Server.<br><br>
- **Step 3: Server sends a request to update user.** <br>
    Server sends a request to DB to update the User's password. DB updates the User and sends a response to the Server.<br><br>
    
- **Step 4: Invalidation of All User's Access Tokens**<br>
    Server sends request to Redis to invalidate all Access Tokens of a current logged-in user.
    Redis deletes all Access Tokens and sends a response to the Server. <br><br>
    
- **Step 5: Invalidation of All User's Refresh Tokens**<br>
    Server sends request to DB to invalidate all Refresh Tokens of a current logged-in user.
    DB will update all Refresh Tokens to revoked and send a response to the Server.<br><br>

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

**Response 400 Bad Request **<br>
"Unable to invalidate tokens"

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

- **Step 1: User sends a change email request** <br>
    Server checks user's HTTP Authorization header for the JWT bearer token and the User's Claims in the Access Token.
    If that token and claims are valid, as well as the request body, go to 1b midstep and then to step 2 and 3.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that the User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Check User and Request's Data**<br>
    Server sends request to DB to check and validate if User exists, if a new email is used, and if the current email in the request body is correct. DB sends response to the Server.<br><br>

- **Step 3: Server sends a request to update the user.** <br>
    Server sends a request to DB to update the User's email. DB updates the User and sends response to the Server.<br><br>
    
- **Step 4: Invalidation of All User's Access Tokens**<br>
    Server sends a request to Redis to invalidate all Access Tokens of a current logged-in user.
    Redis deletes all Access Tokens and sends a response to the Server. <br><br>
    
- **Step 5: Invalidation of All User's Refresh Tokens**<br>
    Server sends a request to DB to invalidate all Refresh Tokens of a current logged-in user.
    DB will update all Refresh Tokens to be revoked and send a response to the Server.<br><br>

- **Step 6: Server sends a response to the User**<br>
    

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

**Response 400 Bad Request **<br>
"Unable to invalidate tokens"
    
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

<!--BEGIN POST /api/Auth/ToggleUserActivation/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **POST** **api/Auth/ToggleUserActivation/{id}**
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
    
**api/Auth/ToggleUserActivation/{id}**

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

- **Step 1: User send a request to activate/deactivate a specific User** <br>
   Server checks the User's HTTP Authorization header for the JWT bearer token and the user's Claims in the Access Token.
    If that token and claims are valid, as well as the request body, go to 1b midstep and then to step 2.<br><br>
- **Step 1b: Validation of the Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that the User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Check User and Request's Data**<br>
    Server sends request to the DB to check and validate if User exists. DB sends a response to the Server.<br><br>
    
- **Step 3: Server sends request to update user.** <br>
    Server sends request to the DB to update User. DB updates the User and sends a response to the Server. Go to steps 4 and 5 is user is deactivated and then to Step 6.<br><br>
    
- **Step 4: Invalidation of All User's Access Tokens**<br>
    Server sends a request to Redis to invalidate all Access Tokens for a current logged-in user.
    Redis deletes all Access Tokens and sends a response to the Server. Go to step 6. <br><br>
    
- **Step 5: Invalidation of All User's Refresh Tokens**<br>
    Server sends request to DB to invalidate all Refresh Tokens for a current logged-in user.
    DB will update all Refresh Tokens to be revoked and send a response to the Server. Go to step 6. <br><br>

- **Step 6: Server sends a response to the User**<br>

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

</td>
<td colspan="1">
    
**Response 200 OK**<br>
 "user: {user.Email} deactivated"<br>
 "user: {user.Email} activated"
 
</td>
<td colspan="1">
    
**Response 404 Not Found**<br>
"User does not exists"

**Response 400 Bad Request **<br>
"Unable to invalidate tokens"
    
**Response 401 Unauthorized**

**Response 403 Forbidden**

</td>
</tr>
</table>

</details>
 <br>
<!--END POST /api/auth/ToggleUserActivation/{id} ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


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

 - **Step 1: User sends a request to get new Access Token** <br>
    Server validates the request body.
    The Access Token has to be expired and generated by the Server.
    Go to step 2<br><br>
- **Step 2: Validation of the Refresh Token from the request body**<br>
    Server sends a request to DB. The Refresh Token must exist in DB, must not be used, expired or revoked as well as it's AccessTokenId property  has to be the same as  AccessToken Id from body's Access Token.
    If Accesss Token is not expired go to optional Step 3 otherwise go to Step 4.<br><br>
- **Step 3: Invalidation of User's Access Token**<br>
    Server sends request to Redis to invalidate Access Token from request body.
    Redis deletes Access Token and sends response to the Server. Go to step 4. <br><br>
- **Step 4: Validation of the User from Access Token**<br>
    Server sends request to DB to check and validate if User from expired Access Token exists or is active. DB sends response to the Server.<br><br>
- **Step 5: Creation of Access Token**<br>
    Server sends request to Redis to create Access Token.
    Server gets newly created Access Token from Redis.<br><br>  
- **Step 6: Creation of the Refresh Token**<br>
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
    
**Response 400 Bad Request **<br>
"Unable to invalidate tokens"

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
    Server checks if specific user exists in DB. If user exists go to Step 3 and Step 4.<br><br>
    
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

 ```json
[
  {
    "id": 1,
    "title": "Chore 1",
    "description": "chore chore 1",
    "isFinished": false,
    "isApproved": false,
    "createdBy": {
      "id": 3,
      "username": "alicemanager",
      "email": "alice@authdemo.com"
    },
    "updatedBy": null,
    "userAssignee": null,
    "createdAt": "2024-04-05T08:27:38.009824+00:00",
    "updatedAt": null
  },
  {
    "id": 2,
    "title": "Chore 2",
    "description": "chore desc2",
    "isFinished": false,
    "isApproved": false,
    "createdBy": {
      "id": 3,
      "username": "alicemanager",
      "email": "alice@authdemo.com"
    },
    "updatedBy": null,
    "userAssignee": null,
    "createdAt": "2024-04-05T08:29:04.774988+00:00",
    "updatedAt": null
  }
]
```

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


- **Step 1: User send a request to create a chore** <br>
    Server checks user's HTTP Authorization header for the JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Creation of Chore**<br>
    Server sends request to DB to create a Chore. DB creates a Chore and sends data to the Server.<br><br>
- **Step 3: Server sends a response to the User**<br>
   

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

- **Step 1: User send a request to get specific chore**<br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Fetching specific Chore**<br>
    Server sends request to DB to get specific Chore. DB sends data to the Server.<br><br>

- **Step 3: Server sends a response to the User**<br>       

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**


</td>
<td colspan="1">
    
**Response 200 OK**

```json
{
  "id": 2,
  "title": "Chore 2",
  "description": "chore desc2",
  "isFinished": false,
  "isApproved": false,
  "createdBy": {
    "id": 3,
    "username": "alicemanager",
    "email": "alice@authdemo.com"
  },
  "updatedBy": null,
  "userAssignee": null,
  "createdAt": "2024-04-05T08:29:04.774988+00:00",
  "updatedAt": null
}
```
 
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


- **Step 1: User send a request to delete a chore** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of the Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Validation of Chore**<br>
    Server sends request to DB to check if Chore exists. DB sends respond to the Server.<br><br>

- **Step 3: Deleteion of Chore**<br>
    Server sends request to DB to delete a Chore. DB deletes a Chore and sends data to the Server.<br><br>
    
- **Step 4: Server sends a response to the User**<br> 

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

- **Step 1: User send request to assign specific User to the Chore** <br>
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

- **Step 1: User send a request to finish the Chore** <br>
    Server checks user's HTTP Authorization header for JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>
- **Step 2: Validation of Chore** <br>
   Server sends a requests to the DB to check for Chore. Db sends response to the Server. <br><br>
- **Step 3: Update of Chore**<br>
    Server sends a request to the DB to update a Chore. DB updates a Chore and sends data to the Server.<br><br>
- **Step 4: Server sends a response to the User**<br>   

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

- **Step 1: User send a request to approve the Chore** <br>
    Server checks user's HTTP Authorization header for the JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid, go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of the Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that the User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>
- **Step 2: Validation of Chore** <br>
   Server sends requests to DB to check for Chore. Db sends response to the Server. <br><br>
- **Step 3: Update of Chore**<br>
    Server sends a request to DB to update a Chore. DB updates a Chore and sends data to the Server.<br><br>
- **Step 4: Server sends a response to the User**<br>   

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

- **Step 1: User send a request to get all Users** <br>
    Server checks user's HTTP Authorization header for the JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid, go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of the Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that the User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Fetching all Users**<br>
    Server sends a request to DB to get all Users. DB sends data to the Server.<br><br>

- **Step 3: Server sends a response to the User**<br>   

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

</td>
<td colspan="1">
    
**Response 200 OK**

  ```json
[
  {
    "id": 2,
    "username": "adminauthdemo",
    "email": "admin@authdemo.com",
    "createdBy": {
      "id": 1,
      "username": "system",
      "email": null
    },
    "updatedBy": null,
    "createdAt": null,
    "updatedAt": null
  },
  {
    "id": 3,
    "username": "alicemanager",
    "email": "alice@authdemo.com",
    "createdBy": {
      "id": 2,
      "username": "adminauthdemo",
      "email": "admin@authdemo.com"
    },
    "updatedBy": null,
    "createdAt": "2024-04-04T19:09:18.463805+00:00",
    "updatedAt": null
  },
  {
    "id": 4,
    "username": "bobemployee",
    "email": "bob@authdemo.com",
    "createdBy": {
      "id": 2,
      "username": "adminauthdemo",
      "email": "admin@authdemo.com"
    },
    "updatedBy": null,
    "createdAt": "2024-04-04T19:12:44.189219+00:00",
    "updatedAt": null
  }
]
```

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

- **Step 1: User send a request to create a User** <br>
    Server checks user's HTTP Authorization header for the JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valida go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of the Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Validation of new User's email**<br>
    Server sends a request to DB to check if email is already used. DB sends respond to the Server.<br><br>

- **Step 3: Creation of the User**<br>
    Server sends a request to DB to create a User. DB sends data to the Server.<br><br>
    
- **Step 4: Server sends a response to the User**<br>   

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

- **Step 1: User send a request to get specific User** <br>
    Server checks user's HTTP Authorization header for the JWT bearer token and User's Claims in the Access Token.
    If Access Token and User's Claims are valid, go to 1b midstep and then to step 2.<br><br>

- **Step 1b: Validation of the Access Token (Auth Handler)**<br>
    We do this on each Authorized Endpoint to make sure that the User's Access Token was not invalidated (User was deactivated, changed role, email or password).<br><br>

- **Step 2: Fetching specific User**<br>
    Server sends a request to DB to get specific User. DB sends data to the Server.<br><br>

- **Step 3: Server sends a response to the User**<br>    

</td>
</tr>
<tr>
<td colspan="2">
    
**Request**

</td>
<td colspan="1">
    
**Response 200 OK**

```json
 {
  "id": 4,
  "username": "bobemployee",
  "email": "bob@authdemo.com",
  "createdBy": {
    "id": 2,
    "username": "adminauthdemo",
    "email": "admin@authdemo.com"
  },
  "updatedBy": null,
  "createdAt": "2024-04-04T19:12:44.189219+00:00",
  "updatedAt": null
}
```

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


## Data Objects 


<!--BEGIN Access Token ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **Access Token** 

AuthDemo Access Token is a JWT Token. JWT stands for JSON Web Token, open industry standard [RFC 7519](https://datatracker.ietf.org/doc/html/rfc7519) method for representing claims securely between two parties.

It is like a digital passport that helps Server recognize who you are (Authentication) and what you are allowed to do (Authorization).

JWT Token contains Claims which are pieces of information about the user like: user role, user email, user Id, token expiration, etc.

JWT Token has three components: Header, Payload and Signature.
- Header typically consists of two parts: the type of token (JWT) and the signing algorithm being used, such as HMAC SHA256 or RSA.
- Payload: The payload contains the claims. Claims are statements about an entity (typically, the user) and additional data.
- Signature: The signature is created by combining the encoded header, encoded payload, and a secret (or private key) using the specified algorithm. This signature verifies that the sender of the JWT is who it says it is and ensures that the message wasn't changed along the way.
- 
Encoded JWT Token looks like:

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW5hdXRoZGVtbyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFkbWluQGF1dGhkZW1vLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IjEiLCJqdGkiOiJmMzk1NzBiNy01NTZjLTQ2MzEtYWU4OS1iZGY5Y2EwMGRjNzQiLCJleHAiOjE3MTE3MTQyMTEsImlzcyI6IkF1dGhEZW1vIiwiYXVkIjoiQXV0aERlbW8ifQ.azNf-2S6s8aNNrorRxaqP5YTn9Kc1mZq03Xh2ITJ6IM"
}
```
Decoded JWT Token looks like this (visit [JWT.io](https://jwt.io/) to decode):

```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "adminauthdemo",
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": "admin@authdemo.com",
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "2",
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "1",
    "jti": "f39570b7-556c-4631-ae89-bdf9ca00dc74",
    "exp": 1711714211,
    "iss": "AuthDemo",
    "aud": "AuthDemo"
  },
  "signature": "HMACSHA256(base64UrlEncode(header) + \".\" + base64UrlEncode(payload), your-256-bit-secret)"
}
```
<br>
The Encoded/Decoded JWT Token is shared to the User, but on Server side we use Redis to store it as a key, value pair.
Redis is an in-memory data structure, and it is not persistent if not configured to save data on a disk. In AuthDemo we do not care if our Redis instance crashes/loses data, it basically means that all Users will have to log in again.
<br><br>

Run this command in your Redis terminal:
 ```powershell
    redis-cli KEYS '*'
 ```

You should get a list of KEYs, Key looks like:
 ```powershell
   "authdemo:user:2:accesstoken:aa921d69-4f9d-4212-88f1-1bc3b8249c82"
 ```

Check this for naming Redis keys [How to name Redis Keys](https://riptutorial.com/redis/example/13636/key-naming-schemes).
 <br>

Value is serialized json that is structured like this:
```csharp
public sealed class UserAgentInfo
{
    public required string BrowserName { get; set; }
    public required string Version { get; set; }
    public required string Platform { get; set; }

}

public class AccessToken
{
    public required string UserId { get; set; }
    public required string TokenId { get; set; }
    public  string? RefreshToken { get; set; }
    public required DateTime TokenExpiration { get; set; }
    public required TimeSpan TokenDuration { get; set; }
    public UserAgentInfo? UserAgentInfo { get; set; }
}
```
Property **UserAgenInfo** contains info like browser and OS user is using to log in.

 
 <br>
 <br>
<!--END Access Token ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>



<!--BEGIN Refresh Token ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **Refresh Token** 

```csharp
using System;

[ExcludeFromAuditLog]
public class Token : BaseEntity, IAuditableEntity
{
    public long? UserId { get; set; }
    public User? User { get; set; }
    public string JwtAccessTokenId { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTimeOffset Expires { get; set; }
    public DateTimeOffset? Revoked { get; set; }
    public string? ReplacedByRefreshToken { get; set; }
    public string? ReasonRevoked { get; set; }
    public bool IsExpired => DateTimeOffset.UtcNow >= Expires;
    public bool IsRevoked => Revoked != null;
    public bool IsActive => !IsRevoked && !IsExpired && !string.IsNullOrEmpty(ReplacedByRefreshToken);
    public long? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    public long? UpdatedById { get; set; }
    public User? UpdatedBy { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
```
A Refresh token is long-lived random string token (on the UI side, in DB it is stored as a Token class) used in authentication system to requets a new short-lived access token when they expire.

AuthDemo Refresh Token contains properties that describe the User that RefreshToken is meant for **User**, Id of Access Token **JwtAccessTokenId** that was generated with that RefreshToken.

In AuthDemo, for each new Access Token we also create a new Refresh Token, old Refresh Token's property **ReplacedByRefreshToken** is set to new random string **RefreshToken** therefore we know that that Refresh Token has been used.

Properties **Revoked**, **ReasonRevoked** are used when User's role, email or password has been changed and we want the user to log in again. Property **Expires** defines a time and date when the Refresh Token will expire.

Refresh Token is not sent to the user as a whole Token class, we only send to the user property **RefreshToken** which looks like this:

```json
{
  "refreshToken": "A84SdiKyDfPgy2zzg1vs9uXueGr101s5uPOEa1DdKvfIsei4LI0L8UBndZE0zL2Hc/jzjbwSiw2mbZHFLMSapQ=="
}
```

 <br>
<!--END Refresh Token ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>




<!--BEGIN User ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **User**

```csharp
public class User : IdentityUser<long>, IAuditableEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    [Required]
    public long RoleId { get; set; }
    public Role? Role { get; set; }

    public bool IsActive { get; set; }
    public long? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    public long? UpdatedById { get; set; }
    public User? UpdatedBy { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
```
User class represents the user. It inherits IdentityUser from [ASP.NET Core Identity](#asp.net-core-identity).<br>
More info about ASP.NET Identity User can be found [here](https://dotnettutorials.net/lesson/asp-net-core-identity-tables/).<br>

 <br>
<!--END User ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>



<!--BEGIN Role ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **Role**

```csharp
public class Role : IdentityRole<long>
{
    public virtual ICollection<User>? Users { get; set; }
}
```
Role class represents the role. It inherits IdentityRole from [ASP.NET Core Identity](#asp.net-core-identity).<br>
More info about ASP.NET Identity Role can be found [here](https://dotnettutorials.net/lesson/asp-net-core-identity-tables/).<br>

 <br>
<!--END Role ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>



<!--BEGIN Chore ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **Chore**

```csharp
 public class Chore : BaseEntity, IAuditableEntity
 {
     [MinLength(3)]
     public required string Title { get; set; }
     public string? Description { get; set; }
     public long? UserAssigneeId { get; set; }
     public User? UserAssignee { get; set; }
     public bool IsFinished { get; set; }
     public bool IsApproved { get; set; }
     public long? CreatedById { get; set; }
     public User? CreatedBy { get; set; }
     public long? UpdatedById { get; set; }
     public User? UpdatedBy { get; set; }
     public DateTimeOffset? CreatedAt { get; set; }
     public DateTimeOffset? UpdatedAt { get; set; }
 }
```
Chore class represents the chore.<br>
It contains the fields like: the user assigned to it, isFinished, isApproved statuses, etc.<br> 

 <br>
<!--END Chore ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


<!--BEGIN Base Entity ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **BaseEntity**
```csharp
 public abstract class BaseEntity
 {
     public long Id { get; set; }
 }
```
 
Base entity is used to define the primary key of all entities in DB.
<br>
 
<!--END Base Entity ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


<!--BEGIN IAuditableEntity ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **IAuditableEntity**
```csharp
  public interface IAuditableEntity
  {
      long? CreatedById { get; set; }
      long? UpdatedById { get; set; }
      DateTimeOffset? CreatedAt { get; set; }
      DateTimeOffset? UpdatedAt { get; set; }

  }
```
IAuditableEntity interface is part of the Audit functionality.<br>
On each DELETE, UPDATE or CREATE of an entity in DB, it will update the corresponding properties of a class that implements IAuditableEntity.<br> 
Each change of object that implements IAuditableEntity will generate audit Logs in DB. <br>

 <br>
<!--END IAuditableEntity ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


<!--BEGIN AuditLog ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **AuditLog**
```csharp
 public class AuditLog : BaseEntity
{
    public long? UserId { get; set; }
    public User? User { get; set; }
    public string? EntityType { get; set; }
    public long? EntityId { get; set; }
    public string? Action { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public virtual ICollection<AuditLogDetail> AuditLogDetails { get; set; } = new HashSet<AuditLogDetail>();
}
```
AuditLog class represents audit logs in DB.<br>
Only classes that implement IAuditableEntity will be audited, if we do not want to audit a class that implements IAuditableEntity then add the attribute ExcludeFromAuditLogAttribute.<br>

 <br>
<!--END AuditLog ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


<!--BEGIN AuditLogDetail ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **AuditLogDetail**
```csharp
public class AuditLogDetail: BaseEntity
{
    public required long AuditLogId { get; set; }
    public required AuditLog AuditLog { get; set; }
    public required string Property { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}
```
AuditLogDetail class represents the details of each audit log in DB.<br>

 <br>
<!--END AuditLogDetail ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


<!--BEGIN ExcludeFromAuditLogAttribute ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>
### **ExcludeFromAuditLogAttribute**
```csharp
[AttributeUsage(AttributeTargets.Class)]
internal class ExcludeFromAuditLogAttribute : Attribute
{
}
```
This attribute is added to the classes that implement IAuditableEntity, but for which we do not want to generate audit logs.<br>

 <br>
<!--END ExcludeFromAuditLogAttribute ------------------------------------------------------------------------------------------------------------------------------->
<!------------------------------------------------------------------------------------------------------------------------------------------------------------>


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


<br>
<br>
<br>

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

## Future Enhancements

| Feature               | Description                                                                                               |
|-----------------------|-----------------------------------------------------------------------------------------------------------|
| Pagination | Implementation of pagination support in API endpoints for efficient handling of large datasets. |
| Global Error Handler  | Centralized error handling for improved error logging, standardized error responses, and user feedback.  |
| Logging               | Incorporation of logging mechanisms for monitoring, debugging, and better visibility into application behavior. |
| Database Concurrency  | Implementation of concurrency control mechanisms to prevent data inconsistency issues during concurrent access. |
| Multi-Factor Authentication | Implementation of multifactor authentication for enhanced security by requiring additional verification steps during login. |

<br>
<hr>

## Contributions

Contributions to this repository are welcome!

## Acknowledgments

- Gifs and images created using [Figma](https://www.figma.com) and [Motion UI Figma plugin](https://www.figma.com/community/plugin/889777319208467032/motion-ui-and-games-animation).
- Guided by [Microsoft Security & Identity Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-8.0).
- Inspiration was drawn from tutorials and resources on authorization and authentication in .NET Web API primarily provided by [Barry Dorrans](https://github.com/blowdart/idunno.Authentication) and [Mohamad Lawand](https://github.com/mohamadlawand087), as well as various other contributors in the field.
- Special thanks to [Sandi Zeher](https://github.com/sandizeher) for helping me understand the concept of Refresh Tokens and explaining their practical application in real-life scenarios and projects.

## Contact

For any inquiries or feedback, please contact me at [domagojkk@gmail.com](mailto:domagojkk@gmail.com).




