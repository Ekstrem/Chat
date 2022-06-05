namespace Chat.Contracts.Views
{
    public class ChatInfoView
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } 
        public IEnumerable<string> Messages { get; set; }
    }
}
