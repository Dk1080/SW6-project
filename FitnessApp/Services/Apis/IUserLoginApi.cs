using DTOs;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Services.APIs
{
    public interface IUserLoginApi
    {
        [Post("/login")]
        Task<HttpResponseMessage> Execute(UserRequest userRequest);


    }
}
