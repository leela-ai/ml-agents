using System.Collections.Generic;
using Cozy_House_Generator.Scripts.Core.DataTypes;
using Cozy_House_Generator.Scripts.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Cozy_House_Generator.Scripts.Core.Pipes
{   
    ////////////////////////////////////////////////////////////////////////////
    /// <summary>  Finds possible places for props and place them  </summary>
    ///////////////////////////////////////////////////////////////////////////
    public class PlaceProps : IGeneratorPipe
    {   
        private int                useCount;
        private UseOnFloorResolver floors;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Finds possible places for props and place them  </summary>
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
            var roomsInteriorsData = GetInfoAboutRoomsInteriors(blueprint, floor);

            foreach (var room in roomsInteriorsData)
                TryToPlaceProps(interiors[room.Value].possibleProps, blueprint, floor, room.Key, rnd);
            
        }
        
        
        public int UseCount()
        {
            return useCount;
        }
     

        private Dictionary<int, int> GetInfoAboutRoomsInteriors(Blueprint blueprint, int floor)
        {
            Dictionary<int, int> dataForProps = new Dictionary<int, int>();            
            
            for (int x = 0; x < blueprint.size; x++)
            {
                for (int y = 0; y < blueprint.size; y++)
                {
                    var cell = blueprint.GetCell(floor, x, y);

                    if (dataForProps.ContainsKey(cell.RoomId) == false)
                    {
                        dataForProps.Add(cell.RoomId, cell.interiorId);
                    }
                }
            }

            return dataForProps;
        }
        
        
        public UseOnFloorResolver FloorsRangeData()
        {
            return floors;
        }
        
        
        public string GetPipeName()
        {
            return "Place Props";
        }
        
        
        public IGeneratorPipe MakeNew(int count, UseOnFloorResolver floors)
        {
            return new PlaceProps {useCount = count, floors = floors};
        }

        
        private static void TryToPlaceProps(IEnumerable<PropsInfo> possibleProps, Blueprint blueprint, int floor, int roomId, Random rnd)
        {
            foreach (var propsInfo in possibleProps)
                if (propsInfo.count > 0)
                    for (int i = 0; i < propsInfo.count; i++)
                        propsInfo.props.TryToPlace(blueprint, floor, roomId, rnd);
        }

    }
    
}