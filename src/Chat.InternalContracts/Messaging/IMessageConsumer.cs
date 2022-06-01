using System.Threading;
using System.Threading.Tasks;

namespace Chat.InternalContracts.Messaging
{
    public interface IMessageConsumer
    {
        Task Consume<TMessage>(TMessage message, CancellationToken cancellationToken)
            where TMessage : IMessage;
    }
}