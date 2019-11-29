using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using UnityEngine;

namespace Cozy_House_Generator.Editor.CustomInspector.Props
{
    public class CellToggles
    {
        private readonly ToggleStyle     toggleStyle            = new ToggleStyle();
        private const int                ToggleSize             = 75;
        private const int                Indent                 = 25;
        private CellRequirements         selectedCell;
        public  Dir                      selectedCellPosition;
        private bool                     isSelectedCentral;

        
        private RuleOfPropsGeneration lastSelectedRule;
        /////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Drawing grid of cell toggles  </summary>
        /// 
        /// <param name="indentOfGrid">            A space between the beginning of the
        ///                                        inspector window and the grid of cells  </param>
        /// <param name="selectedRule">            Selected rule which will be modify      </param>
        /// <param name="isSelectedCentralCell">   Is selected cell central?               </param>
        /// 
        /// <returns>  A cell which was selected  </returns>
        ////////////////////////////////////////////////////////////////////////////////////
        public CellRequirements Draw(int indentOfGrid, RuleOfPropsGeneration selectedRule, out bool isSelectedCentralCell)
        {

            if (lastSelectedRule != selectedRule)
            {
                lastSelectedRule = selectedRule;
                PressCentralCell(selectedRule);
            }
            
            
            if (selectedRule == null)
            {
                isSelectedCentralCell = false;
                return null;
            }

            GUILayout.Space(260);

            if (DrawCell(selectedRule.forwardLeftCellRequirements, selectedRule.lookAt, Indent, Indent + indentOfGrid,
                selectedCellPosition == Dir.ForwardLeft, false))
            {
                PressForwardLeftCell(selectedRule);
                isSelectedCentral = false;
            }

            if (DrawCell(selectedRule.forwardCellRequirements, selectedRule.lookAt, Indent + ToggleSize,
                Indent + indentOfGrid, selectedCellPosition == Dir.Forward, false))
            {
                PressForwardCell(selectedRule);
                isSelectedCentral = false;
            }

            if (DrawCell(selectedRule.forwardRightCellRequirements, selectedRule.lookAt, Indent + ToggleSize * 2,
                Indent + indentOfGrid, selectedCellPosition == Dir.ForwardRight, false))
            {
                PressForwardRightCell(selectedRule);
                isSelectedCentral = false;
            }

            if (DrawCell(selectedRule.leftCellRequirements, selectedRule.lookAt, Indent,
                Indent + ToggleSize + indentOfGrid, selectedCellPosition == Dir.Left, false))
            {
                PressLeftCell(selectedRule);
                isSelectedCentral = false;
            }

            if (DrawCell(selectedRule.centralCellRequirements, selectedRule.lookAt, Indent + ToggleSize,
                Indent + ToggleSize + indentOfGrid, selectedCellPosition == Dir.Null, true))
            {
                PressCentralCell(selectedRule);
                isSelectedCentral = true;
            }

            if (DrawCell(selectedRule.rightCellRequirements, selectedRule.lookAt, Indent + ToggleSize * 2,
                Indent + ToggleSize + indentOfGrid, selectedCellPosition == Dir.Right, false))
            {
                PressRightCell(selectedRule);
                isSelectedCentral = false;
            }

            if (DrawCell(selectedRule.backwardLeftCellRequirements, selectedRule.lookAt, Indent,
                Indent + ToggleSize * 2 + indentOfGrid, selectedCellPosition == Dir.BackwardLeft, false))
            {
                PressBackwardLeftCell(selectedRule);
                isSelectedCentral = false;
            }

            if (DrawCell(selectedRule.backwardCellRequirements, selectedRule.lookAt, Indent + ToggleSize,
                Indent + ToggleSize * 2 + indentOfGrid, selectedCellPosition == Dir.Backward, false))
            {
                PressBackwardCell(selectedRule);
                isSelectedCentral = false;
            }

            if (DrawCell(selectedRule.backwardRightCellRequirements, selectedRule.lookAt, Indent + ToggleSize * 2,
                Indent + ToggleSize * 2 + indentOfGrid, selectedCellPosition == Dir.BackwardRight, false))
            {
                PressBackwardRightCell(selectedRule);
                isSelectedCentral = false;
            }

            isSelectedCentralCell = isSelectedCentral;
            return selectedCell;
        }



        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Drawing a cell which shows cell requirements  </summary>
        /// 
        /// <param name="cellRequirements">  Info about requirements of the cell    </param>
        /// <param name="propsLookAtDir">    Look at direction of props             </param>
        /// <param name="posX">              The position of a cell by X axis       </param>
        /// <param name="posY">              The position of a cell by Y axis       </param>
        /// <param name="isSelected">        Is this cell selected?                 </param>
        /// <param name="isCentral">         Is this a central cell?                </param>
        /// 
        /// <returns>  Status of a cell selection (Is this cell selected?)  </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        private bool DrawCell(CellRequirements cellRequirements, SimpleDir propsLookAtDir, int posX, int posY, 
            bool isSelected, bool isCentral)
        {
            Rect toggleRect = new Rect(posX, posY, ToggleSize, ToggleSize);
            
            if (isSelected)
                GUI.DrawTexture(toggleRect, toggleStyle.floor);
            
            switch (cellRequirements.state)
            {
                case CellStateForToggle.Any:
                    GUI.DrawTexture(toggleRect, toggleStyle.any);
                    return DrawInvisibleToggle(toggleRect, false);
                    
                case CellStateForToggle.OutsideOfTheRoom:
                    GUI.DrawTexture(toggleRect, toggleStyle.forbidden);
                    return DrawInvisibleToggle(toggleRect, false);
            }
            
            DrawWall(cellRequirements.forwardWall,  posX,                   posY,                   true);
            DrawWall(cellRequirements.rightWall,    posX + ToggleSize - 16, posY,                   false);
            DrawWall(cellRequirements.leftWall,     posX,                   posY,                   false);
            
            
            if (isCentral || cellRequirements.state == CellStateForToggle.PropsZone)
            {
                switch (propsLookAtDir)
                {
                    case SimpleDir.Forward:
                        GUI.DrawTexture(toggleRect, toggleStyle.propLooksForward);
                        break;
                    case SimpleDir.Backward:
                        GUI.DrawTexture(toggleRect, toggleStyle.propLooksBackward);
                        break;
                    case SimpleDir.Right:
                        GUI.DrawTexture(toggleRect, toggleStyle.propLooksRight);
                        break;
                    case SimpleDir.Left:
                        GUI.DrawTexture(toggleRect, toggleStyle.propLooksLeft);
                        break;
                }
            }
            DrawWall(cellRequirements.backwardWall, posX,                   posY + ToggleSize - 16, true);

            if (isCentral == false && cellRequirements.state != CellStateForToggle.PropsZone && cellRequirements.state == CellStateForToggle.OtherProps)
                GUI.DrawTexture(toggleRect, toggleStyle.props1);

            return DrawInvisibleToggle(toggleRect, false);
        }

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Drawing invisible toggle on top of the cell which helps to select the cell  </summary>
        /// 
        /// <param name="rect">        Toggle position and size  </param>
        /// <param name="isSelected">  Is this cell selected?    </param>
        /// 
        /// <returns>  True if a toggle pressed  </returns>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool DrawInvisibleToggle(Rect rect, bool isSelected)
        {
            return GUI.Toggle(rect, isSelected, "", toggleStyle.cellToggle);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Drawing wall in the cell which shows requirements of the wall  </summary>
        /// 
        /// <param name="wallType">      How must be a wall? Is this a door, window or something else?  </param>
        /// <param name="posX">          The position of a cell by X axis                               </param>
        /// <param name="posY">          The position of a cell by Y axis                               </param>
        /// <param name="isHorizontal">  Is this a horizontal wall?                                     </param>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DrawWall( WallTypeEditor wallType, int posX, int posY, bool isHorizontal )
        {          
            switch (wallType)
            {
                case WallTypeEditor.Any:
                    if (isHorizontal)
                        GUI.DrawTexture(new Rect(posX, posY, ToggleSize, 16), toggleStyle.wallAnyHorizontal);
                    else
                        GUI.DrawTexture(new Rect(posX, posY, 16, ToggleSize), toggleStyle.wallAnyVertical);
                    return;
                
                case WallTypeEditor.AnyWall:
                    if (isHorizontal)
                        GUI.DrawTexture(new Rect(posX, posY, ToggleSize, 16), toggleStyle.doorOrWallOrWindowHorizontal);
                    else
                        GUI.DrawTexture(new Rect(posX, posY, 16, ToggleSize), toggleStyle.doorOrWallOrWindowVertical);
                    return;
                
                case WallTypeEditor.Wall:
                    if (isHorizontal)
                        GUI.DrawTexture(new Rect(posX, posY, ToggleSize, 16), toggleStyle.wallHorizontal);
                    else
                        GUI.DrawTexture(new Rect(posX, posY, 16, ToggleSize), toggleStyle.wallVertical);
                    return;
                
                case WallTypeEditor.Door:
                    if (isHorizontal)
                        GUI.DrawTexture(new Rect(posX, posY, ToggleSize, 16), toggleStyle.doorHorizontal);
                    else
                        GUI.DrawTexture(new Rect(posX, posY, 16, ToggleSize), toggleStyle.doorVertical);
                    return;
                
                case WallTypeEditor.Window:
                    if (isHorizontal)
                        GUI.DrawTexture(new Rect(posX, posY, ToggleSize, 16), toggleStyle.windowHorizontal);
                    else
                        GUI.DrawTexture(new Rect(posX, posY, 16, ToggleSize), toggleStyle.windowVertical);
                    return;
                    
                case WallTypeEditor.WallOrDoor:
                    if (isHorizontal)
                        GUI.DrawTexture(new Rect(posX, posY, ToggleSize, 16), toggleStyle.doorOrWallHorizontal);
                    else
                        GUI.DrawTexture(new Rect(posX, posY, 16, ToggleSize), toggleStyle.doorOrWindowVertical);
                    return;
                
                case WallTypeEditor.WallOrWindow:
                    if (isHorizontal)
                        GUI.DrawTexture(new Rect(posX, posY, ToggleSize, 16), toggleStyle.wallOrWindowHorizontal);
                    else
                        GUI.DrawTexture(new Rect(posX, posY, 16, ToggleSize), toggleStyle.wallOrWindowVertical);
                    return;
                
                case WallTypeEditor.DoorOrWindow:
                    if (isHorizontal)
                        GUI.DrawTexture(new Rect(posX, posY, ToggleSize, 16), toggleStyle.doorOrWindowHorizontal);
                    else
                        GUI.DrawTexture(new Rect(posX, posY, 16, ToggleSize), toggleStyle.doorOrWindowVertical);
                    return;
            }
            
            
           
        }
        


        #region Press

        private void PressForwardLeftCell(RuleOfPropsGeneration selectedRule)
        {
            selectedCell           = selectedRule.forwardLeftCellRequirements;
            selectedCellPosition   = Dir.ForwardLeft;
        }
        
        private void PressForwardCell(RuleOfPropsGeneration selectedRule)
        {
            selectedCell          = selectedRule.forwardCellRequirements;
            selectedCellPosition  = Dir.Forward;
        }
        
        private void PressForwardRightCell(RuleOfPropsGeneration selectedRule)
        {
            selectedCell            = selectedRule.forwardRightCellRequirements;
            selectedCellPosition    = Dir.ForwardRight;
        }
        
        private void PressLeftCell(RuleOfPropsGeneration selectedRule)
        {
            selectedCell           = selectedRule.leftCellRequirements;
            selectedCellPosition   = Dir.Left;
        }
        
        private void PressCentralCell(RuleOfPropsGeneration selectedRule)
        {
            selectedCell           = selectedRule.centralCellRequirements;
            selectedCellPosition   = Dir.Null;
        }
        
        private void PressRightCell(RuleOfPropsGeneration selectedRule)
        {
            selectedCell          = selectedRule.rightCellRequirements;
            selectedCellPosition  = Dir.Right;
        }
        
        private void PressBackwardLeftCell(RuleOfPropsGeneration selectedRule)
        {
            selectedCell             = selectedRule.backwardLeftCellRequirements;
            selectedCellPosition     = Dir.BackwardLeft;
        }
        
        private void PressBackwardCell(RuleOfPropsGeneration selectedRule)
        {
            selectedCell          = selectedRule.backwardCellRequirements;
            selectedCellPosition  = Dir.Backward;
        }
        
        private void PressBackwardRightCell(RuleOfPropsGeneration selectedRule)
        {
            selectedCell              = selectedRule.backwardRightCellRequirements;
            selectedCellPosition      = Dir.BackwardRight;
        }
        
        #endregion
    }
}