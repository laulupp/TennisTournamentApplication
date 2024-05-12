using Backend.Api.Controllers;
using Backend.Attributes;
using Backend.Persistence.Models;
using Backend.Persistence.Repositories.Interfaces;
using Backend.Persistence.Repositories;
using Backend.Services;
using Backend.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System.Net;
using static Backend.Exceptions.CustomExceptions;

namespace Backend.Middleware;

class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository, ITokenService tokenService)
    {
        var endpoint = context.GetEndpoint();

        var username = context.Request.Headers[Constants.Headers.Username].FirstOrDefault();
        var token = context.Request.Headers[Constants.Headers.Token].FirstOrDefault();
        User? user;

        var isUserValid = tokenService.IsValidUser(token, username);

        if (context.Request.Path.ToString().EndsWith("verifytoken"))
        {
            context.Response.StatusCode = isUserValid ? (int)HttpStatusCode.OK : (int)HttpStatusCode.Unauthorized;
            await context.Response.CompleteAsync();
            return;
        }

        if (endpoint != null)
        {
            var controllerActionDescriptor = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();

            //Skip if the endpoint is Auth
            if (controllerActionDescriptor != null && controllerActionDescriptor.ControllerTypeInfo.AsType() != typeof(AuthController))
            {


                if (username == null || token == null || !isUserValid || (user = await userRepository.GetByUsernameAsync(username)) == null)
                {
                    throw new InvalidAuthentication();
                }

                context.Items[Constants.Items.IsAdmin] = user.Role == Role.Admin;
                context.Items[Constants.Items.IsReferee] = user.Role == Role.Referee;

                var permissionAttribute = endpoint.Metadata.GetMetadata<PermissionAttribute>();
                if (permissionAttribute != null)
                {
                    if (user == null || !HasPermission(Enum.GetName(typeof(Role), user.Role)!, permissionAttribute.Permissions))
                    {
                        throw new UserUnauthorizedException();
                    }
                }
            }
        }

        await _next(context);
    }

    private bool HasPermission(string userRole, string[] permissions)
    {
        foreach (var permission in permissions)
        {
            if (permission.Equals(userRole))
            {
                return true;
            }
        }

        return false;
    }
}
