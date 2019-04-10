using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Artorio
{
    /// <summary>
    /// Interaction logic for ColorItemCast.xaml
    /// </summary>
    public partial class ColorItemCast : UserControl
    {
        private static int idIncrement;
        private static System.Windows.Forms.ColorDialog colorDialog;

        public event Action<int> OnRemoveThisClick;

        public int ID { get; private set; }

        public IReadOnlyFilterConfig GetFilterConfig => filterConfig;

        private FilterConfig filterConfig;

        public ColorItemCast(FilterConfig _filterConfig)
        {
            InitializeComponent();
            ID = idIncrement++;

            filterConfig = new FilterConfig(false, Colors.White, Colors.White, null);
            itemName.SelectionChanged += ItemName_SelectionChanged;
            itemName.SelectedIndex = 0;
            useRangeColor.Checked += UseRangeColor_Checked;
            useRangeColor.Unchecked += UseRangeColor_Unchecked;

            if (_filterConfig != null)
            {
                if (_filterConfig.UseRangeColor)
                    useRangeColor.IsChecked = true;

                for (int i = 0; i < itemName.Items.Count; i++)
                {
                    if (((string)((ComboBoxItem)itemName.Items[i]).Content).Equals(_filterConfig.ItemName))
                    {
                        itemName.SelectedIndex = i;
                        break;
                    }
                }

                filterConfig.FromColor = _filterConfig.FromColor;
                colorFrom.Background = new SolidColorBrush(filterConfig.FromColor);
                colorFrom.ToolTip = filterConfig.FromColor.ToString();

                filterConfig.ToColor = _filterConfig.ToColor;
                colorTo.Background = new SolidColorBrush(filterConfig.ToColor);
                colorTo.ToolTip = filterConfig.ToColor.ToString();
            }
        }

        private void UseRangeColor_Unchecked(object sender, RoutedEventArgs e)
        {
            filterConfig.UseRangeColor = false;
        }

        private void UseRangeColor_Checked(object sender, RoutedEventArgs e)
        {
            filterConfig.UseRangeColor = true;
        }

        private void ItemName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string placeItem = (string)((ComboBoxItem)itemName.SelectedItem).Content;
            filterConfig.ItemName = placeItem;
        }

        private bool SelectColor(Button owner, out Color? selectedColor)
        {
            selectedColor = null;

            if (colorDialog == null)
            {
                colorDialog = new System.Windows.Forms.ColorDialog
                {
                    AllowFullOpen = true,
                    FullOpen = true
                };
            }

            colorDialog.Color = Extra.ToWinFormColor(((SolidColorBrush)owner.Background).Color);

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var color = Extra.ToWPFColor(colorDialog.Color);
                owner.Background = new SolidColorBrush(color);
                selectedColor = color;
                return true;
            }
            return false;
        }

        private void removeThisClick(object sender, RoutedEventArgs e)
        {
            OnRemoveThisClick.Invoke(ID);
        }

        private void ColorFromClick(object sender, RoutedEventArgs e)
        {
            if (SelectColor(colorFrom, out Color? color))
            {
                var fromColor = color.Value;
                filterConfig.FromColor = fromColor;
                colorFrom.ToolTip = fromColor.ToString();
            }
        }

        private void ColorToClick(object sender, RoutedEventArgs e)
        {
            if (SelectColor(colorTo, out Color? color))
            {
                var toColor = color.Value;
                filterConfig.ToColor = toColor;
                colorTo.ToolTip = toColor.ToString();
            }
        }
    }
}
