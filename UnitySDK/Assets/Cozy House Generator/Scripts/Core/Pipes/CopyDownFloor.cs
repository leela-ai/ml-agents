using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    public class CopyDownFloor : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;
        
        
        public void Run(Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial,
                        Material  facadeColumnMaterial, Random rnd)
        {
            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var downCell = blueprint.GetCell(floor - 1, x, y);
                    if (downCell == null)
                        continue;
                    
                    blueprint.SetCell(floor, x, y, new BlueprintCell(downCell));
                }
            }
        }

        
        public string GetPipeName()
        {
            return "Copy Down Floor";
        }

        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new CopyDownFloor {useCount = count, floors = floors};
        }

        
        public int UseCount()
        {
            return useCount;
        }

        
        public UseOnFloorResolver FloorsRangeData()
        {
            return floors;
        }
    }
}