using System;
using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using UnityEngine;

namespace Cozy_House_Generator.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Cozy House Generator/Pipeline Preset"), Serializable]
    public class PipelinePreset : ScriptableObject
    {
        public Pipeline pipeline = new Pipeline();

        [HideInInspector] public List<PipeUIData> firstStepPipes;
        [HideInInspector] public List<PipeUIData> secoundStepPipes;


        public Pipeline GetPipeline()
        {
            pipeline.Init(firstStepPipes.ToArray(), secoundStepPipes.ToArray());
            return pipeline;
        }
    }
}