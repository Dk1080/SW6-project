using DTOs;
using FitnessApp.Services.Apis;
using FitnessApp.ViewModels;
using MongoDB.Bson;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Tests.ViewModelTests
{
    public class ChatBotViewModelTest
    {

        [Fact]
        public async Task SendQuery_AddsNewMessasgeToLog()
        {
            //Arrange
            var mockUserService = new Mock<IChatApi>();
            mockUserService.Setup(service => service.SendChat(It.IsAny<ChatDTO>()))
                .Returns(() =>
                {
                    return Task.FromResult(new ChatDTO("Return Value", ObjectId.Empty, "assistant"));
                });
            mockUserService.Setup(service => service.GetuserChats()).Returns(() => { return Task.FromResult(new ChatHistoriesResponse()); });

            var vm = new ChatBotViewModel(mockUserService.Object);
            vm.Query = "Send value";

            //Act
            await vm.SendQuery();

            //Assert
            //Check that the two messesages where added to the chat
            Assert.Contains("Send value", vm.CurrentChat.First().Text);
            Assert.Contains("Return Value", vm.CurrentChat.Last().Text);

        }



    }
}
