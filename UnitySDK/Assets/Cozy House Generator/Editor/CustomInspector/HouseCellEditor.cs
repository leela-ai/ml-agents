using Cozy_House_Generator.Scripts;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEditor;
using UnityEngine;
using static Cozy_House_Generator.Editor.CustomInspector.Prefab;
using static UnityEditor.EditorGUILayout;

namespace Cozy_House_Generator.Editor.CustomInspector
{
    [CustomEditor(typeof(HouseCell))]
    public class HouseCellEditor : UnityEditor.Editor
    {
        private           HouseCell  tar;
        private readonly  TextStyle  textStyle        = new TextStyle();
        private static    bool       _showDebugInfo;


        private void OnEnable()
        {
            tar = (HouseCell) target;
            textStyle.Initialize();
        }

        public override void OnInspectorGUI()
        {
            LabelField("CELL", textStyle.headerMontserrat, GUILayout.Height(45));
            
            LabelField("Settings", textStyle.smallHeaderMontserrat, GUILayout.Height(25));
            SetValue(ref tar.cellSize,  FloatField("Cell Size", tar.cellSize));
            SetValue(ref tar.cellHeight,  FloatField("Cell Height", tar.cellHeight));
            GUILayout.Space(10);
            
            LabelField("Gameobjects Links", textStyle.smallHeaderMontserrat, GUILayout.Height(25));
            
            SetValue(ref tar.floor,   (GameObject) ObjectField("Floor",   tar.floor,   typeof(GameObject), true));
            SetValue(ref tar.ceiling, (GameObject) ObjectField("Ceiling", tar.ceiling, typeof(GameObject), true));
            
            SetValue(ref tar.forwardWall,  (Wall) ObjectField("Forward Wall",  tar.forwardWall,  typeof(Wall), true));
            SetValue(ref tar.backwardWall, (Wall) ObjectField("Backward Wall", tar.backwardWall, typeof(Wall), true));
            SetValue(ref tar.rightWall,    (Wall) ObjectField("Right Wall",    tar.rightWall,    typeof(Wall), true));
            SetValue(ref tar.leftWall,     (Wall) ObjectField("Left Wall",     tar.leftWall,     typeof(Wall), true));

            SetValue(ref tar.forwardRightColumnInside,
                            (GameObject) ObjectField("Forward Right Column Inside",
                                                                              tar.forwardRightColumnInside,
                                                                              typeof(GameObject),
                                                                              true));
            SetValue(ref tar.forwardRightColumnFacade,
                            (GameObject) ObjectField("Forward Right Column Facade",
                                                                              tar.forwardRightColumnFacade,
                                                                              typeof(GameObject),
                                                                              true));
            

            SetValue(ref tar.forwardLeftColumnInside,
                            (GameObject) ObjectField("Forward Left Column Inside",
                                                                             tar.forwardLeftColumnInside,
                                                                             typeof(GameObject),
                                                                             true));
            SetValue(ref tar.forwardLeftColumnFacade,
                            (GameObject) ObjectField("Forward Left Column Facade",
                                                                             tar.forwardLeftColumnFacade,
                                                                             typeof(GameObject),
                                                                             true));
            
            
            SetValue(ref tar.backwardRightColumnInside,
                            (GameObject) ObjectField("Backward Right Column Inside",
                                                                               tar.backwardRightColumnInside, 
                                                                               typeof(GameObject), 
                                                                               true));
            SetValue(ref tar.backwardRightColumnFacade,
                            (GameObject) ObjectField("Backward Right Column Facade",
                                                                               tar.backwardRightColumnFacade, 
                                                                               typeof(GameObject), 
                                                                               true));
            

            SetValue(ref tar.backwardLeftColumnInside,
                            (GameObject) ObjectField("Backward Left Column Inside",
                                                                              tar.backwardLeftColumnInside,
                                                                              typeof(GameObject),
                                                                              true));
            SetValue(ref tar.backwardLeftColumnFacade,
                            (GameObject) ObjectField("Backward Left Column Facade",
                                                                              tar.backwardLeftColumnFacade,
                                                                              typeof(GameObject),
                                                                              true));
            
            
            
            

            SetValue(ref tar.propsContainer, (GameObject) ObjectField("Props Container",
                                                                          tar.propsContainer,
                                                                          typeof(GameObject),
                                                                          true));
                    
            GUILayout.Space(10);
            LabelField("Material IDs", textStyle.smallHeaderMontserrat, GUILayout.Height(25));
            SetValue(ref tar.floorMaterialId,    IntField("Floor Material ID",   tar.floorMaterialId));
            SetValue(ref tar.ceilingMaterialsId, IntField("Ceiling Material ID", tar.ceilingMaterialsId));
          
            GUILayout.Space(10);
            _showDebugInfo = ToggleLeft("Debug Info", _showDebugInfo);
            
            if (_showDebugInfo == false)
                return;
            
            GUILayout.Space(10);
            
            LabelField("Debug Info", textStyle.smallHeaderMontserrat, GUILayout.Height(25));
            
            LabelField("Coordinates: " + tar.x + tar.y);
            LabelField("Room id: "     + tar.roomId);
            LabelField("Interior id: " + tar.interiorId);
            
            ObjectField("Interior",  tar.interior, typeof(Interior), false);
            ObjectField("Props",     tar.props,    typeof(GameObject), true);
            if (tar.isIntersectedByProps)
            {
                LabelField("Intersected by: " + tar.intersectProps);
            }
        }
    }
}