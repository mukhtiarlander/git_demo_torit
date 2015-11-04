using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Calendar;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RDN.Library.Classes.Account.Classes;
using ScheduleWidget.Enums;
using RDN.Library.Classes.Calendar.Enums;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Library.Classes.Controls.Calendar;

namespace RDN.League.Models.Calendar
{
    public class NewCalendarEvent : CalendarEvent
    {



        public SelectList Locations { get; set; }
        [Required]
        public string SelectedLocationId { get; set; }

        public Guid IdOfOwner { get; set; }

        public string SelectedMemberId { get; set; }

        //daily, monthly, weekly, yearly
        public SelectList RepeatsFrequencyDropDown { get; set; }
        public string RepeatsFrequencySelectedId { get; set; }

        public SelectList EventTypes { get; set; }
        public string SelectedEventTypeId { get; set; }

        ////repeats time interval.  1-50 weeks,daily, monthly
        //public SelectList RepeatEveryCountDropDown { get; set; }
        //public string RepeatEveryCountSelectedId { get; set; }
        /// <summary>
        /// select a day of the week when selecting a monthhly date to repeat
        /// </summary>
        public SelectList MonthlyInterval { get; set; }
        public string MonthlyIntervalId { get; set; }


       

        //ends on and after dates
        public string EndsWhen { get; set; }
        public string EndsDate { get; set; }
        public string EndsOccurences { get; set; }
        public Guid LeagueId { get; set; }
        public SelectList ColorList { get; set; }

        [AllowHtml]
        [Display(Name = "Notes: ")]
        public string Notes { get; set; }
        public string NotesHtml { get; set; }
        public NewCalendarEvent()
        {
        }
        public NewCalendarEvent(CalendarEvent calEvent)
        {
            if (calEvent != null)
            {
                this.SiteUrl = calEvent.SiteUrl;
                this.TicketUrl = calEvent.TicketUrl;
                this.AllowSelfCheckIn = calEvent.AllowSelfCheckIn;
                this.IsCurrentMemberApartOfEvent = calEvent.IsCurrentMemberApartOfEvent;
                this.EventType = calEvent.EventType;
                this.Attendees = calEvent.Attendees.OrderBy(x => x.DerbyName).ToList();
                this.IsPublicEvent = calEvent.IsPublicEvent;
                this.CalendarItemId = calEvent.CalendarItemId;
                this.CalendarId = calEvent.CalendarId;
                this.CalendarType = calEvent.CalendarType;
                this.EndDate = calEvent.EndDate;
                this.EndDateDisplay = calEvent.EndDate.ToShortDateString() + " " + calEvent.EndDate.ToShortTimeString();
                this.StartDate = calEvent.StartDate;
                this.StartDateDisplay = calEvent.StartDate.ToShortDateString() + " " + calEvent.StartDate.ToShortTimeString();
                this.Link = calEvent.Link;
                this.Location = calEvent.Location;
                this.Name = calEvent.Name;
                this.Notes = calEvent.Notes;
                this.NotesHtml = calEvent.NotesHtml;
                this.IsReoccurring = calEvent.IsReoccurring;
                this.CalendarReoccurringId = calEvent.CalendarReoccurringId;
                this.EndDateReoccurring = calEvent.EndDateReoccurring;
                this.EndDateReoccurringDisplay = calEvent.EndDateReoccurringDisplay;
                this.StartDateReoccurring = calEvent.StartDateReoccurring;
                this.StartDateReoccurringDisplay = calEvent.StartDateReoccurringDisplay;
                this.EventType.CalendarEventTypeId = calEvent.EventType.CalendarEventTypeId;
                this.NextEventId = calEvent.NextEventId;
                this.PreviousEventId = calEvent.PreviousEventId;
                this.GoogleCalendarUrl = calEvent.GoogleCalendarUrl;
                this.ColorTempSelected = calEvent.ColorTempSelected;
                this.MembersApartOfEvent = calEvent.MembersApartOfEvent;
                this.MembersToCheckIn = calEvent.MembersToCheckIn;
                this.GroupsForEvent = calEvent.GroupsForEvent;
                foreach (var group in calEvent.GroupsForEvent)
                {
                    this.ToGroupIds += group.Id + ",";
                }
                if (calEvent.EventReoccurring != null)
                {
                    this.IsReoccurring = true;
                    this.IsSunday = calEvent.EventReoccurring.DaysOfWeekOptions.HasFlag(DayOfWeekEnum.Sun);
                    this.IsMonday = calEvent.EventReoccurring.DaysOfWeekOptions.HasFlag(DayOfWeekEnum.Mon);
                    this.IsTuesday = calEvent.EventReoccurring.DaysOfWeekOptions.HasFlag(DayOfWeekEnum.Tue);
                    this.IsWednesday = calEvent.EventReoccurring.DaysOfWeekOptions.HasFlag(DayOfWeekEnum.Wed);
                    this.IsThursday = calEvent.EventReoccurring.DaysOfWeekOptions.HasFlag(DayOfWeekEnum.Thu);
                    this.IsFriday = calEvent.EventReoccurring.DaysOfWeekOptions.HasFlag(DayOfWeekEnum.Fri);
                    this.IsSaturday = calEvent.EventReoccurring.DaysOfWeekOptions.HasFlag(DayOfWeekEnum.Sat);
                    this.RepeatsFrequencySelectedId = calEvent.EventReoccurring.Frequency.ToString();
                    this.MonthlyIntervalId = calEvent.EventReoccurring.MonthlyInterval.ToString();
                    if (this.EndDateReoccurring.HasValue)
                    {
                        this.EndsWhen =RDN.Portable.Classes.Controls.Calendar.Enums. EndsWhenReoccuringEnum.On.ToString();
                    }
                    else
                        this.EndsWhen = RDN.Portable.Classes.Controls.Calendar.Enums.EndsWhenReoccuringEnum.Never.ToString();
                }
            }
        }
    }
}