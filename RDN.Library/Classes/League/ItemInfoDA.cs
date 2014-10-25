using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.League
{
    public class ItemInfoDA
    {
        public long ItemId { get; set; }
        public string ItemSerialNo { get; set; }
        public string ItemName { get; set; }
        public string UnitePice { get; set; }
        public string Location { get; set; }
        public string Quantity { get; set; }
        public string Notes { get; set; }
        public Guid ItemsForLeague { get; set; }
        public Guid ItemAddByMember { get; set; }

        public ItemInfoDA()
        { 
        
        }

        public static bool Add_New_Item(RDN.Library.Classes.League.ItemInfoDA NewItem)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.League.ItemInfo con = new DataModels.League.ItemInfo();
                con.ItemSerialNo = NewItem.ItemSerialNo;
                con.ItemName = NewItem.ItemName;
                con.Location = NewItem.Location;
                con.Notes = NewItem.Notes;
                con.Quantity = NewItem.Quantity;
                con.UnitePice = NewItem.UnitePice;

                con.ItemsForLeague = dc.Leagues.Where(x => x.LeagueId == NewItem.ItemsForLeague).FirstOrDefault();
                con.ItemAddByMember = dc.Members.Where(x => x.MemberId == NewItem.ItemAddByMember).FirstOrDefault();
                dc.ItemInfos.Add(con);

                int c = dc.SaveChanges();

                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }

        /// <summary>
        /// This function used for Edit and View Details
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="ItemsForLeague"></param>
        /// <returns>Item details</returns>
        public static ItemInfoDA GetData(long ItemId, Guid ItemsForLeague)//This Function used for "Edit" and "View" Form
        {
            try
            {
                var dc = new ManagementContext();
                var itemInfo = dc.ItemInfos.Where(x => x.ItemId == ItemId && x.ItemsForLeague.LeagueId == ItemsForLeague).FirstOrDefault();
                if (itemInfo != null)
                {
                    return DisplayItemList(itemInfo);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static List<ItemInfoDA> GetItemList(Guid leagueId)
        {
            List<ItemInfoDA> itemLists = new List<ItemInfoDA>();
            try
            {
                var dc = new ManagementContext();
                var ItemList = dc.ItemInfos.Where(x => x.ItemsForLeague.LeagueId == leagueId && x.IsDeleted == false).ToList();

                foreach (var b in ItemList)
                {
                    itemLists.Add(DisplayItemList(b));
                }
                return itemLists;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return itemLists;
        }

        public static bool UpdateItemInfo(RDN.Library.Classes.League.ItemInfoDA ItemToUpdate)
        {
            try
            {

                var dc = new ManagementContext();
                var dbItem = dc.ItemInfos.Where(x => x.ItemId == ItemToUpdate.ItemId).FirstOrDefault();
                if (dbItem == null)
                    return false;
                dbItem.ItemName = ItemToUpdate.ItemName;
                dbItem.Location = ItemToUpdate.Location;
                dbItem.Notes = ItemToUpdate.Notes;
                dbItem.Quantity = ItemToUpdate.Quantity;
                dbItem.UnitePice = ItemToUpdate.UnitePice;
                dbItem.ItemSerialNo = ItemToUpdate.ItemSerialNo;
                 
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        private static ItemInfoDA DisplayItemList(DataModels.League.ItemInfo oItem)
        {
            ItemInfoDA bl = new ItemInfoDA();
            bl.ItemId = oItem.ItemId;
            bl.ItemName = oItem.ItemName;
            bl.ItemSerialNo = oItem.ItemSerialNo;
            bl.Location = oItem.Location;
            bl.Quantity = oItem.Quantity;
            bl.Notes = oItem.Notes;
            bl.UnitePice = oItem.UnitePice;
            bl.ItemAddByMember = oItem.ItemAddByMember.MemberId;
            bl.ItemsForLeague = oItem.ItemsForLeague.LeagueId;

            return bl;
        }

        public static bool DeleteItem(long sponsorId, Guid leagueId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbItem = dc.ItemInfos.Where(x => x.ItemId == sponsorId && x.ItemsForLeague.LeagueId == leagueId).FirstOrDefault();
                if (dbItem == null)
                    return false;
                dbItem.IsDeleted = true;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

    }

}
