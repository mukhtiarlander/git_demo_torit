using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Game;
using Scoreboard.Library.StopWatch;
using Scoreboard.Library.ViewModel;

namespace RDN.Library.Classes.Game
{
  public   class GameClock
  {  /// <summary>
      /// gets the intermission clock for the game.
      /// </summary>
      /// <param name="id"></param>
      /// <param name="game"></param>
      /// <returns></returns>
      public  static GameStopwatch getIntermissionClock(Guid id, GameViewModel game)
      {
          try
          {
              ManagementContext db = new ManagementContext();

              var getClock = (from xx in db.GameStopWatch
                              where xx.StopwatchForId == id
                              where xx.Type == (int)StopWatchTypeEnum.IntermissionClock
                              select xx).FirstOrDefault();
              if (getClock != null)
              {
                  game.IntermissionClock = new StopwatchWrapper();
                  game.IntermissionClock.IsClockAtZero = getClock.IsClockAtZero == 1 ? true : false;
                  game.IntermissionClock.IsRunning = getClock.IsRunning == 1 ? true : false;
                  game.IntermissionClock.StartTime = getClock.StartDateTime;
                  game.IntermissionClock.TimeElapsed = getClock.TimeElapsed;
                  game.IntermissionClock.TimeRemaining = getClock.TimeRemaining;
                  game.IntermissionClock.TimerLength = getClock.Length;
              }
              return getClock;
          }
          catch (Exception exception)
          {
              ErrorDatabaseManager.AddException(exception, exception.GetType());
          }
          return null;
      }
      /// <summary>
      /// gets the current line up clock for the game.
      /// </summary>
      /// <param name="id"></param>
      /// <param name="game"></param>
      /// <returns></returns>
      public  static GameStopwatch getLineUpClock(Guid id, GameViewModel game)
      {
          try
          {
              ManagementContext db = new ManagementContext();
              var getClock = (from xx in db.GameStopWatch
                              where xx.StopwatchForId == id
                              where xx.Type == (int)StopWatchTypeEnum.LineUpClock
                              select xx).FirstOrDefault();
              if (getClock != null)
              {
                  game.CurrentLineUpClock = new StopwatchWrapper();
                  game.CurrentLineUpClock.IsClockAtZero = getClock.IsClockAtZero == 1 ? true : false;
                  game.CurrentLineUpClock.IsRunning = getClock.IsRunning == 1 ? true : false;
                  game.CurrentLineUpClock.StartTime = getClock.StartDateTime;
                  game.CurrentLineUpClock.TimeElapsed = getClock.TimeElapsed;
                  game.CurrentLineUpClock.TimeRemaining = getClock.TimeRemaining;
                  game.CurrentLineUpClock.TimerLength = getClock.Length;
              }
              return getClock;
          }
          catch (Exception exception)
          {
              ErrorDatabaseManager.AddException(exception, exception.GetType());
          }
          return null;
      }
      /// <summary>
      /// gets the current time out clock for the game.
      /// </summary>
      /// <param name="id"></param>
      /// <param name="game"></param>
      /// <returns></returns>
      public  static GameStopwatch getTimeOutClock(Guid id, GameViewModel game)
      {
          try
          {
              ManagementContext db = new ManagementContext();
              var getClock = (from xx in db.GameStopWatch
                              where xx.StopwatchForId == id
                              where xx.Type == (int)StopWatchTypeEnum.TimeOutClock
                              select xx).FirstOrDefault();
              if (getClock != null)
              {
                  game.CurrentTimeOutClock = new StopwatchWrapper();
                  game.CurrentTimeOutClock.IsClockAtZero = getClock.IsClockAtZero == 1 ? true : false;
                  game.CurrentTimeOutClock.IsRunning = getClock.IsRunning == 1 ? true : false;
                  game.CurrentTimeOutClock.StartTime = getClock.StartDateTime;
                  game.CurrentTimeOutClock.TimeElapsed = getClock.TimeElapsed;
                  game.CurrentTimeOutClock.TimeRemaining = getClock.TimeRemaining;
                  game.CurrentTimeOutClock.TimerLength = getClock.Length;
              }
              return getClock;
          }
          catch (Exception exception)
          {
              ErrorDatabaseManager.AddException(exception, exception.GetType());
          }
          return null;
      }
      /// <summary>
      /// gets the current period clock for the game.
      /// </summary>
      /// <param name="id"></param>
      /// <param name="game"></param>
      /// <returns></returns>
      public  static GameStopwatch getPeriodClock(Guid id, GameViewModel game)
      {
          ManagementContext db = new ManagementContext();
          var getPeriod = (from xx in db.GameStopWatch
                           where xx.StopwatchForId == id
                           where xx.Type == (int)StopWatchTypeEnum.PeriodClock
                           select xx).FirstOrDefault();
          if (getPeriod != null)
          {
              game.PeriodClock = new StopwatchWrapper();
              game.PeriodClock.IsClockAtZero = getPeriod.IsClockAtZero == 1 ? true : false;
              game.PeriodClock.IsRunning = getPeriod.IsRunning == 1 ? true : false;
              game.PeriodClock.StartTime = getPeriod.StartDateTime;
              game.PeriodClock.TimeElapsed = getPeriod.TimeElapsed;
              game.PeriodClock.TimeRemaining = getPeriod.TimeRemaining;
              game.PeriodClock.TimerLength = getPeriod.Length;
          }
          return getPeriod;
      }

    }
}
