using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Dues;
using RDN.Library.Classes.Dues.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.MemberFees;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Dues;
using RDN.Portable.Classes.Controls.Dues.Classify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Controls.Dues
{
    public class FeeClassificationFactory
    {

        public FeeClassificationFactory()
        {
        }

        public static bool AddNewClassification(FeeClassified fee)
        {
            RDN.Library.DataModels.MemberFees.FeeClassification f = new DataModels.MemberFees.FeeClassification();

            try
            {
                var dc = new ManagementContext();
                f.FeeItem = dc.FeeManagement.Where(x => x.FeeManagementId == fee.DuesId).FirstOrDefault();
                f.FeeRequired = fee.FeeRequired;
                f.DoesNotPayDues = fee.DoesNotPayDues;
                f.Name = fee.Name;
                dc.FeeClassification.Add(f);
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool ChangeClassificationForMember(Guid duesManagementId, long classificationId, Guid memberId)
        {
            FeeClassificationFactory fClass = new FeeClassificationFactory();
            try
            {
                var dc = new ManagementContext();
                var classy = dc.FeeClassificationByMember.Where(x => x.FeeItem.FeeItem.FeeManagementId == duesManagementId && x.Member.MemberId == memberId).FirstOrDefault();
                if (classificationId == 0)
                {
                    dc.FeeClassificationByMember.Remove(classy);
                }
                else if (classy == null)
                {
                    FeesClassificationByMember m = new FeesClassificationByMember();
                    var classification = dc.FeeClassification.Where(x => x.FeeItem.FeeManagementId == duesManagementId && x.FeeClassificationId == classificationId).FirstOrDefault();

                    m.FeeItem = classification;
                    m.Member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                    classification.MembersClassified.Add(m);
                    dc.FeeClassificationByMember.Add(m);
                }
                else
                {
                    classy.FeeItem = dc.FeeClassification.Where(x => x.FeeItem.FeeManagementId == duesManagementId && x.FeeClassificationId == classificationId).FirstOrDefault();
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
        public static bool UpdateClassification(FeeClassified fee)
        {
            try
            {
                var dc = new ManagementContext();
                var clas = (from xx in dc.FeeClassification
                            where xx.FeeItem.FeeManagementId == fee.DuesId
                            where xx.FeeClassificationId == fee.FeeClassificationId
                            select xx).FirstOrDefault();
                clas.Name = fee.Name;
                clas.FeeRequired = fee.FeeRequired;
                clas.DoesNotPayDues = fee.DoesNotPayDues;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool DeleteClassification(Guid duesManagementId, long classificationId)
        {
            try
            {
                var dc = new ManagementContext();
                var clas = (from xx in dc.FeeClassification.Include("MembersClassified")
                            where xx.FeeItem.FeeManagementId == duesManagementId
                            where xx.FeeClassificationId == classificationId
                            select xx).FirstOrDefault();
                var classificeds = clas.MembersClassified.ToList();
                for (int i = 0; i < classificeds.Count; i++)
                    dc.FeeClassificationByMember.Remove(classificeds[i]);

                dc.FeeClassification.Remove(clas);
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static FeeClassified PullClassification(Guid duesManagementId, long classificationId)
        {
            try
            {
                var dc = new ManagementContext();
                var clas = (from xx in dc.FeeClassification
                            where xx.FeeItem.FeeManagementId == duesManagementId
                            where xx.FeeClassificationId == classificationId
                            select new FeeClassified
                            {
                                DuesId = duesManagementId,
                                FeeClassificationId = xx.FeeClassificationId,
                                FeeRequired = xx.FeeRequired,

                                LeagueOwnerId = xx.FeeItem.LeagueOwner.LeagueId,
                                OwnerEntity = "league",
                                Name = xx.Name,
                                DoesNotPayDues = xx.DoesNotPayDues
                            }).FirstOrDefault();


                clas.FeeRequiredInput = clas.FeeRequired.ToString("N2");
                return clas;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static DuesPortableModel PullClassifications(Guid duesManagementId, Guid memId)
        {
            DuesPortableModel fClass = new DuesPortableModel();
            try
            {
                var dc = new ManagementContext();
                var clas = (from xx in dc.FeeManagement.Include("FeeClassifications.MembersClassified").Include("FeeClassifications.MembersClassified.Member")
                            where xx.FeeManagementId == duesManagementId
                            select xx).FirstOrDefault();

                fClass.LeagueOwnerId = clas.LeagueOwner.LeagueId;
                fClass.LeagueOwnerName = clas.LeagueOwner.Name;
                fClass.DuesId = clas.FeeManagementId;
                fClass.OwnerEntity = DuesOwnerEntityEnum.league.ToString();
                fClass.DuesCost = clas.FeeCostDefault;
                var members = MemberCache.GetLeagueMembers(memId, fClass.LeagueOwnerId);
                foreach (var m in members.OrderBy(x => x.DerbyName))
                {
                    MemberDisplayDues mem = new MemberDisplayDues();
                    mem.MemberId = m.MemberId;
                    mem.DerbyName = m.DerbyName;
                    mem.PlayerNumber = m.PlayerNumber;
                    mem.LastName = m.LastName;

                    var classification = clas.FeeClassifications.Where(x => x.MembersClassified.Where(y => y.Member.MemberId == mem.MemberId).Count() > 0).FirstOrDefault();
                    if (classification != null)
                        mem.ClassificationId = classification.FeeClassificationId;

                    fClass.Members.Add(mem);
                }
                foreach (var c in clas.FeeClassifications)
                {
                    FeeClassified fc = new FeeClassified();
                    fc.FeeClassificationId = c.FeeClassificationId;
                    fc.FeeRequired = c.FeeRequired;
                    fc.Name = c.Name;
                    fClass.Classifications.Add(fc);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return fClass;
        }
    }
}
