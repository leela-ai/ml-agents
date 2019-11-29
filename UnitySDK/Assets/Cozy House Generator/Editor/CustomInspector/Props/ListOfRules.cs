using System;
using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Cozy_House_Generator.Editor.CustomInspector.Props
{
    public class ListOfRules
    {
        private           ReorderableList                listOfRules;
        private readonly  SerializedObject               serializedObject;
        private readonly  Action<RuleOfPropsGeneration>  setNewSelectedRule;
        private readonly  Func<RuleOfPropsGeneration>    getSelectedRule;
        private readonly  List<RuleOfPropsGeneration>    rules;
        
        //////////////////////////////////////////////////////////////////////////////////////////
        /// <param name="rules">                  List of props generation rules      </param>
        /// <param name="serializedObject">       Prop instance which will be modify  </param>
        /// <param name="setNewSelectedRule">     Select new rule delegate            </param>
        /// <param name="getSelectedRule">        Get selected rule delegate          </param>
        //////////////////////////////////////////////////////////////////////////////////////////
        public ListOfRules(List<RuleOfPropsGeneration> rules, SerializedObject serializedObject,
            Action<RuleOfPropsGeneration> setNewSelectedRule, Func<RuleOfPropsGeneration> getSelectedRule)
        {
            this.rules               = rules;
            this.serializedObject    = serializedObject;
            this.setNewSelectedRule  = setNewSelectedRule;
            this.getSelectedRule     = getSelectedRule;
            
            PrepareReorderableList();
        }


        //////////////////////////////////////////////////
        /// <summary>  Drawing list of rules  </summary>
        ///////////////////////////////////////////////// 
        public void Draw()
        {
            serializedObject  .Update();
            listOfRules       .DoLayoutList();
            serializedObject  .ApplyModifiedProperties();
        }
        

        ////////////////////////////////////////////////////////////////
        /// <summary>  Preparing list of rules for using  </summary>
        ///////////////////////////////////////////////////////////////
        private void PrepareReorderableList()
        {
            listOfRules = new ReorderableList(serializedObject,
                serializedObject.FindProperty("rulesOfGeneration"),
                true, true, true, true)
            {
                drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Possible places for placing props"); }
            };


            listOfRules.drawElementCallback = 
                (rect, index, isActive, isFocused) => {
                    var element = listOfRules.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("ruleName").stringValue);
				
                    EditorGUI.PropertyField(new Rect(rect.x + 200, rect.y, 30, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("isEnabled"), GUIContent.none);
                };
            
            listOfRules.onSelectCallback = list => {
                setNewSelectedRule(rules[list.index]);
            };
            
            listOfRules.onAddDropdownCallback = (buttonRect, l) => {
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent("New"),           false, clickHandlerMakeNew, l);
                menu.AddItem(new GUIContent("Copy selected"), false, clickHandlerCopySelected, l);
                    
                menu.ShowAsContext();
            };
            
            listOfRules.onAddCallback = list => 
            {
                var index   = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index  = index;
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("isEnabled").boolValue   = true;
                element.FindPropertyRelative("ruleName") .stringValue = "New rule";
            };
            
            
            listOfRules.onRemoveCallback = list => {
                if (list.count <= 0)
                    return;
                
                rules.RemoveAt(list.index);
                list.index   = rules.Count - 1;

                if (rules.Count - 1 > 1)
                    setNewSelectedRule(rules[rules.Count - 1]);
            };
            
        }
        
        /////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Event handler which handles a "Make New Rule" event  </summary>
        /// 
        /// <param name="target">  New rule  </param>
        ///////////////////////////////////////////////////////////////////////////////
        private void clickHandlerMakeNew(object target) {
            AddNewReorderableListElement(target);
            
            
            
            int newRuleIndex = rules.Count;
            
            if (rules.Count > 0)
                newRuleIndex--;
            
            rules[newRuleIndex].Reset();
            setNewSelectedRule(rules[newRuleIndex]);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Event handler which handles a "Copy Selected Rule" event  </summary>
        /// 
        /// <param name="target">  Copied rule  </param>
        ////////////////////////////////////////////////////////////////////////////////////
        private void clickHandlerCopySelected(object target) {
            
            int index = AddNewReorderableListElement(target);

            rules[index] = new RuleOfPropsGeneration(getSelectedRule());
            setNewSelectedRule(rules[rules.Count - 1]);
        }

        private int AddNewReorderableListElement(object target)
        {
            var l     = (ReorderableList)target;
            int index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index   = index;

            serializedObject.ApplyModifiedProperties();
            return index;
        }
    }
}