using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace RDNScoreboard.Code
{
    internal static class FadeExtension
    {
        internal enum TransitionSpeed
        {
            Instant = 0,
            Fast = 100,
            Normal = 200,
            Slow = 500,
            VerySlow = 1000,
            SuperSlow = 2000
        }

        /// <summary>
        /// Toggles the opacity of a control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="speed">The speed.</param>
        internal static void ToggleControlFade(Control control, TransitionSpeed speed)
        {
            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, (int)speed); 
            
            DoubleAnimation animation = new DoubleAnimation { From = 1.0, To = 0.0, Duration = new Duration(duration) };
            if (control.Opacity == 0.0)
            {
                animation = new DoubleAnimation { From = 0.0, To = 1.0, Duration = new Duration(duration) };
            }

            Storyboard.SetTargetName(animation, control.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity", 0));
            storyboard.Children.Add(animation);

            storyboard.Begin(control);
        }

        /// <summary>
        /// Toggles the opacity of a control.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="speed">The speed in milliseconds.</param>
        internal static void ToggleControlFade(Control control, int speed)
        {
            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, speed); 

            DoubleAnimation animation = new DoubleAnimation { From = 1.0, To = 0.0, Duration = new Duration(duration) };
            if (control.Opacity == 0.0)
            {
                animation = new DoubleAnimation { From = 0.0, To = 1.0, Duration = new Duration(duration) };
            }

            Storyboard.SetTargetName(animation, control.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity", 0));
            storyboard.Children.Add(animation);

            storyboard.Begin(control);
        }
    }
}
