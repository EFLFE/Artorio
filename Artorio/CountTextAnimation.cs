using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Artorio
{
    internal class CountTextAnimation
    {
        private TextBlock textBlock;
        private DispatcherTimer timer;
        private float targetValue;
        private float currentValue;
        private float timeLerp;
        private string displayText;

        private const float timeSpeed = 0.03f;

        public CountTextAnimation(TextBlock textBlock, Dispatcher dispatcher, string displayText)
        {
            timeLerp = 1f;
            this.displayText = displayText;
            this.textBlock = textBlock;
            timer = new DispatcherTimer(TimeSpan.FromSeconds(0.01), DispatcherPriority.Background, Tick, dispatcher);
        }

        public void SetTarget(int value, string displayText = null)
        {
            if (displayText != null)
                this.displayText = displayText;

            targetValue = value;
            timeLerp = 0f;
            timer.Start();
        }

        private void Tick(object s, EventArgs e)
        {
            if (timeLerp >= 1f)
            {
                timer.Stop();
                return;
            }

            timeLerp += timeSpeed;

            if (timeLerp > 1f)
                timeLerp = 1f;

            currentValue += timeLerp * (targetValue - currentValue);
            textBlock.Text = displayText.Replace("#", Math.Round(currentValue).ToString());
        }

    }
}
