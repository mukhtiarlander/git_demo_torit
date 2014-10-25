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
using System.Windows.Shapes;
using Scoreboard.Library.ViewModel;

using RDN.Utilities.Config;
using Scoreboard.Library.ViewModel.Members;

namespace RDNScoreboard.Views.LineupTemplates
{
    /// <summary>
    /// Interaction logic for TemplateChooserView.xaml
    /// </summary>
    public partial class TemplateChooserView : Window
    {
        TeamViewModel _team;
        PlainLogo _plainLogo = new PlainLogo();
        PlainProfile _plainProfile = new PlainProfile();
        SideBarLogo _sideBarLogo = new SideBarLogo();
        SideBarProfile _sideBarProfile = new SideBarProfile();

        public TemplateChooserView(TeamViewModel team)
        {
            InitializeComponent();
            this.Title = "LineUp Templates Manager - " + ScoreboardConfig.SCOREBOARD_NAME;
            //sets up the view.
            //loads the images of the objects into the view so users can see what the pages look like.
            PlainLogoFrame.Content = _plainLogo;
            PlainProfileFrame.Content = _plainProfile;
            SideBarLogoFrame.Content = _sideBarLogo;
            SideBarProfileFrame.Content = _sideBarProfile;

            _team = team;
            if (_team.LineUpSettings == null)
                _team.LineUpSettings = new LineUpViewModel();
            {
                switch (_team.LineUpSettings.LineUpTypeSelected)
                {
                    case LineUpTypesEnum.PlainLineUp:
                        PlainLogoOn.IsChecked = true;
                        break;
                    case LineUpTypesEnum.SideBarLineUp:
                        SideBarLogoOn.IsChecked = true;
                        break;
                }//sets all the defined colors.
                setPlainBackgroundColor();
                setPlainBorderColor();
                setPlainTextColor();
                setSidebarBackgroundColor();
                setSidebarColor();
                setSideBarSkaterColor();
                setSidebarTextColor();
            }

            
           

        }

        private void PlainLogoOn_Click(object sender, RoutedEventArgs e)
        {
            _team.LineUpSettings.LineUpTypeSelected = LineUpTypesEnum.PlainLineUp;
            PlainLogoOn.IsChecked = true;
            SideBarLogoOn.IsChecked = false;
        }

        private void SideBarLogoOn_Click(object sender, RoutedEventArgs e)
        {
            _team.LineUpSettings.LineUpTypeSelected = LineUpTypesEnum.SideBarLineUp;
            PlainLogoOn.IsChecked = false;
            SideBarLogoOn.IsChecked = true;

        }

        private void PlainBorderColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cPicker = new ColorPickerDialog();

            Color color;
            if (string.IsNullOrEmpty(_team.LineUpSettings.PlainBorderColor))
                color = (Color)ColorConverter.ConvertFromString("#774ca6");
            else
                color = (Color)ColorConverter.ConvertFromString(_team.LineUpSettings.PlainBorderColor);

            cPicker.StartingColor = color;
            cPicker.Owner = this;

            bool? dialogResult = cPicker.ShowDialog();
            if (dialogResult != null && (bool)dialogResult == true)
            {
                _team.LineUpSettings.PlainBorderColor = cPicker.SelectedColor.ToString();
                setPlainBorderColor();
            }
        }
        /// <summary>
        /// sets the plain border color.
        /// </summary>
        private void setPlainBorderColor()
        {
            if (String.IsNullOrEmpty(_team.LineUpSettings.PlainBorderColor))
                return;
            _plainLogo.setBorderColor(_team.LineUpSettings.PlainBorderColor);
            _plainProfile.setBorderColor(_team.LineUpSettings.PlainBorderColor);
        }

        private void PlainTextColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cPicker = new ColorPickerDialog();

            Color color;
            if (string.IsNullOrEmpty(_team.LineUpSettings.PlainTextColor))
                color = (Color)ColorConverter.ConvertFromString("#774ca6");
            else
                color = (Color)ColorConverter.ConvertFromString(_team.LineUpSettings.PlainTextColor);

            cPicker.StartingColor = color;
            cPicker.Owner = this;

