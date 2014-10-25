using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Scoreboard.Library.ViewModel;
using RDN.Utilities.Util;

namespace RDNScoreboard.Views.LineupTemplates
{
    /// <summary>
    /// Interaction logic for SideBarLogo.xaml
    /// </summary>
    public partial class SideBarLogo : Page
    {
        public SideBarLogo()
        {
            InitializeComponent();
        }
        /// <summary>
        /// sets the side bar panel text
        /// </summary>
        /// <param name="panelText"></param>
        public void setSideBarPanelText(string panelText)
        {
            try
            {
                if (String.IsNullOrEmpty(panelText))
                    return;
                Logger.Instance.logMessage("setting teamname:" + panelText, LoggerEnum.message);
                SidePanelText.Text = panelText;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// sets the logo of the page
        /// </summary>
        /// <param name="logoLocation"></param>
        public void setLogo(string logoLocation)
        {
            try
            {
                if (String.IsNullOrEmpty(logoLocation))
                    return;
                Logger.Instance.logMessage("setting Logo:" + logoLocation, LoggerEnum.message);
                ImageSource imageLocation = new BitmapImage(new Uri(logoLocation));
                LogoPicture.Source = imageLocation;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// sets the background color of the page
        /// </summary>
        /// <param name="hexCode"></param>
        public void setBackgroundColor(string hexCode)
        {
            try
            {
                if (String.IsNullOrEmpty(hexCode))
                    return;
                Logger.Instance.logMessage("changing background Color:" + hexCode, LoggerEnum.message);
                Color color = (Color)ColorConverter.ConvertFromString(hexCode);
                BackgroundColor.Fill = new SolidColorBrush(color);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// sets the sidebar panel text color.
        /// </summary>
        /// <param name="hexCode"></param>
        public void setSidebarTextColor(string hexCode)
        {
            try
            {
                if (String.IsNullOrEmpty(hexCode))
                    return;
                Logger.Instance.logMessage("changing sidebar panel text Color:" + hexCode, LoggerEnum.message);
                Color color = (Color)ColorConverter.ConvertFromString(hexCode);
            SidePanelText.Foreground= new SolidColorBrush(color);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// sets the border color of the page.
        /// </summary>
        /// <param name="hexCode"></param>
        public void setSidebarColor(string hexCode)
        {
            try
            {
                if (String.IsNullOrEmpty(hexCode))
                    return;
                Logger.Instance.logMessage("changing sidebar Color:" + hexCode, LoggerEnum.message);
                Color color = (Color)ColorConverter.ConvertFromString(hexCode);
                SidePanelColor.Fill = new SolidColorBrush(color);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
    }
}
