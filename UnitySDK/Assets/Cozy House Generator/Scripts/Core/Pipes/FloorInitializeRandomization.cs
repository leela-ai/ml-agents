using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    public class FloorInitializeRandomization : IGeneratorPipe
    {
        private UseOnFloorResolver floors;
        
        
        public void Run(Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial, Material facadeColumnMaterial,
                        Random    rnd)
        {
            bool[,] rndResult = Convert.Bool1D_to_Bool2D (RND.Next (rnd, blueprint.size,136));

            for (int x = 0; x < blueprint.size; x++)
                for (int y = 0; y < blueprint.size; y++)
                    blueprint.GetCell(floor, x, y).RoomId = rndResult[x, y] ? 0 : -1;
        }

        
        public string GetPipeName()
        {
            return "Floor Initialize Randomization";
        }

        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new FloorInitializeRandomization { floors = floors };
        }

        
        public int UseCount()
        {
            return 1;
        }

        
        public UseOnFloorResolver FloorsRangeData()
        {
            return floors;
        }
    }
}