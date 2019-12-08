using System;
using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.ScriptableObjects
{   ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Scriptable Object what instantiates some props by special rules  </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    [CreateAssetMenu(menuName = "Cozy House Generator/Props")]
    public class Props : ScriptableObject
    {
        public GameObject                   prefab;
        public PropsType                    propsType;
        public Vector3                      lookAtForwardLocalPos;
        public Quaternion                   lookAtForwardLocalRot;
        public Transform                    cloneLookAtForwardTarget;
        public List<RuleOfPropsGeneration>  rulesOfGeneration;
        public bool                         combineAfterInstantiate   = true;
        public ShadowCastingMode            shadowCastingMode         = ShadowCastingMode.On;

        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Places props if room responds requirements which define in Generation Rules.  </summary>
        /// 
        /// <param name="blueprint">  The blueprint what will be modified  </param>
        /// <param name="roomId">     Current room ID                      </param>
        /// <param name="rnd">        .Net standard random object          </param>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void TryToPlace(Blueprint blueprint,int floor, int roomId, Random rnd)
        {
            var cellToPlace = FindSpaceToPlaceEvenly(blueprint, floor, roomId, out var acceptedRule, rnd); 

            if (cellToPlace == null) 
                return;
            
            cellToPlace.propsToInstantiate    = this;
            cellToPlace.lookAtForwardLocalPos = lookAtForwardLocalPos;
            cellToPlace.lookAtForwardLocalRot = lookAtForwardLocalRot;
            cellToPlace.propsDirection        = acceptedRule.lookAt;

            if (acceptedRule.forwardCellRequirements.state == CellStateForToggle.PropsZone)
                SetIsPropZoneStatus(cellToPlace.neighbours.Forward(floor));
            
            if (acceptedRule.backwardCellRequirements.state == CellStateForToggle.PropsZone)
                SetIsPropZoneStatus(cellToPlace.neighbours.Backward(floor));
            
            if (acceptedRule.rightCellRequirements.state == CellStateForToggle.PropsZone)
                SetIsPropZoneStatus(cellToPlace.neighbours.Right(floor));
            
            if (acceptedRule.leftCellRequirements.state == CellStateForToggle.PropsZone)
                SetIsPropZoneStatus(cellToPlace.neighbours.Left(floor));
            
            if (acceptedRule.forwardRightCellRequirements.state == CellStateForToggle.PropsZone)
                SetIsPropZoneStatus(cellToPlace.neighbours.ForwardRight(floor));
            
            if (acceptedRule.forwardLeftCellRequirements.state == CellStateForToggle.PropsZone)
                SetIsPropZoneStatus(cellToPlace.neighbours.ForwardLeft(floor));
            
            if (acceptedRule.backwardRightCellRequirements.state == CellStateForToggle.PropsZone)
                SetIsPropZoneStatus(cellToPlace.neighbours.BackwardRight(floor));
            
            if (acceptedRule.backwardLeftCellRequirements.state == CellStateForToggle.PropsZone)
                SetIsPropZoneStatus(cellToPlace.neighbours.BackwardLeft(floor));
        }

        private void SetIsPropZoneStatus(BlueprintCell cell)
        {
            if (cell != null)
            {
                cell.isPropsZone = true;
                cell.typeOfPropsOnPropsZone = propsType;
            }
        }

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Finds space for props that responds requirements which define in Generation Rules.
        ///            This algorithm fills the room fast.                                                    </summary>
        /// 
        /// <param name="blueprint">     The blueprint what will be modified    </param>
        /// <param name="roomId">        Current room ID                        </param>
        /// <param name="acceptedRule">  Returns a rule which was satisfied     </param>
        /// 
        /// <returns>  Cell what responds requirements  </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private BlueprintCell FindSpaceToPlaceFast(Blueprint blueprint, int floor, int roomId, out RuleOfPropsGeneration acceptedRule)
        {
            if (rulesOfGeneration == null)
            {
                acceptedRule = null;
                return null;
            }

            foreach (var rule in rulesOfGeneration)
            {
                
                var currentRule = new RuleOfPropsGeneration(rule);
                
                if (currentRule.isEnabled == false)
                    continue;

                for (int x = 0; x < blueprint.size; x++)
                {
                    for (int y = 0; y < blueprint.size; y++)
                    {
                        var cell = blueprint.GetCell(floor, x, y);
                        
                        if (cell.RoomId             != roomId ||
                            cell.propsToInstantiate != null   ||
                            cell.isPropsZone)
                            continue;

                        if (currentRule.IsOkForPlacing(cell, floor))
                        {
                            acceptedRule = currentRule;
                            return cell;
                        }

                        currentRule.RotateRight();
                        if (currentRule.IsOkForPlacing(cell, floor))
                        {
                            acceptedRule = currentRule;
                            return cell;
                        }

                        currentRule.RotateRight();
                        if (currentRule.IsOkForPlacing(cell, floor))
                        {
                            acceptedRule = currentRule;
                            return cell;
                        }

                        currentRule.RotateRight();
                        if (currentRule.IsOkForPlacing(cell, floor))
                        {
                            acceptedRule = currentRule;
                            return cell;
                        }
                    }
                }
            }

            acceptedRule = null;
            return null;
        }  
        
        
        private class SuitablePlace
        {
            public BlueprintCell         cell;
            public RuleOfPropsGeneration acceptedRule;
        }
        
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Finds space for props that responds requirements which define in Generation Rules.
        ///            This algorithm tries to fill the room evenly.                                        </summary>
        /// 
        /// <param name="blueprint">     The blueprint what will be modified    </param>
        /// <param name="roomId">        Current room ID                        </param>
        /// <param name="acceptedRule">  Returns a rule which was satisfied     </param>
        /// <param name="rnd">           .Net standard random object            </param>
        /// 
        /// <returns>  Cell what responds requirements  </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private BlueprintCell FindSpaceToPlaceEvenly(Blueprint blueprint, int floor, int roomId, out RuleOfPropsGeneration acceptedRule, Random rnd)
        {
            if (rulesOfGeneration == null)
            {
                acceptedRule = null;
                return null;
            }
            
            var suitablePlaces = new List<SuitablePlace>();

            foreach (var rule in rulesOfGeneration)
            {
                var currentRule = new RuleOfPropsGeneration(rule);
                if (currentRule.isEnabled == false)
                    continue;

                for (int x = 0; x < blueprint.size; x++)
                {
                    for (int y = 0; y < blueprint.size; y++)
                    {
                        var cell = blueprint.GetCell(floor, x, y);
                        if (IsPlaceOk(cell, roomId) == false)
                            continue;

                        CheckForPlace(currentRule,               cell, suitablePlaces, floor);
                        CheckForPlace(currentRule.RotateRight(), cell, suitablePlaces, floor);
                        CheckForPlace(currentRule.RotateRight(), cell, suitablePlaces, floor);
                        CheckForPlace(currentRule.RotateRight(), cell, suitablePlaces, floor);
                    }
                }
            }


            if (suitablePlaces.Count > 1)
            {
                int randIndex = rnd.Next(0, suitablePlaces.Count - 1);
                acceptedRule = suitablePlaces[randIndex].acceptedRule;
                return suitablePlaces[randIndex].cell;
            }

            if (suitablePlaces.Count == 1)
            {
                acceptedRule = suitablePlaces[0].acceptedRule;
                return suitablePlaces[0].cell;
            }

            acceptedRule = null;
            return null;
        }
        

        private static bool IsPlaceOk(BlueprintCell cell, int roomId)
        {
            return cell.RoomId             == roomId &&
                   cell.propsToInstantiate == null   &&
                   cell.isPropsZone        == false;
        }
        

        private static void CheckForPlace(RuleOfPropsGeneration rule, BlueprintCell cell, ICollection<SuitablePlace> suitablePlaces, int floor)
        {
            if (rule.IsOkForPlacing(cell, floor))
            {
                suitablePlaces.Add(new SuitablePlace
                {
                    acceptedRule = new RuleOfPropsGeneration(rule),
                    cell         = cell
                });
            }
        }
        
        
    }
}