using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{
    ///////////////////////////////////////////////////////
    /// <summary>  Places columns on corners  </summary>
    //////////////////////////////////////////////////////
    public class PlaceColumns : IGeneratorPipe
    {
        private int                useCount;
        private UseOnFloorResolver floors;

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Places columns  </summary>
        /// 
        /// <param name="blueprint">             The blueprint that will be modded                      </param>
        /// <param name="interiors">             A list of interiors which will be applied              </param>
        /// <param name="facadeColumnMaterial">  House facade material (brick material for example)     </param>
        /// <param name="facadeMaterial">        House facade material (brick material for example)     </param>
        /// <param name="rnd">                   .Net standard random object                            </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Run(Blueprint blueprint, int floor, Interior[] interiors, Material facadeMaterial, 
                        Material facadeColumnMaterial, Random rnd)
        {
            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);

                    var forwardCell       = cell.neighbours.Forward(floor);
                    var backwardCell      = cell.neighbours.Backward(floor);
                    var rightCell         = cell.neighbours.Right(floor);
                    var leftCell          = cell.neighbours.Left(floor);
                    var forwardRightCell  = cell.neighbours.ForwardRight(floor);
                    var forwardLeftCell   = cell.neighbours.ForwardLeft(floor);
                    var backwardRightCell = cell.neighbours.BackwardRight(floor);
                    var backwardLeftCell  = cell.neighbours.BackwardLeft(floor);

                    int forwardRoomId       = forwardCell       == null ? -1 : forwardCell.RoomId;
                    int backwardRoomId      = backwardCell      == null ? -1 : backwardCell.RoomId;
                    int rightRoomId         = rightCell         == null ? -1 : rightCell.RoomId;
                    int leftRoomId          = leftCell          == null ? -1 : leftCell.RoomId;
                    int forwardRightRoomId  = forwardRightCell  == null ? -1 : forwardRightCell.RoomId;
                    int forwardLeftRoomId   = forwardLeftCell   == null ? -1 : forwardLeftCell.RoomId;
                    int backwardRightRoomId = backwardRightCell == null ? -1 : backwardRightCell.RoomId;
                    int backwardLeftRoomId  = backwardLeftCell  == null ? -1 : backwardLeftCell.RoomId;


                    bool isForwardRightCorner  = IsLooksLikeCorner(cell.RoomId, forwardRightRoomId,forwardRoomId, rightRoomId);
                    bool isForwardLeftCorner   = IsLooksLikeCorner(cell.RoomId, forwardLeftRoomId, forwardRoomId, leftRoomId);
                    bool isBackwardRightCorner = IsLooksLikeCorner(cell.RoomId, backwardRightRoomId, backwardRoomId, rightRoomId);
                    bool isBackwardLeftCorner  = IsLooksLikeCorner(cell.RoomId, backwardLeftRoomId, backwardRoomId, leftRoomId);


                    var columnsData = cell.columnsData;


                    if (isForwardRightCorner)
                        PlaceColumn(forwardRightCell, interiors, facadeColumnMaterial, columnsData.forwardRight);

                    if (isForwardLeftCorner)
                        PlaceColumn(forwardLeftCell, interiors, facadeColumnMaterial, columnsData.forwardLeft);

                    if (isBackwardRightCorner)
                        PlaceColumn(backwardRightCell, interiors, facadeColumnMaterial, columnsData.backwardRight);

                    if (isBackwardLeftCorner)
                        PlaceColumn(backwardLeftCell, interiors, facadeColumnMaterial, columnsData.backwardLeft);


                }
            }
        }


        private bool IsLooksLikeCorner(int currentRoomId, int aRoomId, int bRoomId, int cRoomId)
        {
            return aRoomId == bRoomId && bRoomId == cRoomId && cRoomId != currentRoomId;
        }
        
        
        public string GetPipeName()
        {
            return "Place Columns";
        }
        
        
        public int UseCount()
        {
            return useCount;
        }

        
        public UseOnFloorResolver FloorsRangeData()
        {
            return floors;
        }
        
        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new PlaceColumns {useCount = count, floors = floors};
        }

        
        private static void PlaceColumn(BlueprintCell cell, Interior[] interiors, Material facadeColumnMaterial, ColumnInfo columnInfo)
        {
            bool isFacadeColumn = cell == null || cell.IsRoom() == false;
                    
            if (isFacadeColumn)
            {
                columnInfo.type     = ColumnType.Facade;
                columnInfo.material = facadeColumnMaterial;
            }
            else
            {
                columnInfo.type     = ColumnType.Inside;
                columnInfo.material = interiors[cell.interiorId].column;
            }
        }
        
    }
}