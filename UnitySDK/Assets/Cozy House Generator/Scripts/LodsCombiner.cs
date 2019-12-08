using System;
using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Cozy_House_Generator.Scripts
{
    public class LodsCombiner : MonoBehaviour
    {
        private MeshCombiner      meshCombiner = new MeshCombiner();
        private LODCombineLevel[] lodCombineLevels;

        
        
        public void Combine(int culledPercent, ShadowCastingMode shadowCastingMode)
        {
            AutoComplete();
            
            if (lodCombineLevels == null || lodCombineLevels.Length == 0)
                return;
            
            var lodsToDisable = GetComponentsInChildren(typeof(LODGroup), false);
            
            var lodGroup = GetComponent<LODGroup>();
            if (lodGroup == null)
                lodGroup = gameObject.AddComponent<LODGroup>();
            
            LOD[] lods = new LOD[lodCombineLevels.Length];

            for (int i = 0; i < lods.Length; i++)
            {
                var lodParent = new GameObject("LOD_" + i, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
                lodParent.transform.parent = transform;
                SetNewParent(lodCombineLevels[i].rends, lodParent);
                meshCombiner.Combine(lodParent);

                lodParent.GetComponent<MeshRenderer>().shadowCastingMode = shadowCastingMode;
                Renderer[] renderers = { lodParent.GetComponent<MeshRenderer>() };
                
                if (i == lods.Length - 1)
                    lods[i] = new LOD(culledPercent / 100f, renderers);
                else
                    lods[i] = new LOD(1.0F / ((i + 1f)*1.5f), renderers);
            }
            
            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
            foreach (LODGroup lod in lodsToDisable)
                lod.enabled = false;

            lodGroup.enabled = true;
        }

        
        public void AutoComplete()
        {
            var lodGroups = GetComponentsInChildren<LODGroup>();
            int lodCount  = lodGroups[0].lodCount;
            
            lodCombineLevels = new LODCombineLevel[lodCount];

            for (int i = 0; i < lodCount; i++)
            {
                lodCombineLevels[i] = new LODCombineLevel();
                foreach (var lodGroup in lodGroups)
                    lodCombineLevels[i].rends.AddRange(lodGroup.GetLODs()[i].renderers);
            }
        }

        
        private void SetNewParent(IEnumerable<Renderer> targets, GameObject newParent)
        {
            foreach (var tar in targets)
            {
                if (tar.gameObject.activeInHierarchy)
                    tar.transform.SetParent(newParent.transform);

                var lodGroup = tar.GetComponent<LODGroup>();
                if (lodGroup)
                    lodGroup.enabled = false;
            }
        }


    }

    [Serializable]
    public class LODCombineLevel
    {
        public List<Renderer> rends = new List<Renderer>();
    }
}