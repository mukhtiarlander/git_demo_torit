using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game.Teams
{
    [Table("RDN_Game_Team_LineupSettings")]
    public class GameTeamLineupSettings : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LineUpSettingsId { get; set; }
        public byte LineUpTypeSelected { get; set; }
        public string PlainBorderColor { get; set; }
        public string PlainTextColor { get; set; }
        public string PlainBackgroundColor { get; set; }
        public string SidebarColor { get; set; }
        public string SidebarTextColor { get; set; }
        public string SidebarSkaterTextColor { get; set; }
        public string SidebarBackgroundColor { get; set; }

        public GameTeamLineupSettings()
        {

        }


        #region References

        [Required]
        public virtual GameTeam Team { get; set; }

        #endregion
    }
}
