using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;

namespace Cozy_House_Generator.Scripts.Core.DataTypes
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Contains info about a wall in blueprint room cell  </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class WallInfo
    {
        public WallType          wallType;
        public SimpleDir         wallDir;
        public BlueprintCell     ownCell;
        public BlueprintCell     nearCell;
        public bool              placeInternalDoor;
        public bool              internalDoorPlaced;


        ///////////////////////////////////////////////////////////
        /// <param name="wallType">  Type of wall       </param>
        /// <param name="wallDir">   Direction of wall  </param>
        ///////////////////////////////////////////////////////////
        public WallInfo(WallType wallType, SimpleDir wallDir)
        {
            this.wallType = wallType;
            this.wallDir  = wallDir;
        }


        public WallInfo(WallInfo copySource)
        {
            wallType              = copySource.wallType;
            wallDir               = copySource.wallDir;
            ownCell               = copySource.ownCell;
            nearCell              = copySource.nearCell;
            placeInternalDoor     = copySource.placeInternalDoor;
            internalDoorPlaced    = copySource.internalDoorPlaced;
        }
    
    
        //////////////////////////////////////////////////////////
        /// <param name="wallDir">  Direction of wall  </param>
        //////////////////////////////////////////////////////////
        public WallInfo(SimpleDir wallDir)
        {
            this.wallDir = wallDir;
        }
    }
}