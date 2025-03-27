using System.Collections.ObjectModel;

namespace FitnessApi.Models.Api_DTOs
{
    public class ChatHistoriesResponse
    {
        public ObservableCollection<ChatHistoryDTO> histories { get; set; } = new ObservableCollection<ChatHistoryDTO>();


        public ChatHistoriesResponse() { }
    }


}
