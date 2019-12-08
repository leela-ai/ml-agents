using Cozy_House_Generator.Scripts.Core;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.House_Builders
{
    public class BuilderData
    {
        public readonly Blueprint     blueprint;
        public readonly GameObject    cellPrefab;
        public readonly Interior[]    interiors;
        public readonly Random        rnd;
        public readonly bool          hideCeiling;
        public readonly Transform     root;
        public readonly Material      facadeMaterial;
        public readonly GameObject    externalDoorPrefab;
        public readonly GameObject    internalDoorPrefab;
        public readonly GameObject    windowFramePrefab;
        public readonly int           lodCullingPercent;
        public readonly Material      decoratorSample1;
        public readonly Material      decoratorSample2;
        public readonly Material      decoratorSample3;
        

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  A data for house builder  </summary>
        /// 
        /// <param name="blueprint">             A house blueprint                                   </param>
        /// <param name="cellPrefab">            House Cell prefab                                   </param>
        /// <param name="interiors">             List of house interiors                             </param>
        /// <param name="rnd">                   .NET Random object for make some chaos              </param>
        /// <param name="hideCeiling">           Do you want to hide a ceiling?                      </param>
        /// <param name="root">                  House root(parent) GameObject                       </param>
        /// <param name="facadeMaterial">        House facade material (brick material for example)  </param>
        /// <param name="externalDoorPrefab">    External Door prefab                                </param>
        /// <param name="internalDoorPrefab">    Internal Door prefab                                </param>
        /// <param name="windowFramePrefab">     Window Frame Prefab                                 </param>
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        public BuilderData(Blueprint blueprint, GameObject cellPrefab, Interior[] interiors,    Random rnd, 
                           bool hideCeiling,    Transform root,        Material facadeMaterial, Material decoratorSample1, Material decoratorSample2, Material decoratorSample3,
                           GameObject externalDoorPrefab, GameObject internalDoorPrefab, GameObject windowFramePrefab, int lodCullingPercent)
        {
            this.blueprint          = blueprint;
            this.cellPrefab         = cellPrefab;
            this.interiors          = interiors;
            this.rnd                = rnd;
            this.hideCeiling        = hideCeiling;
            this.root               = root;
            this.facadeMaterial     = facadeMaterial;
            this.externalDoorPrefab = externalDoorPrefab;
            this.internalDoorPrefab = internalDoorPrefab;
            this.windowFramePrefab  = windowFramePrefab;
            this.lodCullingPercent  = lodCullingPercent;
            this.decoratorSample1   = decoratorSample1;
            this.decoratorSample2   = decoratorSample2;
            this.decoratorSample3   = decoratorSample3;
        }
    }
}