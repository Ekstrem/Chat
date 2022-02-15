namespace Chat.Contracts.Views
{
    public record ChatInfoView(string UserName, IEnumerable<string> Messages);
}
