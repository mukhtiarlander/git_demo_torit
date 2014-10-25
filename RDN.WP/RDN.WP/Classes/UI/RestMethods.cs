using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RDN.WP.Classes.UI
{
    public static class RestMethods
    {
        public static void ShowProgress(bool show)
        {
            if (show)
                ((App)Application.Current).m_progressBar.Visibility = Visibility.Visible;
            else
                ((App)Application.Current).m_progressBar.Visibility = Visibility.Collapsed;
        }
    }
}
