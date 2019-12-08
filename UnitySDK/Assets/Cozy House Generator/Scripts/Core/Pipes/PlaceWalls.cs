using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  The object which finds the boundaries of the rooms and installs walls on them  </summary>
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PlaceWalls : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;

        
        //////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Finds the boundaries of the rooms and install walls on them  </summary>
        /// 
        /// <param name="blueprint">             The blueprint that will be modded                      </param>
        /// <param name="interiors">             A list of interiors which will be applied              </param>
        /// <param name="facadeColumnMaterial">  House facade material (brick material for example)     </param>
        /// <param name="facadeMaterial">        House facade material (brick material for example)     </param>
        /// <param name="rnd">                   .Net standard random object                            </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Run (Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial, 
                         Material facadeColumnMaterial, Random rnd)
        {
            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);
                    
                    bool isCellARoom     = cell.RoomId >= 0;
                    bool isCellACorridor = cell.RoomId < 0;

                    if (!isCellACorridor && !isCellARoom || !isCellARoom)
                        continue;

                    cell.RightWall.ownCell     = cell;
                    cell.LeftWall.ownCell      = cell;
                    cell.ForwardWall.ownCell   = cell;
                    cell.BackwardWall.ownCell  = cell;
                    
                    SetupWall(cell, cell.neighbours.Forward(floor),  cell.ForwardWall);
                    SetupWall(cell, cell.neighbours.Backward(floor), cell.BackwardWall);
                    SetupWall(cell, cell.neighbours.Right(floor),    cell.RightWall);
                    SetupWall(cell, cell.neighbours.Left(floor),     cell.LeftWall);
                }
            }
        }
        
        
        public int UseCount()
        {
            return useCount;
        }

        
        public UseOnFloorResolver FloorsRangeData()
        {
            return floors;
        }
        
        
        public string GetPipeName()
        {
            return "Place Walls";
        }
        

        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new PlaceWalls {useCount = count, floors = floors};
        }
        

        private void SetupWall(BlueprintCell cell, BlueprintCell nearCell, WallInfo wallInfo)
        {
            if (nearCell == null)
                wallInfo.wallType = WallType.ExternalWall;
            else
            {
                var nearCellType  = AnalyseCell(nearCell, cell);
                wallInfo.wallType = GetWallType(nearCellType);

                if (nearCellType == NearCellType.AnotherRoom)
                {
                    wallInfo.nearCell = nearCell;
                    PlaceDoors(wallInfo);
                }
            }
        }


        private static void PlaceDoors(WallInfo wall)
        {
            if (wall.internalDoorPlaced == false) //Place internal door
            {
                wall.internalDoorPlaced = true;
                wall.placeInternalDoor = true;

                switch (wall.wallDir)
                {
                    case SimpleDir.Forward:
                        wall.nearCell.BackwardWall.internalDoorPlaced = true;
                        break;
                    
                    case SimpleDir.Backward:
                        wall.nearCell.ForwardWall.internalDoorPlaced = true;
                        break;
                    
                    case SimpleDir.Right:
                        wall.nearCell.LeftWall.internalDoorPlaced = true;
                        break;
                    
                    case SimpleDir.Left:
                        wall.nearCell.RightWall.internalDoorPlaced = true;
                        break;
                }
            }
        }
        
        
        //////////////////////////////////////////////////////////////////////////////
        /// <summary>  Analyzes space around a cell to find boundaries  </summary>
        /// 
        /// <param name="cellForAnalyse">  A cell which will be analyzed  </param>
        /// <param name="originRoom">      Current cell  </param>
        /// 
        /// <returns>  Info about the cell  </returns>
        /////////////////////////////////////////////////////////////////////////////
        private static NearCellType AnalyseCell (BlueprintCell cellForAnalyse, BlueprintCell originRoom)
        {
            if (cellForAnalyse.RoomId < 0) return NearCellType.Void;
        
            var    roomForAnalyse         = cellForAnalyse;
            return roomForAnalyse.RoomId == originRoom.RoomId? NearCellType.SameRoom : NearCellType.AnotherRoom;

        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns a type of wall what must be on the current cell  </summary>
        /// 
        /// <param name="nearCellType">  Info about the cell which stays in front of the current cell  </param>
        /// 
        /// <returns>  Type of wall what must be  </returns>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static WallType GetWallType (NearCellType nearCellType)
        {
            switch (nearCellType)
            {
                case NearCellType.Void:
                    return WallType.ExternalWall;
            
                case NearCellType.SameRoom:
                    return WallType.Void;
            
                case NearCellType.AnotherRoom:
                    return WallType.InternalWall;
            }

            return WallType.Void;
        }

    }


    


    
}