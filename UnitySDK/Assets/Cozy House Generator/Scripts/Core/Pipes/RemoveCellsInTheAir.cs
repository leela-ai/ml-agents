using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    public class RemoveCellsInTheAir : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;

        
        public void Run(Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial,
                        Material  facadeColumnMaterial,
                        Random rnd)
        {
            if (floor < 1)
                return;
            
            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);
                    if (blueprint.GetCell(floor -1, x, y).IsRoom())
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
            return "Remove Cells In The Air";
        }
        
        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new RemoveCellsInTheAir { useCount = count, floors = floors };
        }
    }
}