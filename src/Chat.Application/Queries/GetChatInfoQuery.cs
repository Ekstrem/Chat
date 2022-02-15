using Chat.Contracts.Views;
using FluentValidation;
using MediatR;
using System;

namespace Chat.Application.Queries
{
    public class GetChatInfoQuery : IRequest<ChatInfoView>
    {
        public GetChatInfoQuery(Guid dialogId, long dialogVersion = default)
        {
            DialogId = dialogId;
            DialogVersion = dialogVersion;
        }

        /// <summary>
        /// Идентификатор диалога.
        /// </summary>
        public Guid DialogId { get; }

        /// <summary>
        /// Версия агрегата диалога.
        /// </summary>
        public long DialogVersion { get; }
    }

    public class GetBankAccountShortInfoValidator : AbstractValidator<GetChatInfoQuery>
    {
        public GetBankAccountShortInfoValidator()
        {
            RuleFor(request => request.DialogId).NotEmpty();
        }
    }
}
