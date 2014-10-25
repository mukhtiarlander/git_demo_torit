using RDN.iOS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.iOS.Models
{
    public class MainModel
    {

        public MainScreenEnum ScreenType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Checked { get; set; }
        public string ImageName { get; set; }

        public MainModel()
        {

        }

    }
}
