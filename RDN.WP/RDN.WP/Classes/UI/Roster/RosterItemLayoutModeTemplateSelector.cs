using Microsoft.Phone.Controls;
using RDN.Portable.Classes.Account.Classes;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RDN.WP.Classes.UI.Roster
{
    public class RosterItemLayoutModeTemplateSelector : RosterDataTemplateSelector
    {
        public DataTemplate RosterTemplate
        {
            get;
            set;
        }

        public DataTemplate DatesTemplate
        {
            get;
            set;
        }
        public DataTemplate JobsTemplate
        {
            get;
            set;
        }
        public DataTemplate InsuranceNumbersTemplate
        {
            get;
            set;
        }
        public DataTemplate SkatingLevelTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
           var blah = (MemberDisplayAPI)item;

           

            //switch (blah.RosterTemplateEnum)
            //{
            //    case (int)RosterTemplateEnum.RosterTemplate:
            //        return RosterTemplate;
            //    case (int)RosterTemplateEnum.DatesTemplate:
            //        return DatesTemplate;
            //    case (int)RosterTemplateEnum.InsuranceNumbersTemplate:
            //        return InsuranceNumbersTemplate;
            //    case (int)RosterTemplateEnum.JobsTemplate:
            //        return JobsTemplate;
            //    case (int)RosterTemplateEnum.SkatingLevelTemplate:
            //        return SkatingLevelTemplate;
            //}

            return base.SelectTemplate(item, container);
        }
    }
}
