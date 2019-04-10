using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Artorio
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            version.Text = "Version " + App.VERSION;
            InsertLink("https://github.com/EFLFE/Artorio", githubLink);
            InsertLink("https://github.com/leonbloy/pngcs", pngcs);
            InsertLink("http://www.componentace.com/zlib_.NET.htm", zlib);
        }

        private void InsertLink(string link, TextBlock target)
        {
            var hype = new Hyperlink();
            hype.Inlines.Add(link);
            hype.Click += (_, __) => Process.Start(link);
            target.Inlines.Add(hype);
        }

        private void EnableExtremeMode(object sender, RoutedEventArgs e)
        {
            extremeModeBtn.IsEnabled = false;
            App.ExtremeMode = true;
        }
    }
}
