using FitnessApi.Models;
using FitnessApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApi.Endpoints
{
    public static class LoginEndpoints
    {


        public static WebApplication MapLoginEndpoints(this WebApplication app)
        {

            

            app.MapGet("/Test", () =>
            {
                return "Hello there!!";
            });


            app.MapPost("/AddUser", (User user , IUserService userService) =>
            {
                try
                {
                    userService.AddUser(user);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Results.InternalServerError();
                }

                return Results.Ok("User added");
                
            });


            app.MapPost("/login", (HttpContext httpContext, User user, IUserService userService) =>
            {
                // Get the user based on their name.
                User gottenUser = userService.GetUserByName(user.Name);

                if (gottenUser == null)
                {
                    return Results.BadRequest();
                }
                else if (gottenUser.Password == user.Password)
                {
                    // Set session data
                    httpContext.Session.SetString("Username", gottenUser.Name);
                    return Results.Ok();
                }
                else
                {
                    return Results.BadRequest();
                }
            });



            app.MapGet("/session", (HttpContext httpContext) => {

                var name = httpContext.Session.Get("Username");
                Console.WriteLine(name);

                if(name == null)
                {
                    return Results.Unauthorized();
                }
                else
                {
                    return Results.Ok($"Welcome {name.ToString}");
                }



            });



            return app;
        }
    }
}
