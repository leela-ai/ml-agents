namespace Cozy_House_Generator.Scripts.Core
{
    public class BlueprintFloor
    {
        private BlueprintCell[,] cells;
        private int size;


        public BlueprintFloor(int size, int floor, Blueprint blueprint)
        {
            this.size = size;
            cells = new BlueprintCell[size, size];
            
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    cells[x, y] = new BlueprintCell(blueprint, floor, x, y);
                }
            }
        }
        
        
        public BlueprintFloor(BlueprintFloor copySource)
        {
            size = copySource.size;
            cells = new BlueprintCell[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    cells[x, y] = new BlueprintCell(copySource.cells[x, y]);
                }
            }
        }
        
        
        public int Size()
        {
            return size;
        }


        public void SetCell(int x, int y, BlueprintCell newCell)
        {
            if (x < 0 || y < 0 || x >= size || y >= size)
                return;
            
            cells[x, y] = newCell;
        }
        
        
        public BlueprintCell GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size || y >= size)
                return null;

            return cells[x, y];
        }

    }
}