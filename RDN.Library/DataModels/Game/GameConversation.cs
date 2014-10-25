using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game
{
    [Table("RDN_Game_Conversation")]
    public class GameConversation : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ConversationId { get; set; }

        public virtual string Text { get; set; }


        #region References
        public virtual Member.Member Owner { get; set; }
        [Required]
        public virtual Game Game { get; set; }
        #endregion
    }
}
