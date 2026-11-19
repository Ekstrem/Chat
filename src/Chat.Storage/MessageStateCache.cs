using System.Collections.Concurrent;
using Chat.Domain.Abstraction;

namespace Chat.Storage;

/// <summary>
/// Согласованное в процессе состояние агрегата: проекция пишет сюда синхронно,
/// поэтому следующая команда того же процесса видит результат немедленно,
/// не дожидаясь асинхронной записи в Read Store.
/// </summary>
public sealed class MessageStateCache
{
    private readonly ConcurrentDictionary<Guid, IChatAnemicModel> _models = new();

    /// <summary>Положить/заменить состояние.</summary>
    public void Set(Guid id, IChatAnemicModel model) => _models[id] = model;

    /// <summary>Прочитать состояние, если оно уже спроецировано в этом процессе.</summary>
    public bool TryGet(Guid id, out IChatAnemicModel model) => _models.TryGetValue(id, out model!);

    /// <summary>Очистить кеш (rebuild).</summary>
    public void Clear() => _models.Clear();
}
