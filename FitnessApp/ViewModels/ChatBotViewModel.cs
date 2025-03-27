using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnesApp.Services.Apis;
using FitnessApp.Models.Api_DTOs;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FitnesApp.ViewModels
{
    public partial class ChatBotViewModel : ObservableObject
    {


        [ObservableProperty] private string _query = string.Empty;
        [ObservableProperty] private string _response = string.Empty;

        [ObservableProperty] ChatHistoriesResponse chatLog = new();

        private ObjectId threadId = new ObjectId();



        //Create collection holding chat records.
        public ObservableCollection<ChatMessageDTO> CurrentChat { get; set; } = new();

        private IChatApi _chatApi;



        public ChatBotViewModel(IChatApi chatApi)
        {
            _chatApi = chatApi;
            Getchats();
        }



        [RelayCommand]
        public async Task Getchats()
        {

            try
            {
                chatLog = await _chatApi.GetuserChats();
                Console.WriteLine(chatLog);
                OnPropertyChanged(nameof(ChatLog));

                //AutoPopulate chat with the first result being the current one in focus
                var topChatHistory = chatLog.histories.FirstOrDefault();
                if (topChatHistory != null) {
                    foreach (ChatMessageDTO message in topChatHistory.ChatHistory)
                    {
                        CurrentChat.Add(message);
                    }
                    threadId = ObjectId.Parse(topChatHistory.Id);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }



        [RelayCommand]
        public async Task SendQuery()
        {
            // Check that the query is not empty
            if (string.IsNullOrEmpty(Query))
            {
                Console.WriteLine("Query is empty");
                return;
            }


            var query = new ChatDTO(Query,threadId,"user");

            //Add the query to the UI
            CurrentChat.Add(new ChatMessageDTO("user", Query));





            // Sending the query to the server.
            try
            {

               var response =  await _chatApi.SendChat(query);


                //If this is a new chat then add it to the chatlog
                if (threadId == ObjectId.Empty)
                {
                    List<ChatMessageDTO> tmpHis = new();
                    tmpHis.Add(new ChatMessageDTO("user", Query));
                    chatLog.histories.Add(new ChatHistoryDTO(ObjectId.Parse(response.ThreadId), tmpHis));
                    OnPropertyChanged(nameof(ChatLog));
                    OnPropertyChanged(nameof(ChatLog.histories));
                }


                //Set the new threadID
                threadId = ObjectId.Parse(response.ThreadId);

                CurrentChat.Add(new ChatMessageDTO("assistant", response.Query));

                OnPropertyChanged(nameof(ChatLog));


            }
            catch (Exception e) { 

            Console.WriteLine(e.ToString());  
                
            }

        }


        [RelayCommand]
        public async Task NewChat()
        {
            CurrentChat.Clear();
            threadId = ObjectId.Empty;
        }



        [RelayCommand]
        async Task GoToDashboard()
        {
            await Shell.Current.GoToAsync(nameof(DashboardPage));
        }


        private double _menuPosition = 250; // Menu starts off-screen
        private bool _isMenuOpen = false;

        public double MenuPosition
        {
            get => _menuPosition;
            set
            {
                _menuPosition = value;
                OnPropertyChanged(nameof(MenuPosition));
            }
        }

        [RelayCommand]

        private async void ToggleMenu()
        {
            if (_isMenuOpen)
            {
                MenuPosition = 250; // Hide menu (move right)
            }
            else
            {
                MenuPosition = 0; // Show menu (move left)
            }
            _isMenuOpen = !_isMenuOpen;
        }




    }


    
}