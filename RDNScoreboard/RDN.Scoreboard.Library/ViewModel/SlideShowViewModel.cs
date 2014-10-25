using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using RDN.Utilities.Config;

namespace Scoreboard.Library.ViewModel
{
    public class SlideShowViewModel
    {

        public bool IsShowing { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        [XmlIgnore]
        public string Path { get; set; }

        [XmlIgnore]
        public string FileLocation
        {
            get
            {
                if (Path == null)
                    Path = ScoreboardConfig.SAVE_SLIDESHOW_FOLDER;
                return System.IO.Path.Combine(Path, FileName + FileExtension);
            }
        }

        public SlideShowViewModel()
        { }
        public SlideShowViewModel(bool isShowing, string path, string fileName)
        {
            this.Path = path;
            this.IsShowing = isShowing;
            this.FileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
            this.FileExtension = System.IO.Path.GetExtension(fileName);

            try
            {
                var id = new Guid(this.FileName);
            }
            catch
            {
                //the advert file should be named after its ID.  So, we are making sure all adverts are named after their ids.
                //if its not, well we save it as its id.
                var id = Guid.NewGuid();
                try
                {
                    this.Path = ScoreboardConfig.SAVE_SLIDESHOW_FOLDER;
                    this.FileName = id.ToString().Replace("-", "") + System.IO.Path.GetExtension(fileName);

                    File.Copy(System.IO.Path.Combine(path, fileName), this.FileLocation, true);
                    File.Delete(System.IO.Path.Combine(path, fileName));
                }
                catch (Exception exception)
                {
                    ErrorViewModel.Save(exception, GetType());
                }
            }
        }


        /// <summary>
        /// gets the adverts from the directory.
        /// </summary>
        public static void getSlidesFromDirectory()
        {
            DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_SLIDESHOW_FOLDER);
            if (dir.Exists)
            {
                var files = dir.GetFiles();
                int fileCount = files.Count();

                for (int i = 0; i < fileCount; i++)
                {
                    if (files[i].ToString().Contains(".jpg") || files[i].ToString().Contains(".png") || files[i].ToString().Contains(".gif"))
                    {
                        try
                        {
                            if (GameViewModel.Instance.SlideShowSlides.Where(x => x.FileLocation == ScoreboardConfig.SAVE_SLIDESHOW_FOLDER + files[i].ToString()).FirstOrDefault() == null)
                                GameViewModel.Instance.SlideShowSlides.Add(new SlideShowViewModel(false, ScoreboardConfig.SAVE_SLIDESHOW_FOLDER, files[i].ToString()));
                        }
                        catch (Exception exception)
                        {
                            ErrorViewModel.Save(exception, exception.GetType());
                        }
                    }
                }
            }
        }

    }
}
