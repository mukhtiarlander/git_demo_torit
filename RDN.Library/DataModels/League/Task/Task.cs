using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.League.Task
{
    [Table("RDN_League_Task")]
    public class Task : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TaskId { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DeadLine { get; set; }
        public int Completed { get; set; }
        public int StatusId { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }

        #region References
        public virtual RDN.Library.DataModels.League.Task.TaskList TaskBelongsTo { get; set; }
        public virtual RDN.Library.DataModels.Member.Member TaskAddByMember { get; set; }
        #endregion References
    }
}
