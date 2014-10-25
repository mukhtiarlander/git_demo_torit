using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDNationLibrary.Static;
using System.IO;

namespace RDNationLibrary.ViewModel
{
    public class AdvertisementViewModel
    {
        public AdvertisementViewModel()
        { }
        public AdvertisementViewModel(bool isShowing, string fileLocation)
        {
            this.FileLocation = fileLocation;
            this.IsShowing = isShowing;
        }
        public bool IsShowing { get; set; }
        public string FileLocation { get; set; }

        public static void getAdvertsFromDirectory()
        {
            DirectoryInfo dir = new DirectoryInfo(Config.SAVE_ADVERTS_FOLDER);
            if (dir.Exists)
            {
                var files = dir.GetFiles();
                int fileCount = files.Count();

                for (int i = 0; i < fileCount; i++)
                {
                    if (files[i].ToString().Contains(".jpg") || files[i].ToString().Contains(".png"))
                    {
                        if (GameViewModel.Instance.Advertisements.Where(x => x.FileLocation == Config.SAVE_ADVERTS_FOLDER + files[i].ToString()).FirstOrDefault() == null)
                            GameViewModel.Instance.Advertisements.Add(new AdvertisementViewModel(true, Config.SAVE_ADVERTS_FOLDER + files[i].ToString()));
                    }
                }
            }
        }
    }
}
