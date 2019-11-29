using Cozy_House_Generator.Scripts;
using UnityEditor;
using UnityEngine;

namespace Cozy_House_Generator.Editor.CustomInspector
{
    [CustomEditor(typeof(PropsRandomizer))]
    public class PropsRandomizerEditor : UnityEditor.Editor
    {
        private           PropsRandomizer  tar;
        private readonly  TextStyle        textStyle = new TextStyle();
        private static    bool             _debug;

        private void OnEnable()
        {
            tar = (PropsRandomizer) target;
            textStyle.Initialize();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("PROP \nRANDOMIZER", textStyle.headerMontserrat, GUILayout.Height(90));

            _debug = EditorGUILayout.ToggleLeft("Debug", _debug);
            if (_debug == false)
                return;
            
            DrawDefaultInspector();
            if (GUILayout.Button("Randomize"))
                tar.CastomizeButton(tar.seed);

            if (GUILayout.Button("Randomize with random SEED"))
                tar.CastomizeWithRandomSeedButton();
            
            if (GUILayout.Button("Reset"))
                tar.Reset();
        }
    }
}