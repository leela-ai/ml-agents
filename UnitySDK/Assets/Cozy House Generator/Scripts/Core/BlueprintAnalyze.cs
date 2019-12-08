namespace Cozy_House_Generator.Scripts.Core
{
    public static class BlueprintAnalyze
    {
        //////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns true if current cell and cell by direction are equal  </summary>
        /////////////////////////////////////////////////////////////////////////////////////////
        public static class IsSameRoom
        {   ///////////////////////////////////////////////////////////////////////////////////
            /// <summary>  Return true if current cell and forward cell are equal  </summary>
            /// 
            /// <param name="x">            x coordinate of the current cell      </param>
            /// <param name="y">            y coordinate of the current cell      </param>
            /// <param name="blueprint">    Current blueprint                     </param>
            //////////////////////////////////////////////////////////////////////////////////
            public static bool Forward (int floor, int x, int y, Blueprint blueprint)
            {
                var cell = blueprint.GetCell(floor, x, y);
                if (cell == null)
                    return false;

                return IsRoomsSame(cell, cell.neighbours.Forward(floor));
            }


            private static bool IsRoomsSame(BlueprintCell cellA, BlueprintCell cellB)
            {
                if (cellA == null || cellB == null || cellA.IsRoom() == false || cellB.IsRoom() == false)
                    return false;

                return cellA.RoomId == cellB.RoomId;
            }
            
            
            ///////////////////////////////////////////////////////////////////////////////////
            /// <summary>  Returns true if current cell and backward cell are equal  </summary>
            /// 
            /// <param name="x">            x coordinate of the current cell      </param>
            /// <param name="y">            y coordinate of the current cell      </param>
            /// <param name="blueprint">    Current blueprint                     </param>
            //////////////////////////////////////////////////////////////////////////////////
            public static bool Backward (int floor, int x, int y, Blueprint blueprint)
            {
                var cell = blueprint.GetCell(floor, x, y);
                if (cell == null)
                    return false;

                return IsRoomsSame(cell, cell.neighbours.Backward(floor));
            }
            
            
            ///////////////////////////////////////////////////////////////////////////////////
            /// <summary>  Returns true if current cell and right cell are equal  </summary>
            /// 
            /// <param name="x">            x coordinate of the current cell      </param>
            /// <param name="y">            y coordinate of the current cell      </param>
            /// <param name="blueprint">    Current blueprint                     </param>
            //////////////////////////////////////////////////////////////////////////////////
            public static bool Right (int floor, int x, int y, Blueprint blueprint)
            {
                var cell = blueprint.GetCell(floor, x, y);
                if (cell == null)
                    return false;

                return IsRoomsSame(cell, cell.neighbours.Right(floor));
            }
            
            
            ///////////////////////////////////////////////////////////////////////////////////
            /// <summary>  Returns true if current cell and left cell are equal  </summary>
            /// 
            /// <param name="x">            x coordinate of the current cell      </param>
            /// <param name="y">            y coordinate of the current cell      </param>
            /// <param name="blueprint">    Current blueprint                     </param>
            //////////////////////////////////////////////////////////////////////////////////
            public static bool Left (int floor, int x, int y, Blueprint blueprint)
            {
                var cell = blueprint.GetCell(floor, x, y);
                if (cell == null)
                    return false;

                return IsRoomsSame(cell, cell.neighbours.Left(floor));
            }
            
            
            ///////////////////////////////////////////////////////////////////////////////////
            /// <summary>  Returns true if backward cell and forward cell are equal
            ///            (relative to the current cell) 
            /// </summary>
            /// 
            /// <param name="x">            x coordinate of the current cell      </param>
            /// <param name="y">            y coordinate of the current cell      </param>
            /// <param name="blueprint">    Current blueprint                     </param>
            //////////////////////////////////////////////////////////////////////////////////
            public static bool ForwardBackward (int floor, int x, int y, Blueprint blueprint)
            {
                var cell = blueprint.GetCell(floor, x, y);
                if (cell == null)
                    return false;

                var forwardCell = cell.neighbours.Forward(floor);
                var backCell = cell.neighbours.Backward(floor);

                return IsRoomsSame(forwardCell, backCell);
            }
    
            
            ///////////////////////////////////////////////////////////////////////////////////
            /// <summary>  Returns true if right cell and left cell are equal
            ///            (relative to the current cell) 
            /// </summary>
            /// 
            /// <param name="x">            x coordinate of the current cell      </param>
            /// <param name="y">            y coordinate of the current cell      </param>
            /// <param name="blueprint">    Current blueprint                     </param>
            //////////////////////////////////////////////////////////////////////////////////
            public static bool RightLeft (int floor, int x, int y, Blueprint blueprint)
            {
                var cell = blueprint.GetCell(floor, x, y);
                if (cell == null)
                    return false;

                var leftCell  = cell.neighbours.Left(floor);
                var rightCell = cell.neighbours.Right(floor);

                return IsRoomsSame(leftCell, rightCell);
            }
    
    
            ///////////////////////////////////////////////////////////////////////////////////
            /// <summary>  Returns true if forward cell and right cell are equal
            ///            (relative to the current cell) 
            /// </summary>
            /// 
            /// <param name="x">            x coordinate of the current cell      </param>
            /// <param name="y">            y coordinate of the current cell      </param>
            /// <param name="blueprint">    Current blueprint                     </param>
            //////////////////////////////////////////////////////////////////////////////////
            public static bool ForwardRight (int floor, int x, int y, Blueprint blueprint)
            {
                var cell = blueprint.GetCell(floor, x, y);
                if (cell == null)
                    return false;

                var forwardCell  = cell.neighbours.Forward(floor);
                var rightCell = cell.neighbours.Right(floor);

                return IsRoomsSame(forwardCell, rightCell);
            }
    
    
            ///////////////////////////////////////////////////////////////////////////////////
            /// <summary>  Returns true if forward cell and left cell are equal
            ///            (relative to the current cell) 
            /// </summary>
            /// 
            /// <param name="x">            x coordinate of the current cell      </param>
            /// <param name="y">            y coordinate of the current cell      </param>
            /// <param name="blueprint">    Current blueprint                     </param>
            //////////////////////////////////////////////////////////////////////////////////
            public static bool ForwardLeft (int floor, int x, int y, Blueprint blueprint)
            {
                var cell = blueprint.GetCell(floor, x, y);
                if (cell == null)
                    return false;

                var leftCell  = cell.neighbours.Left(floor);
                var forwardCell = cell.neighbours.Forward(floor);

                return IsRoomsSame(leftCell, forwardCell);
            }
    
    
            ///////////////////////////////////////////////////////////////////////////////////
            /// <summary>  Returns true if backward cell and right cell are equal
            ///            (relative to the current cell) 
            /// </summary>
            /// 
            /// <param name="x">            x coordinate of the current cell      </param>
            /// <param name="y">            y coordinate of the current cell      </param>
            /// <param name="blueprint">    Current blueprint                     </param>
            //////////////////////////////////////////////////////////////////////////////////
            public static bool BackwardRight (int floor, int x, int y, Blueprint blueprint)
            {
                var cell = blueprint.GetCell(floor, x, y);
                if (cell == null)
                    return false;

                var backCell    = cell.neighbours.Backward(floor);
                var rightCell = cell.neighbours.Right(floor);

                return IsRoomsSame(backCell, rightCell);
            }
    
    
            ///////////////////////////////////////////////////////////////////////////////////
            /// <summary>  Returns true if backward cell and left cell are equal
            ///            (relative to the current cell) 
            /// </summary>
            /// 
            /// <param name="x">            x coordinate of the current cell      </param>
            /// <param name="y">            y coordinate of the current cell      </param>
            /// <param name="blueprint">    Current blueprint                     </param>
            //////////////////////////////////////////////////////////////////////////////////
            public static bool BackwardLeft (int floor, int x, int y, Blueprint blueprint)
            {
                var cell = blueprint.GetCell(floor, x, y);
                if (cell == null)
                    return false;

                var backCell  = cell.neighbours.Backward(floor);
                var leftCell = cell.neighbours.Left(floor);

                return IsRoomsSame(backCell, leftCell);
            }
        }
    }
}