using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Store
{
    public class ItemReview
    {
        public string userEmail { get; set; }
        public string userName { get; set; }
        public string title { get; set; }
        public string comment { get; set; }
        public string rate { get; set; }
        public long ReviewId { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }

        #region References
        public Guid MemberId { get; set; }
        public int StoreItemId { get; set; }
        public long InvoiceItemId { get; set; }
        #endregion

        public ItemReview()
        {

        }

        public static bool AddReview(ItemReview GetReview)
        {

            try
            {
                var dc = new ManagementContext();
                if (GetReview.ReviewId > 0)
                {
                    var review = dc.Reviews.Where(x => x.ReviewId == GetReview.ReviewId).FirstOrDefault();
                    review.comment = GetReview.comment;
                    review.title = GetReview.title;
                    review.rate = GetReview.rate;
                    review.StoreItem = review.StoreItem;
                }
                else
                {
                    DataModels.Store.Review con = new DataModels.Store.Review();
                    con.rate = GetReview.rate;
                    int s = GetReview.StoreItemId;
                    con.StoreItem = dc.StoreItems.Where(x => x.StoreItemId == GetReview.StoreItemId).FirstOrDefault();
                    con.comment = GetReview.comment;
                    con.Member = dc.Members.Where(x => x.MemberId == GetReview.MemberId).FirstOrDefault();
                    con.title = GetReview.title;
                    con.ItemReviewed = dc.InvoiceItems.Where(x => x.InvoiceItemId == GetReview.InvoiceItemId).FirstOrDefault();
                    dc.Reviews.Add(con);
                }
                int c = dc.SaveChanges();
                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }

        public static List<ItemReview> GetItemReview(int storeItemId)
        {
            List<ItemReview> Reviewse = new List<ItemReview>();
            try
            {
                var dc = new ManagementContext();
                var ReviewsList = dc.Reviews.Where(x => x.StoreItem.StoreItemId == storeItemId && x.IsPublished == true).ToList();

                foreach (var b in ReviewsList)
                {
                    Reviewse.Add(DisplayReviewList(b));
                }
                return Reviewse;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Reviewse;
        }
        public static ItemReview GetItemReviewForInvoiceItem(long InvoiceItemId, int storeItemId)
        {
            try
            {
                var dc = new ManagementContext();
                var ReviewsList = dc.Reviews.Where(x => x.ItemReviewed.InvoiceItemId == InvoiceItemId && x.StoreItem.StoreItemId == storeItemId).FirstOrDefault();
                if (ReviewsList != null)
                    return DisplayReviewList(ReviewsList);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static ItemReview DisplayReviewList(DataModels.Store.Review ReviewList)
        {
            ItemReview bl = new ItemReview();
            bl.StoreItemId = ReviewList.StoreItem.StoreItemId;
            bl.rate = ReviewList.rate;
            bl.title = ReviewList.title;
            bl.comment = ReviewList.comment;
            bl.IsDeleted = ReviewList.IsDeleted;
            bl.IsPublished = ReviewList.IsPublished;
            bl.ReviewId = ReviewList.ReviewId;
            bl.Created = ReviewList.Created;
            if (ReviewList.Member != null)
            {
                bl.MemberId = ReviewList.Member.MemberId;
                bl.userName = ReviewList.Member.DerbyName;
            }
            else
            {
                bl.userName = "Anonymous";
            }


            return bl;
        }

    }
}