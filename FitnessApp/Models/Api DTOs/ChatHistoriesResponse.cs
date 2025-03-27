using System.Collections.ObjectModel;

namespace FitnessApp.Models.Api_DTOs
{
    public class ChatHistoriesResponse
    {

        public ObservableCollection<ChatHistoryDTO> histories { get; set; } = new ObservableCollection<ChatHistoryDTO>();

        public ChatHistoriesResponse() { }


    }
}
