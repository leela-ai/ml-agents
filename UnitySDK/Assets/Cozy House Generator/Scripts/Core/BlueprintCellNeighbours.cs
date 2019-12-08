namespace Cozy_House_Generator.Scripts.Core
{
    public class BlueprintCellNeighbours
    {
        private int              blueprintSize;
        //private int              floor;
        private int              x;
        private int              y;
        private Blueprint        blueprint;
        
        public BlueprintCellNeighbours(BlueprintCell cell)
        {
            //floor         = cell.floor;
            x             = cell.x;
            y             = cell.y;
            blueprint     = cell.blueprint;
        }

        public BlueprintCellNeighbours(BlueprintCellNeighbours copySource)
        {
            //floor = copySource.floor;
            x = copySource.x;
            y = copySource.y;
            blueprint = copySource.blueprint;
        }
        
        
        //////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns a cell that is backward of the current cell  </summary>
        ///  
        /// <returns> Backward cell </returns>
        /////////////////////////////////////////////////////////////////////////////////////////
        public BlueprintCell Backward(int floor)
        {
            return blueprint.GetCell(floor, x, y - 1);
        }
    
    
        /////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns a cell that is forward of the current cell  </summary>
        ///  
        /// <returns> Forward cell </returns>
        ////////////////////////////////////////////////////////////////////////////////////////
        public BlueprintCell Forward(int floor)
        {
            return blueprint.GetCell(floor, x , y + 1);
        }
    
    
        ///////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns a cell that is right of the current cell  </summary>
        /// 
        /// <returns> Right cell </returns>
        //////////////////////////////////////////////////////////////////////////////////////
        public BlueprintCell Right(int floor)
        {
            return blueprint.GetCell(floor, x + 1, y );
        }
    
    
        //////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns a cell that is left of the current cell  </summary>
        /// 
        /// <returns> Left cell </returns>
        ////////////////////////////////////////////////////////////////////////////////////
        public BlueprintCell Left(int floor)
        {
            return blueprint.GetCell(floor, x - 1, y);
        }
    
    
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns a cell that is forward right of the current cell  </summary>
        ///  
        /// <returns> Forward right cell </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public BlueprintCell ForwardRight(int floor)
        {
            return blueprint.GetCell(floor, x + 1, y + 1);
        }
    
    
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns a cell that is forward left of the current cell  </summary>
        /// 
        /// <returns> Forward left cell </returns>
        ///////////////////////////////////////////////////////////////////////////////////////////
        public BlueprintCell ForwardLeft(int floor)
        {
            return blueprint.GetCell(floor, x - 1, y + 1);
        }
    
    
        //////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns a cell that is backward right of the current cell  </summary>
        ///  
        /// <returns> Backward right Cell </returns>
        /////////////////////////////////////////////////////////////////////////////////////////////
        public BlueprintCell BackwardRight(int floor)
        {
            return blueprint.GetCell(floor, x + 1, y - 1);
        }
    
    
        /////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Returns a cell that is backward left of the current cell  </summary>
        /// 
        /// <returns> Backward left Cell </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public BlueprintCell BackwardLeft(int floor)
        {
            return blueprint.GetCell(floor, x - 1, y - 1);
        }
    }
}