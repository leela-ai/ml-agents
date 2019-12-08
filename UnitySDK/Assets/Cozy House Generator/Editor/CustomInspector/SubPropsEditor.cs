using System.Collections.Generic;
using System.Linq;
using Cozy_House_Generator.Scripts;
using UnityEditor;
using UnityEngine;

namespace Cozy_House_Generator.Editor.CustomInspector
{
    [CustomEditor(typeof(SubProps)), CanEditMultipleObjects]
    public class SubPropsEditor : UnityEditor.Editor
    {
        private SubProps[]          tars;
        private SubProps            tar;
        private GUIStyle            selectedButtonStyle;
        private GUIStyle            headerTextStyle;
        private readonly TextStyle  textStyle              = new TextStyle();


        private void OnEnable()
        {
            tar  = (SubProps) target;
            tars = new SubProps[targets.Length];
            for (int i = 0; i < tars.Length; i++)
                tars[i] = targets[i] as SubProps;
            
            headerTextStyle = textStyle.smallHeaderMontserrat;
            textStyle.Initialize();
        }

        
        public override void OnInspectorGUI()
        {
            if (selectedButtonStyle == null)
                selectedButtonStyle = new GUIStyle("Button") {fontStyle = FontStyle.Italic};
            
            EditorGUILayout.LabelField("SUB PROP", textStyle.headerMontserrat, GUILayout.Height(45));

            DrawChanceToPlace(tars, headerTextStyle);
            
            GUILayout.Space(20);
            DrawDefaultPosAndRot(tars, tar, headerTextStyle);

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Offset", headerTextStyle, GUILayout.Height(25));
            DrawPositionOffset(tars, selectedButtonStyle);
            DrawRotationOffset(tars, selectedButtonStyle);
        }
        

        private static void DrawChanceToPlace(SubProps[] tars, GUIStyle headerLabelStyle)
        {
            EditorGUILayout.LabelField("General", headerLabelStyle, GUILayout.Height(25));

            EditorGUI.BeginChangeCheck();
            string chanceToPlaceLabel = IsValuesEqual(tars.Select(l => l.chanceToPlace).ToList())
                ? "Chance to place"
                : "Chance to place*";
            
            int chanceToPlace = EditorGUILayout.IntSlider(chanceToPlaceLabel, tars.First().chanceToPlace, 0, 100);

            if (EditorGUI.EndChangeCheck())
            {
                foreach (var tar in tars)
                    tar.chanceToPlace = chanceToPlace;
                Prefab.MarkDirty();
            }
        }
        

        private static void DrawDefaultPosAndRot(SubProps[] tars, SubProps tar, GUIStyle headerLabelStyle)
        {
            EditorGUILayout.LabelField("Default Transform", headerLabelStyle, GUILayout.Height(25));
            
            if (tars.Length > 1)
                GUILayout.Box("Can't show for multiple objects");
            else
            {
                Prefab.SetValue(ref tar.defaultPosition,
                    EditorGUILayout.Vector3Field("Default Position", tar.defaultPosition));
                
                Prefab.SetValue(ref tar.defaultRotation,
                    Quaternion.Euler(EditorGUILayout.Vector3Field("Default Rotation",
                        tar.defaultRotation.eulerAngles)));
            }

            if (GUILayout.Button("Set current transform as default", GUILayout.ExpandWidth(false)))
            {
                foreach (var subProps in tars)
                    subProps.SetCurrentTransformAsDefault();
                Prefab.MarkDirty();
            }
        }
        
        
        private static void DrawPositionOffset(SubProps[] tars, GUIStyle selectedButtonStyle)
        {
            EditorGUI.BeginChangeCheck();
            bool addRandomPosOffset;
            bool allTarsHasSameAddPosStatus = IsValuesEqual(tars.Select(l => l.addRandomPositionOffset).ToList());

            if (allTarsHasSameAddPosStatus)
                addRandomPosOffset = GUILayout.Toggle(tars.First().addRandomPositionOffset,
                    "Add random position offset", "Button");
            else
                addRandomPosOffset = GUILayout.Toggle(false, "Add random position offset", selectedButtonStyle);
            
            if (EditorGUI.EndChangeCheck())
                foreach (var tar in tars)
                {
                    tar.addRandomPositionOffset = addRandomPosOffset;
                    Prefab.MarkDirty();
                }

            if (tars.First().addRandomPositionOffset && allTarsHasSameAddPosStatus)
            {
                DrawMinXPosOffsetField(tars);
                DrawMaxXPosOffsetField(tars);
                EditorGUILayout.Space();

                DrawMinZPosOffsetField(tars);
                DrawMaxZPosOffsetField(tars);
                EditorGUILayout.Space();
            }
        }


