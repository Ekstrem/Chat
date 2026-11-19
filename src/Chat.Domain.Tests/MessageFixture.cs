using Chat.Domain;
using Chat.Domain.Abstraction;
using Chat.Domain.Implementation;
using DigiTFactory.Libraries.SeedWorks.Characteristics;
using DigiTFactory.Libraries.SeedWorks.Events;

namespace Chat.Domain.Tests;

/// <summary>РЎС‚СЂРѕРёС‚РµР»СЊ СЃРѕСЃС‚РѕСЏРЅРёСЏ Рё РєРѕРјР°РЅРґ РґР»СЏ С‚РµСЃС‚РѕРІ Р°РіСЂРµРіР°С‚Р° Message.</summary>
internal static class MessageFixture
{
    public const string Product = "Chat";
    public const string Role = "@BE";

    public static IChatAggregate EmptyAggregate(Guid? id = null)
        => Aggregate.CreateInstance(DefaultAnemicModel.Create(id ?? Guid.NewGuid()));

    public static IChatAggregate ExistingMessage(string text = "РїСЂРёРІРµС‚", string to = "@DO")
        => Aggregate.CreateInstance(new AnemicModel
        {
            Id = Guid.NewGuid(),
            Version = 1,
            Root = MessageRoot.CreateInstance(Product, Role, 1),
            Text = MessageText.CreateInstance(text),
            Addressee = MessageAddressee.CreateInstance(to),
            Attachments = MessageAttachments.Empty(),
            ExtendedFields = MessageExtendedFields.Empty(),
            ReadReceipts = Array.Empty<IReadReceipt>()
        });

    public static ICommandToAggregate Command(string name, long version = 0)
        => CommandToAggregate.Commit(Guid.NewGuid(), name, Role, version);
}
