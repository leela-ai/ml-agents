using Cozy_House_Generator.Scripts;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cozy_House_Generator.Editor.CustomInspector.Prefab;
using static UnityEditor.EditorGUILayout;


namespace Cozy_House_Generator.Editor.CustomInspector
{
	[CustomEditor(typeof(HouseGenerator))]
	public class HouseGeneratorEditor : UnityEditor.Editor
	{
		private 		  ReorderableList  interiorList;
		private 		  HouseGenerator   tar;
		private readonly  TextStyle 	   textStyle = new TextStyle();
		private 		  Texture2D 	   backgroundImage;
		
		private SerializedProperty pipelinePresetData;
		private UnityEditor.Editor pipelinePresetEditor;
		
		
		
		private void OnEnable()
		{
			tar = (HouseGenerator) target;
			textStyle.Initialize();
			
			pipelinePresetData = serializedObject.FindProperty("pipelinePreset");
			ResetMonsterPropertyEditor();
			
			interiorList = new ReorderableList(serializedObject, serializedObject.FindProperty("interiors"), true, true,
			                                   true, true)
			               {
				               drawHeaderCallback  = rect => { EditorGUI.LabelField(rect, "Interiors"); },
				               drawElementCallback = (rect, index, isActive, isFocused) =>
				                                     {
					                                     rect.y += 2;
					                                     if (index == 0)
					                                     {
						                                     tar.interiors[index] =
							                                     (Interior) EditorGUI.ObjectField(new Rect(rect.x + 50,
							                                                                               rect.y, 170,
							                                                                               EditorGUIUtility
								                                                                              .singleLineHeight),
							                                                                      "",
							                                                                      tar.interiors[index],
							                                                                      typeof(Interior),
							                                                                      false);
						                                     EditorGUI
							                                    .HelpBox(new Rect(rect.x, rect.y, 45, EditorGUIUtility.singleLineHeight),
							                                             "Default", MessageType.None);
					                                     }
					                                     else
					                                     {
						                                     tar.interiors[index] =
							                                     (Interior) EditorGUI.ObjectField(new Rect(rect.x,
							                                                                               rect.y, 200,
							                                                                               EditorGUIUtility
								                                                                              .singleLineHeight),
							                                                                      "",
							                                                                      tar.interiors[index],
							                                                                      typeof(Interior),
							                                                                      false);
					                                     }
				                                     },
				               onAddCallback = l =>
				                               {
					                               var index = l.serializedProperty.arraySize;
					                               l.serializedProperty.arraySize++;
					                               l.index = index;
					                               l.serializedProperty.GetArrayElementAtIndex(index)
					                                .objectReferenceValue = null;
					                               MarkDirty();
				                               },
				               onReorderCallback = l => { MarkDirty(); },
				               onRemoveCallback = l =>
				                                  {
					                                  var index = l.serializedProperty.arraySize;
					                                  l.serializedProperty.arraySize--;
					                                  l.index = index - 2;
					                                  MarkDirty();
				                                  }
			               };
		}


		private void ResetMonsterPropertyEditor() {
			pipelinePresetEditor = CreateEditor(pipelinePresetData.objectReferenceValue); 
			serializedObject.Update();
		}


		private static bool showPipeline = false;
		
		public override void OnInspectorGUI()
		{
			//Generate Panel------------------------------------------------------------------------------------------//
			LabelField("HOUSE \nGENERATOR", textStyle.headerMontserrat, GUILayout.Height(45));
			GUILayout.Space(50);
			LabelField("Generate", textStyle.smallHeaderMontserrat, GUILayout.Height(25));

			if (GUILayout.Button("Generate Random"))
			{
				tar.GenerateWithRandomSeed();
				if (EditorApplication.isPlaying == false)
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

				GUI.FocusControl(null);
				return;
			}

			BeginHorizontal();
			if (GUILayout.Button("Generate from SEED"))
			{
				tar.Generate();
				if (EditorApplication.isPlaying == false)
					EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
				return;
			}

			SetValue(ref tar.seed, IntField(tar.seed));
			EndHorizontal();
			GUILayout.Space(30);
			//--------------------------------------------------------------------------------------------------------//

			//Pipeline------------------------------------------------------------------------------------------------//
			serializedObject.Update();   
			EditorGUI.BeginChangeCheck();
			
			SetValue(ref tar.pipelinePreset,
			         (PipelinePreset) ObjectField("Pipeline Preset",
			                                      tar.pipelinePreset,
			                                      typeof(PipelinePreset),
			                                      false));
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				ResetMonsterPropertyEditor();
				pipelinePresetEditor = CreateEditor(pipelinePresetData.objectReferenceValue);
			}

