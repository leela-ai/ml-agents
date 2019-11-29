using System.Collections.Generic;
using System.Linq;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{   
    ///////////////////////////////////////////////////////////////////////////////////
    /// <summary> Finds possible places for internal doors and place them </summary>
    //////////////////////////////////////////////////////////////////////////////////
    public class PlaceInternalDoors : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;

        private ContactRoomWallsInfo[]   wallsBetweenRooms;

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Finds possible places for internal doors and place them  </summary>
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
            var linkedRoomWalls = new List<ContactRoomWallsInfo>();

            //Get walls that separate rooms by vertical and set doors//
            wallsBetweenRooms = GetWallsSortedByContact (GetInternalWallsVertical (blueprint, floor));
            if (wallsBetweenRooms.Length == 0) 
                return;
            
            foreach (var contactRoomWallsInfo in wallsBetweenRooms)
            {
                if (linkedRoomWalls.Any(linkedRoomWall => linkedRoomWall.EqualsByRoomIds(contactRoomWallsInfo))) continue;
                contactRoomWallsInfo.PlaceDoors();
                linkedRoomWalls.Add(contactRoomWallsInfo);
            }
            //------------------------------------------------------//
                
            //Get walls that separate rooms by horizontal and set doors//
            wallsBetweenRooms = GetWallsSortedByContact (GetInternalWallsHorizontal (blueprint, floor));
            if (wallsBetweenRooms.Length == 0) 
                return;
            
            foreach (var contactRoomWallsInfo in wallsBetweenRooms)
            {
                if (linkedRoomWalls.Any(linkedRoomWall => linkedRoomWall.EqualsByRoomIds(contactRoomWallsInfo))) continue;
                contactRoomWallsInfo.PlaceDoors();
                linkedRoomWalls.Add(contactRoomWallsInfo);
            }
                //contactRoomWallsInfo.PlaceDoors();
            //--------------------------------------------------------//
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
            return "Place Internal Doors";
        }

        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new PlaceInternalDoors {useCount = count, floors = floors};
        }
        

        ///////////////////////////////////////////////////////////////////////////////
        /// <summary>  Gets all vertical internal walls (for all rooms)  </summary>
        /// 
        /// <param name="blueprint">  The blueprint that will be modified  </param>
        /// 
        /// <returns>  All vertical internal walls  </returns>
        /////////////////////////////////////////////////////////////////////////////
        private static IEnumerable<WallInfo> GetInternalWallsVertical (Blueprint blueprint, int floor)
        {
            var walls = new List<WallInfo>();

            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);
                    
                    if (cell.BackwardWall.wallType == WallType.InternalWall)
                        walls.Add(cell.BackwardWall);

                    if (cell.ForwardWall.wallType == WallType.InternalWall)
                        walls.Add(cell.ForwardWall);
                }
            }

            return walls.ToArray();
        }
        
        
        ///////////////////////////////////////////////////////////////////////////////
        /// <summary>  Gets all horizontal internal walls (for all rooms)  </summary>
        /// 
        /// <param name="blueprint">  The blueprint that will be modified  </param>
        /// 
        /// <returns>  All horizontal internal walls  </returns>
        //////////////////////////////////////////////////////////////////////////////
        private static IEnumerable<WallInfo> GetInternalWallsHorizontal (Blueprint blueprint, int floor)
        {
            var walls = new List<WallInfo>();

            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);
                    
                    if (cell.RightWall.wallType == WallType.InternalWall)
                        walls.Add(cell.RightWall);

                    if (cell.LeftWall.wallType == WallType.InternalWall)
                        walls.Add(cell.LeftWall);
                }
            }

            return walls.ToArray();
        }


        //////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Merges two sides of the wall into one "contact" wall  </summary>
        /// 
        /// <param name="walls">  Walls for process  </param>
        /// 
        /// <returns>  "Contact" walls  </returns>
        /////////////////////////////////////////////////////////////////////////////////
        private static ContactRoomWallsInfo[] GetWallsSortedByContact (IEnumerable<WallInfo> walls)
        {
            var contactWalls = new List<ContactRoomWallsInfo>();
            
            foreach (var wall in walls)
            {
                bool isAdded = false;

                if (contactWalls.Count > 0)
                    foreach (var wallBetweenRooms in contactWalls)
                        isAdded = isAdded || wallBetweenRooms.TryToAdd (wall);

                if (!isAdded)
                {
                    var newRoomWallsInfo = new ContactRoomWallsInfo
                    {
                        nearRoomId = wall.nearCell.RoomId,
                        originRoomId = wall.ownCell.RoomId
                    };
                    newRoomWallsInfo.walls.Add (wall);
                    contactWalls.Add (newRoomWallsInfo);
                }
            }
            return contactWalls.ToArray();
        }

    }

    
    
}