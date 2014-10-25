using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using RDN.Utilities.Error;
using Scoreboard.Library.ViewModel.Members.Officials;
using Scoreboard.Library.ViewModel.Officials.Enums;

namespace Scoreboard.Library.ViewModel.Officials
{
    public enum OfficialsEnum { NsoMember1, NsoMember2, NsoMember3, NsoMember4, NsoMember5, NsoMember6, NsoMember7, NsoMember8, NsoMember9, NsoMember10, RefereeMember1, RefereeMember2, RefereeMember3, RefereeMember4, RefereeMember5, RefereeMember6, RefereeMember7, RefereeMember8, RefereeMember9, RefereeMember10 }
    public class Officials : INotifyPropertyChanged
    {
        /// <summary>
        /// dummy constructor for XML export
        /// </summary>
        public Officials()
        {
            Referees = new ObservableCollection<RefereeMember>();
            Nsos = new ObservableCollection<NSOMember>();
        }

        private ObservableCollection<RefereeMember> _referees = new ObservableCollection<RefereeMember>();

        public ObservableCollection<RefereeMember> Referees
        {
            get { return _referees; }
            set { _referees = value; }
        }

        private ObservableCollection<NSOMember> _nsos = new ObservableCollection<NSOMember>();

        public ObservableCollection<NSOMember> Nsos
        {
            get { return _nsos; }
            set { _nsos = value; }
        }

        public void RemoveSkaterFromRefs(RefereeMember skater)
        {
            try
            {
                var member = this.Referees.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault();
                this.Referees.Remove(member);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        public void RemoveSkaterFromNsos(NSOMember skater)
        {
            try
            {
                var member = this.Nsos.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault();
                this.Nsos.Remove(member);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

        public void AddOfficial(string skaterName, RefereeTypeEnum position, string league, CertificationLevelEnum Cert)
        {
            try
            {
                RefereeMember teamMember = new RefereeMember();
                teamMember.SkaterId = Guid.NewGuid();
                teamMember.SkaterName = skaterName;
                teamMember.Cert = Cert;
                teamMember.League = league;
                teamMember.RefereeType = position;
                this.Referees.Add(teamMember);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }
        public void AddOfficial(string skaterName, NSOTypeEnum position, string league, CertificationLevelEnum cert)
        {
            try
            {
                NSOMember teamMember = new NSOMember();
                teamMember.SkaterId = Guid.NewGuid();
                teamMember.SkaterName = skaterName;
                teamMember.League = league;
                teamMember.Cert = cert;
                teamMember.RefereeType = position;
                this.Nsos.Add(teamMember);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI);
            }
        }

       

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
