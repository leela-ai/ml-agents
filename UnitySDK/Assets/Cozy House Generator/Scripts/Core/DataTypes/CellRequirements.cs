using System;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;

namespace Cozy_House_Generator.Scripts.Core.DataTypes
{   ///////////////////////////////////////////////////////////
    /// <summary>  Defines which cell should be  </summary>
    ///////////////////////////////////////////////////////////
    [Serializable]
    public class CellRequirements
    {
        public CellStateForToggle state = CellStateForToggle.Any;
        
        public WallTypeEditor forwardWall;
        public WallTypeEditor backwardWall;
        public WallTypeEditor rightWall;
        public WallTypeEditor leftWall;
        public PropsType      propsThatCanBe;

        
        public CellRequirements(){}

        
        /////////////////////////////////////////////////////////////////////////////
        /// <summary>  Dublicating  </summary>
        /// 
        /// <param name="copySource">  An object which will be dublicated  </param>
        ////////////////////////////////////////////////////////////////////////////
        public CellRequirements(CellRequirements copySource)
        {
            state             = copySource.state;
            forwardWall       = copySource.forwardWall;
            backwardWall      = copySource.backwardWall;
            rightWall         = copySource.rightWall;
            leftWall          = copySource.leftWall;
            propsThatCanBe    = copySource.propsThatCanBe;
        }
        
        
        ////////////////////////////////////////////////////////////////////////////////////        
        /// <summary>  Returns true if cell info meets one of a possible case  </summary>
        /// 
        /// <param name="cell">    The cell for checking        </param>
        /// <param name="roomId">  An ID of the current room    </param>
        /// 
        /// <returns>  Is this cell applies requirements?  </returns>
        ///////////////////////////////////////////////////////////////////////////////////
        public bool IsApply(BlueprintCell cell, int roomId)
        {
            if (state == CellStateForToggle.Any)
                return true;
            
            if (cell == null || cell.RoomId != roomId || roomId < 0)
                return state == CellStateForToggle.OutsideOfTheRoom;

            
            if (IsWallOk(cell.ForwardWall.wallType, forwardWall) == false)
                return false;
            
            if (IsWallOk(cell.BackwardWall.wallType, backwardWall) == false)
                return false;
            
            if (IsWallOk(cell.RightWall.wallType, rightWall) == false)
                return false;
            
            if (IsWallOk(cell.LeftWall.wallType, leftWall) == false)
                return false;


            if (state == CellStateForToggle.OtherProps)
            {
                bool cellHasEqualProps = cell.propsToInstantiate != null &&
                                        (propsThatCanBe & cell.propsToInstantiate.propsType) ==
                                        cell.propsToInstantiate.propsType;

                bool cellHasEqualIntersectProps = cell.isPropsZone &&
                                                 (propsThatCanBe & cell.typeOfPropsOnPropsZone) ==
                                                 cell.typeOfPropsOnPropsZone;

                if (cellHasEqualProps == false && cellHasEqualIntersectProps == false)
                    return false;
            }
            return true;
        }

        
        /////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns true if wall applies requirements  </summary>
        /// 
        /// <param name="cellWall">       Wall for checking   </param>
        /// <param name="necessaryWall">  How must be a wall  </param>
        /// 
        /// <returns>  Is this wall applies requirements? </returns>
        ///////////////////////////////////////////////////////////////////////
        private bool IsWallOk(WallType cellWall, WallTypeEditor necessaryWall)
        {
            if (necessaryWall == WallTypeEditor.Any)
                return true;

            switch (cellWall)
            {
                case WallType.Void:
                    return necessaryWall == WallTypeEditor.Void;
                    
                case WallType.ExternalWall:
                case WallType.InternalWall:
                    return necessaryWall == WallTypeEditor.AnyWall      ||
                           necessaryWall == WallTypeEditor.Wall         ||
                           necessaryWall == WallTypeEditor.WallOrDoor   ||
                           necessaryWall == WallTypeEditor.WallOrWindow;
                
                case WallType.InternalDoor:
                case WallType.ExternalDoor:
                    return necessaryWall == WallTypeEditor.AnyWall    ||
                           necessaryWall == WallTypeEditor.Door       ||
                           necessaryWall == WallTypeEditor.WallOrDoor ||
                           necessaryWall == WallTypeEditor.DoorOrWindow;
                
                case WallType.Window:
                    return necessaryWall == WallTypeEditor.AnyWall      ||
                           necessaryWall == WallTypeEditor.Window       ||
                           necessaryWall == WallTypeEditor.WallOrWindow ||
                           necessaryWall == WallTypeEditor.DoorOrWindow;
            }

            return false;
        }

        
        /////////////////////////////////////////////////////////////////////
        /// <summary>  Rotating cell requirements scheme right  </summary>
        ////////////////////////////////////////////////////////////////////
        public void RotateRight()
        {
            var cache    = forwardWall;
            forwardWall  = leftWall;
            leftWall     = backwardWall;
            backwardWall = rightWall;
            rightWall    = cache;
        }
        
        ////////////////////////////////////////////////////////////////////
        /// <summary>  Rotating cell requirements scheme left  </summary>
        ///////////////////////////////////////////////////////////////////
        public void RotateLeft()
        {
            var cache    = forwardWall;
            forwardWall  = rightWall;
            rightWall    = backwardWall;
            backwardWall = leftWall;
            leftWall     = cache;
            
        }
    }
}