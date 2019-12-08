using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    //////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Finds possible places for external Doors and places them  </summary>
    /////////////////////////////////////////////////////////////////////////////////////
    public class PlaceExternalDoors : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Finds possible places for external Doors and places them  </summary>
        /// 
        /// <param name="blueprint">             The blueprint that will be modded                      </param>
        /// <param name="interiors">             A list of interiors which will be applied              </param>
        /// <param name="facadeColumnMaterial">  House facade material (brick material for example)     </param>
        /// <param name="facadeMaterial">        House facade material (brick material for example)     </param>
        /// <param name="rnd">                   .Net standard random object                            </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Run (Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial, Material facadeColumnMaterial, 
                         Random rnd)
        {
            if (floor > 0)
                return;
            
            bool breakLoop = false;
            for (int x = 0; x < blueprint.size; x++)
            {
                if (breakLoop)
                    break;
                
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);
                    
                    
                    if (cell.ForwardWall.wallType == WallType.ExternalWall)
                    {
                        cell.ForwardWall.wallType = WallType.ExternalDoor;
                        breakLoop = true;
                        break;
                    }
                    if (cell.BackwardWall.wallType == WallType.ExternalWall)
                    {
                        cell.BackwardWall.wallType = WallType.ExternalDoor;
                        breakLoop = true;
                        break;
                    }
                    if (cell.RightWall.wallType == WallType.ExternalWall)
                    {
                        cell.RightWall.wallType = WallType.ExternalDoor;
                        breakLoop = true;
                        break;
                    }
                    if (cell.LeftWall.wallType == WallType.ExternalWall)
                    {
                        cell.LeftWall.wallType = WallType.ExternalDoor;
                        breakLoop = true;
                        break;
                    }
                }
            }
        }

        
        public string GetPipeName()
        {
            return "Place External Doors";
        }
        
        
        public int UseCount()
        {
            return useCount;
        }
        

        public UseOnFloorResolver FloorsRangeData()
        {
            return floors;
        }
        
        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new PlaceExternalDoors { useCount = count, floors = floors };
        }
    }
}