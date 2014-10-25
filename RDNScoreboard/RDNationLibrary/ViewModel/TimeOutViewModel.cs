using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDNationLibrary.StopWatch;
using RDNationLibrary.Static.Enums;

namespace RDNationLibrary.ViewModel
{
  public   class TimeOutViewModel
    {
      public StopwatchWrapper TimeOutClock { get; set; }
      public TimeOutTypeEnum TimeOutType {get;set;}

      /// <summary>
      /// dummy constructor to export to xml
      /// </summary>
      public TimeOutViewModel()
      { }

      public TimeOutViewModel(StopwatchWrapper timeOutClock, TimeOutTypeEnum type)
      {
          TimeOutClock = timeOutClock;
          TimeOutType = type;
      }
    }
}
