using UnityEngine;

namespace Cozy_House_Generator.Scripts.House_Builders
{
    public class RawBuilder : IHouseBuilder
    {
        private HouseCell[][,] floors;
        
        
        public void Build (BuilderData data)
        {
            //fix rotation issues-------------------------------------------------------------------------------------//
            var currentRot = data.root.rotation;
            data.root.rotation = Quaternion.Euler(Vector3.zero);
            //--------------------------------------------------------------------------------------------------------// 
            
            floors = new HouseCell[data.blueprint.floorsCount][,];
            for (int floor = 0; floor < data.blueprint.floorsCount; floor++)
            {
                CreateCells(data, floor);

                for (int x = 0; x < data.blueprint.Size(); x++)
                {
                    for (int y = 0; y < data.blueprint.Size(); y++)
                    {
                        if (floors[floor] == null || floors[floor].Length == 0)
                        {
                            Debug.LogWarning("House Generator: CELL GRID IS EMPTY. POSSIBLY CELL PROPS IS NULL");
                            return;
                        }

                        floors[floor][x, y].Build(data.blueprint.GetCell(floor, x, y), data, floor);
                    }
                }
            }

            //end fix-------------------------------------------------------------------------------------------------//
            data.root.rotation = currentRot;
            //--------------------------------------------------------------------------------------------------------//
        }
        
        
        //////////////////////////////////////////////////////////////////
        /// <summary>  Resets cells  </summary>
        /// 
        /// <param name="data">  Necessary data for the builder  </param>
        /////////////////////////////////////////////////////////////////
        private void CreateCells(BuilderData data, int floor)
        {
            float   cellSize     = data.cellPrefab.GetComponent<HouseCell>().cellSize;
            float   cellHeight   = data.cellPrefab.GetComponent<HouseCell>().cellHeight;
            int     size         = data.blueprint.Size();
            
            if (data.cellPrefab == null)
            {
                Debug.LogWarning("House Generator: CELL PREFAB IS EMPTY");
                return;
            }

            floors[floor] = new HouseCell[size, size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    var position = data.root.position;
                    var cellPos = new Vector3
                                  {
                                      x = position.x + x          * cellSize,
                                      y = position.y + floor      * cellHeight,
                                      z = position.z + y          * cellSize
                                  };

                    floors[floor][x, y] = Object
                                    .Instantiate(data.cellPrefab, cellPos, Quaternion.Euler(Vector3.zero), data.root)
                                    .GetComponent<HouseCell>();
                    floors[floor][x, y].name = "Cell - floor: " + floor + " x: " + x + " y: "  + y;
                }
            }
        }
    }
}