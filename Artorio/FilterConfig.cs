using System.Windows.Media;

namespace Artorio
{
    public interface IReadOnlyFilterConfig
    {
        bool UseRangeColor { get; }
        Color FromColor { get; }
        Color ToColor { get; }
        ItemData Item { get; }
    }

    public sealed class FilterConfig : IReadOnlyFilterConfig
    {
        public bool UseRangeColor { get; set; }
        public Color FromColor { get; set; }
        public Color ToColor { get; set; }
        public ItemData Item { get; set; }

        public FilterConfig(bool useRangeColor, Color fromColor, Color toColor, ItemData item)
        {
            UseRangeColor = useRangeColor;
            FromColor = fromColor;
            ToColor = toColor;
            Item = item;
        }
    }
}
