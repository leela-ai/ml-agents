using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    /////////////////////////////////////////////////////////////////
    /// <summary>  Finds room unevenness and fix them  </summary>
    ////////////////////////////////////////////////////////////////
    public class MakeRoomsSmoother : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Finds room unevenness and fix them  </summary>
        /// 
        /// <param name="blueprint">             The blueprint that will be modded                      </param>
        /// <param name="interiors">             A list of interiors which will be applied              </param>
        /// <param name="facadeColumnMaterial">  House facade column material                           </param>
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
                    
                    if (cell.RoomId >= 0)
                        continue;
                    var similarCell = FindSimilarCell(blueprint, floor, x, y);
                    if (similarCell != null)
                        cell.RoomId = similarCell.RoomId;
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
            return "Make Rooms Smoother";
        }
        
        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new MakeRoomsSmoother {useCount = count, floors = floors};
        }
        
        
        private static BlueprintCell FindSimilarCell(Blueprint blueprint, int floor, int x, int y)
        {
            var cell = blueprint.GetCell(floor, x, y);
            if (cell == null)
                return null;
            
            if (BlueprintAnalyze.IsSameRoom.ForwardBackward (floor, x, y, blueprint))
                return cell.neighbours.Forward(floor);

            if (BlueprintAnalyze.IsSameRoom.ForwardRight (floor, x, y, blueprint))
                return cell.neighbours.Forward(floor);

            if (BlueprintAnalyze.IsSameRoom.ForwardLeft (floor, x, y, blueprint))
                return cell.neighbours.Forward(floor);

            if (BlueprintAnalyze.IsSameRoom.BackwardRight (floor, x, y, blueprint))
                return cell.neighbours.Backward(floor);

            if (BlueprintAnalyze.IsSameRoom.BackwardLeft (floor, x, y, blueprint))
                return cell.neighbours.Backward(floor);

            if (BlueprintAnalyze.IsSameRoom.RightLeft (floor, x, y, blueprint))
                return cell.neighbours.Right(floor);

            return null;
        }
    }
}