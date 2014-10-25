using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Models.User
{
    public class ViewMember : Library.Classes.Account.Classes.ViewMember
    {
        public List<SelectListItem> Teams { get; set; }
        public Guid SelectedTeamId { get; set; }
        
        public ViewMember()
        {
            Teams = new List<SelectListItem>();
        }
    }
}