using System;
using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core.Pipes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>  The blueprint that will use to build a house  </summary>
    /////////////////////////////////////////////////////////////////////////
    public class Blueprint
    {
        private BlueprintFloor[] floors;
        public  readonly int     size;
        public  readonly int     floorsCount;
        
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Makes a blueprint that will use to build a house	</summary>
        /// 
        /// <param name="rnd">			         The standard .NET random that will use					  </param>
        /// <param name="pipeline">		         The pipeline that will process the blueprint			  </param>
        /// <param name="facadeColumnMaterial">  House facade column material                             </param>
        ///                                      don't apply requirements                                 </param>
        /// <param name="interiors">             list of interiors which used by pipeline process         </param>
        /// <param name="facadeMaterial">        House facade material (brick material for example)       </param>
        /// 
        /// <returns>  The blueprint  </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Blueprint(Random rnd, int size, int floorsCount, Pipeline pipeline, Interior[] interiors,   
                         Material facadeMaterial, Material facadeColumnMaterial)
                         
        {
            floors = new BlueprintFloor[floorsCount];
            for (int f = 0; f < floorsCount; f++)
                floors[f] = new BlueprintFloor(size, f, this);
            
            this.size = size;
            this.floorsCount = floorsCount;

            Generate(rnd, pipeline, interiors, facadeMaterial, facadeColumnMaterial);
        }


        private void Generate(Random   rnd,            Pipeline pipeline, Interior[] interiors,
                              Material facadeMaterial, Material facadeColumnMaterial)
        {

            for (int floor = 0; floor < floorsCount; floor++)
            {
                pipeline.ProcessFirstStep(this, floor, interiors, facadeMaterial, facadeColumnMaterial, rnd);
            }
            
            for (int floor = 0; floor < floorsCount; floor++)
            {
                pipeline.ProcessSecoundStep(this, floor, interiors, facadeMaterial, facadeColumnMaterial, rnd);
            }
        }

        
        public BlueprintCell GetCell(int floor, int x, int y)
        {
            if (x < 0 || y < 0 || x >= size || y >= size)
                return null;

            return floors[floor].GetCell(x, y);
        }
        
        
        public void SetCell(int floor, int x, int y, BlueprintCell cell)
        {
            if (x < 0 || y < 0 || x >= size || y >= size)
                return;

            floors[floor].SetCell(x, y, cell);
        }


        public int Size()
        {
            return size;
        }
    }
}