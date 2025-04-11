using System.Collections.ObjectModel;

namespace DTOs
{
    public class ChatHistoriesResponse
    {
        public ObservableCollection<ChatHistoryDTO> histories { get; set; } = new ObservableCollection<ChatHistoryDTO>();


        public ChatHistoriesResponse() { }
    }


}
