using Chat.Domain;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.InternalContracts;

/// <summary>
/// РџСЂРѕРµРєС†РёСЏ РєСѓСЂСЃРѕСЂР° С‡С‚РµРЅРёСЏ (MV <c>last_read</c>): РїРѕСЃР»РµРґРЅСЏСЏ РїСЂРѕС‡РёС‚Р°РЅРЅР°СЏ РјРµС‚РєР° РїРѕ СЂРѕР»Рё.
/// </summary>
public sealed class LastReadReadModel : IReadModel<IChat>
{
    /// <summary>Р”РµС‚РµСЂРјРёРЅРёСЂРѕРІР°РЅРЅС‹Р№ РєР»СЋС‡ СЃС‚СЂРѕРєРё (РїРѕ СЂРѕР»Рё).</summary>
    public Guid Id { get; set; }

    /// <summary>Р РѕР»СЊ, РґР»СЏ РєРѕС‚РѕСЂРѕР№ С…СЂР°РЅРёС‚СЃСЏ РєСѓСЂСЃРѕСЂ.</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>РџРѕСЃР»РµРґРЅСЏСЏ РїСЂРѕС‡РёС‚Р°РЅРЅР°СЏ РјРµС‚РєР°, Unix ms; null вЂ” СЂРѕР»СЊ РµС‰С‘ РЅРёС‡РµРіРѕ РЅРµ С‡РёС‚Р°Р»Р°.</summary>
    public long? LastReadAt { get; set; }
}
