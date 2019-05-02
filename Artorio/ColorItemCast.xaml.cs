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
        public event Action<int> OnMoveUpClick;
        public event Action<int> OnMoveDownClick;

        public int ID { get; private set; }

        public IReadOnlyFilterConfig GetFilterConfig
        {
            get
            {
                filterConfig.ItemName = itemName.Text;
                return filterConfig;
            }
        }

        private FilterConfig filterConfig;
        private bool isUnloaded;

        public ColorItemCast(FilterConfig _filterConfig)
        {
            InitializeComponent();
            ID = idIncrement++;

            filterConfig = new FilterConfig(false, Colors.White, Colors.White, null);
            itemName.SelectionChanged += ItemName_SelectionChanged;
            useRangeColor.Checked += UseRangeColor_Checked;
            useRangeColor.Unchecked += UseRangeColor_Unchecked;

            //if (App.ExtremeMode)
            itemName.IsEditable = true;

            //App.OnExtremeModeChanged += App_OnExtremeModeChanged;

            if (_filterConfig != null)
            {
                if (_filterConfig.UseRangeColor)
                    useRangeColor.IsChecked = true;

                for (int i = 0; i < itemName.Items.Count; i++)
                {
                    // I love WPF
                    if (((string)((ComboBoxItem)itemName.Items[i]).Content).Equals(_filterConfig.ItemName))
                    {
                        itemName.SelectedIndex = i;
                        break;
                    }
                }

                if (itemName.SelectedIndex == -1 /*&& App.ExtremeMode*/)
                {
                    itemName.Text = _filterConfig.ItemName;
                }

                filterConfig.FromColor = _filterConfig.FromColor;
                colorFrom.Background = new SolidColorBrush(filterConfig.FromColor);
                colorFrom.ToolTip = filterConfig.FromColor.ToString();

                filterConfig.ToColor = _filterConfig.ToColor;
                colorTo.Background = new SolidColorBrush(filterConfig.ToColor);
                colorTo.ToolTip = filterConfig.ToColor.ToString();
            }
            else
            {
                itemName.SelectedIndex = 0;
            }
        }

        /*
        private void App_OnExtremeModeChanged(bool mode)
        {
            itemName.IsEditable = mode;

            if (!mode)
                itemName.SelectedIndex = 0;
        }
        */

        public void Unload()
        {
            if (isUnloaded)
                return;

            isUnloaded = true;
            //App.OnExtremeModeChanged -= App_OnExtremeModeChanged;
        }

        private void FindItemColor(string itemName)
        {
            itemName = itemName.ToLower();
            /*
             * chest  0, 93, 148
             * belts  140, 138, 140
             * solar? 25, 32, 33
             * turret 25, 32, 33
             * wall   189, 202, 189
             * gate   123, 125, 123

             * floor:
             * stone    49, 49, 49
             * concrete 99, 101, 99
             * hazard   123, 125, 0
             */

            switch (itemName)
            {
                case "stone-path":
                    itemColorRect.Fill = new SolidColorBrush(Color.FromRgb(49, 49, 49));
                    break;

                case "concrete":
                case "refined-concrete":
                    itemColorRect.Fill = new SolidColorBrush(Color.FromRgb(99, 101, 99));
                    break;

                case "hazard-concrete-left":
                case "refined-hazard-concrete-left":
                    itemColorRect.Fill = new SolidColorBrush(Color.FromRgb(123, 125, 0));
                    break;

                case "transport-belt":
                    itemColorRect.Fill = new SolidColorBrush(Color.FromRgb(140, 138, 140));
                    break;

                case "wooden-chest":
                    itemColorRect.Fill = new SolidColorBrush(Color.FromRgb(0, 93, 148));
                    break;

                case "stone-wall":
                    itemColorRect.Fill = new SolidColorBrush(Color.FromRgb(189, 202, 189));
                    break;

                case "gate":
                    itemColorRect.Fill = new SolidColorBrush(Color.FromRgb(123, 125, 123));
                    break;

                default:
                    itemColorRect.Fill = null;
                    break;
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
            if (itemName.SelectedIndex != -1)
            {
                var comboBoxItem = (ComboBoxItem)itemName.SelectedItem;
                string placeItem = (string)comboBoxItem.Content;
                filterConfig.ItemName = placeItem;
                FindItemColor(placeItem);
            }
            else
            {
                itemColorRect.Fill = null;
            }
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

        private void MoveUpClick(object sender, RoutedEventArgs e)
        {
            OnMoveUpClick?.Invoke(ID);
        }

        private void MoveDownClick(object sender, RoutedEventArgs e)
        {
            OnMoveDownClick?.Invoke(ID);
        }
    }
}
