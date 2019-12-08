using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    public static class PipeUseResolver
    {
        public static void Resolve(IGeneratorPipe pipe, Random rnd, int floor, Blueprint blueprint, Interior[] interiors, Material facadeMaterial, Material facadeColumnMaterial)
        {
            if (pipe.FloorsRangeData().CanUseOnFloor(floor,blueprint.floorsCount) == false)
                return;
            
            int useCount = pipe.UseCount();
            
            while (useCount > 0)
            {
                useCount--;
                pipe.Run(blueprint, floor, interiors, facadeMaterial, facadeColumnMaterial, rnd);
            }
        }

    }
}