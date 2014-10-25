using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RDNation.Droid.Classes.Images
{
  public   class Images
    {
      public List<Image> Imgs { get; set; }

        static Images instance = new Images();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Images()
        {
            instance.Imgs = new List<Image>();
        }

        public static Images Instance
        {
            get
            {
                return instance;
            }
        }
    }
}