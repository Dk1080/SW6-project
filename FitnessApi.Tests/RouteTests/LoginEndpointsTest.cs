using System.Net;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using FitnessApi.Services;
using FitnessApi.Models;
using MongoDB.Bson;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;


namespace FitnessApi.Tests.RouteTests
{
    public class LoginEndpointsTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public LoginEndpointsTest(WebApplicationFactory<Program> factory)
        {
            //This is needed to create a version of our API
            _factory = factory.WithWebHostBuilder(builder =>
            {
                //Add the mocked services.
                builder.ConfigureServices((services) => {
                //Mock UserService.
                var mockUserService = new Mock<IUserService>();
                mockUserService.Setup(service => service.AddUser(It.IsAny<User>()));
                mockUserService.Setup(service => service.GetUserByName(It.IsAny<string>()))
                    .Returns((string username) =>
                    {
                        if (username == "test")
                        {
                            return new User(ObjectId.Empty, username, "4321");
                        }
                        else
                        {
                            return null;
                        }
                    });


                services.AddSingleton(mockUserService.Object);

                //Mock the PasswordHasher.
                var mockPasswordHasher = new Mock<IPasswordHasher>();
                    //When you want to test the hashing of the password pass 4321 to verify the result.
                mockPasswordHasher.Setup(service => service.hashPassword(It.IsAny<string>())).Returns("4321");
                    mockPasswordHasher.Setup(service => service.verifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                        .Returns((string password, string hashPassword) =>  password == hashPassword );

                    services.AddSingleton(mockPasswordHasher.Object);

                });
            }); 




        }

        [Fact]
        public async Task TestRoute_Returns_HelloThere()
        {
            //Arange
            var client = _factory.CreateClient();

            //Act
            var response = await client.GetAsync("/Test");
            //Asert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("Hello there!!", content);
        }


        [Fact]
        public async Task AddUserRoute_Returns_OKWithValidData()
        {
            //Arange
            var client = _factory.CreateClient();
            
            //Create user.
            var user = new User(ObjectId.Empty, "Test", "Test");

            //Act
            var response = await client.PostAsJsonAsync("/AddUser", user);

            //Assert
                //The return code should be ok and "User added" should be sent back.
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var content = await response.Content.ReadAsStringAsync();
                // Trim extra quotes around the string
                var trimmedContent = content.Trim('"');
            Assert.Equal("User added", trimmedContent);
        }

        [Fact]
        public async Task Login_Returns_OkAndSessionWithValidData()
        {
            //Arange
            var client = _factory.CreateClient();

            //Create user
            var user = new User(ObjectId.Empty, "test", "4321");

            //Act
            var response = await client.PostAsJsonAsync("/login", user);

            //Assert
            //The return code should be ok
            Assert.Equal(HttpStatusCode.OK,response.StatusCode);
            //The session data should be set
            Assert.Contains("Set-Cookie", response.Headers.ToString());
        }

        [Fact]
        public async Task Login_Returns_BadRequestWithInvalidUsername()
        {
            //Arange
            var client = _factory.CreateClient();

            //Create user
            var user = new User(ObjectId.Empty, "invalid", "4321");

            //Act
            var response = await client.PostAsJsonAsync("/login", user);

            //Assert
            //The return code should be Bad Request
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_Returns_UnauthorizedWithInvalidBadPassword()
        {
            //Arange
            var client = _factory.CreateClient();

            //Create user
            var user = new User(ObjectId.Empty, "test", "1234");

            //Act
            var response = await client.PostAsJsonAsync("/login", user);

            //Assert
            //The return code should be Bad Request
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }





    }
}
