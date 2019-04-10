using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Artorio
{
    /// <summary>
    /// Interaction logic for GetJSONBP.xaml
    /// </summary>
    public partial class GetJSONBP : Window
    {
        public GetJSONBP()
        {
            InitializeComponent();
        }

        private void GetJsonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(output.Text))
                return;

            try
            {
                string result = FactorioBPMaker.Decompress(output.Text);
                output.Text = result;
            }
            catch (Exception ex)
            {
                output.Text = ex.ToString();
            }
        }
    }
}
