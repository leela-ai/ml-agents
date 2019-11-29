using System;
using System.Collections.Generic;
using System.Linq;
using Cozy_House_Generator.Scripts.Core;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.Core.Pipes;
using Cozy_House_Generator.Scripts.House_Builders;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;


namespace Cozy_House_Generator.Scripts
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  The object that builds a house from a procedurally generated blueprint  </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class HouseGenerator : MonoBehaviour
    {
    
        public int                    seed;
        public BuilderType            builderType = BuilderType.RawBuilder;
        public GameObject             cellPrefab;
        public GameObject             internalDoorPrefab;
        public GameObject             externalDoorPrefab;
        public GameObject             windowFramePrefab;
        public int                    lodCullingPercent = 10;
        public int                    size = 8;
        public int                    floorsCount = 1;
        public bool                   hideCeiling = true;
        public Material               facadeMaterial;
        public Material               facadeColumnMaterial;
        public Material               decoratorSample1;
        public Material               decoratorSample2;
        public Material               decoratorSample3;
        public List<Interior>         interiors             = new List<Interior>();
        public PipelinePreset         pipelinePreset;
        
        private MeshCombiner  meshCombiner;
        private GameObject    combinedRoot;
        private GameObject    combinedWalls;
        private GameObject    combinedFloors;
        private GameObject    combinedCeilings;
        private GameObject    combinedColumns;
        private GameObject    combinedAll;
        private IHouseBuilder builder;



        ////////////////////////////////////////////////////////////////////////
        /// <summary>  Generates and builds a house from the seed  </summary>
        ///////////////////////////////////////////////////////////////////////
        public void Generate()
        {
            if (IsEverythingOk() == false)
                return;

            var rnd       = new Random(seed);
            var pipeline  = pipelinePreset.GetPipeline();
            var blueprint = new Blueprint(rnd, size, floorsCount, pipeline, interiors.ToArray(), facadeMaterial,
                                          facadeColumnMaterial);
            
            //fix rotation issues-------------------------------------------------------------------------------------//
            var currentRot       = transform.rotation;
            transform.rotation   = Quaternion.Euler(Vector3.zero);
            //--------------------------------------------------------------------------------------------------------//      
            
            Build (blueprint, rnd, interiors.ToArray(), builderType);
            
            //end fix-------------------------------------------------------------------------------------------------//
            transform.rotation = currentRot;
            //--------------------------------------------------------------------------------------------------------//
        }

 
        private bool IsEverythingOk()
        {
            bool isOk = true;
            
            if (pipelinePreset == null)
            {
                Debug.LogWarning(name + ".House Generator: PIPELINE IS EMPTY");
                isOk = false;
            }
            
            if (cellPrefab == null)
            {
                Debug.LogWarning(name +".House Generator: CELL PREFAB IS NULL");
                isOk = false;
            }
            
            if (internalDoorPrefab == null)
            {
                Debug.LogWarning(name +".House Generator: INTERNAL DOOR PREFAB IS NULL");
                isOk = false;
            }
            
            if (externalDoorPrefab == null)
            {
                Debug.LogWarning(name +".House Generator: EXTERNAL DOOR PREFAB IS NULL");
                isOk = false;
            }
            
            if (windowFramePrefab == null)
            {
                Debug.LogWarning(name +".House Generator: WINDOW FRAME PREFAB IS NULL");
                isOk = false;
            }

            if (size < 1)
            {
                Debug.LogWarning(name +".House Generator: SIZE LESS THAN 1");
                isOk = false;
            }
            
            if (facadeMaterial == null)
            {
                Debug.LogWarning(name +".House Generator: FACADE MATERIAL IS NULL");
                isOk = false;
            }
            
            if (facadeColumnMaterial == null)
            {
                Debug.LogWarning(name +".House Generator: FACADE COLUMN MATERIAL IS NULL");
                isOk = false;
            }
            
            if (interiors == null || interiors.Count == 0)
            {
                Debug.LogWarning(name +".House Generator: INTERIORS IS EMPTY");
                isOk = false;
            }

            return isOk;
        }

    
        /////////////////////////////////////////////////
        /// <summary>  Builds random house  </summary>
        ////////////////////////////////////////////////
        public void Build (Blueprint blueprint, Random rnd, Interior[] interiors, BuilderType builderType)
        {
            
#if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
                UnityEditor.PrefabUtility.UnpackPrefabInstance(gameObject, UnityEditor.PrefabUnpackMode.OutermostRoot, 
                                                               UnityEditor.InteractionMode.UserAction);
#endif           
            Cleanup();
            
            switch (builderType)
            {
                case BuilderType.RawBuilder:
                    builder = new RawBuilder();
                    break;
                
                case BuilderType.CombinedBuilder:
                    builder = new CombinedBuilder();
                    break;
            }
            
            var data = new BuilderData(blueprint,
                                       cellPrefab,
                                       interiors,
                                       rnd,
                                       hideCeiling,
                                       transform,
                                       facadeMaterial,
                                       decoratorSample1,
                                       decoratorSample2,
                                       decoratorSample3,
                                       externalDoorPrefab,
                                       internalDoorPrefab,
                                       windowFramePrefab,
                                       lodCullingPercent);
            
            builder.Build(data);
        }
        
        
        private void Cleanup()
        {
            var objsToRemove = new GameObject[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
                objsToRemove[i] = transform.GetChild(i).gameObject;

            for (int i = 0; i < objsToRemove.Length; i++)
                DestroyImmediate(objsToRemove[i]);
        }

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Generates and builds a house from a random seed (calculates from the time)  </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GenerateWithRandomSeed()
        {
            seed = RND.Seed();
            Generate();
        }
        
        
        ////////////////////////////////////////////////////////////////////////
        /// <summary>  Generates and builds a house from the seed  </summary>
        ///////////////////////////////////////////////////////////////////////
        public void GenerateWithSeed(int seed)
        {
            this.seed = seed;
            Generate();
        }
    
    }
}






