using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{   
    ///////////////////////////////////////////////////////////////////////
    /// <summary>  Finds thin room cells and make them wider  </summary>
    //////////////////////////////////////////////////////////////////////
    public class MakeThinRoomCellsWider : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Finds thin room cells and make them wider  </summary>
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
                    int rightRoomId = GetNearRoomId(cell.neighbours.Right(floor));
                    int leftRoomId  = GetNearRoomId(cell.neighbours.Left(floor));

                    
                    if (leftRoomId != cell.RoomId && rightRoomId != cell.RoomId)
                    {
                        cell.RoomId = leftRoomId;
                        continue;
                    }

                    int forwardRoomId = GetNearRoomId(cell.neighbours.Forward(floor));
                    int backRoomId    = GetNearRoomId(cell.neighbours.Backward(floor));
                    

                    if (backRoomId != cell.RoomId && forwardRoomId != cell.RoomId)
                        cell.RoomId = backRoomId;
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
            return "Make thin room cells wider";
        }
        

        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new MakeThinRoomCellsWider {useCount = count, floors = floors};
        }
        
        
        private int GetNearRoomId(BlueprintCell nearCell)
        {
            int nearRoomId = -1;
            if (nearCell != null)
                nearRoomId = nearCell.RoomId;
            
            return nearRoomId;
        }
    }
}