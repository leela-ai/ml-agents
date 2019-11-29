using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Cozy_House_Generator.Editor.CustomInspector.Props
{
    [CustomEditor(typeof(Scripts.ScriptableObjects.Props))]
    public class PropsEditor : UnityEditor.Editor
    {
        private Scripts.ScriptableObjects.Props                  tar;
        private CellToggles            cellToggles;
        private ListOfRules            listOfRules;
        private RuleOfPropsGeneration  selectedRule;
        private readonly TextStyle     textStyle      = new TextStyle();

        
        private void OnEnable()
        {
            tar = (Scripts.ScriptableObjects.Props) target;
            textStyle.Initialize();
            
            if (tar.rulesOfGeneration == null)
                tar.rulesOfGeneration = new List<RuleOfPropsGeneration>();
            
            listOfRules = new ListOfRules(tar.rulesOfGeneration, serializedObject, SelectRule, GetSelectedRule);
            cellToggles = new CellToggles();
        }

        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("PROPS", textStyle.headerMontserrat, GUILayout.Height(45));
            GUILayout.Space(20);

            EditorGUILayout.LabelField("General", textStyle.smallHeaderMontserrat, GUILayout.Height(25));
            DrawMainSettings(tar);
            GUILayout.Space(30);
            
            EditorGUILayout.LabelField("Combine Settings", textStyle.smallHeaderMontserrat, GUILayout.Height(25));
            DrawCombineSettings(tar);
            GUILayout.Space(20);
            
            EditorGUILayout.LabelField("Origin Position", textStyle.smallHeaderMontserrat, GUILayout.Height(25));
            DrawPositionSettings(tar);
            GUILayout.Space(40);
            
            EditorGUILayout.LabelField("Place Requirements", textStyle.smallHeaderMontserrat, GUILayout.Height(25));
            listOfRules.Draw();

            if (tar.rulesOfGeneration != null && tar.rulesOfGeneration.Count > 0)
            {
                DrawRuleNameField(selectedRule);
                bool isSelectedCentralCell;
                DrawCellSettings(
                    cellToggles.Draw(tar.rulesOfGeneration.Count * 20 + 500, selectedRule, out isSelectedCentralCell),
                    selectedRule, cellToggles, isSelectedCentralCell);
            }
            
            Repaint();
            EditorUtility.SetDirty(tar);
        }

        
        ////////////////////////////////////////////////////////////////
        /// <summary>  Draw prefab field and type field  </summary>
        /// 
        /// <param name="tar">  Props which will be modify  </param>
        //////////////////////////////////////////////////////////////
        private static void DrawMainSettings(Scripts.ScriptableObjects.Props tar)
        {
            tar.propsType = (PropsType) EditorGUILayout.EnumPopup("Type", tar.propsType);
            tar.prefab    = EditorGUILayout.ObjectField("Prefab", tar.prefab, typeof(GameObject), false) as GameObject;
        }

        
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Drawing settings which determine where an object will be placed  </summary>
        /// 
        /// <param name="tar">  Props which will be modify  </param>
        ////////////////////////////////////////////////////////////////////////////////////////////
        private static void DrawPositionSettings(Scripts.ScriptableObjects.Props tar)
        {
            tar.cloneLookAtForwardTarget =
                EditorGUILayout.ObjectField("Target", tar.cloneLookAtForwardTarget, typeof(Transform), true) as Transform;
            
            if (GUILayout.Button("Clone Position Info") && tar.cloneLookAtForwardTarget != null)
            {
                tar.lookAtForwardLocalPos = tar.cloneLookAtForwardTarget.localPosition;
                tar.lookAtForwardLocalRot = tar.cloneLookAtForwardTarget.localRotation;
            }
            
            GUILayout.Label("Origin Position");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            tar.lookAtForwardLocalPos = EditorGUILayout.Vector3Field("", tar.lookAtForwardLocalPos);
            EditorGUILayout.EndHorizontal();
                
            GUILayout.Label("Origin Rotation");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            tar.lookAtForwardLocalRot = Quaternion.Euler(EditorGUILayout.Vector3Field("", tar.lookAtForwardLocalRot.eulerAngles));
            EditorGUILayout.EndHorizontal();
        }


        private static void DrawCombineSettings(Scripts.ScriptableObjects.Props tar)
        {
            tar.combineAfterInstantiate = EditorGUILayout.Toggle("Combine", tar.combineAfterInstantiate);
            tar.shadowCastingMode =
                (ShadowCastingMode) EditorGUILayout.EnumPopup("Shadows", tar.shadowCastingMode);
//            tar.combinedLODPercentForCulling =
//                EditorGUILayout.IntSlider("Combined LOD Percent For Culling", tar.combinedLODPercentForCulling, 1, 25);
        }

        
        /////////////////////////////////////////////////////////////////
        /// <summary>  Drawing name of rule field  </summary>
        /// 
        /// <param name="rule">  Rule which will be modify  </param>
        ////////////////////////////////////////////////////////////////
        private static void DrawRuleNameField(RuleOfPropsGeneration rule)
        {
            if (rule == null)
                return;
            rule.ruleName = EditorGUILayout.TextField("Rule name:", rule.ruleName);
        }              
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////        
        /// <summary>  Drawing cell settings, which determine how must be a cell for placing props  </summary>
        /// 
        /// <param name="cell">                   Selected cell which will be modify             </param>
        /// <param name="rule">                   Selected rule                                  </param>
        /// <param name="cellToggles">            CellToggles instance which draw cell toggles   </param>
        /// <param name="isSelectedCellCentral">  Is selected cell central?                      </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void DrawCellSettings(CellRequirements cell, RuleOfPropsGeneration rule, CellToggles cellToggles, 
            bool isSelectedCellCentral)
        {
            if (cell == null || rule == null || cellToggles == null) 
                return;
            
            if (isSelectedCellCentral)
            {
                cell.state = CellStateForToggle.PropsZone;
            }
            else
            {
                cell.state = (CellStateForToggle) EditorGUILayout.EnumPopup(
                    "State: ",
                    cell.state,
                    GUILayout.MinWidth(10), GUILayout.MaxWidth(250));
            }
            
            DrawRequirePropsSettings(cell, cellToggles.selectedCellPosition == Dir.Null);
            
            if (isSelectedCellCentral || cell.state == CellStateForToggle.PropsZone)
                rule.lookAt = (SimpleDir)EditorGUILayout.EnumPopup("Props Direction", rule.lookAt);
            
            EditorGUILayout.Space();
            DrawCellWallsSettings(cell);
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Drawing walls settings, which determine how must be walls for placing props  </summary>
        /// 
        /// <param name="cell">  Selected cell which will be modify  </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void DrawCellWallsSettings(CellRequirements cell)
        {
            if (cell == null || cell.state == CellStateForToggle.Any || cell.state == CellStateForToggle.OutsideOfTheRoom) 
                return;
            
            cell.forwardWall = (WallTypeEditor) EditorGUILayout.EnumPopup(
                "Forward Wall: ",
                cell.forwardWall,
                GUILayout.MinWidth(10), GUILayout.MaxWidth(250));
            
            cell.backwardWall = (WallTypeEditor) EditorGUILayout.EnumPopup(
                "Backward Wall: ",
                cell.backwardWall,
                GUILayout.MinWidth(10), GUILayout.MaxWidth(250));

            cell.rightWall = (WallTypeEditor) EditorGUILayout.EnumPopup(
                "Right Wall: ",
                cell.rightWall,
                GUILayout.MinWidth(10), GUILayout.MaxWidth(250));
            
            cell.leftWall = (WallTypeEditor) EditorGUILayout.EnumPopup(
                "Left Wall: ",
                cell.leftWall,
                GUILayout.MinWidth(10), GUILayout.MaxWidth(250));
        }
        

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Drawing settings of the props which must be on the cell  </summary>
        /// 
        /// <param name="cell">                    Selected cell which will be modify                  </param>
        /// <param name="selectedCellIsCentral">   True if a cell is in the center of the cell grid    </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void DrawRequirePropsSettings(CellRequirements cell, bool selectedCellIsCentral)
        {
            if (cell == null || cell.state == CellStateForToggle.Any || cell.state == CellStateForToggle.OutsideOfTheRoom || selectedCellIsCentral)
                return;

            if (cell.state == CellStateForToggle.OtherProps)
            {
                cell.propsThatCanBe = (PropsType) EditorGUILayout.EnumFlagsField("Possible Props: ",
                                                                                 cell.propsThatCanBe,
                                                                                 GUILayout.MinWidth(10),
                                                                                 GUILayout.MaxWidth(250));
            }
        }
        
       
        private void SelectRule(RuleOfPropsGeneration newSelectedRule)
        {
            selectedRule = newSelectedRule;
        }


        private RuleOfPropsGeneration GetSelectedRule()
        {
            return selectedRule;
        }

    }
}