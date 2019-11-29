using System.Collections.Generic;

namespace Cozy_House_Generator.Scripts.Core.DataTypes
{
    ////////////////////////////////////////////////////////////////////
    /// <summary> Info about an external walls of the room </summary>
    ///////////////////////////////////////////////////////////////////
    public class RoomExternalWallsInfo
    {
        public int             roomId;
        public int             windowsRate;
        public List<WallInfo>  walls;

            
        public RoomExternalWallsInfo(int roomId, int windowsRate)
        {
            this.roomId      = roomId;
            this.windowsRate = windowsRate;
            walls            = new List<WallInfo>();
        }
    }
}