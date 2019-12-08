using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    ////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Finds and removes rooms which are outside of the house  </summary>
    ///////////////////////////////////////////////////////////////////////////////////
    public class RemoveIsolatedRooms : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////     
        /// <summary>  Finds and removes rooms which are outside of the house  </summary>
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
                    blueprint.GetCell(floor, x, y).tempId = 1;
                }
            }

            bool isBreak = false;
            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);
                    if (cell.IsRoom())
                    {
                        cell.tempId = 2;
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak)
                    break;
            }
            
            
            while (true)
            {
                var coupleCell = GetNextCoupleCell(blueprint, floor);
                
                if (coupleCell == null)
                    break;

                var forwardCell  = coupleCell.neighbours.Forward(floor);
                var backwardCell = coupleCell.neighbours.Backward(floor);
                var rightCell    = coupleCell.neighbours.Right(floor);
                var leftCell     = coupleCell.neighbours.Left(floor);

                if (forwardCell != null && forwardCell.IsRoom() && forwardCell.tempId != 3)
                    forwardCell.tempId = 2;

                if (backwardCell != null && backwardCell.IsRoom() && backwardCell.tempId != 3)
                    backwardCell.tempId = 2;

                if (rightCell != null && rightCell.IsRoom() && rightCell.tempId != 3)
                    rightCell.tempId = 2;

                if (leftCell != null && leftCell.IsRoom() && leftCell.tempId != 3)
                    leftCell.tempId = 2;
                
            }

            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);
                    if (cell.tempId != 3)
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
            return "Remove Isolated Rooms";
        }
        
        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new RemoveIsolatedRooms {useCount = count, floors = floors};
        }


        private static BlueprintCell GetNextCoupleCell( Blueprint blueprint, int floor)
        {
            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);
                    if (cell.tempId == 2)
                    {
                        cell.tempId = 3;
                        return cell;
                    }
                }
            }

            return null;
        }

    }
}