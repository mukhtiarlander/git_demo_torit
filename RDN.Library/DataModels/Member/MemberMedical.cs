using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Member
{
    [Table("RDN_Member_Medical")]
    public class MemberMedical : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MedicalId { get; set; }

        public bool Epilepsy { get; set; }
        public bool Diabetes { get; set; }
        public bool HeartProblems { get; set; }
        public bool HeartMurmur { get; set; }
        public bool AsthmaBronchitis { get; set; }
        public bool Hernia { get; set; }
        public bool Concussion { get; set; }
        public bool WearGlasses { get; set; }
        public bool ContactLenses { get; set; }
        public int HardSoftLensesEnum { get; set; }
        public bool FractureInThreeYears { get; set; }
        public string FractureText { get; set; }
        public bool Dislocation { get; set; }
        public string DislocationText { get; set; }
        public bool ReoccurringPain { get; set; }
        public string ReoccurringPainText { get; set; }
        public bool BackNeckPain { get; set; }
        public bool TreatedForInjury { get; set; }
        public string DoAnyConditionsAffectPerformanceText { get; set; }
        public string RegularMedsText { get; set; }
        public string SportsInjuriesText { get; set; }
        public string AdditionalDetailsText { get; set; }


    }
}
