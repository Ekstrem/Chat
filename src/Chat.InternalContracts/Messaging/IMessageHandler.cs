using MediatR;

namespace Chat.InternalContracts.Messaging
{
    public interface IMessageHandler<in TMessage> : INotificationHandler<TMessage>
        where TMessage : IMessage
    {
    }
}