			showPipeline = Foldout(showPipeline, "Details");
			if (showPipeline)
			{
				if (pipelinePresetEditor != null)
				{
					GUILayout.BeginVertical(EditorStyles.helpBox);
					pipelinePresetEditor.OnInspectorGUI();
					EndVertical();
				}
				else
				{
					Separator();
					HelpBox("Pipeline is empty!", MessageType.Error);
				}
			}

			serializedObject.ApplyModifiedProperties();
			//--------------------------------------------------------------------------------------------------------//
	
			
			//General Settings----------------------------------------------------------------------------------------//			
			GUILayout.Space(30);
			LabelField("General Settings", textStyle.smallHeaderMontserrat, GUILayout.Height(25));
			
			SetValue(ref tar.builderType, (BuilderType) EnumPopup("Builder", tar.builderType));

			SetValue(ref tar.cellPrefab, (GameObject) ObjectField("Cell Prefab", tar.cellPrefab,
			                                                                      typeof(GameObject),
			                                                                      false));

			SetValue(ref tar.internalDoorPrefab, (GameObject) ObjectField("Internal Door Prefab",
			                                                                              tar.internalDoorPrefab,
			                                                                              typeof(GameObject),
			                                                                              false));

			SetValue(ref tar.externalDoorPrefab, (GameObject) ObjectField("External Door Prefab",
			                                                                              tar.externalDoorPrefab,
			                                                                              typeof(GameObject),
			                                                                              false));

			SetValue(ref tar.windowFramePrefab, (GameObject) ObjectField("Window Frame Prefab",
			                                                                             tar.windowFramePrefab,
			                                                                             typeof(GameObject),
			                                                                             false));

			SetValue(ref tar.lodCullingPercent, IntSlider("LOD Culling Percent", tar.lodCullingPercent, 1, 25));
			SetValue(ref tar.floorsCount, IntField("Floors", 		 tar.floorsCount));
			SetValue(ref tar.size,        IntField("Size",   		 tar.size));
			SetValue(ref tar.hideCeiling, Toggle  ("Hide Ceiling", tar.hideCeiling));

			GUILayout.Space(30);
			LabelField("Interiors", textStyle.smallHeaderMontserrat, GUILayout.Height(25));

			SetValue(ref tar.facadeMaterial, (Material) ObjectField("Facade Material",
			                                                                        tar.facadeMaterial,
			                                                                        typeof(Material),
			                                                                        false));

			SetValue(ref tar.facadeColumnMaterial, (Material) ObjectField("Facade Column Material",
			                                                                              tar.facadeColumnMaterial,
			                                                                              typeof(Material),
			                                                                              false));
			//--------------------------------------------------------------------------------------------------------//
			

		    //Interiors-----------------------------------------------------------------------------------------------//
			serializedObject.Update();
			interiorList	.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
			//--------------------------------------------------------------------------------------------------------//

			
			//Decorators----------------------------------------------------------------------------------------------//
			GUILayout.Space(30);
			LabelField("Decorators", textStyle.smallHeaderMontserrat, GUILayout.Height(25));

			SetValue(ref tar.decoratorSample1,
			         (Material) ObjectField("Decorator Sample 1", tar.decoratorSample1, typeof(Material), false));

			SetValue(ref tar.decoratorSample2,
			         (Material) ObjectField("Decorator Sample 2", tar.decoratorSample2, typeof(Material), false));

			SetValue(ref tar.decoratorSample3,
			         (Material) ObjectField("Decorator Sample 3", tar.decoratorSample3, typeof(Material), false));

			GUILayout.Space(30);
			//--------------------------------------------------------------------------------------------------------//
		}

	}
}
