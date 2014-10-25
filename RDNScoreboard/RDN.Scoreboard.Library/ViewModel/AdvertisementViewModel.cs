using System;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using RDN.Utilities.Config;
using RDN.Utilities.Error;

namespace Scoreboard.Library.ViewModel
{
    public class AdvertisementViewModel
    {
        public AdvertisementViewModel()
        {
            this.Path = ScoreboardConfig.SAVE_ADVERTS_FOLDER;
            this.AdvertGameId = Guid.NewGuid();
        }
        public AdvertisementViewModel(bool isShowing, string path, string fileName)
        {
            try
            {
                this.Path = path;
                this.IsShowing = isShowing;
                this.FileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                this.FileExtension = System.IO.Path.GetExtension(fileName);
                if (fileName.Contains("."))
                    fileName = fileName.Split('.')[0];
                this.AdvertGameId = new Guid(fileName);
            }
            catch
            {
                //the advert file should be named after its ID.  So, we are making sure all adverts are named after their ids.
                //if its not, well we save it as its id.
                this.AdvertGameId = Guid.NewGuid();
                try
                {
                    this.Path = ScoreboardConfig.SAVE_ADVERTS_FOLDER;
                    this.FileName = this.AdvertGameId.ToString().Replace("-", "") + System.IO.Path.GetExtension(fileName);

                    File.Copy(System.IO.Path.Combine(path, fileName), this.FileLocation, true);
                    File.Delete(System.IO.Path.Combine(path, fileName));
                }
                catch (Exception exception)
                {
                    ErrorViewModel.Save(exception, GetType());
                }
            }
        }
        public AdvertisementViewModel(Guid advertId, bool isShowing, string path, string fileName)
        {

            try
            {

                this.AdvertGameId = advertId;
                this.Path = path;
                this.IsShowing = isShowing;
                this.FileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                this.FileExtension = System.IO.Path.GetExtension(fileName);

            }
            catch
            {
                //the advert file should be named after its ID.  So, we are making sure all adverts are named after their ids.
                //if its not, well we save it as its id.
                try
                {
                    this.Path = ScoreboardConfig.SAVE_ADVERTS_FOLDER;
                    this.FileName = this.AdvertGameId.ToString().Replace("-", "") + System.IO.Path.GetExtension(fileName);

                    File.Copy(System.IO.Path.Combine(path, fileName), this.FileLocation, true);
                    File.Delete(System.IO.Path.Combine(path, fileName));
                }
                catch (Exception exception)
                {
                    ErrorViewModel.Save(exception, GetType());
                }
            }
        }
        public Guid AdvertGameId { get; set; }
        public bool IsShowing { get; set; }
        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public byte[] AdvertismentPictureCompressed { get; set; }
        [XmlIgnore]
        public string Path { get; set; }

        [XmlIgnore]
        public string FileLocation
        {
            get
            {
                if (FileName.Contains("."))
                    return System.IO.Path.Combine(Path, FileName);
                else
                    return System.IO.Path.Combine(Path, FileName + FileExtension);

            }
        }


        /// <summary>
        /// gets the adverts from the directory.
        /// </summary>
        public static void getAdvertsFromDirectory()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_ADVERTS_FOLDER);
                if (dir.Exists)
                {
                    var files = dir.GetFiles();
                    int fileCount = files.Count();

                    for (int i = 0; i < fileCount; i++)
                    {
                        if (files[i].ToString().Contains(".jpg") || files[i].ToString().Contains(".png"))
                        {
                            if (GameViewModel.Instance.Advertisements.Where(x => x.FileLocation == ScoreboardConfig.SAVE_ADVERTS_FOLDER + files[i].ToString()).FirstOrDefault() == null)
                                GameViewModel.Instance.Advertisements.Add(new AdvertisementViewModel(true, ScoreboardConfig.SAVE_ADVERTS_FOLDER, files[i].ToString()));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, e.GetType(), ErrorGroupEnum.UI);
            }
        }
    }
}
