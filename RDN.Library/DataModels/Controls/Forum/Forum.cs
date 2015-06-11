using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Forum
{
    [Table("RDN_Forum")]
    public class Forum : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ForumId { get; set; }
        public string ForumName { get; set; }
        /// <summary>
        /// braodcase forum message to everyone
        /// </summary>
        public bool BroadcastToEveryone { get; set; }

        #region References
        public virtual ICollection<ForumTopic> Topics { get; set; }
        public virtual ICollection<ForumCategories> Categories { get; set; }
        public virtual Federation.Federation FederationOwner { get; set; }
        public virtual League.League LeagueOwner { get; set; }
      
        #endregion

        #region Contructor

        public Forum()
        {
            Topics = new Collection<ForumTopic>();
            Categories = new Collection<ForumCategories>();
        }

        #endregion
    }
}
