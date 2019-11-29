using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using UnityEngine;

namespace Cozy_House_Generator.Scripts.Core
{
    public class MeshCombiner
    {
        public void Combine(GameObject tar)
        {      
            var filters   = tar.GetComponentsInChildren<MeshFilter>    (false);
            var rends     = tar.GetComponentsInChildren<MeshRenderer>  (false);
            var materials = GetMaterials                               (rends, tar.transform);       
            var submeshes = CombineMeshesPerMaterials                  (materials, filters, tar);
            var finalMesh = FinalCombine                               (submeshes, tar.transform);
               
            tar.GetComponent<MeshFilter>().   mesh       = finalMesh;
            tar.GetComponent<MeshRenderer>(). materials  = materials;
            tar.GetComponent<MeshCollider>(). sharedMesh = finalMesh;
        }
    
    
    
        public Mesh Combine(ObjectToCombineDataGroup objectToCombineData, Transform root)
        {
            var submeshes = CombineMeshesPerMaterials  (objectToCombineData);
            return FinalCombine                        (submeshes, root);          
        }
    
    
    
        private Material[] GetMaterials(IEnumerable<MeshRenderer> rends, Transform tar)
        {
            var materials = new List<Material>();
        
            foreach (var rend in rends)
            {
                if (rend.transform == tar)
                    continue;

                var localMats = rend.sharedMaterials;
                foreach (var localMat in localMats)
                    if (!materials.Contains(localMat))
                        materials.Add(localMat);
            }
            return materials.ToArray();
        }


        private static Mesh[] CombineMeshesPerMaterials(IEnumerable<Material> materials, MeshFilter[] filters, GameObject self, bool disableGameobjects = true)
        {
            //Each material will have a mesh for it.
            var submeshes = new List<Mesh>();
        
            foreach (var material in materials)
            {
                // Make a combiner for each (sub)mesh that is mapped to the right material.
                var combiners = new List<CombineInstance>();
            
                foreach (var filter in filters)
                {
                    // The filter doesn't know what materials are involved get the renderer
                    var renderer = filter.GetComponent<MeshRenderer>();
                    if (renderer == null)
                    {
                        Debug.LogError(filter.name + " has no MeshRenderer");
                        continue;
                    }
                
                    // Let's see if their materials are the one we want right now.
                    var localMaterials = renderer.sharedMaterials;
                    for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                    {
                        if (localMaterials[materialIndex] != material)
                            continue;
                    
                        //this submesh is the material we're looking for right now
                        var ci = new CombineInstance();
                        ci.mesh         = filter.sharedMesh;
                        ci.subMeshIndex = materialIndex;
                        ci.transform    = filter.transform.localToWorldMatrix;
                        combiners.Add(ci);
                    }

                    if (disableGameobjects && filter.gameObject != self)
                    {
                        filter.gameObject.SetActive(false);
                    }
                
                
                }
                // Flatten into a single mesh.
                var mesh = new Mesh();
                mesh.CombineMeshes(combiners.ToArray(), true);
                submeshes.Add(mesh);
            }

            return submeshes.ToArray();
        }
    
    
    
    
    
        private static Mesh[] CombineMeshesPerMaterials(ObjectToCombineDataGroup combineDataGroup)
        {
            //Each material will have a mesh for it.
            var                   submeshes      = new List<Mesh>();
            Material[]            materials      = combineDataGroup.Materials.ToArray();
            ObjectToCombineData[] combineObjects = combineDataGroup.MeshData.ToArray();
        
            foreach (var material in materials)
            {
                var combiners = new List<CombineInstance>();
            
                foreach (var combineObject in combineObjects)
                {
                    var localMaterials = combineObject.materials;
                    for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                    {
                        if (localMaterials[materialIndex] != material)
                            continue;
                    
                        var ci = new CombineInstance();
                        ci.mesh         = combineObject.mesh;
                        ci.subMeshIndex = materialIndex;
                        ci.transform    = combineObject.worldPosition;
                        combiners.Add(ci);
                    }
                }
                // Flatten into a single mesh.
                var mesh = new Mesh();
                mesh.CombineMeshes(combiners.ToArray(), true);
                submeshes.Add(mesh);
            }

            return submeshes.ToArray();
        }
    
    
    
        private static Mesh FinalCombine(Mesh[] submeshes, Transform rootTransform)
        {
            var finalCombiners = new List<CombineInstance>();
            foreach (var mesh in submeshes)
            {
                var ci = new CombineInstance();
                ci.mesh         = mesh;
                ci.subMeshIndex = 0;
                ci.transform    = rootTransform.worldToLocalMatrix;
                finalCombiners.Add(ci);
            }

            var finalMesh = new Mesh();
            finalMesh.CombineMeshes(finalCombiners.ToArray(), false);
            return finalMesh;
        } 
    
    }
}
