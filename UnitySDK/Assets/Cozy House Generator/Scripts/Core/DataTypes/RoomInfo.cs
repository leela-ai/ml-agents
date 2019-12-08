using System.Collections.Generic;

namespace Cozy_House_Generator.Scripts.Core.DataTypes
{
    ///////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  An information about a room which usually uses for interior setts  </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////// 
    public class RoomInfo
    {
        public int                          roomId;
        public int                          size;
        public int                          countOfDoors;
        public readonly List<BlueprintCell> cells = new List<BlueprintCell>();
    }
}