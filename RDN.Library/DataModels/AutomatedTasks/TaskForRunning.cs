using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.AutomatedTasks
{
    /// <summary>
    /// used for both defining automated tasks and running them.
    /// </summary>
    [Table("RDN_Automated_Tasks")]
    public class TaskForRunning : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TaskId { get; set; }
        [Required]
        public DateTime LastRun { get; set; }
        [Required]
        public DateTime FirstRun { get; set; }
        [Required]
        public int HoursBetweenEachRunOfTask { get; set; }
        [Required]
        public int TaskIdForDescription { get; set; }
    }
}
