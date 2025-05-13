using DTOs;
using FitnessApi.Models;
using FitnessApi.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace FitnessApi.Endpoints
{
    public static class LoginEndpoints
    {


        public static WebApplication MapLoginEndpoints(this WebApplication app)
        {

            //Check if the person is logged in and if so then send them to the dashboard.


            app.MapGet("/Test", () =>
            {
                return "Hello there!!";
            });


            app.MapPost("/AddUser", (User user , IUserService userService, IPasswordHasher passwordHasher) =>
            {

                //Hash the password of the user.
                user.Password = passwordHasher.hashPassword(user.Password);


                //Check that the username is not already taken.
                var gottenUser = userService.GetUserByName(user.Username);

                if (gottenUser == null)
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
                }
                else
                {
                    return Results.BadRequest("Username is taken");
                }



            });


            app.MapPost("/login", (HttpContext httpContext, UserRequest user, IUserService userService, IPasswordHasher passwordHasher) =>
            {
                // Get the user based on their name.
                User gottenUser = userService.GetUserByName(user.Username);

                if (gottenUser == null)
                {
                    return Results.BadRequest();
                }
                //Check the password against the hashed password stored in the database.
                else if (passwordHasher.verifyPassword(user.Password, gottenUser.Password))
                {
                    // Set session data
                    httpContext.Session.SetString("Username", gottenUser.Username);
                    return Results.Ok();
                }
                else
                {
                    return Results.Unauthorized();
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
