﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DTOs;
using FitnessApi.Models.Api_DTOs;
using FitnessApp.Services.Apis;
using Microsoft.Extensions.AI;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FitnessApp.ViewModels
{

    public class ScrollToBottomMessage{}

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
                    ScrollToBottom();
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

            var query = new ChatDTO(Query, threadId, "user");

            // Gem besked i currentchat så den kan blive displayed hvis man skifter
            var userMessage = new ChatMessageDTO(ChatRole.User, Query);
            CurrentChat.Add(userMessage);

            // Gem den i den tilsvarende chat :3
            var currentChatHistory = chatLog.histories.FirstOrDefault(chat => chat.Id == threadId.ToString());

            if (currentChatHistory != null)
            {
                currentChatHistory.ChatHistory.Add(userMessage);
            }

            // Gør så teksten forsvinder efter send 
            Query = string.Empty;
            ScrollToBottom();

            // Sending the query to the server.
            try
            {
                var response = await _chatApi.SendChat(query);

                // Tilføj chattens besked så den også kan blive displayed
                var botMessage = new ChatMessageDTO(ChatRole.Assistant, response.Query);
                CurrentChat.Add(botMessage);

                // Gem i den tilsvarende chat ╰(*°▽°*)╯
                if (currentChatHistory != null)
                {
                    currentChatHistory.ChatHistory.Add(botMessage);
                }

                //If this is a new chat then add it to the chatlog
                if (threadId == ObjectId.Empty)
                {
                    threadId = ObjectId.Parse(response.ThreadId);
                    chatLog.histories.Add(new ChatHistoryDTO(threadId, new List<ChatMessageDTO> { userMessage, botMessage }));
                }

                OnPropertyChanged(nameof(CurrentChat));
                OnPropertyChanged(nameof(ChatLog));
                ScrollToBottom();
            }
            catch (Exception e)
            {
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
        private async Task SwapChat(String chatId)
        {
            try
            {
                CurrentChat.Clear();
                
                var selectedChat = ChatLog.histories.FirstOrDefault(chat => chat.Id == chatId);

                if (selectedChat != null) 
                { 
                    threadId = ObjectId.Parse(chatId);

                    foreach (ChatMessageDTO message in selectedChat.ChatHistory)
                    {
                        CurrentChat.Add(message);
                    }

                    OnPropertyChanged(nameof(CurrentChat));
                    ScrollToBottom();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error swapping chat: {ex.Message}");
            }

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

        [RelayCommand]
        public void ScrollToBottom()
        {
            // Send a message to trigger scrolling in the view
            WeakReferenceMessenger.Default.Send(new ScrollToBottomMessage());
        }


    }

    

    
}