using System;
using Chat.Domain.Abstraction;

namespace Chat.Domain.Implementation
{
    public sealed class DefaultAnemicModel : AnemicModel
    {
        private DefaultAnemicModel(Guid id)
            : base(id, default, default, default, default,
                default, default, default, default)
        { }

        public static IChatAnemicModel Create(Guid id) => new DefaultAnemicModel(id);
    }
}
