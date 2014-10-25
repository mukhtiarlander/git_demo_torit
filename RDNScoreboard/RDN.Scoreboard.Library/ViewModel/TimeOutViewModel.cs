using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.StopWatch;
using Scoreboard.Library.Static.Enums;

namespace Scoreboard.Library.ViewModel
{
  public   class TimeOutViewModel
    {
      public StopwatchWrapper TimeOutClock { get; set; }
      public TimeOutTypeEnum TimeOutType {get;set;}
      public Guid TimeoutId { get; set; }
      /// <summary>
      /// dummy constructor to export to xml
      /// </summary>
      public TimeOutViewModel()
      { }

      public TimeOutViewModel(StopwatchWrapper timeOutClock, TimeOutTypeEnum type)
      {
          TimeOutClock = timeOutClock;
          TimeOutType = type;
          TimeoutId = Guid.NewGuid();
      }
    }
}