        private static void DrawMinXPosOffsetField(SubProps[] tars)
        {
            EditorGUI.BeginChangeCheck();
            var fieldLabel = IsValuesEqual(tars.Select(l => l.minXPosOffsetCm).ToList())
                                                                              ? tars.First().minXPosOffsetCm.ToString()
                                                                              : "-";
            if (int.TryParse(EditorGUILayout.TextField("Min X pos offset", fieldLabel), out var newMinXPosOffset))
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var tar in tars)
                        tar.minXPosOffsetCm = newMinXPosOffset;
                    Prefab.MarkDirty();
                }
        }
        
        
        private static void DrawMaxXPosOffsetField(SubProps[] tars)
        {
            EditorGUI.BeginChangeCheck();
            var fieldLabel = IsValuesEqual(tars.Select(l => l.maxXPosOffsetCm).ToList())
                                                                              ? tars.First().maxXPosOffsetCm.ToString()
                                                                              : "-";
            if (int.TryParse(EditorGUILayout.TextField("Max X pos offset", fieldLabel), out var newMaxXPosOffset))
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var tar in tars)
                        tar.maxXPosOffsetCm = newMaxXPosOffset;
                    Prefab.MarkDirty();
                }
        }
        
        
        private static void DrawMinZPosOffsetField(SubProps[] tars)
        {
            EditorGUI.BeginChangeCheck();
            var fieldLabel = IsValuesEqual(tars.Select(l => l.minZPosOffsetCm).ToList())
                                                                              ? tars.First().minZPosOffsetCm.ToString()
                                                                              : "-";
            if (!int.TryParse(EditorGUILayout.TextField("Min Z pos offset", fieldLabel),
                              out var newMinZPosOffset)) return;

            if (!EditorGUI.EndChangeCheck()) return;
            
            
            foreach (var tar in tars)
                tar.minZPosOffsetCm = newMinZPosOffset;
            Prefab.MarkDirty();
        }
        
        
        private static void DrawMaxZPosOffsetField(SubProps[] tars)
        {
            EditorGUI.BeginChangeCheck();
            var fieldLabel = IsValuesEqual(tars.Select(l => l.maxZPosOffsetCm).ToList())
                                                                              ? tars.First().maxZPosOffsetCm.ToString()
                                                                              : "-";
            if (int.TryParse(EditorGUILayout.TextField("Max Z pos offset", fieldLabel), out var newMaxZPosOffset))
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var tar in tars)
                        tar.maxZPosOffsetCm = newMaxZPosOffset;
                    Prefab.MarkDirty();
                }
        }
        

        private static void DrawRotationOffset(SubProps[] tars, GUIStyle selectedButtonStyle)
        {
            EditorGUI.BeginChangeCheck();
            bool addRandomRotOffset;
            bool allTarsHasSameAddRotStatus = IsValuesEqual(tars.Select(l => l.addRandomRotationOffset).ToList());

            if (allTarsHasSameAddRotStatus)
                addRandomRotOffset = GUILayout.Toggle(tars.First().addRandomRotationOffset,
                    "Add random rotation offset", "Button");
            else
                addRandomRotOffset = GUILayout.Toggle(false, "Add random rotation offset", selectedButtonStyle);

            if (EditorGUI.EndChangeCheck())
            {
                foreach (var subProps in tars)
                    subProps.addRandomRotationOffset = addRandomRotOffset;
                Prefab.MarkDirty();
            }

            if (tars.First().addRandomRotationOffset && allTarsHasSameAddRotStatus)
            {
                DrawMinRotOffset(tars);
                DrawMaxRotOffset(tars);
            }
        }
        
        
        private static void DrawMinRotOffset(SubProps[] tars)
        {
            EditorGUI.BeginChangeCheck();
            bool isMinRotEqual = IsValuesEqual(tars.Select(l => l.minRandomRotation).ToList());
            
            var minRandRotLabel = isMinRotEqual
                ? "Min rotation offset"
                : "Min rotation offset*";
            
            int minRandRot = EditorGUILayout.IntSlider(minRandRotLabel,
                tars.First().minRandomRotation, -90, tars.First().maxRandomRotation);

            if (EditorGUI.EndChangeCheck())
            {
                foreach (var tar in tars)
                    tar.minRandomRotation = minRandRot;
                Prefab.MarkDirty();
            }
        }
        
        
        private static void DrawMaxRotOffset(SubProps[] tars)
        {
            EditorGUI.BeginChangeCheck();
            bool isMaxRotEqual = IsValuesEqual(tars.Select(l => l.maxRandomRotation).ToList());
            
            var maxRandRotLabel = isMaxRotEqual
                ? "Max rotation offset"
                : "Max rotation offset*";
            
            int maxRandRot = EditorGUILayout.IntSlider(maxRandRotLabel,
                tars.First().maxRandomRotation, tars.First().minRandomRotation, 90);
            
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var tar in tars)
                    tar.maxRandomRotation = maxRandRot;
                Prefab.MarkDirty();
            }
        }
        
        private static bool IsValuesEqual<T>(IEnumerable<T> values)
        {
            var enumerable = values as T[] ?? values.ToArray();
            var val = enumerable.First();
            return enumerable.All(x=>x.Equals(val)); 
        }
        
    }
}