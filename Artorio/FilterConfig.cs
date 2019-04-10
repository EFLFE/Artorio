using System.Windows.Media;

namespace Artorio
{
    public interface IReadOnlyFilterConfig
    {
        bool UseRangeColor { get; }
        Color FromColor { get; }
        Color ToColor { get; }
        string ItemName { get; }
    }

    public sealed class FilterConfig : IReadOnlyFilterConfig
    {
        public bool UseRangeColor { get; set; }
        public Color FromColor { get; set; }
        public Color ToColor { get; set; }
        public string ItemName { get; set; }

        public FilterConfig(bool useRangeColor, Color fromColor, Color toColor, string itemName)
        {
            UseRangeColor = useRangeColor;
            FromColor = fromColor;
            ToColor = toColor;
            ItemName = itemName;
        }
    }
}
