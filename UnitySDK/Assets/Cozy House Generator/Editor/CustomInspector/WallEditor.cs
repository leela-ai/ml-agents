using Cozy_House_Generator.Scripts;
using UnityEditor;
using UnityEngine;
using static Cozy_House_Generator.Editor.CustomInspector.Prefab;
using static UnityEditor.EditorGUILayout;

namespace Cozy_House_Generator.Editor.CustomInspector
{
    [CustomEditor(typeof(Wall))]
    public class WallEditor : UnityEditor.Editor
    {
        private          Wall      tar;
        private readonly TextStyle textStyle = new TextStyle();

        
        private void OnEnable()
        {
            tar = (Wall) target;
            textStyle.Initialize();
        }

        public override void OnInspectorGUI()
        {
            LabelField("WALL", textStyle.headerMontserrat, GUILayout.Height(45));
            LabelField("Gameobjects Links", textStyle.smallHeaderMontserrat, GUILayout.Height(25));

            SetValue(ref tar.internalWall, (GameObject) ObjectField(
                                                                    "Internal Wall",
                                                                    tar.internalWall,
                                                                    typeof(GameObject),
                                                                    true));

            SetValue(ref tar.externalWall, (GameObject) ObjectField(
                                                                    "External Wall",
                                                                    tar.externalWall,
                                                                    typeof(GameObject),
                                                                    true));

            SetValue(ref tar.internalDoorway, (GameObject) ObjectField(
                                                                       "Internal Doorway",
                                                                       tar.internalDoorway,
                                                                       typeof(GameObject),
                                                                       true));

            SetValue(ref tar.internalDoor, (GameObject) ObjectField(
                                                                    "Internal Door",
                                                                    tar.internalDoor,
                                                                    typeof(GameObject),
                                                                    true));

            SetValue(ref tar.externalDoorway, (GameObject) ObjectField(
                                                                       "External Doorway",
                                                                       tar.externalDoorway,
                                                                       typeof(GameObject),
                                                                       true));
            SetValue(ref tar.externalDoor, (GameObject) ObjectField(
                                                                    "External Door",
                                                                    tar.externalDoor,
                                                                    typeof(GameObject),
                                                                    true));


            SetValue(ref tar.window, (GameObject) ObjectField(
                                                              "Window",
                                                              tar.window,
                                                              typeof(GameObject),
                                                              true));


            SetValue(ref tar.windowFrame, (GameObject) ObjectField(
                                                                   "Window Frame",
                                                                   tar.windowFrame,
                                                                   typeof(GameObject),
                                                                   true));

            GUILayout.Space(20);
            LabelField("Materials IDs", textStyle.smallHeaderMontserrat, GUILayout.Height(25));
            
            
            SetValue(ref tar.internalWallMatId,       IntField("Internal Wall",        tar.internalWallMatId));
            SetValue(ref tar.extFacadeMaterialId,     IntField("External Wall Facade", tar.extFacadeMaterialId));
            SetValue(ref tar.extInsideMaterialId,     IntField("External Wall Inside", tar.extInsideMaterialId));
            SetValue(ref tar.internalDoorMatId,       IntField("Internal Door",        tar.internalDoorMatId));
            SetValue(ref tar.extDoorInsideMaterialId, IntField("External Door Inside", tar.extDoorInsideMaterialId));
            SetValue(ref tar.extDoorFacadeMaterialId, IntField("External Door Facade", tar.extDoorFacadeMaterialId));
            SetValue(ref tar.windowInsideMaterialId,  IntField("Window Inside",        tar.windowInsideMaterialId));
            SetValue(ref tar.windowFacadeMaterialId,  IntField("Window Facade",        tar.windowFacadeMaterialId));
                                                                               
        }
    }
}