using Chat.Domain.Abstraction;
using FluentValidation;
using System;

namespace Chat.Application.Commands
{
    public class RequestQuestionCommand :
        CorrelationByRequest<Guid>
    {
        private RequestQuestionCommand(string userName, string message, MessageType type, Platform? platform, Domain.Abstraction.Application application, Guid correlationId)
            : base(correlationId)
        {
            SessionId = Guid.NewGuid();
            Message = message;
            UserName = userName;
            Type = type;
            Platform = platform;
            Application = application;
        }

        public Guid SessionId { get; }
        public string Message { get; }
        public string UserName { get; }
        public MessageType Type { get; }
        public Platform? Platform { get; }
        public Domain.Abstraction.Application Application { get; }

        public static RequestQuestionCommand CreateInstanceByIncommingCall(string userName, string message, Guid correlationId)
            => new(userName, message, MessageType.Voice, null, Domain.Abstraction.Application.IncommingCall, correlationId);

        public static RequestQuestionCommand CreateInstanceBySiteWidget(string userName, string message, Guid correlationId)
            => new(userName, message, MessageType.Text, null, Domain.Abstraction.Application.Site, correlationId);

        public static RequestQuestionCommand CreateInstanceByApplication(string userName, string message, MessageType type, Platform platform, Guid correlationId)
            => new(userName, message, type, platform, Domain.Abstraction.Application.Application, correlationId);
    }

    public class RequestQuestionValidator : AbstractValidator<RequestQuestionCommand>
    {
        public RequestQuestionValidator()
        {
            RuleFor(request => request.Message).NotEmpty();
            RuleFor(request => request.UserName).NotEmpty();
            RuleFor(request => request.SessionId).NotEmpty();
        }
    }
}
