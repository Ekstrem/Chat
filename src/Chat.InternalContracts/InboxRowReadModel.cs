using Chat.Domain;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.InternalContracts;

/// <summary>
/// РџСЂРѕРµРєС†РёСЏ inbox (MV <c>inbox</c>): РїРѕ СЃС‚СЂРѕРєРµ РЅР° РїР°СЂСѓ (Р°РґСЂРµСЃР°С‚, СЃРѕРѕР±С‰РµРЅРёРµ).
/// РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ РґРµС‚РµСЂРјРёРЅРёСЂРѕРІР°РЅ РїР°СЂРѕР№ СЂРѕР»СЊ+СЃРѕРѕР±С‰РµРЅРёРµ, С‡С‚РѕР±С‹ upsert Р±С‹Р» РёРґРµРјРїРѕС‚РµРЅС‚РЅС‹Рј.
/// </summary>
public sealed class InboxRowReadModel : IReadModel<IChat>
{
    /// <summary>Р”РµС‚РµСЂРјРёРЅРёСЂРѕРІР°РЅРЅС‹Р№ РєР»СЋС‡ СЃС‚СЂРѕРєРё.</summary>
    public Guid Id { get; set; }

    /// <summary>Р РѕР»СЊ-Р°РґСЂРµСЃР°С‚.</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>РРґРµРЅС‚РёС„РёРєР°С‚РѕСЂ СЃРѕРѕР±С‰РµРЅРёСЏ.</summary>
    public Guid MessageId { get; set; }

    /// <summary>РђРІС‚РѕСЂ СЃРѕРѕР±С‰РµРЅРёСЏ.</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>РўРµРєСЃС‚ СЃРѕРѕР±С‰РµРЅРёСЏ.</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>РњРµС‚РєР° РІСЂРµРјРµРЅРё СЃРѕРѕР±С‰РµРЅРёСЏ, Unix ms.</summary>
    public long Timestamp { get; set; }

    /// <summary>РњРѕРјРµРЅС‚ РїСЂРѕС‡С‚РµРЅРёСЏ Р°РґСЂРµСЃР°С‚РѕРј, Unix ms; null вЂ” РЅРµ РїСЂРѕС‡РёС‚Р°РЅРѕ.</summary>
    public long? ReadAt { get; set; }
}
