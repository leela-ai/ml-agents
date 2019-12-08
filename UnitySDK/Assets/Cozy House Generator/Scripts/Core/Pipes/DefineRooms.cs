using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    ////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Looks for rooms in the blueprint and writes them down.  </summary>
    /////////////////////////////////////////////////////////////////////////////////// 
    public class DefineRooms : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;
        private List<Vector2>      checkedCells;
        private int                roomCounter;

    
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Defines rooms  </summary>
        /// 
        /// <param name="blueprint">              The blueprint that will be modified                    </param>
        /// <param name="interiors">              A list of interiors which will be applied              </param>
        /// <param name="facadeColumnMaterial">   House facade column material                           </param>
        /// <param name="facadeMaterial">         House facade material (brick material for example)     </param>
        /// <param name="rnd">                    .Net standard random object                            </param>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Run (Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial, 
                         Material facadeColumnMaterial, Random rnd)
        {
            roomCounter  = 0;
            checkedCells = new List<Vector2>();
            
            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);
                    
                    if (cell.RoomId < 0) continue;
                    if (checkedCells.Contains(new Vector2(x, y))) continue;

                    cell.RoomId = roomCounter;

                    FindNextRoom(floor, x, y, blueprint, roomCounter, ref checkedCells); //recursive find next room
                    roomCounter++;
                }
            }
        }

        
        public string GetPipeName()
        {
            return "Define Rooms";
        }

        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new DefineRooms {useCount = count, floors = floors};
        }

        
        public int UseCount()
        {
            return useCount;
        }

        
        public UseOnFloorResolver FloorsRangeData()
        {
            return floors;
        }


        ///////////////////////////////////////////////////////////////////////////////
        /// <summary>  Starts finding next room from coordinates  </summary>
        /// 
        /// <param name="x">             Current x coordinate.                 </param> 
        /// <param name="y">             Current y coordinate.                 </param>
        /// <param name="blueprint">     The blueprint that will be modified.  </param>
        /// <param name="roomId">        The ID of the last found room.        </param>
        /// <param name="checkedCells">  Cells that already checked.           </param>
        ///////////////////////////////////////////////////////////////////////////////
        private void FindNextRoom (int floor, int x, int y, Blueprint blueprint, int roomId, ref List<Vector2> checkedCells)
        {
            if (!checkedCells.Contains (new Vector2 (x, y))) 
                checkedCells.Add (new Vector2 (x, y));

        
            CheckCellForRoom (floor, x + 1, y , blueprint, roomId, ref checkedCells); //right cell
            CheckCellForRoom (floor, x - 1, y , blueprint, roomId, ref checkedCells); //left cell
            CheckCellForRoom (floor, x, y - 1, blueprint, roomId, ref checkedCells); //up cell
            CheckCellForRoom (floor, x, y + 1, blueprint, roomId, ref checkedCells); //down cell
        }

    
        //////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Writes a cell as a room into blueprint if it's a room  </summary>
        /// 
        /// <param name="x">             Current x coordinate.                 </param>
        /// <param name="y">             Current y coordinate.                 </param>
        /// <param name="blueprint">     The blueprint that will be modified.  </param>
        /// <param name="roomId">        The ID of the last found room.        </param>
        /// <param name="checkedCells">  Cells that already checked.           </param>
        //////////////////////////////////////////////////////////////////////////////////
        private void CheckCellForRoom (int floor, int x, int y, Blueprint blueprint, int roomId, ref List<Vector2> checkedCells)
        {
            var cell = blueprint.GetCell(floor, x, y);
            
            if (checkedCells.Contains (new Vector2 (x, y))) //is already checked?
                return;       
        
            checkedCells.Add (new Vector2 (x, y)); //mark as checked
        
            if (cell == null || cell.RoomId != 0) //return if checking cell isn't exists or already processed
                return;
        
            cell.RoomId = roomId;
        
            FindNextRoom (floor, x, y, blueprint, roomId, ref checkedCells); //recursive find next room
        }
    }
}
