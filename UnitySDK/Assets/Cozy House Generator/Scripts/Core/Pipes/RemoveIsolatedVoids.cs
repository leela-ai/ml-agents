using System.Linq;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{   ////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Finds and replace voids into rooms with size equals 1 cell  </summary>
    ///////////////////////////////////////////////////////////////////////////////////////
    public class RemoveIsolatedVoids : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////     
        /// <summary>  Finds and replace voids into rooms with size equals 1 cell  </summary>
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
                    if (cell.IsRoom() == false)
                        continue;

                    if (IsVoidStickOut(new []{cell.neighbours.Backward(floor), cell.neighbours.Right(floor),cell.neighbours.Left(floor)}) == false)
                        continue;

                    if (IsVoidStickOut(new []{cell.neighbours.Forward(floor), cell.neighbours.Right(floor),cell.neighbours.Left(floor)}) == false)
                        continue;

                    if (IsVoidStickOut(new []{cell.neighbours.Backward(floor), cell.neighbours.Forward(floor),cell.neighbours.Left(floor)}) == false)
                        continue;

                    if (IsVoidStickOut(new []{cell.neighbours.Backward(floor), cell.neighbours.Right(floor),cell.neighbours.Forward(floor)}) == false)
                        continue;

                    if (BlueprintAnalyze.IsSameRoom.RightLeft     (floor, x, y, blueprint)     ||
                        BlueprintAnalyze.IsSameRoom.BackwardRight (floor, x, y, blueprint)     ||
                        BlueprintAnalyze.IsSameRoom.ForwardRight  (floor, x, y, blueprint))

                        cell.RoomId = cell.neighbours.Right(floor).RoomId;

                    if (BlueprintAnalyze.IsSameRoom.BackwardLeft (floor, x, y, blueprint)   ||
                        BlueprintAnalyze.IsSameRoom.ForwardLeft  (floor, x, y, blueprint))
                        
                        cell.RoomId = cell.neighbours.Left(floor).RoomId;

                    if (BlueprintAnalyze.IsSameRoom.ForwardBackward (floor, x, y, blueprint))
                        cell.RoomId = cell.neighbours.Backward(floor).RoomId;

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
            return "Remove Isolated Voids";
        }
        
        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new RemoveIsolatedVoids {useCount = count, floors = floors};
        }
        

        public bool IsVoidStickOut(BlueprintCell[] neighbourCells)
        {
            return neighbourCells.All(neighbourCell => neighbourCell != null && !neighbourCell.IsRoom());
        }

    }
}