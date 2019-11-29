using System;
using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    /////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Finds possible places for external windows and places them  </summary>
    ////////////////////////////////////////////////////////////////////////////////////////
    public class PlaceWindows : IGeneratorPipe
    {   
        private int            useCount;
        private UseOnFloorResolver floors;

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Finds possible places for external windows and places them  </summary>
        /// 
        /// <param name="blueprint">             The blueprint that will be modded                      </param>
        /// <param name="interiors">             A list of interiors which will be applied              </param>
        /// <param name="facadeColumnMaterial">  House facade material (brick material for example)     </param>
        /// <param name="facadeMaterial">        House facade material (brick material for example)     </param>
        /// <param name="rnd">                   .Net standard random object                            </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Run(Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial, 
                        Material facadeColumnMaterial, Random rnd)
        {
            var walls = GetWalls(blueprint, floor, WallType.ExternalWall);
            var rooms = GetExternalWallsInfo(walls, blueprint, floor);
            
            foreach (var room in rooms)
                MakeWindowsByRate(room.walls.ToArray(), room.windowsRate);
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
            return "Place Windows";
        }

        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new PlaceWindows {useCount = count, floors = floors};
        }

        
        //////////////////////////////////////////////////////////////////////
        /// <summary>  Places windows with spacing between them  </summary>
        /// 
        /// <param name="walls">  walls that will be modificated   </param>
        /// <param name="rate">   Space between windows            </param>
        /////////////////////////////////////////////////////////////////////
        private void MakeWindowsByRate(WallInfo[] walls, int rate)
        {
            if (rate < 1)
                return;

            foreach (var wall in walls)
            {
                if ((wall.wallDir == SimpleDir.Forward || wall.wallDir == SimpleDir.Backward)  &&
                     wall.ownCell.x % rate == 0                                                && 
                     wall.wallType == WallType.ExternalWall)
                {
                    wall.wallType = WallType.Window;
                }
                else if ((wall.wallDir == SimpleDir.Left || wall.wallDir == SimpleDir.Right) &&
                          wall.ownCell.y % rate == 0                                         && 
                          wall.wallType == WallType.ExternalWall)
                {
                    wall.wallType = WallType.Window;
                }
            }
        }

        
        ////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Gets array what contains info about all external walls  </summary>
        /// 
        /// <param name="walls">  Current walls  </param>
        ///////////////////////////////////////////////////////////////////////////////////
        private RoomExternalWallsInfo[] GetExternalWallsInfo(IEnumerable<WallInfo> walls, Blueprint blueprint, int floor)
        {
            var roomsWithWallsInfo = new List<RoomExternalWallsInfo>();
            
            foreach (var wall in walls)
            {
                var cell = blueprint.GetCell(floor, wall.ownCell.x, wall.ownCell.y);
                var roomWallsInfo = FindRoomExternalWallsInfoByRoomID(roomsWithWallsInfo, cell.RoomId);
                
                if (roomWallsInfo != null)
                    roomWallsInfo.walls.Add(wall);
                else
                {
                    var roomExtWallsInf = new RoomExternalWallsInfo(cell.RoomId, cell.windowsRate);
                    roomExtWallsInf.walls.Add(wall);
                    roomsWithWallsInfo.Add(roomExtWallsInf);
                }
            }
            
            return roomsWithWallsInfo.ToArray();
        }

        
        ///////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Gets walls which belong a room  </summary>
        /// 
        /// <param name="roomWallsInfo">   Info about all external walls                    </param>
        /// <param name="roomId">          The id of the room which walls we want to find   </param>
        /// 
        /// <returns>  Info about walls in the room  </returns>
        /////////////////////////////////////////////////////////////////////////////////////////////
        private RoomExternalWallsInfo FindRoomExternalWallsInfoByRoomID(List<RoomExternalWallsInfo> roomWallsInfo, int roomId)
        {
            foreach (var wallsInfo in roomWallsInfo)
                if (wallsInfo.roomId == roomId)
                    return wallsInfo;

            return null;
        }
        
        
        /////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Gets info about walls in the house   </summary>
        /// 
        /// <param name="blueprint">  The blueprint in that we will inspect walls     </param>
        /// <param name="wallType">   Type of walls what we want to find and inspect  </param>
        /// 
        /// <returns> Info about walls </returns>
        ////////////////////////////////////////////////////////////////////////////////////////
        private static IEnumerable<WallInfo> GetWalls(Blueprint blueprint, int floor, WallType wallType)
        {
            var walls = new List<WallInfo>();

            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    {
                        var cell = blueprint.GetCell(floor, x, y);

                        if (cell.ForwardWall.wallType == wallType)
                            walls.Add(cell.ForwardWall);

                        if (cell.RightWall.wallType == wallType)
                            walls.Add(cell.RightWall);

                        if (cell.BackwardWall.wallType == wallType)
                            walls.Add(cell.BackwardWall);

                        if (cell.LeftWall.wallType == wallType)
                            walls.Add(cell.LeftWall);
                    }
                }
            }

            return walls.ToArray();
        }
        
        /////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Gets info about walls in the house   </summary>
        /// 
        /// <param name="blueprint">  The blueprint in that we will inspect walls     </param>
        /// <param name="wallType">   Type of walls what we want to find and inspect  </param>
        /// 
        /// <returns> Info about walls </returns>
        ////////////////////////////////////////////////////////////////////////////////////////
        private static IEnumerable<WallInfo> GetWalls(Blueprint blueprint, int floor, WallType wallType, SimpleDir dir)
        {
            var walls = new List<WallInfo>();

            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    {
                        var cell = blueprint.GetCell(floor, x, y);

                        switch (dir)
                        {
                            case SimpleDir.Forward:
                                walls.Add(cell.ForwardWall);
                                break;
                            case SimpleDir.Backward:
                                walls.Add(cell.BackwardWall);
                                break;
                            case SimpleDir.Right:
                                walls.Add(cell.RightWall);
                                break;
                            case SimpleDir.Left:
                                walls.Add(cell.LeftWall);
                                break;
                        }
                    }
                }
            }

            return walls.ToArray();
        }
        
        
    }
}