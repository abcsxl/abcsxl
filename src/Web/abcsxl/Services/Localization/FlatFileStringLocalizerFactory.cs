using System.Collections.Concurrent;
using System.Globalization;
using System.Resources;
using System.Xml;
using Microsoft.Extensions.Localization;

namespace abcsxl.Services.Localization;

public class FlatFileStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly string _resourcesPath;

    public FlatFileStringLocalizerFactory(IWebHostEnvironment env)
    {
        _resourcesPath = Path.Combine(env.ContentRootPath, "Resources");
    }

    public IStringLocalizer Create(Type resourceSource) =>
        new FlatFileStringLocalizer("Shared", _resourcesPath);

    public IStringLocalizer Create(string baseName, string location)
    {
        var normalized = NormalizeBaseName(baseName, location);
        return new FlatFileStringLocalizer(normalized, _resourcesPath);
    }

    private static string NormalizeBaseName(string baseName, string location)
    {
        var result = baseName;
        if (!string.IsNullOrEmpty(location) && result.StartsWith(location + ".", StringComparison.Ordinal))
        {
            result = result.Substring(location.Length + 1);
        }
        const string ViewsPrefix = "Views.";
        if (result.StartsWith(ViewsPrefix, StringComparison.Ordinal))
        {
            result = result.Substring(ViewsPrefix.Length);
        }
        return result;
    }
}

public class FlatFileStringLocalizer : IStringLocalizer
{
    private readonly string _baseName;
    private readonly string _resourcesPath;
    private readonly ConcurrentDictionary<string, string?> _cache = new();

    public FlatFileStringLocalizer(string baseName, string resourcesPath)
    {
        _baseName = baseName;
        _resourcesPath = resourcesPath;
    }

    public LocalizedString this[string name] => new(name, GetString(name) ?? name);

    public LocalizedString this[string name, params object[] arguments] =>
        new(name, string.Format(CultureInfo.CurrentUICulture, GetString(name) ?? name, arguments));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var ui = CultureInfo.CurrentUICulture;
        var set = LoadSet(ui.Name);
        if (set == null && includeParentCultures)
        {
            set = LoadSet(ui.TwoLetterISOLanguageName);
        }
        if (set == null && includeParentCultures)
        {
            set = LoadSet(null);
        }
        return set?.Select(kv => new LocalizedString(kv.Key, kv.Value)) ??
               Enumerable.Empty<LocalizedString>();
    }

    public IStringLocalizer WithCulture(CultureInfo culture) => this;

    private string? GetString(string name)
    {
        var ui = CultureInfo.CurrentUICulture;
        var key = $"{ui.Name}::{name}";
        if (_cache.TryGetValue(key, out var cached)) return cached;

        var set = LoadSet(ui.Name)
                ?? LoadSet(ui.TwoLetterISOLanguageName)
                ?? LoadSet(null);

        var value = set != null && set.TryGetValue(name, out var v) ? v : null;
        _cache[key] = value;
        return value;
    }

    private Dictionary<string, string>? LoadSet(string? culture)
    {
        var fileName = string.IsNullOrEmpty(culture)
            ? $"{_baseName}.resx"
            : $"{_baseName}.{culture}.resx";
        var path = Path.Combine(_resourcesPath, fileName);
        if (!File.Exists(path)) return null;

        var dict = new Dictionary<string, string>(StringComparer.Ordinal);
        var doc = new XmlDocument();
        doc.Load(path);
        foreach (XmlNode node in doc.GetElementsByTagName("data"))
        {
            var name = node.Attributes?["name"]?.Value;
            var value = node["value"]?.InnerText;
            if (!string.IsNullOrEmpty(name))
            {
                dict[name] = value ?? string.Empty;
            }
        }
        return dict;
    }
}
