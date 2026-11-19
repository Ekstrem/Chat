using Chat.Domain;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.InternalContracts;

/// <summary>
/// РџСЂРѕРµРєС†РёСЏ СЃРѕРѕР±С‰РµРЅРёСЏ (MV <c>messages</c>): РїРѕР»РЅРѕРµ СЃРѕСЃС‚РѕСЏРЅРёРµ Р°РіСЂРµРіР°С‚Р° Message.
/// </summary>
public sealed class MessageReadModel : IReadModel<IChat>
{
    /// <summary>РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ СЃРѕРѕР±С‰РµРЅРёСЏ.</summary>
    public Guid Id { get; set; }

    /// <summary>РџСЂРѕРµРєС‚ вЂ” РєРѕСЂРµРЅСЊ РёРґРµРЅС‚РёС‡РЅРѕСЃС‚Рё.</summary>
    public string Product { get; set; } = string.Empty;

    /// <summary>Р РѕР»СЊ-Р°РІС‚РѕСЂ.</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>РњРµС‚РєР° РІСЂРµРјРµРЅРё, Unix ms (РѕРЅР° Р¶Рµ Version Р°РіСЂРµРіР°С‚Р°).</summary>
    public long Timestamp { get; set; }

    /// <summary>РђРґСЂРµСЃР°С‚С‹ СЃРѕРѕР±С‰РµРЅРёСЏ.</summary>
    public List<string> To { get; set; } = new();

    /// <summary>РўРµРєСЃС‚ СЃРѕРѕР±С‰РµРЅРёСЏ.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Р’Р»РѕР¶РµРЅРёСЏ.</summary>
    public List<Guid> Attachments { get; set; } = new();

    /// <summary>РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ СЃРѕРѕР±С‰РµРЅРёСЏ, РЅР° РєРѕС‚РѕСЂРѕРµ РѕС‚РІРµС‡Р°РµРј.</summary>
    public Guid? ReplyTo { get; set; }

    /// <summary>Р Р°СЃС€РёСЂСЏРµРјС‹Рµ РїРѕР»СЏ РєР°РЅР°Р»Р°.</summary>
    public Dictionary<string, string> ExtendedFields { get; set; } = new();

    /// <summary>РћС‚РјРµС‚РєРё Рѕ РїСЂРѕС‡С‚РµРЅРёРё РїРѕ Р°РґСЂРµСЃР°С‚Р°Рј.</summary>
    public List<ReadReceiptView> ReadReceipts { get; set; } = new();

    /// <summary>Р’РµСЂСЃРёСЏ СЃРѕСЃС‚РѕСЏРЅРёСЏ (Unix ms).</summary>
    public long Version { get; set; }

    /// <summary>РњРѕРјРµРЅС‚ СЃРѕР·РґР°РЅРёСЏ, ISO 8601.</summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>РњРѕРјРµРЅС‚ РїРѕСЃР»РµРґРЅРµРіРѕ РёР·РјРµРЅРµРЅРёСЏ, ISO 8601.</summary>
    public string UpdatedAt { get; set; } = string.Empty;
}

/// <summary>РћС‚РјРµС‚РєР° Рѕ РїСЂРѕС‡С‚РµРЅРёРё РІ СЃРѕСЃС‚Р°РІРµ РїСЂРѕРµРєС†РёРё СЃРѕРѕР±С‰РµРЅРёСЏ.</summary>
public sealed class ReadReceiptView
{
    /// <summary>РђРґСЂРµСЃР°С‚.</summary>
    public string Addressee { get; set; } = string.Empty;

    /// <summary>РњРѕРјРµРЅС‚ РїСЂРѕС‡С‚РµРЅРёСЏ, Unix ms.</summary>
    public long Moment { get; set; }
}
