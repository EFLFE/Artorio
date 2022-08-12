using System.Windows;
using System.Windows.Media;

namespace Artorio
{
    /// <summary>
    /// Interaction logic for ColorsPickerWIndow.xaml
    /// </summary>
    public partial class ColorsPickerWIndow : Window
    {
        public static ColorsPickerWIndow Instance { get; private set; }

        public static void OpenWindow(Color? selectColor = null)
        {
            if (Instance == null)
                Instance = new ColorsPickerWIndow();

            if (selectColor.HasValue)
                Instance.SelectColor(selectColor.Value);
            
            Instance.ShowDialog();
        }

        private bool stayOpen;
        public Color GetSelectedColor => main.SelectedColor;
        public bool ColorWasSelected { get; private set; }

        public ColorsPickerWIndow()
        {
            InitializeComponent();
            stayOpen = true;
            Closing += ColorsPickerWIndow_Closing;
        }

        public void CloseWindow()
        {
            stayOpen = false;
            Close();
        }

        public void SelectColor(Color clr)
        {
            main.SelectedColor = clr;
        }

        private void ColorsPickerWIndow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (stayOpen)
            {
                e.Cancel = false;
            }
        }

        private void OnApplyClick(object sender, RoutedEventArgs e)
        {
            ColorWasSelected = true;
            Hide();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            ColorWasSelected = false;
            Hide();
        }
    }
}
