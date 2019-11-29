using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{   
    ////////////////////////////////////////////////////////////////////////////
    /// <summary>  Finds and remove rooms with size equals 1 cell  </summary>
    ///////////////////////////////////////////////////////////////////////////
    public class RemoveTooSmallRooms : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Finds and remove rooms with size equals 1 cell  </summary>
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
            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);
                    
                    var forwardCell  = cell.neighbours.Forward(floor);
                    var backwardCell = cell.neighbours.Backward(floor);
                    var rightCell    = cell.neighbours.Right(floor);
                    var leftCell     = cell.neighbours.Left(floor);

                    bool isForwardRoom  = forwardCell   != null && forwardCell  .IsRoom();
                    bool isBackwardRoom = backwardCell  != null && backwardCell .IsRoom();
                    bool isRightRoom    = rightCell     != null && rightCell    .IsRoom();
                    bool isLeftRoom     = leftCell      != null && leftCell     .IsRoom();
                    
                    if (isForwardRoom || isBackwardRoom || isRightRoom || isLeftRoom)
                        continue;

                    cell.RoomId = -1;
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
            return "Remove Too Small Rooms";
        }
        
        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new RemoveTooSmallRooms {useCount = count, floors = floors};
        }
    }
}