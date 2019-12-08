using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{   
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Analyzes rooms by special rules to finding a right interior and place it  </summary>
    /////////////////////////////////////////////////////////////////////////////////////////////////////
    public class SetInteriors : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Analyzes rooms by special rules to finding a right interior and place it  </summary>
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
            var usedInteriors = new List<int>();
            var roomsInfo     = GetInfoAboutRooms(blueprint, floor);
            
            foreach (var roomInfo in roomsInfo)
            {
                if (roomInfo.roomId < 0) continue;
                

                int[] suitableInteriors =
                    FindSuitableInteriors(interiors, usedInteriors, roomInfo.countOfDoors, roomInfo.size);

                if (suitableInteriors.Length == 0)
                {
                    continue;
                }

                int interiorId = suitableInteriors[rnd.Next(0, suitableInteriors.Length)];
                
                foreach (var cellRoom in roomInfo.cells)
                {
                    cellRoom.interiorId  = interiorId;
                    cellRoom.windowsRate = interiors[interiorId].windowsRate;}

                if (interiorId > 0 && interiors[interiorId].placeOnlyOnce)
                    usedInteriors.Add(interiorId);
                
            }
        }

        
        public string GetPipeName()
        {
            return "Set Interiors";
        }
        
        
        public int UseCount()
        {
            return useCount;
        }
        
        
        public UseOnFloorResolver FloorsRangeData()
        {
            return floors;
        }

        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new SetInteriors {useCount = count, floors = floors};
        }
        
        
        private static int[] FindSuitableInteriors(Interior[] interiors, ICollection<int> exceptIds, int countOfDoors, 
            int roomSize)
        {
            var result = new List<int>();
            
            for (int i = 1; i < interiors.Length; i++)
            {
                if (!exceptIds.Contains(i) && interiors[i].IsGoodEnough(roomSize, countOfDoors))
                {
                    result.Add(i);
                }
            }

            return result.ToArray();
        }
        

        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Inspects the blueprint to get an array with info about all rooms  </summary>
        /// 
        /// <param name="blueprint">  The blueprint what you want to inspect  </param>
        /// 
        /// <returns>  Info about all rooms  </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public static IEnumerable<RoomInfo> GetInfoAboutRooms(Blueprint blueprint, int floor)
        {
            var roomsInfo = new List<RoomInfo>();

            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);

                    var roomInfo = GetRoomInfoByRoomId(roomsInfo, cell.RoomId);
                    roomInfo.size++;
                    roomInfo.cells.Add(cell);

                    if (cell.ForwardWall.wallType == WallType.InternalDoor ||
                        cell.ForwardWall.wallType == WallType.ExternalDoor)
                        roomInfo.countOfDoors++;

                    if (cell.BackwardWall.wallType == WallType.InternalDoor ||
                        cell.BackwardWall.wallType == WallType.ExternalDoor)
                        roomInfo.countOfDoors++;

                    if (cell.RightWall.wallType == WallType.InternalDoor ||
                        cell.RightWall.wallType == WallType.ExternalDoor)
                        roomInfo.countOfDoors++;

                    if (cell.LeftWall.wallType == WallType.InternalDoor ||
                        cell.LeftWall.wallType == WallType.ExternalDoor)
                        roomInfo.countOfDoors++;
                }
            }

            return roomsInfo.ToArray();
        }

        
        //////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Finds info about a room into information array by ID  </summary>
        /// 
        /// <param name="roomsInfo">  The array with information about rooms          </param>
        /// <param name="roomId">     The id of the room what you want to inspect     </param>
        /// 
        /// <returns>  Info about a room  </returns>
        /////////////////////////////////////////////////////////////////////////////////////////
        private static RoomInfo GetRoomInfoByRoomId(ICollection<RoomInfo> roomsInfo, int roomId)
        {
            foreach (var roomInfo in roomsInfo)
                if (roomInfo.roomId == roomId)
                    return roomInfo;

            var newRoomInfo = new RoomInfo {roomId = roomId};
            roomsInfo.Add(newRoomInfo);
            return newRoomInfo;
        }
    }
}