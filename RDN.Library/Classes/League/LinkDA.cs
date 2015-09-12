using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;

namespace RDN.Library.Classes.League
{

    public class LinkDA
    {
        
        public long LinkId { get; set; }
        public string Link { get; set; }
        public string Notes { get; set; }
        public bool IsRemoved { set; get; }

        public Guid LinksForLeague { get; set; }
        public Guid LinksAddByMember { get; set; }

        public LinkDA() { }


        public static bool DeleteItem(long linkedID, Guid leagueId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbItem = dc.Links.Where(x => x.LinkId == linkedID && x.LinksForLeague.LeagueId == leagueId).FirstOrDefault();
                if (dbItem == null)
                    return false;
                dbItem.IsRemoved = true;

                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool AddLink(LinkDA linkItem)
        {

            try
            {
                var dc = new ManagementContext();

                DataModels.League.Links objLinks = new DataModels.League.Links();


                objLinks.Link = linkItem.Link;
                objLinks.Notes = linkItem.Notes;

                objLinks.LinksForLeague = dc.Leagues.Where(x => x.LeagueId == linkItem.LinksForLeague).FirstOrDefault();
                objLinks.LinksAddByMember = dc.Members.Where(x => x.MemberId == linkItem.LinksAddByMember).FirstOrDefault();
               
                dc.Links.Add(objLinks);
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }

        public static LinkDA GetLeagueLink(long linkId, Guid LinkForLeague)
        {
            try
            {
                var dc = new ManagementContext();
                var link = dc.Links.Where(x => x.LinkId == linkId && x.LinksForLeague.LeagueId == LinkForLeague).FirstOrDefault();

                if (link != null)
                { 
                    return new LinkDA {  Link=link.Link, Notes=link.Notes,LinksAddByMember=link.LinksAddByMember.MemberId,LinksForLeague=link.LinksForLeague.LeagueId};
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static List<LinkDA> GetLeagueLinks(Guid leagueId)
        {
            List<LinkDA> itemLists = new List<LinkDA>();
            try
            {
                var dc = new ManagementContext();

                var links = dc.Links.Where(x => x.LinksForLeague.LeagueId == leagueId && x.IsRemoved == false).ToList();

                if (links != null)
                {
                    foreach (var item in links)
                    {
                        itemLists.Add(new LinkDA { Link = item.Link, Notes = item.Notes, LinksAddByMember = item.LinksAddByMember.MemberId, LinksForLeague = item.LinksForLeague.LeagueId });
                    }
                }
                return itemLists;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static bool EditLink(LinkDA link)
        {
            try
            {

                var dc = new ManagementContext();
                var linkItem = dc.Links.Where(x => x.LinkId == link.LinkId).FirstOrDefault();
                
                if (linkItem == null)
                    return false;
                linkItem.Link = link.Link;
                linkItem.Notes = link.Notes; 
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
