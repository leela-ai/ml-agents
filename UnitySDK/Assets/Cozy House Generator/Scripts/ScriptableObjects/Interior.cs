using System;
using UnityEngine;

namespace Cozy_House_Generator.Scripts.ScriptableObjects
{   ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Contains information about an interior  </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [CreateAssetMenu(menuName = "Cozy House Generator/Interior"), Serializable]
    public class Interior : ScriptableObject
    { 
        public int              windowsRate;
        public Material         floor;
        public Material         walls;
        public Material         ceiling;
        public Material         column;
        public PropsInfo[]      possibleProps;
        public int              maxRoomSize;
        public int              minRoomSize;
        public int              maxCountOfDoors;
        public bool             placeOnlyOnce;

        public bool IsGoodEnough(int roomSize, int countOfDoors)
        {
            return roomSize < maxRoomSize && roomSize > minRoomSize && countOfDoors <= maxCountOfDoors;
        }
    }

    
    ////////////////////////////////////////////
    /// <summary>  Props metadata  </summary>
    ///////////////////////////////////////////
    [Serializable]
    public struct PropsInfo
    {
        public Props props;
        public int   count;
    }
}