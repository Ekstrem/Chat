using Chat.Domain.Abstraction;
using Hive.SeedWorks.Invariants;
using Hive.SeedWorks.Result;
using Hive.SeedWorks.TacticalPatterns;
using System.Collections.Generic;
using System.Linq;

namespace Chat.Domain.Implementation
{
    internal class ChatResult : AggregateResult<IChat, IChatAnemicModel>
    {
        private readonly DomainOperationResultEnum _result;
        private readonly IEnumerable<string> _reason;

        internal ChatResult(
            IChatAnemicModel oldVersion,
            IChatAnemicModel newVersion,
            DomainOperationResultEnum result,
            IEnumerable<string> reason)
            : base(MakeBusinessOperationData(oldVersion, newVersion))
        {
            _result = result;
            _reason = reason;
        }

        internal ChatResult(AggregateResult<IChat, IChatAnemicModel> result)
            : base(result.BusinessOperationData)
        {
            _result = result.Result;
            _reason = result.Reason;
        }

        private static BusinessOperationData<IChat, IChatAnemicModel> MakeBusinessOperationData(
            IChatAnemicModel oldVersion, IChatAnemicModel newVersion)
            => BusinessOperationData<IChat, IChatAnemicModel>
                .Commit<IChat, IChatAnemicModel>(oldVersion, newVersion);

        public override DomainOperationResultEnum Result => _result;

        public override IEnumerable<string> Reason => _reason;

        public override IDictionary<string, IValueObject> ChangeValueObjects 
        {
            get 
            {
                var diff = new Dictionary<string, IValueObject>();

                if (!Equals(BusinessOperationData.Aggregate.Root, BusinessOperationData.Model.Root))
                {
                    diff.Add(nameof(IChatAnemicModel.Root), BusinessOperationData.Model.Root);
                }

                if (!Equals(BusinessOperationData.Aggregate.Actor, BusinessOperationData.Model.Actor))
                {
                    diff.Add(nameof(IChatAnemicModel.Actor), BusinessOperationData.Model.Actor);
                }

                if (!Equals(BusinessOperationData.Aggregate.Feedback, BusinessOperationData.Model.Feedback))
                {
                    diff.Add(nameof(IChatAnemicModel.Feedback), BusinessOperationData.Model.Feedback);
                }

                if (ValueObjectCollectionNotEqual(BusinessOperationData.Aggregate.Messages, BusinessOperationData.Model.Messages))
                {
                    diff.Add(nameof(IChatAnemicModel.Messages), BusinessOperationData.Model.Messages.ToValueObject());
                }

                return diff;
            }
        }

        private bool ValueObjectCollectionNotEqual(
            IReadOnlyCollection<IValueObject> collection1,
            IReadOnlyCollection<IValueObject> collection2)
        {
            return (collection1 == null && collection2 != null) ||
                (collection1 != null && collection2 == null) ||
                (collection1.Count != collection2.Count) ||
                (collection1.Any(c1 => !collection2.Any(c2 => Equals(c1, c2))));
        }
    }
}
