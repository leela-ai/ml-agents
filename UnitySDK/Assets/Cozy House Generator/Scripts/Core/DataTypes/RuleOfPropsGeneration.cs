using System;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;

namespace Cozy_House_Generator.Scripts.Core.DataTypes
{   ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Rules which define props placing  </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class RuleOfPropsGeneration
    {
        public  bool              isEnabled                        = true;
        public  string            ruleName                         = "New Rule";
        public  SimpleDir         lookAt;
        public  CellRequirements  forwardCellRequirements;
        public  CellRequirements  backwardCellRequirements;
        public  CellRequirements  rightCellRequirements;
        public  CellRequirements  leftCellRequirements;
        public  CellRequirements  forwardRightCellRequirements;
        public  CellRequirements  forwardLeftCellRequirements;
        public  CellRequirements  backwardRightCellRequirements;
        public  CellRequirements  backwardLeftCellRequirements;
        public  CellRequirements  centralCellRequirements;

        public RuleOfPropsGeneration(){}

        public void Reset()
        {
            isEnabled                     = true;
            ruleName                      = "New Rule";
            lookAt                        = SimpleDir.Backward;
            forwardCellRequirements       = new CellRequirements();
            backwardCellRequirements      = new CellRequirements();
            rightCellRequirements         = new CellRequirements();
            leftCellRequirements          = new CellRequirements();
            forwardRightCellRequirements  = new CellRequirements();
            forwardLeftCellRequirements   = new CellRequirements();
            backwardRightCellRequirements = new CellRequirements();
            backwardLeftCellRequirements  = new CellRequirements();
            centralCellRequirements       = new CellRequirements      {state = CellStateForToggle.PropsZone};
        }
        
        //////////////////////////////////////////////////////////////////////////////
        /// <summary> Dublicating  </summary>
        /// 
        /// <param name="copySource">  An object which will be dublicated  </param>
        /////////////////////////////////////////////////////////////////////////////
        public RuleOfPropsGeneration(RuleOfPropsGeneration copySource)
        {
            isEnabled = copySource.isEnabled;
            ruleName  = copySource.ruleName;
            lookAt    = copySource.lookAt;
            forwardCellRequirements       = new CellRequirements(copySource.forwardCellRequirements);
            backwardCellRequirements      = new CellRequirements(copySource.backwardCellRequirements);
            rightCellRequirements         = new CellRequirements(copySource.rightCellRequirements);
            leftCellRequirements          = new CellRequirements(copySource.leftCellRequirements);
            forwardRightCellRequirements  = new CellRequirements(copySource.forwardRightCellRequirements);
            forwardLeftCellRequirements   = new CellRequirements(copySource.forwardLeftCellRequirements);
            backwardRightCellRequirements = new CellRequirements(copySource.backwardRightCellRequirements);
            backwardLeftCellRequirements  = new CellRequirements(copySource.backwardLeftCellRequirements);
            centralCellRequirements       = new CellRequirements(copySource.centralCellRequirements);
        }

        
        ////////////////////////////////////////////////////////////////////
        /// <summary>  Returns true if requirements satisfied  </summary>
        /// 
        /// <param name="cell">  Info for checking  </param>
        /// 
        /// <returns>  Can you place the props?  </returns>
        ///////////////////////////////////////////////////////////////////
        public bool IsOkForPlacing(BlueprintCell cell, int floor)
        {
            return forwardCellRequirements        .IsApply(cell.neighbours.Forward(floor),       cell.RoomId) &&
                   backwardCellRequirements       .IsApply(cell.neighbours.Backward(floor),      cell.RoomId) &&
                   rightCellRequirements          .IsApply(cell.neighbours.Right(floor),         cell.RoomId) &&
                   centralCellRequirements        .IsApply(cell,                            cell.RoomId) &&
                   leftCellRequirements           .IsApply(cell.neighbours.Left(floor),          cell.RoomId) &&
                   forwardRightCellRequirements   .IsApply(cell.neighbours.ForwardRight(floor),  cell.RoomId) &&
                   forwardLeftCellRequirements    .IsApply(cell.neighbours.ForwardLeft(floor),   cell.RoomId) &&
                   backwardRightCellRequirements  .IsApply(cell.neighbours.BackwardRight(floor), cell.RoomId) &&
                   backwardLeftCellRequirements   .IsApply(cell.neighbours.BackwardLeft(floor),  cell.RoomId);

        }
        
        
        /////////////////////////////////////////////////////////////////////////////
        /// <summary>  Rotating rule of props generation scheme right  </summary>
        ////////////////////////////////////////////////////////////////////////////
        public RuleOfPropsGeneration RotateRight()
        {
            forwardCellRequirements.        RotateRight();
            backwardCellRequirements.       RotateRight();
            rightCellRequirements.          RotateRight();
            leftCellRequirements.           RotateRight();
            forwardRightCellRequirements.   RotateRight();
            forwardLeftCellRequirements.    RotateRight();
            backwardRightCellRequirements.  RotateRight();
            backwardLeftCellRequirements.   RotateRight();
            centralCellRequirements.        RotateRight();

            var cache                 = forwardCellRequirements;
            forwardCellRequirements   = leftCellRequirements;
            leftCellRequirements      = backwardCellRequirements;
            backwardCellRequirements  = rightCellRequirements;
            rightCellRequirements     = cache;

            cache                         = forwardRightCellRequirements;
            forwardRightCellRequirements  = forwardLeftCellRequirements;
            forwardLeftCellRequirements   = backwardLeftCellRequirements;
            backwardLeftCellRequirements  = backwardRightCellRequirements;
            backwardRightCellRequirements = cache;
            
            switch (lookAt)
            {
                case SimpleDir.Forward:
                    lookAt = SimpleDir.Right;
                    break;
                
                case SimpleDir.Right:
                    lookAt = SimpleDir.Backward;
                    break;
                
                case SimpleDir.Backward:
                    lookAt = SimpleDir.Left;
                    break;
                
                case SimpleDir.Left:
                    lookAt = SimpleDir.Forward;
                    break;
            }

            return this;
        }


        /////////////////////////////////////////////////////////////////////////////
        /// <summary>  Rotating rule of props generation scheme left  </summary>
        ////////////////////////////////////////////////////////////////////////////
        public RuleOfPropsGeneration RotateLeft()
        {
            forwardCellRequirements.        RotateLeft();
            backwardCellRequirements.       RotateLeft();
            rightCellRequirements.          RotateLeft();
            leftCellRequirements.           RotateLeft();
            forwardRightCellRequirements.   RotateLeft();
            forwardLeftCellRequirements.    RotateLeft();
            backwardRightCellRequirements.  RotateLeft();
            backwardLeftCellRequirements.   RotateLeft();
            centralCellRequirements.        RotateLeft();

            var cache                 = forwardCellRequirements;
            forwardCellRequirements   = rightCellRequirements;
            rightCellRequirements     = backwardCellRequirements;
            backwardCellRequirements  = leftCellRequirements;
            leftCellRequirements      = cache;

            cache                          = forwardRightCellRequirements;
            forwardRightCellRequirements   = backwardRightCellRequirements;
            backwardRightCellRequirements  = backwardLeftCellRequirements;
            backwardLeftCellRequirements   = forwardLeftCellRequirements;
            forwardLeftCellRequirements    = cache;
        
            switch (lookAt)
            {
                case SimpleDir.Forward:
                    lookAt = SimpleDir.Left;
                    break;
                
                case SimpleDir.Left:
                    lookAt = SimpleDir.Backward;
                    break;
                
                case SimpleDir.Backward:
                    lookAt = SimpleDir.Right;
                    break;
                
                case SimpleDir.Right:
                    lookAt = SimpleDir.Forward;
                    break;
                
            }

            return this;
        }
    }
}