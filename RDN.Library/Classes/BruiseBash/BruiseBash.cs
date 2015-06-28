using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.BruiseBash;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Config;

namespace RDN.Library.Classes.BruiseBash
{
    public class BruiseBash
    {
        public BruiseBash()
        {
            Comments = new List<BruiseBashComment>();
            Ratings = new List<BruiseBashRatings>();
        }
        public Guid BruiseId { get; set; }
        public string Title { get; set; }
        public string Story { get; set; }
        public string File { get; set; }
        public List<BruiseBashComment> Comments { get; set; }
        public List<BruiseBashRatings> Ratings { get; set; }

        public static BruiseBash GetBruiseBashItem(Guid idOfBruise)
        {
            try
            {
                var dc = new ManagementContext();
                var bruise = dc.BruiseBashItem.Where(x => x.BruiseBashId == idOfBruise).FirstOrDefault();

                BruiseBash bashObject = new BruiseBash();
                bashObject.Story = bruise.Story;
                bashObject.Title = bruise.Title;
                bashObject.File = bruise.Image.ImageUrl;
                bashObject.BruiseId = bruise.BruiseBashId;

                foreach (var comment in bruise.Comments)
                {
                    BruiseBashComment comTemp = new BruiseBashComment();
                    comTemp.Comment = comment.Comment;
                    comTemp.CommentId = comment.CommentId;
                    comTemp.Created = comment.Created;
                    if (comment.Owner != null)
                        comTemp.OwnerId = comment.Owner.MemberId;
                    bashObject.Comments.Add(comTemp);
                }
                foreach (var rating in bruise.Ratings)
                {
                    BruiseBashRatings ratTemp = new BruiseBashRatings();
                    ratTemp.Created = rating.Created;
                    if (rating.Rater != null)
                        ratTemp.RaterId = rating.Rater.MemberId;
                    ratTemp.RatingId = rating.RatingId;

                    if (rating.Loser != null)
                    {
                        BruiseBash loserBash = new BruiseBash();
                        loserBash.Story = rating.Loser.Story;
                        loserBash.Title = rating.Loser.Title;
                        loserBash.File = rating.Loser.Image.ImageUrl;
                        loserBash.BruiseId = rating.Loser.BruiseBashId;
                        ratTemp.Loser = loserBash;
                    }

                    if (rating.Winner != null)
                    {
                        BruiseBash winnerBash = new BruiseBash();
                        winnerBash.Story = rating.Winner.Story;
                        winnerBash.Title = rating.Winner.Title;
                        winnerBash.File = rating.Winner.Image.ImageUrl;
                        winnerBash.BruiseId = rating.Winner.BruiseBashId;
                        ratTemp.Winner = winnerBash;
                    }
                    bashObject.Ratings.Add(ratTemp);
                }
                return bashObject;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        /// <summary>
        /// saves the image to botht the database and the directory.
        /// </summary>
        /// <param name="bashItem"></param>
        /// <param name="fileStream"></param>
        /// <param name="MemberId"></param>
        public static Guid SaveBruiseBashItem(BruiseBash bashItem, Stream fileStream, Guid MemberId)
        {
            try
            {
                var dc = new ManagementContext();
                BruiseBashItem item = new BruiseBashItem();
                item.Story = bashItem.Story;
                item.Title = bashItem.Title;
                //time stamp for the save location
                DateTime timeOfSave = DateTime.UtcNow;

                FileInfo info = new FileInfo(bashItem.File);

                //the file name when we save it
                string fileName = "roller_derby_bruise_" + timeOfSave.ToFileTimeUtc() + info.Extension;


                string url = LibraryConfig.ImagesBaseUrl + "/bruise/" + timeOfSave.Year + "/" + timeOfSave.Month + "/" + timeOfSave.Day + "/";
                string imageLocationToSave = LibraryConfig.ImagesBaseSaveLocation+ @"\bruise\" + timeOfSave.Year + @"\" + timeOfSave.Month + @"\" + timeOfSave.Day + @"\";
                //creates the directory for the image
                if (!Directory.Exists(imageLocationToSave))
                    Directory.CreateDirectory(imageLocationToSave);

                url += fileName;
                imageLocationToSave += fileName;


                BruiseBashImage image = new BruiseBashImage();
                image.ImageUrl = url;
                image.Name = fileName;
                image.SaveLocation = imageLocationToSave;
                item.Image = image;
                //sets owner of the image
                item.Owner = dc.Members.Where(x => x.MemberId == MemberId).FirstOrDefault();

                using (var newfileStream = new FileStream(imageLocationToSave, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fileStream.CopyTo(newfileStream);
                }
                //saves the image to the db
                dc.BruiseBashItem.Add(item);
                dc.SaveChanges();
                return item.BruiseBashId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();

        }


    }
}
