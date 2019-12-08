using System;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static Cozy_House_Generator.Editor.CustomInspector.Prefab;


namespace Cozy_House_Generator.Editor.CustomInspector
{
    [CustomEditor(typeof(PipelinePreset))]
    public class PipelinePresetEditor : UnityEditor.Editor
    {
        private PipelinePreset 		tar;
        private readonly TextStyle 	textStyle = new TextStyle();
        private ReorderableList 	firstStep;
        private ReorderableList 	secoundStep;
        private int 				selectedStep;
        private int 				firstStepSelectedPipeIndex   = -1;
        private int 				secoundStepSelectedPipeIndex = -1;
        
        
        private void OnEnable()
        {
            tar = (PipelinePreset) target;
            textStyle.Initialize();

            firstStep   = InitPipeline("firstStepPipes",   DrawFirstStepElementCallback,   1);
            secoundStep = InitPipeline("secoundStepPipes", DrawSecoundStepElementCallback, 2);
        }


        public override void OnInspectorGUI()
        {
	        EditorGUILayout.LabelField("Pipeline", textStyle.headerMontserrat, GUILayout.Height(45));
	        GUILayout.Space(20);
	        //First Step----------------------------------------------------------------------------------------------//
	        EditorGUILayout.LabelField("First Step", textStyle.smallHeaderMontserrat, GUILayout.Height(25));
	        //GUILayout.Space(30);

	        serializedObject.Update();
	        firstStep.DoLayoutList();
	        serializedObject.ApplyModifiedProperties();

	        if (selectedStep == 1 && firstStepSelectedPipeIndex > -1)
	        {
		        try
		        {
			        ShowStepSettings(tar.firstStepPipes[firstStepSelectedPipeIndex]);
		        }
		        catch
		        {
		        }
	        }
	        //--------------------------------------------------------------------------------------------------------//

	        

	        //Secound Step--------------------------------------------------------------------------------------------//
	        GUILayout.Space(30);
	        EditorGUILayout.LabelField("Secound Step", textStyle.smallHeaderMontserrat, GUILayout.Height(25));

	        serializedObject.Update();
	        secoundStep.DoLayoutList();
	        serializedObject.ApplyModifiedProperties();

	        if (selectedStep == 2 && secoundStepSelectedPipeIndex > -1)
	        {
		        try
		        {
			        ShowStepSettings(tar.secoundStepPipes[secoundStepSelectedPipeIndex]);
		        }
		        catch
		        {
		        }
	        }
	        //--------------------------------------------------------------------------------------------------------//

        }


        private ReorderableList InitPipeline(string pipelineName, Action<Rect, int, bool, bool> DrawElementCallback, int stepId)
        {
            var pipes        = serializedObject.FindProperty(pipelineName);
            var pipelineList = new ReorderableList(serializedObject, pipes, true, true, true, true);

            pipelineList.drawHeaderCallback  = rect                  => { EditorGUI.LabelField(rect, "Pipeline"); };
            pipelineList.drawElementCallback = (rect, i, b, focused) => { DrawElementCallback (rect, i, b, focused); };
            pipelineList.onSelectCallback    = list 				   => { selectedStep = stepId; };
            pipelineList.onAddDropdownCallback = (buttonRect, l) =>
                                                 {
                                                     var menu      = new GenericMenu();
                                                     var pipeNames = tar.pipeline.GetNames();

                                                     for (int i = 0; i < pipeNames.Length; i++)
                                                     {
                                                         string pipeName = pipeNames[i];
                                                         menu.AddItem(new GUIContent(pipeName), false, AddClickHandler, i);
                                                     }

                                                     menu.ShowAsContext();
                                                 };
			
            
            void AddClickHandler(object target)
            {
                var pipeId = (int)target;
                var index  = pipelineList.serializedProperty.arraySize;
				
                pipelineList.serializedProperty.arraySize++;
                pipelineList.index = index;
		
                var element = pipelineList.serializedProperty.GetArrayElementAtIndex(index);

                element.FindPropertyRelative("pipeId")    .intValue  = pipeId;
                element.FindPropertyRelative("isEnabled") .boolValue = true;
                element.FindPropertyRelative("useCount")  .intValue  = 1;
		
                serializedObject.ApplyModifiedProperties();
                MarkDirty();
            }

            return pipelineList;
        }
        
        
        private void DrawFirstStepElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = firstStep.serializedProperty.GetArrayElementAtIndex(index);
            int pipeID  = element.FindPropertyRelative("pipeId").intValue;
					
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                                 tar.pipeline.GetNames()[pipeID]);
					
            EditorGUI.PropertyField(new Rect(rect.x + 200, rect.y, 30, EditorGUIUtility.singleLineHeight),
                                    element.FindPropertyRelative("isEnabled"), GUIContent.none);

            if (isActive)
                firstStepSelectedPipeIndex = index;
        }
		
        private void DrawSecoundStepElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = secoundStep.serializedProperty.GetArrayElementAtIndex(index);
            int pipeID  = element.FindPropertyRelative("pipeId").intValue;
			
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                                 tar.pipeline.GetNames()[pipeID]);
					
            EditorGUI.PropertyField(new Rect(rect.x + 200, rect.y, 30, EditorGUIUtility.singleLineHeight),
                                    element.FindPropertyRelative("isEnabled"), GUIContent.none);

            if (isActive)
                secoundStepSelectedPipeIndex = index;
        }
        
        
        private void ShowStepSettings(PipeUIData pipe)
        {
	        GUILayout.Space(10);
	        EditorGUILayout.LabelField(tar.pipeline.GetNames()[pipe.pipeId] + " Settings", textStyle.nanoHeaderMontserrat, GUILayout.Height(25));	
	        pipe.useCount = EditorGUILayout.IntField("Use Count", pipe.useCount);
	        GUILayout.Space(5);

	        UseOnFloorResolverDrawer.Draw(pipe.floors);
	        GUILayout.Space(20);
        }

    }
}