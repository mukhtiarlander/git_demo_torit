using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using RDN.Utilities.Config;

namespace RDNScoreboard.Views
{
    /// <summary>
    /// Interaction logic for ReleaseNotes.xaml
    /// </summary>
    public partial class ReleaseNotes : Window
    {        
        public ReleaseNotes()
        {
            InitializeComponent();
            this.Title = "Release Notes - " + ScoreboardConfig.SCOREBOARD_NAME;
            var rnd = new ReleaseNotesData();
            rnd.Notes = File.ReadAllText(@"ReleaseNotes.txt");
            txtHistory.DataContext = rnd;
        }
    }

    public class ReleaseNotesData
    {
        public string Notes { get; set; }
    }
}
