# MultipleAuthenticationSolution 

This solution is a demo for multiple types of login.
Includes many different project and step by step it will be updated.

## The first release include:
# MVC PROJECT
This project consume the WebAPI, leveraging the authentication with JWT.
After login it create:
1. Authentication cookie
2. JWT Token authorization for webApi
3. Refresh token used when JWT expires

# WEB API PROJECT
This project is used like a first type of Identity Provider and also for retrive the application data from database. It use a EF Core with two data context; one db context for Identity authentication and one db context for application data and authentication without identity. This application is configurable both to use Identity then use classical username/password authentication directly from database. This parameter is configurable in appsettings.

# FUTURE IMPLEMENTATIONS
Is my desire to release another parts of this application in the future, like a OAuth Identity Provider with IdentityServer and one Blazor app for consuming the varius type of Authentication and Authorization.