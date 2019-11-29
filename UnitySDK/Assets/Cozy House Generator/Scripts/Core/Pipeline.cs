using System.Collections.Generic;
using System.Linq;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.Pipes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core
{
    public class Pipeline
    {
        private List<IGeneratorPipe> firstStep;
        private List<IGeneratorPipe> secoundStep;
        private IGeneratorPipe[]     possiblePipeTypes;
        private string[]             pipeNames;
        
        
        public Pipeline()
        {
            possiblePipeTypes = new IGeneratorPipe[]
                                {
                                    new DefineRooms(),
                                    new RemoveTooSmallRooms(),
                                    new MakeRoomsSmoother(),
                                    new RemoveIsolatedVoids(),
                                    new MakeThinRoomCellsWider(),
                                    new PlaceWalls(),
                                    new PlaceInternalDoors(),
                                    new PlaceExternalDoors(),
                                    new PlaceWindows(),
                                    new SetInteriors(),
                                    new PlaceProps(),
                                    new RemoveIsolatedRooms(),
                                    new PlaceColumns(),
                                    new RemoveCellsInTheAir(), 
                                    new RooficationCells(),
                                    new CopyDownFloor(),
                                    new FloorInitializeRandomization()
                                };

            pipeNames = possiblePipeTypes.Select(pipelineType => pipelineType.GetPipeName()).ToArray();
        }


        public string[] GetNames()
        {
            return pipeNames;
        }
        
        
        public void Init(PipeUIData[] firstStepData, PipeUIData[] secoundStepData)
        {
            InitStep(firstStepData, ref firstStep);
            InitStep(secoundStepData, ref secoundStep);
        }

        
        private void InitStep(PipeUIData[] stepData, ref List<IGeneratorPipe> step)
        {
            if (stepData == null || stepData.Length == 0)
            {
                Debug.LogWarning("House Generator: PIPELINE STEP IS EMPTY");
                return;
            }
            
            step = new List<IGeneratorPipe>();
            foreach (var uiData in stepData)
                if (uiData.isEnabled)
                    step.Add( possiblePipeTypes[uiData.pipeId].MakeNew(uiData.useCount, uiData.floors));
        }

        
        public void ProcessFirstStep(Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial, Material facadeColumnMaterial, Random rnd)
        {
            foreach (var pipe in firstStep)
            {
                PipeUseResolver.Resolve(pipe, rnd, floor, blueprint, interiors, facadeMaterial, facadeColumnMaterial);
            }
        }
        
        
        public void ProcessSecoundStep(Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial, Material facadeColumnMaterial, Random rnd)
        {
            foreach (var pipe in secoundStep)
            {
                PipeUseResolver.Resolve(pipe, rnd, floor, blueprint, interiors, facadeMaterial, facadeColumnMaterial);
            }
        }
    }
}