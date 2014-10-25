using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Admin.Download
{
    [Table("RDN_Admin_Download_Scoreboard")]
    public class ScoreboardDownload : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScoreboardDownloadId { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
        [MaxLength(30)]
        public string IP { get; set; }        
        public string HttpRaw { get; set; }
        [MaxLength(20)]
        public string Version { get; set; }
    }
}
