using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;

namespace RDN.Library.Classes.League.Classes
{
    public class LeagueMemberClass
    {
        public long IdOfClass { get; set; }
        public string NameOfClass { get; set; }

        public static List<LeagueMemberClass> GetAllMemberClasses()
        {
            try
            {
                var dc = new ManagementContext();
                var cl = (from xx in dc.LeagueMemberClasses
                          select new LeagueMemberClass
                          {
                              IdOfClass = xx.ClassId,
                              NameOfClass = xx.ClassName
                          }).OrderBy(x=>x.NameOfClass).ToList();
                return cl;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<LeagueMemberClass>();
        }

        public static bool CreateNewClass(string className)
        {
            try
            {
                var dc = new ManagementContext();
                RDN.Library.DataModels.League.LeagueMemberClass cla = new DataModels.League.LeagueMemberClass();
                cla.ClassName = className;
                dc.LeagueMemberClasses.Add(cla);
                int changes = dc.SaveChanges();
                if (changes > 0)
                    return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
    }


}
