using System.Globalization;
using Avalonia.Data.Converters;
using GlashClientDesktop.Core.ProxyTypes;

namespace GlashClientDesktop.Converters;

/// <summary>
/// 把 ProxyRuleInfo.ProxyType（完整类型名 id）转换成用户可读的代理类型短名（RDP/SSH/Web/...）。
/// </summary>
public class ProxyTypeNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string id)
        {
            var info = ProxyTypeManager.Instance.GetProxyTypeInfo(id);
            if (info != null)
                return info.Name;
        }
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// 端点展示：端口为 0 表示自动分配且尚未分配（未启用/未绑定），渲染为 "host:自动分配"；
/// 已分配实际端口（启用并绑定成功）时原样返回 "host:port"。
/// </summary>
public class EndPointDisplayConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string ep && !string.IsNullOrEmpty(ep))
        {
            var idx = ep.LastIndexOf(':');
            if (idx > 0 && idx < ep.Length - 1 && ep.Substring(idx + 1) == "0")
                return ep.Substring(0, idx) + ":自动分配";
        }
        return value ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
