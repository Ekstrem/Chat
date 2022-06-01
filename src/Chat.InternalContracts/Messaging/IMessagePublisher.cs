namespace Chat.InternalContracts.Messaging
{
    public interface IMessagePublisher
    {
        void Publish<TMessage>(TMessage message)
            where TMessage : IMessage;
    }
}