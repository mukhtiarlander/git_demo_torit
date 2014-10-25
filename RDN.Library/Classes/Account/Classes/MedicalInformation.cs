using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Member;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Account.Enums;

namespace RDN.Library.Classes.Account.Classes
{
    public class MedicalInformationFactory
    {


        public static void AttachMemberMedicalInformation(Member member, MemberDisplay mem)
        {
            if (member.MedicalInformation != null)
            {
                MedicalInformation meds = new MedicalInformation();
                meds.AdditionalDetailsText = member.MedicalInformation.AdditionalDetailsText;
                meds.AsthmaBronchitis = member.MedicalInformation.AsthmaBronchitis;
                meds.BackNeckPain = member.MedicalInformation.BackNeckPain;
                meds.Concussion = member.MedicalInformation.Concussion;
                meds.ContactLenses = member.MedicalInformation.ContactLenses;
                meds.HardSoftLensesEnum = (ContactLensesEnum)Enum.Parse(typeof(ContactLensesEnum), member.MedicalInformation.HardSoftLensesEnum.ToString());
                meds.Diabetes = member.MedicalInformation.Diabetes;
                meds.Dislocation = member.MedicalInformation.Dislocation;
                meds.LastModified = member.MedicalInformation.Created;
                if (member.MedicalInformation.LastModified != null)
                    meds.LastModified = member.MedicalInformation.LastModified.GetValueOrDefault();
                meds.MedicalId = member.MedicalInformation.MedicalId;
                meds.RegularMedsText = member.MedicalInformation.RegularMedsText;
                meds.ReoccurringPain = member.MedicalInformation.ReoccurringPain;
                meds.ReoccurringPainText = member.MedicalInformation.ReoccurringPainText;
                meds.SportsInjuriesText = member.MedicalInformation.SportsInjuriesText;
                meds.TreatedForInjury = member.MedicalInformation.TreatedForInjury;
                meds.WearGlasses = member.MedicalInformation.WearGlasses;
                meds.DislocationText = member.MedicalInformation.DislocationText;
                meds.DoAnyConditionsAffectPerformanceText = member.MedicalInformation.DoAnyConditionsAffectPerformanceText;
                meds.Epilepsy = member.MedicalInformation.Epilepsy;
                meds.FractureInThreeYears = member.MedicalInformation.FractureInThreeYears;
                meds.FractureText = member.MedicalInformation.FractureText;
                meds.HeartMurmur = member.MedicalInformation.HeartMurmur;
                meds.HeartProblems = member.MedicalInformation.HeartProblems;
                meds.Hernia = member.MedicalInformation.Hernia;
                mem.Medical = meds;
            }
            else
            {
                mem.Medical = new MedicalInformation();
            }
        }


    }


}
