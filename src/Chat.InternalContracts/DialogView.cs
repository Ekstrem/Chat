using System;

namespace Chat.InternalContracts
{
    public class DialogView
    {
        public int Id { get; set; }

        public Guid AggregateId { get; set; }

        public string Name { get; set; }
    }
}
