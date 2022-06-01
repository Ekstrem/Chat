using Hive.SeedWorks.TacticalPatterns;
using Hive.SeedWorks.Monads;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chat.Domain.Implementation
{
    internal static class Extension
    {
        private static readonly ConcurrentDictionary<Type, IEnumerable<FieldInfo>> FieldsInfo = new();

        internal static IDictionary<string, IValueObject> GetFields(this object obj)
            => obj
                .PipeTo(p => p.GetType())
                .Do(a =>
                {
                    if (!FieldsInfo.ContainsKey(a))
                    {
                        FieldsInfo.TryAdd(
                            a,
                            a.UnderlyingSystemType
                                .GetRuntimeFields()
                                .Select(m => m)
                                .Where(f => f.FieldType.GetInterface(typeof(IValueObject).Name) != null));
                    }
                })
                .PipeTo(p => FieldsInfo[p])
                .ToDictionary(
                    field => field.Name,
                    field => (IValueObject)field.GetValue(obj));


    }
}
