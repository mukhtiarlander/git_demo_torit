using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
using RDN.Utilities.Config;
using RDN.Utilities.Error;
using RDNScoreboard.Server;
using Scoreboard.Library.Network;

using Scoreboard.Library.ViewModel;
using Scoreboard.Library.ViewModel.Members.Enums;
using Scoreboard.Library.ViewModel.Members.Officials;
using RDN.Utilities.Util;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace RDNScoreboard.Views.ServerTabs
{
    /// <summary>
    /// Interaction logic for OfficialsManagerTab.xaml
    /// </summary>
    public partial class OverlayTab : Page
    {
        public OverlayTab()
        {
            InitializeComponent();
            try
            {
                this.Title = ScoreboardConfig.SCOREBOARD_NAME;

                Run run = new Run(WebServer.Instance.VideoOverlayAddress4x3);
                this.videoOverlayLink.Inlines.Clear();
                this.videoOverlayLink.Inlines.Add(run);
                Run run16x9 = new Run(WebServer.Instance.VideoOverlayAddress16x9);
                this.videoOverlayLink16x9.Inlines.Clear();
                this.videoOverlayLink16x9.Inlines.Add(run16x9);

                Run run2 = new Run(WebServer.Instance.VideoOverlay2Address4x3);
                this.videoOverlay2Link.Inlines.Clear();
                this.videoOverlay2Link.Inlines.Add(run2);

                Run run216x9 = new Run(WebServer.Instance.VideoOverlay2Address16x9);
                this.videoOverlay2Link16x9.Inlines.Clear();
                this.videoOverlay2Link16x9.Inlines.Add(run216x9);

                Run runMainScreen = new Run(WebServer.Instance.IndexForShowingPages);
                this.mainScreenLink.Inlines.Clear();
                this.mainScreenLink.Inlines.Add(runMainScreen);

                LogoOnOff.IsChecked = PolicyViewModel.Instance.VideoOverlay.LogoOnOff;
                OverlayTransparent.IsChecked = PolicyViewModel.Instance.VideoOverlay.IsOverlayTransparent;
                OverlayTopBottom.IsChecked = PolicyViewModel.Instance.VideoOverlay.ScoresOnTop;
                if (PolicyViewModel.Instance.VideoOverlay.ScoresOnTop)
                    VerticalPositionText.Text = "Vertical Position Pixels from Top: ";
                else
                    VerticalPositionText.Text = "Vertical Position Pixels from Bottom: ";
                TurnThirdRowOn.IsChecked = PolicyViewModel.Instance.VideoOverlay.IsBottomRowOn;
                showJamScore.IsChecked = PolicyViewModel.Instance.VideoOverlay.ShowJamScore;
                showTimeOuts.IsChecked = PolicyViewModel.Instance.VideoOverlay.TurnOffTimeOuts;
                VerticalPosition.Text = PolicyViewModel.Instance.VideoOverlay.VerticalPosition.ToString();
                TextSizePosition.Text = PolicyViewModel.Instance.VideoOverlay.TextSizePosition.ToString("N1");

                if (PolicyViewModel.Instance.VideoOverlay != null && !String.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.LogoPictureLocation))
                {
                    FileInfo file = new FileInfo(PolicyViewModel.Instance.VideoOverlay.LogoPictureLocation);
                    LogoNameChange(file.Name);
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void videoOverlayLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string parameters = ServerHelper.addUrlParameters();
                //System.Diagnostics.Process.Start(WebServer.Instance.VideoOverlayAddress4x3 + parameters);
                WebBrowser b = new WebBrowser();
                b.Navigate(WebServer.Instance.VideoOverlayAddress4x3 + parameters, "_blank", null, null);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        private void videoOverlay2Link_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string parameters = ServerHelper.addUrlParameters();
                //System.Diagnostics.Process.Start(WebServer.Instance.VideoOverlay2Address4x3 + parameters);
                WebBrowser b = new WebBrowser();
                b.Navigate(WebServer.Instance.VideoOverlay2Address4x3 + parameters, "_blank", null, null);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void mainScreenLink_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(WebServer.Instance.IndexForShowingPages);
            WebBrowser b = new WebBrowser();
            b.Navigate(WebServer.Instance.IndexForShowingPages, "_blank", null, null);
        }


        private void videoOverlayLink16x9_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string parameters = ServerHelper.addUrlParameters();
                //System.Diagnostics.Process.Start(WebServer.Instance.VideoOverlayAddress16x9 + parameters);
                WebBrowser b = new WebBrowser();
                b.Navigate(WebServer.Instance.VideoOverlayAddress16x9 + parameters, "_blank", null, null);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }
        private void videoOverlay2Link16x9_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string parameters = ServerHelper.addUrlParameters();
                //System.Diagnostics.Process.Start(WebServer.Instance.VideoOverlay2Address16x9 + parameters);
                WebBrowser b = new WebBrowser();
                b.Navigate(WebServer.Instance.VideoOverlay2Address16x9 + parameters, "_blank", null, null);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }

        private void Team1TopColor_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorPickerDialog cPicker = new ColorPickerDialog();

                Color color;
                if (string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.TopTeam1Color))
                    color = (Color)ColorConverter.ConvertFromString("#000000");
                else
                    color = (Color)ColorConverter.ConvertFromString("#" + PolicyViewModel.Instance.VideoOverlay.TopTeam1Color);

                cPicker.StartingColor = color;
                Window window = Window.GetWindow(this);
                cPicker.Owner = window;
                bool? dialogResult = cPicker.ShowDialog();
                if (dialogResult != null && (bool)dialogResult == true)
                {

                    PolicyViewModel.Instance.VideoOverlay.TopTeam1Color = cPicker.SelectedColor.ToString();
                    Team1TopColor.Foreground = new SolidColorBrush(cPicker.SelectedColor);
                    Team1TopColor.Background = new SolidColorBrush(cPicker.SelectedColor);

                    //gets the last 6 chars in string since it throws back opacity and Hash mark.
                    PolicyViewModel.Instance.VideoOverlay.TopTeam1Color = PolicyViewModel.Instance.VideoOverlay.TopTeam1Color.Substring(PolicyViewModel.Instance.VideoOverlay.TopTeam1Color.Length - 6);

                    PolicyViewModel.Instance.savePolicyToXml();

                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }

        private void Team1BottomColor_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorPickerDialog cPicker = new ColorPickerDialog();

                Color color;
                if (string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.BottomTeam1Color))
                    color = (Color)ColorConverter.ConvertFromString("#000000");
                else
                    color = (Color)ColorConverter.ConvertFromString("#" + PolicyViewModel.Instance.VideoOverlay.BottomTeam1Color);

                cPicker.StartingColor = color;
                Window window = Window.GetWindow(this);
                cPicker.Owner = window;
                bool? dialogResult = cPicker.ShowDialog();
                if (dialogResult != null && (bool)dialogResult == true)
                {
                    PolicyViewModel.Instance.VideoOverlay.BottomTeam1Color = cPicker.SelectedColor.ToString();
                    Team1BottomColor.Foreground = new SolidColorBrush(cPicker.SelectedColor);
                    Team1BottomColor.Background = new SolidColorBrush(cPicker.SelectedColor);

                    //gets the last 6 chars in string since it throws back opacity and Hash mark.
                    PolicyViewModel.Instance.VideoOverlay.BottomTeam1Color = PolicyViewModel.Instance.VideoOverlay.BottomTeam1Color.Substring(PolicyViewModel.Instance.VideoOverlay.BottomTeam1Color.Length - 6);
                    Logger.Instance.logMessage("setting bottom team 1 color", LoggerEnum.message);
                    PolicyViewModel.Instance.savePolicyToXml();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }

        private void Team1FontColor_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorPickerDialog cPicker = new ColorPickerDialog();

                Color color;
                if (string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.FontTeam1Color))
                    color = (Color)ColorConverter.ConvertFromString("#000000");
                else
                    color = (Color)ColorConverter.ConvertFromString("#" + PolicyViewModel.Instance.VideoOverlay.FontTeam1Color);

                cPicker.StartingColor = color;
                Window window = Window.GetWindow(this);
                cPicker.Owner = window;
                bool? dialogResult = cPicker.ShowDialog();
                if (dialogResult != null && (bool)dialogResult == true)
                {
                    PolicyViewModel.Instance.VideoOverlay.FontTeam1Color = cPicker.SelectedColor.ToString();
                    Team1FontColor.Foreground = new SolidColorBrush(cPicker.SelectedColor);
                    Team1FontColor.Background = new SolidColorBrush(cPicker.SelectedColor);

                    //gets the last 6 chars in string since it throws back opacity and Hash mark.
                    PolicyViewModel.Instance.VideoOverlay.FontTeam1Color = PolicyViewModel.Instance.VideoOverlay.FontTeam1Color.Substring(PolicyViewModel.Instance.VideoOverlay.FontTeam1Color.Length - 6);
                    Logger.Instance.logMessage("setting font team 1 color", LoggerEnum.message);
                    PolicyViewModel.Instance.savePolicyToXml();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }

        private void Team2TopColor_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorPickerDialog cPicker = new ColorPickerDialog();

                Color color;
                if (string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.TopTeam2Color))
                    color = (Color)ColorConverter.ConvertFromString("#000000");
                else
                    color = (Color)ColorConverter.ConvertFromString("#" + PolicyViewModel.Instance.VideoOverlay.TopTeam2Color);

                cPicker.StartingColor = color;
                Window window = Window.GetWindow(this);
                cPicker.Owner = window;
                bool? dialogResult = cPicker.ShowDialog();
                if (dialogResult != null && (bool)dialogResult == true)
                {
                    PolicyViewModel.Instance.VideoOverlay.TopTeam2Color = cPicker.SelectedColor.ToString();
                    Team2TopColor.Foreground = new SolidColorBrush(cPicker.SelectedColor);
                    Team2TopColor.Background = new SolidColorBrush(cPicker.SelectedColor);

                    //gets the last 6 chars in string since it throws back opacity and Hash mark.
                    PolicyViewModel.Instance.VideoOverlay.TopTeam2Color = PolicyViewModel.Instance.VideoOverlay.TopTeam2Color.Substring(PolicyViewModel.Instance.VideoOverlay.TopTeam2Color.Length - 6);
                    Logger.Instance.logMessage("setting top team 2 color", LoggerEnum.message);
                    PolicyViewModel.Instance.savePolicyToXml();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }

        }

        private void Team2BottomColor_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorPickerDialog cPicker = new ColorPickerDialog();

                Color color;
                if (string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.BottomTeam2Color))
                    color = (Color)ColorConverter.ConvertFromString("#000000");
                else
                    color = (Color)ColorConverter.ConvertFromString("#" + PolicyViewModel.Instance.VideoOverlay.BottomTeam2Color);

                cPicker.StartingColor = color;
                Window window = Window.GetWindow(this);
                cPicker.Owner = window;
                bool? dialogResult = cPicker.ShowDialog();
                if (dialogResult != null && (bool)dialogResult == true)
                {
                    PolicyViewModel.Instance.VideoOverlay.BottomTeam2Color = cPicker.SelectedColor.ToString();
                    Team2BottomColor.Foreground = new SolidColorBrush(cPicker.SelectedColor);
                    Team2BottomColor.Background = new SolidColorBrush(cPicker.SelectedColor);

                    //gets the last 6 chars in string since it throws back opacity and Hash mark.
                    PolicyViewModel.Instance.VideoOverlay.BottomTeam2Color = PolicyViewModel.Instance.VideoOverlay.BottomTeam2Color.Substring(PolicyViewModel.Instance.VideoOverlay.BottomTeam2Color.Length - 6);
                    Logger.Instance.logMessage("setting bottom team 2 color", LoggerEnum.message);
                    PolicyViewModel.Instance.savePolicyToXml();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void Team2FontColor_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorPickerDialog cPicker = new ColorPickerDialog();

                Color color;
                if (string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.FontTeam2Color))
                    color = (Color)ColorConverter.ConvertFromString("#000000");
                else
                    color = (Color)ColorConverter.ConvertFromString("#" + PolicyViewModel.Instance.VideoOverlay.FontTeam2Color);

                cPicker.StartingColor = color;
                Window window = Window.GetWindow(this);
                cPicker.Owner = window;
                bool? dialogResult = cPicker.ShowDialog();
                if (dialogResult != null && (bool)dialogResult == true)
                {
                    PolicyViewModel.Instance.VideoOverlay.FontTeam2Color = cPicker.SelectedColor.ToString();
                    Team2FontColor.Foreground = new SolidColorBrush(cPicker.SelectedColor);
                    Team2FontColor.Background = new SolidColorBrush(cPicker.SelectedColor);

                    //gets the last 6 chars in string since it throws back opacity and Hash mark.
                    PolicyViewModel.Instance.VideoOverlay.FontTeam2Color = PolicyViewModel.Instance.VideoOverlay.FontTeam2Color.Substring(PolicyViewModel.Instance.VideoOverlay.FontTeam2Color.Length - 6);
                    Logger.Instance.logMessage("setting font team 2 color", LoggerEnum.message);
                    PolicyViewModel.Instance.savePolicyToXml();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void TimeOutColor_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorPickerDialog cPicker = new ColorPickerDialog();

                Color color;
                if (string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.TimeOutColor))
                    color = (Color)ColorConverter.ConvertFromString("#000000");
                else
                    color = (Color)ColorConverter.ConvertFromString("#" + PolicyViewModel.Instance.VideoOverlay.TimeOutColor);

                cPicker.StartingColor = color;
                Window window = Window.GetWindow(this);
                cPicker.Owner = window;
                bool? dialogResult = cPicker.ShowDialog();
                if (dialogResult != null && (bool)dialogResult == true)
                {
                    PolicyViewModel.Instance.VideoOverlay.TimeOutColor = cPicker.SelectedColor.ToString();
                    TimeOutColor.Foreground = new SolidColorBrush(cPicker.SelectedColor);
                    TimeOutColor.Background = new SolidColorBrush(cPicker.SelectedColor);

                    //gets the last 6 chars in string since it throws back opacity and Hash mark.
                    PolicyViewModel.Instance.VideoOverlay.TimeOutColor = PolicyViewModel.Instance.VideoOverlay.TimeOutColor.Substring(PolicyViewModel.Instance.VideoOverlay.TimeOutColor.Length - 6);
                    Logger.Instance.logMessage("setting time out color", LoggerEnum.message);
                    PolicyViewModel.Instance.savePolicyToXml();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void TextColor_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorPickerDialog cPicker = new ColorPickerDialog();

                Color color;
                if (string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.TextColor))
                    color = (Color)ColorConverter.ConvertFromString("#000000");
                else
                    color = (Color)ColorConverter.ConvertFromString("#" + PolicyViewModel.Instance.VideoOverlay.TextColor);

                cPicker.StartingColor = color;

                Window window = Window.GetWindow(this);
                cPicker.Owner = window;
                bool? dialogResult = cPicker.ShowDialog();
                if (dialogResult != null && (bool)dialogResult == true)
                {
                    PolicyViewModel.Instance.VideoOverlay.TextColor = cPicker.SelectedColor.ToString();
                    TextColor.Foreground = new SolidColorBrush(cPicker.SelectedColor);
                    TextColor.Background = new SolidColorBrush(cPicker.SelectedColor);

                    //gets the last 6 chars in string since it throws back opacity and Hash mark.
                    PolicyViewModel.Instance.VideoOverlay.TextColor = PolicyViewModel.Instance.VideoOverlay.TextColor.Substring(PolicyViewModel.Instance.VideoOverlay.TextColor.Length - 6);
                    Logger.Instance.logMessage("setting video overlay text color", LoggerEnum.message);
                    PolicyViewModel.Instance.savePolicyToXml();
                }
            }
            catch (Exception exception)
            {
                PolicyViewModel.Instance.VideoOverlay.TextColor = null;
                Logger.Instance.logMessage("setting null text color", LoggerEnum.message);
                PolicyViewModel.Instance.savePolicyToXml();
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void LogoOnOff_Click_1(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.VideoOverlay.LogoOnOff = !PolicyViewModel.Instance.VideoOverlay.LogoOnOff;
            Logger.Instance.logMessage("setting video overlay logo onoff", LoggerEnum.message);
            PolicyViewModel.Instance.savePolicyToXml();
        }

        private void OverlayTopBottom_Click_1(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.VideoOverlay.ScoresOnTop = !PolicyViewModel.Instance.VideoOverlay.ScoresOnTop;
            if (PolicyViewModel.Instance.VideoOverlay.ScoresOnTop)
                VerticalPositionText.Text = "Vertical Position Pixels from Top: ";
            else
                VerticalPositionText.Text = "Vertical Position Pixels from Bottom: ";
            PolicyViewModel.Instance.savePolicyToXml();
        }

        private void OverlayColor_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ColorPickerDialog cPicker = new ColorPickerDialog();

                Color color;
                if (string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.OverlayColor))
                    color = (Color)ColorConverter.ConvertFromString("#000000");
                else
                    color = (Color)ColorConverter.ConvertFromString("#" + PolicyViewModel.Instance.VideoOverlay.OverlayColor);

                cPicker.StartingColor = color;

                Window window = Window.GetWindow(this);
                cPicker.Owner = window;
                bool? dialogResult = cPicker.ShowDialog();
                if (dialogResult != null && (bool)dialogResult == true)
                {
                    PolicyViewModel.Instance.VideoOverlay.OverlayColor = cPicker.SelectedColor.ToString();
                    OverlayColor.Foreground = new SolidColorBrush(cPicker.SelectedColor);
                    OverlayColor.Background = new SolidColorBrush(cPicker.SelectedColor);

                    //gets the last 6 chars in string since it throws back opacity and Hash mark.
                    PolicyViewModel.Instance.VideoOverlay.OverlayColor = PolicyViewModel.Instance.VideoOverlay.OverlayColor.Substring(PolicyViewModel.Instance.VideoOverlay.OverlayColor.Length - 6);
                    Logger.Instance.logMessage("setting video overlay color", LoggerEnum.message);
                    PolicyViewModel.Instance.savePolicyToXml();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }



        private void OverlayTransparent_Click_2(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.VideoOverlay.IsOverlayTransparent = !PolicyViewModel.Instance.VideoOverlay.IsOverlayTransparent;
            Logger.Instance.logMessage("setting overlay transparent", LoggerEnum.message);
            PolicyViewModel.Instance.savePolicyToXml();
        }

        private void TurnThirdRowOn_Click_1(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.VideoOverlay.IsBottomRowOn = !PolicyViewModel.Instance.VideoOverlay.IsBottomRowOn;
            PolicyViewModel.Instance.savePolicyToXml();
        }

        private void GreenScreen_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {
                ColorPickerDialog cPicker = new ColorPickerDialog();

                Color color;
                if (string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.GreenScreen))
                    color = (Color)ColorConverter.ConvertFromString("#000000");
                else
                    color = (Color)ColorConverter.ConvertFromString("#" + PolicyViewModel.Instance.VideoOverlay.GreenScreen);

                cPicker.StartingColor = color;

                Window window = Window.GetWindow(this);
                cPicker.Owner = window;
                bool? dialogResult = cPicker.ShowDialog();
                if (dialogResult != null && (bool)dialogResult == true)
                {
                    PolicyViewModel.Instance.VideoOverlay.GreenScreen = cPicker.SelectedColor.ToString();
                    GreenScreen.Foreground = new SolidColorBrush(cPicker.SelectedColor);
                    GreenScreen.Background = new SolidColorBrush(cPicker.SelectedColor);

                    //gets the last 6 chars in string since it throws back opacity and Hash mark.
                    PolicyViewModel.Instance.VideoOverlay.GreenScreen = PolicyViewModel.Instance.VideoOverlay.GreenScreen.Substring(PolicyViewModel.Instance.VideoOverlay.GreenScreen.Length - 6);
                    Logger.Instance.logMessage("setting greeen screen.", LoggerEnum.message);
                    PolicyViewModel.Instance.savePolicyToXml();
                }
            }
            catch (Exception exception)
            {
                PolicyViewModel.Instance.VideoOverlay.GreenScreen = null;
                Logger.Instance.logMessage("setting green screen null ", LoggerEnum.message);
                PolicyViewModel.Instance.savePolicyToXml();
                ErrorViewModel.Save(exception, GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void showJamScore_Click_1(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.VideoOverlay.ShowJamScore = !PolicyViewModel.Instance.VideoOverlay.ShowJamScore;
            PolicyViewModel.Instance.savePolicyToXml();
        }

        private void ChangeLogo_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Images (*.PNG)|*.PNG";//"Images (*.JPG;*.JPEG;*.PNG)|*.JPG;*.JPEG;*.PNG";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;

                    var info = new FileInfo(filename);
                    if (!String.IsNullOrEmpty(filename) && info.Exists)
                    {
                        try
                        {
                            DateTime datevalue = DateTime.Now;

                            String imageName = datevalue.Day.ToString() + datevalue.Month.ToString() + datevalue.Year.ToString() + datevalue.Hour.ToString() + datevalue.Minute.ToString() + datevalue.Second.ToString();

                            string destinationFilePath = ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG;
                            string destinationFileName = imageName + info.Extension;
                            DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_IMG);
                            if (!dir.Exists)
                                dir.Create();
                            if (PolicyViewModel.Instance.VideoOverlay != null)
                            {
                                PolicyViewModel.Instance.VideoOverlay.LogoPictureLocation = destinationFilePath + destinationFileName;
                                PolicyViewModel.Instance.savePolicyToXml();
                            }

                            File.Copy(filename, System.IO.Path.Combine(destinationFilePath, destinationFileName), true);

                            //Change logo name
                            LogoNameChange(destinationFileName);

                        }
                        catch (Exception exception)
                        {
                            ErrorViewModel.Save(exception, GetType(), null, null, null, Logger.Instance.getLoggedMessages());
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        public void LogoNameChange(string imageName)
        {
            try
            {
                string destinationFilePath = ScoreboardConfig.SAVE_SERVER_FILES_FOLDER_CSS;

                using (StreamWriter sw = new StreamWriter(@destinationFilePath + @"\logo.css"))
                {
                    sw.Write(".logo {background-image: url(" + "'" + "../img/" + imageName + "'" + "); background-position-x:right; background-position-y:20px;  background-repeat: no-repeat; }");
                }

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, this.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        private void ClearLogo_Click(object sender, RoutedEventArgs e)
        {
            if (PolicyViewModel.Instance.VideoOverlay != null)
            {
                PolicyViewModel.Instance.VideoOverlay.LogoPictureLocation = string.Empty;
                PolicyViewModel.Instance.savePolicyToXml();
                LogoNameChange("Rollerball_pink_s100.png");
            }
        }

        private void VerticalPosition_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreOnlyNumbersAllowed(e.Text);
            base.OnPreviewTextInput(e);
        }
        private static bool AreOnlyNumbersAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void VerticalPosition_KeyUp(object sender, KeyEventArgs e)
        {
            if (!String.IsNullOrEmpty(VerticalPosition.Text))
            {
                int temp = 30;
                if (Int32.TryParse(VerticalPosition.Text, out temp))
                {
                    PolicyViewModel.Instance.VideoOverlay.VerticalPosition = Convert.ToInt32(VerticalPosition.Text);
                    PolicyViewModel.Instance.savePolicyToXml();
                }
            }
        }
        private void TextSizePosition_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreOnlyDecimalsAllowed(e.Text);
            base.OnPreviewTextInput(e);
        }
        private static bool AreOnlyDecimalsAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void TextSizePosition_KeyUp(object sender, KeyEventArgs e)
        {
            if (!String.IsNullOrEmpty(TextSizePosition.Text))
            {
                double temp = 1.0;
                if (Double.TryParse(TextSizePosition.Text, out temp))
                {
                    PolicyViewModel.Instance.VideoOverlay.TextSizePosition = temp;
                    PolicyViewModel.Instance.savePolicyToXml();
                }
            }
        }

        private void showTimeOuts_Click(object sender, RoutedEventArgs e)
        {
            PolicyViewModel.Instance.VideoOverlay.TurnOffTimeOuts = !PolicyViewModel.Instance.VideoOverlay.TurnOffTimeOuts;
            PolicyViewModel.Instance.savePolicyToXml();

        }
    }
}
