using System;
using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;

namespace Cozy_House_Generator.Scripts.Core.DataTypes
{
    /////////////////////////////////////////////////////////////////////////
    /// <summary>  "Contact" walls with additional information  </summary>
    ////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class ContactRoomWallsInfo
    {
        public int             originRoomId;
        public int             nearRoomId;
        public List<WallInfo>  walls = new List<WallInfo>();

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  If the current wall contacts a wall from an argument adds this wall to walls array  </summary>
        /// 
        /// <param name="wall">  Wall that will be checked  </param>
        /// 
        /// <returns>  Is the current wall contacts a wall from an argument  </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool TryToAdd (WallInfo wall)
        {
            if (wall.ownCell.RoomId != originRoomId || wall.nearCell.RoomId != nearRoomId) return false;
            
            if (!walls.Contains (wall))
                walls.Add (wall);
            
            return true;
        }

        public bool EqualsByRoomIds(ContactRoomWallsInfo compareTarget)
        {
            return originRoomId == compareTarget.originRoomId && nearRoomId == compareTarget.nearRoomId;
        }

        ///////////////////////////////////////////////////////////
        /// <summary>  Places doors to contact walls  </summary>
        //////////////////////////////////////////////////////////
        public void PlaceDoors()
        {
            walls[walls.Count / 2].wallType = WallType.InternalDoor;
        }
    }
}