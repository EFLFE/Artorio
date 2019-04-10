using System;
using System.Windows;

namespace Artorio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string VERSION = "1.1.0";

        public static event Action<bool> OnExtremeModeChanged;

        // Access to unstable functions
        private static bool extremeMode;

        public static bool ExtremeMode
        {
            get => extremeMode;
            set
            {
                if (extremeMode != value)
                {
                    extremeMode = value;
                    OnExtremeModeChanged?.Invoke(value);
                }
            }
        }

    }
}
