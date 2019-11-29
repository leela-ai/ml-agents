using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Cozy_House_Generator.Editor.CustomInspector
{
    
    [CustomEditor(typeof(Interior))]
    public class InteriorEditor : UnityEditor.Editor
    {
        private           Interior         tar;
        private           ReorderableList  list;
        private readonly  TextStyle        textStyle = new TextStyle();
        
        
        private void OnEnable()
        {
            tar = (Interior) target;
            textStyle.Initialize();
            
            //Using non-documented unity feature for draw a reorderable list that contains pipelines------------------//
            list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("possibleProps"),
                true, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Props"); }
            };


            list.drawElementCallback = 
                (rect, index, isActive, isFocused) => {
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    
                    EditorGUI.ObjectField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("props"), typeof(Scripts.ScriptableObjects.Props) ,GUIContent.none);
				
                    EditorGUI.PropertyField(new Rect(rect.x + 205, rect.y, 30, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("count"), GUIContent.none);
                };
            
            //--------------------------------------------------------------------------------------------------------//
        }
        
        

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("INTERIOR", textStyle.headerMontserrat, GUILayout.Height(45));
            GUILayout.Space(20);
            DrawRequirements();
            GUILayout.Space(30);
            DrawParameters();
            EditorUtility.SetDirty(tar);
        }

        
        private void DrawRequirements()
        {
            EditorGUILayout.LabelField("REQUIREMENTS", textStyle.smallHeaderMontserrat, GUILayout.Height(15));
            GUILayout.Space(10);
            
            tar.minRoomSize      = EditorGUILayout.IntField("Min Size Of Room",   tar.minRoomSize);
            tar.maxRoomSize      = EditorGUILayout.IntField("Max Size Of Room",   tar.maxRoomSize);
            tar.maxCountOfDoors  = EditorGUILayout.IntField("Max Count Of Doors", tar.maxCountOfDoors);
        }

        
        private void DrawParameters()
        {
            EditorGUILayout.LabelField("PARAMETERS", textStyle.smallHeaderMontserrat, GUILayout.Height(15));
            
            GUILayout.Space(10);
            tar.placeOnlyOnce = EditorGUILayout.Toggle("Place only once", tar.placeOnlyOnce);
            
            tar.floor =    EditorGUILayout.ObjectField("Material For Floor",   tar.floor,   typeof(Material), false) as Material;
            tar.ceiling =  EditorGUILayout.ObjectField("Material For Ceiling", tar.ceiling, typeof(Material), false) as Material;
            tar.walls =    EditorGUILayout.ObjectField("Material For Walls",   tar.walls,   typeof(Material), false) as Material;
            tar.column =   EditorGUILayout.ObjectField("Material For Column",  tar.column,  typeof(Material), false) as Material;                
            tar.windowsRate = EditorGUILayout.IntField("Windows Rate", tar.windowsRate);
            
            EditorGUILayout.Space();
            
            //non documented unity black magic for draw reorderable list
            serializedObject.Update();
            list			.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            //----------------------------------------------------------//
        }
    }
}
