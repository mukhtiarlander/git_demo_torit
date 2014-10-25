﻿using System;
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
    /// Interaction logic for PlainProfile.xaml
    /// </summary>
    public partial class PlainProfile : Page
    {
        public PlainProfile()
        {
            InitializeComponent();
        }
       
        /// <summary>
        /// sets the skater name
        /// </summary>
        /// <param name="skaterName"></param>
        public void setSkaterName(string skaterName)
        {
            try
            {
                if (String.IsNullOrEmpty(skaterName))
                    return;
                Logger.Instance.logMessage("setting saterName text:" + skaterName, LoggerEnum.message);
                SkaterName.Text = skaterName;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        public void setSkaterNumber(string skaterNumber)
        {
            try
            {
                if (String.IsNullOrEmpty(skaterNumber))
                    return;
                Logger.Instance.logMessage("setting sater number text:" + skaterNumber, LoggerEnum.message);
                SkaterNumber.Text = skaterNumber;
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
        /// sets the skater picture
        /// </summary>
        /// <param name="pictureLocation"></param>
        public void setSkaterPicture(string pictureLocation)
        {
            try
            {
                if (String.IsNullOrEmpty(pictureLocation))
                    return;
                Logger.Instance.logMessage("setting picture:" + pictureLocation, LoggerEnum.message);
                ImageSource imageLocation = new BitmapImage(new Uri(pictureLocation));
               SkaterPicture.Source = imageLocation;
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
        /// sets the border color of the page.
        /// </summary>
        /// <param name="hexCode"></param>
        public void setBorderColor(string hexCode)
        {
            try
            {
                if (String.IsNullOrEmpty(hexCode))
                    return;
                Logger.Instance.logMessage("changing border Color:" + hexCode, LoggerEnum.message);
                Color color = (Color)ColorConverter.ConvertFromString(hexCode);
                borderBrush.BorderBrush = new SolidColorBrush(color);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// sets the color of the skater text
        /// </summary>
        /// <param name="hexCode"></param>
        public void setSkaterTextColor(string hexCode)
        {
            try
            {
                if (String.IsNullOrEmpty(hexCode))
                    return;
                Logger.Instance.logMessage("changing text Color:" + hexCode, LoggerEnum.message);
                Color color = (Color)ColorConverter.ConvertFromString(hexCode);
              SkaterNumber.Foreground= new SolidColorBrush(color);
              SkaterName.Foreground = new SolidColorBrush(color);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

    }
}