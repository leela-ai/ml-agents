using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cozy_House_Generator.Scripts.Core.DataTypes
{
    [Serializable]
    public class MeshCombineData
    {
        public ObjectToCombineDataGroup walls;
        public ObjectToCombineDataGroup floors;
        public ObjectToCombineDataGroup ceiling;
        public ObjectToCombineDataGroup columns;
        public ObjectToCombineDataGroup all;
        

        public MeshCombineData()
        {
            walls   = new ObjectToCombineDataGroup();
            floors  = new ObjectToCombineDataGroup();
            ceiling = new ObjectToCombineDataGroup();
            columns = new ObjectToCombineDataGroup();
            all     = new ObjectToCombineDataGroup();
        }
    }

    
    [Serializable]
    public class ObjectToCombineData
    {
        public Mesh mesh;
        public Matrix4x4 worldPosition;
        public Material[] materials;
    }
    
    
    [Serializable]
    public class ObjectToCombineDataGroup
    {
        public List<ObjectToCombineData> MeshData { get; }
        public List<Material>            Materials { get; }

        
        public ObjectToCombineDataGroup()
        {
            MeshData = new List<ObjectToCombineData>();
            Materials = new List<Material>();
        }

        
        public void Add(ObjectToCombineData meshData)
        {
            MeshData.  Add(meshData);

            foreach (var newMat in meshData.materials)
                if (!Materials.Contains(newMat))
                    Materials.Add(newMat);
           
        }

        
        public void AddRange(IEnumerable<ObjectToCombineData> meshData)
        {
            if (meshData == null || !meshData.Any())
                return;
            
            foreach (var data in meshData)
                Add(data);
        }
    }
}