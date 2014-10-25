using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPF.Themes
{
    public enum ThemeEnum {  SkatersLineUp, CRGScoreboard, DefaultScoreboard}
    /// <summary>
    /// selection for the each theme.
    /// </summary>
   public  class Theme
    {
       public Theme(string name, ThemeEnum type)
       {
           Name = name;
           Type = type;
       }
        public string Name { get; set; }
        public ThemeEnum Type { get; set; }
    }
}
