using FitnessApp.Models.Api_DTOs;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnesApp.Services.Apis
{
    public interface IChatApi
    {


        [Get("/getChats")]
        Task<ChatHistoriesResponse> GetuserChats();


        [Post("/chat")]
        Task<ChatDTO> SendChat(ChatDTO chatMessage);



    }
}
