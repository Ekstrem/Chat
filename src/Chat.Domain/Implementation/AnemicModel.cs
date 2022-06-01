using System;
using System.Collections.Generic;
using Chat.Domain.Abstraction;
using Hive.SeedWorks.Characteristics;
using Hive.SeedWorks.LifeCircle;
using Hive.SeedWorks.TacticalPatterns;

namespace Chat.Domain.Implementation
{
    public class AnemicModel : IChatAnemicModel
    {
        private readonly Guid _id;
        private readonly long _version;
        private readonly string _commandName;
        private readonly string _subjectName;
        private readonly Guid _correlationToken;
        private readonly IChatRoot _root;
        private readonly IChatActor _actor;
        private readonly IChatFeedback _feedback;
        private readonly IReadOnlyCollection<IChatMessage> _messages;

        protected AnemicModel(
            Guid id,
            long version,
            Guid correlationToken,
            string commandName,
            string subjectName,
            IChatRoot root,
            IChatActor actor,
            IChatFeedback feedback,
            IReadOnlyCollection<IChatMessage> messages)
        {
            _id = id;
            _version = version;
            _correlationToken = correlationToken;
            _commandName = commandName;
            _subjectName = subjectName;
            _root = root;
            _actor = actor;
            _feedback = feedback;
            _messages = messages;
        }

        /// <summary>
        /// Идентификатор агрегата.
        /// </summary>
        public Guid Id => _id;

        /// <summary>
        /// Версия агрегата.
        /// </summary>
        public long Version => _version;

        /// <summary>
        /// Маркер корреляции.
        /// </summary>
        public Guid CorrelationToken => _correlationToken;

        public string CommandName => _commandName;

        public string SubjectName => _subjectName;

        public IDictionary<string, IValueObject> GetValueObjects()
            => ValueObjectHelper.GetValueObjects(this);

        /// <summary>
        /// Корень агрегата.
        /// </summary>
        public IChatRoot Root => _root;

        /// <summary>
        /// Данные пользователя.
        /// </summary>
        public IChatActor Actor => _actor;

        /// <summary>
        /// Полученный ответ.
        /// </summary>
        public IChatFeedback Feedback => _feedback;

        /// <summary>
        /// Сообщения.
        /// </summary>
        public IReadOnlyCollection<IChatMessage> Messages => _messages;

        public static IChatAnemicModel Create(
            Guid id,
            long version,
            Guid correlationToken,
            string commandName,
            string subjectName,
            IChatRoot root,
            IChatActor actor,
            IChatFeedback feedback,
            IReadOnlyCollection<IChatMessage> messages)
            => new AnemicModel(id, version,
                correlationToken, commandName, subjectName,
                root, actor, feedback, messages);

        public static IChatAnemicModel Create(
            Guid id,
            ICommandToAggregate command,
            IChatRoot root,
            IChatActor actor,
            IChatFeedback feedback,
            IReadOnlyCollection<IChatMessage> messages)
            => new AnemicModel(id,
                command.Version, command.CorrelationToken, command.CommandName, command.SubjectName,
                root, actor, feedback, messages);

    }
}