            bool? dialogResult = cPicker.ShowDialog();
            if (dialogResult != null && (bool)dialogResult == true)
            {
                _team.LineUpSettings.PlainTextColor = cPicker.SelectedColor.ToString();
                setPlainTextColor();
            }
        }
        /// <summary>
        /// sets the plain text color
        /// </summary>
        private void setPlainTextColor()
        {
            if (String.IsNullOrEmpty(_team.LineUpSettings.PlainTextColor))
                return;
            _plainProfile.setSkaterTextColor(_team.LineUpSettings.PlainTextColor);
        }

        private void PlainBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cPicker = new ColorPickerDialog();

            Color color;
            if (string.IsNullOrEmpty(_team.LineUpSettings.PlainBackgroundColor))
                color = (Color)ColorConverter.ConvertFromString("#774ca6");
            else
                color = (Color)ColorConverter.ConvertFromString(_team.LineUpSettings.PlainBackgroundColor);

            cPicker.StartingColor = color;
            cPicker.Owner = this;

            bool? dialogResult = cPicker.ShowDialog();
            if (dialogResult != null && (bool)dialogResult == true)
            {
                _team.LineUpSettings.PlainBackgroundColor = cPicker.SelectedColor.ToString();
                setPlainBackgroundColor();

            }
        }
        /// <summary>
        /// sets the plain background color.
        /// </summary>
        private void setPlainBackgroundColor()
        {
            if (String.IsNullOrEmpty(_team.LineUpSettings.PlainBackgroundColor))
                return;
            _plainLogo.setBackgroundColor(_team.LineUpSettings.PlainBackgroundColor);
            _plainProfile.setBackgroundColor(_team.LineUpSettings.PlainBackgroundColor);
        }

        private void SidebarColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cPicker = new ColorPickerDialog();

            Color color;
            if (string.IsNullOrEmpty(_team.LineUpSettings.SidebarColor))
                color = (Color)ColorConverter.ConvertFromString("#000000");
            else
                color = (Color)ColorConverter.ConvertFromString(_team.LineUpSettings.SidebarColor);

            cPicker.StartingColor = color;
            cPicker.Owner = this;

            bool? dialogResult = cPicker.ShowDialog();
            if (dialogResult != null && (bool)dialogResult == true)
            {
                _team.LineUpSettings.SidebarColor = cPicker.SelectedColor.ToString();
                setSidebarColor();
            }
        }

        private void setSidebarColor()
        {
            if (String.IsNullOrEmpty(_team.LineUpSettings.SidebarColor))
                return;
            _sideBarLogo.setSidebarColor(_team.LineUpSettings.SidebarColor);
            _sideBarProfile.setSidebarColor(_team.LineUpSettings.SidebarColor);
        }

        private void SidebarTextColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cPicker = new ColorPickerDialog();

            Color color;
            if (string.IsNullOrEmpty(_team.LineUpSettings.SidebarTextColor))
                color = (Color)ColorConverter.ConvertFromString("#FFFFFF");
            else
                color = (Color)ColorConverter.ConvertFromString(_team.LineUpSettings.SidebarTextColor);

            cPicker.StartingColor = color;
            cPicker.Owner = this;

            bool? dialogResult = cPicker.ShowDialog();
            if (dialogResult != null && (bool)dialogResult == true)
            {
                _team.LineUpSettings.SidebarTextColor = cPicker.SelectedColor.ToString();
                setSidebarTextColor();
            }
        }

        private void setSidebarTextColor()
        {
            if (String.IsNullOrEmpty(_team.LineUpSettings.SidebarTextColor))
                return;
            _sideBarLogo.setSidebarTextColor(_team.LineUpSettings.SidebarTextColor);
            _sideBarProfile.setSidebarTextColor(_team.LineUpSettings.SidebarTextColor);
        }

        private void SidebarSkaterColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cPicker = new ColorPickerDialog();

            Color color;
            if (string.IsNullOrEmpty(_team.LineUpSettings.SidebarSkaterTextColor))
                color = (Color)ColorConverter.ConvertFromString("#000000");
            else
                color = (Color)ColorConverter.ConvertFromString(_team.LineUpSettings.SidebarSkaterTextColor);

            cPicker.StartingColor = color;
            cPicker.Owner = this;

            bool? dialogResult = cPicker.ShowDialog();
            if (dialogResult != null && (bool)dialogResult == true)
            {
                _team.LineUpSettings.SidebarSkaterTextColor = cPicker.SelectedColor.ToString();

                setSideBarSkaterColor();
            }
        }

        private void setSideBarSkaterColor()
        {
            if (String.IsNullOrEmpty(_team.LineUpSettings.SidebarSkaterTextColor))
                return;
            _sideBarProfile.setSkaterTextColor(_team.LineUpSettings.SidebarSkaterTextColor);
        }

        private void SidebarBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cPicker = new ColorPickerDialog();

            Color color;
            if (string.IsNullOrEmpty(_team.LineUpSettings.SidebarBackgroundColor))
                color = (Color)ColorConverter.ConvertFromString("#eb98f4");
            else
                color = (Color)ColorConverter.ConvertFromString(_team.LineUpSettings.SidebarBackgroundColor);

            cPicker.StartingColor = color;
            cPicker.Owner = this;

            bool? dialogResult = cPicker.ShowDialog();
            if (dialogResult != null && (bool)dialogResult == true)
            {
                _team.LineUpSettings.SidebarBackgroundColor = cPicker.SelectedColor.ToString();
                setSidebarBackgroundColor();
            }
        }

        private void setSidebarBackgroundColor()
        {
            if (String.IsNullOrEmpty(_team.LineUpSettings.SidebarBackgroundColor))
                return;
            _sideBarLogo.setBackgroundColor(_team.LineUpSettings.SidebarBackgroundColor);
            _sideBarProfile.setBackgroundColor(_team.LineUpSettings.SidebarBackgroundColor);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
