using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Game.Enums
{
    public enum GameOwnerEnum
    {
        None = 0,
        //can do anything in the game
        Owner = 1,
        //can add and edit members in game.
        Manager = 2,
        //can upload old games
        HeadNso = 3
    }
}
