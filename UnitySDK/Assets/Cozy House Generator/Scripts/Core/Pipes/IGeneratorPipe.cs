using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    //////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  A part of the blueprint pipeline that can modify it by special rules  </summary>
    /////////////////////////////////////////////////////////////////////////////////////////////////
    public interface IGeneratorPipe
    {
        void Run (Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial, Material facadeColumnMaterial, Random rnd);
        
        string GetPipeName();
        
        IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors);
        
        int UseCount();
        
        UseOnFloorResolver FloorsRangeData();

    }
}
