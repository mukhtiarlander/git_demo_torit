using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.League.Task
{
    [Table("RDN_League_TaskList")]
    public class TaskList : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ListId { get; set; }
        public string ListName { get; set; }
        public DateTime? EndDate { get; set; }
        public string AssignedTo { get; set; }
        public bool IsDeleted { get; set; }

        #region References
        public virtual RDN.Library.DataModels.League.League TaskListForLeague { get; set; }
        public virtual RDN.Library.DataModels.Member.Member ListAddByMember { get; set; }
        public virtual ICollection<Task> TaskInList { get; set; }
        #endregion References
    }
}
