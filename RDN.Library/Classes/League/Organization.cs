using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.League.OrganizationChart;
using RDN.Portable.Classes.Account.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.League
{
    public class Organization
    {
        #region Designation
        public long DesignationId { get; set; }
        public string DesigTitle { get; set; }
        public string DesigLavel { get; set; }
        public string ShortNote { get; set; }
        public Guid LeagueId { get; set; }

        #endregion Designation

        #region Organize
        public long OrganizeId { get; set; }
        public string Comment { get; set; }
        public Guid StaffId { get; set; }
        public string OrganizedBy { get; set; }
        public string ReportsTo { get; set; }
        public Guid ManagerId { get; set; }
        public string ManagerName { get; set; }
        public string StaffName { get; set; }
        
        #endregion Organize

        #region Organization
        public long OrganizationId { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Website { get; set; }
        public string Note { get; set; }

        #endregion Organization

        #region Constructor
        public Organization()
        { 
        
        }
        #endregion Constructor

        #region Designation Methods

        public static bool AddNewDesignation(RDN.Library.Classes.League.Organization designation)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.League.OrganizationChart.Designation con = new DataModels.League.OrganizationChart.Designation();
                con.DesignationTitle = designation.DesigTitle;
                con.DesignationLevel = designation.DesigLavel;
                con.ShortNote = designation.ShortNote;
                con.League = dc.Leagues.Where(x => x.LeagueId == designation.LeagueId).FirstOrDefault();
                 
                dc.Designations.Add(con);

                int c = dc.SaveChanges();

                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }


        public static List<Organization> GetDesignationList(Guid leagueId)
        {
            List<Organization> DesignationList = new List<Organization>();
            try
            {
                var dc = new ManagementContext();
                var DesigList = dc.Designations.Where(x => x.League.LeagueId == leagueId).ToList();

                foreach (var b in DesigList)
                {
                    DesignationList.Add(DisplayDesignationList(b));
                }
                return DesignationList;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return DesignationList;
        }


        private static Organization DisplayDesignationList(DataModels.League.OrganizationChart.Designation designation)
        {
            Organization bl = new Organization();
            bl.DesignationId = designation.DesignationId;
            bl.DesigLavel = designation.DesignationLevel;
            bl.DesigTitle = designation.DesignationTitle;
            bl.ShortNote = designation.ShortNote;
            bl.LeagueId = designation.League.LeagueId;
            
            return bl;
        }

        public static Organization GetData(long designationId, Guid leagueId)//This Function used for "Edit" and "View" Form
        {
            try
            {
                var dc = new ManagementContext();
                var designation = dc.Designations.Where(x => x.DesignationId == designationId && x.League.LeagueId == leagueId).FirstOrDefault();
                if (designation != null)
                {
                    return DisplayDesignationList(designation);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

       

        public static bool UpdateDesignation(RDN.Library.Classes.League.Organization oDesignationToUpdate)
        {
            try
            {

                var dc = new ManagementContext();
                var dbDesignation = dc.Designations.Where(x => x.DesignationId == oDesignationToUpdate.DesignationId).FirstOrDefault();
                if (dbDesignation == null)
                    return false;

                dbDesignation.DesignationTitle = oDesignationToUpdate.DesigTitle;
                dbDesignation.DesignationLevel = oDesignationToUpdate.DesigLavel;
                dbDesignation.ShortNote = oDesignationToUpdate.ShortNote;
                 
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }


        #endregion Designation Methods


        #region Organize Methods
        public static bool NewOrganize(RDN.Library.Classes.League.Organization oOrganization)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.League.OrganizationChart.Organize con = new DataModels.League.OrganizationChart.Organize();
                con.OrganizedBy = oOrganization.OrganizedBy;
                con.StaffId = dc.Members.Where(x => x.MemberId == oOrganization.StaffId).FirstOrDefault();
                con.ManagerId = dc.Members.Where(x => x.MemberId == oOrganization.ManagerId).FirstOrDefault();
                con.Comment = oOrganization.Comment;
                con.ReportsToDesignation = oOrganization.ReportsTo;
                con.Designation = dc.Designations.Where(x => x.DesignationId == oOrganization.DesignationId).FirstOrDefault();
                con.League = dc.Leagues.Where(x => x.LeagueId == oOrganization.LeagueId).FirstOrDefault();
                con.Organization = dc.Organizations.Where(x => x.OrganizationId == oOrganization.OrganizationId).FirstOrDefault();
                dc.Organizes.Add(con);

                int c = dc.SaveChanges();

                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }

        private static Organization DisplayOrganizeList(DataModels.League.OrganizationChart.Organize oOrganize)
        {
            Organization bl = new Organization();
            bl.Comment = oOrganize.Comment;
            bl.DesignationId = oOrganize.Designation.DesignationId;
            bl.OrganizedBy = oOrganize.OrganizedBy;
            bl.OrganizeId = oOrganize.OrganizeId;
            bl.StaffId = oOrganize.StaffId.MemberId;
            bl.ReportsTo = oOrganize.ReportsToDesignation; //ReportTo column will be no more necessary
            bl.ManagerId = oOrganize.ManagerId.MemberId;
            bl.LeagueId = oOrganize.League.LeagueId;
            
           // bl.OrganizationId = oOrganize.Organization.OrganizationId;

            if (oOrganize.ManagerId.MemberId == Guid.Empty)
            {
                bl.ManagerName = "";
            }
            else
            {
               // var leagueId = MemberCache.GetLeagueIdOfMember();
               // var Members = League.GetLeagueMembersDisplay(leagueId);

                bl.ManagerName = RDN.Library.Classes.League.Organization.GetMemberName(oOrganize.ManagerId.MemberId).Replace(" ", "").Substring(0,5);
            }

            if (oOrganize.StaffId.MemberId == Guid.Empty)
            {
                bl.StaffName = "";
            }
            else
            {
                // var leagueId = MemberCache.GetLeagueIdOfMember();
                // var Members = League.GetLeagueMembersDisplay(leagueId);

                bl.StaffName = RDN.Library.Classes.League.Organization.GetMemberName(oOrganize.StaffId.MemberId).Replace(" ", "").Substring(0, 5);
            }
            
            return bl;
        }

        //public static bool IsBossExist(long designationId, Guid leagueId)//This Function used for "Edit" and "View" Form
        //{
        //    try
        //    {
        //        var dc = new ManagementContext();
        //        var Data = from org in dc.Organizes from desig in dc.Designations where desig.League.LeagueId == leagueId && org.Designation.DesignationId == designationId select org ;

        //        if (Data != null)
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return true;
        //}

        public static string GetMemberName(Guid memberId)
        {
            var leagueId = MemberCache.GetLeagueIdOfMember();
            var memberList = LeagueFactory.GetLeagueMembersDisplay(leagueId);
            foreach (var a in memberList)
            {
                if (a.MemberId == memberId)
                {
                    return a.DerbyName;
                }
            }

            return "";
        }

        public static List<MemberDisplay> MemberList(Guid leagueId)
        {
          return   LeagueFactory.GetLeagueMembersDisplay(leagueId);
        }

        public static List<Organization> GetOrgList(Guid leagueId,long oRganizationId)
        {
            List<Organization> OrgList = new List<Organization>();
            try
            {
                var dc = new ManagementContext();
                var OrganizationList = dc.Organizes.Where(x => x.League.LeagueId == leagueId && x.Organization.OrganizationId == oRganizationId).ToList();

                if (OrganizationList == null)
                    return OrgList;

                foreach (var b in OrganizationList)
                {
                    OrgList.Add(DisplayOrganizeList(b));
                }
                return OrgList;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return OrgList;
        }

        #endregion Organize Methods

        #region Organization
        public static bool NewOrganization(RDN.Library.Classes.League.Organization oOrganization)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.League.OrganizationChart.Organization con = new DataModels.League.OrganizationChart.Organization();
                con.Name = oOrganization.Name;
                con.Adress = oOrganization.Adress;
                con.Website = oOrganization.Website;
                con.Note = oOrganization.Note;
                con.League = dc.Leagues.Where(x => x.LeagueId == oOrganization.LeagueId).FirstOrDefault();

                dc.Organizations.Add(con);

                int c = dc.SaveChanges();

                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }


        private static Organization DisplayOrganizationList(DataModels.League.OrganizationChart.Organization oOrganization)
        {
            Organization bl = new Organization();
            bl.OrganizationId = oOrganization.OrganizationId;
            bl.Name = oOrganization.Name;
            bl.Note = oOrganization.Note;
            bl.Website = oOrganization.Website;
            bl.Adress = oOrganization.Adress;
            bl.LeagueId = oOrganization.League.LeagueId; 
            
            return bl;
        }

        public static List<Organization> GetOrganizationList(Guid leagueId)
        {
            List<Organization> OrgList = new List<Organization>();
            try
            {
                var dc = new ManagementContext();
                var OrganizationList = dc.Organizations.Where(x => x.League.LeagueId == leagueId).ToList();

                foreach (var b in OrganizationList)
                {
                    OrgList.Add(DisplayOrganizationList(b));
                }
                return OrgList;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return OrgList;
        }

        #endregion Organization


    }
}