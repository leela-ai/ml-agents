using System;

namespace Cozy_House_Generator.Scripts.Core.DataTypes
{
    [Serializable]
    public class UseOnFloorResolver
    {

        public int         onlyFirstRange = 1;
        public int         onlyLastRange = 1;
        public int         exceptFirstRange = 1;
        public int         exceptLastRange = 1;
        public FloorsRange exceptRange;
        public FloorsRange onlyRange;
        public UseOn       useOn;
        public bool        haveExceptionFloors;
        
        public enum UseOn
        {
            All,
            Only
        }
        
        public enum FloorsRange
        {
            First,
            Last,
            FirstAndLast
        }

        public bool CanUseOnFloor(int currentFloor, int floorsCount)
        {
            var floor = currentFloor + 1;
            
            switch (useOn)
            {
                case UseOn.All:
                    if (haveExceptionFloors == false) return true;
                    switch (exceptRange)
                    {
                        case FloorsRange.First:
                            return floor > exceptFirstRange;
                            
                        case FloorsRange.Last:
                            return floor <= floorsCount - exceptLastRange;
                            
                        case FloorsRange.FirstAndLast:
                            return floor > exceptFirstRange && floor <= floorsCount - exceptLastRange;
                    }

                    return true;
                
                case UseOn.Only:
                    switch (onlyRange)
                    {
                        case FloorsRange.First:
                            return floor <= onlyFirstRange;
                        
                        case FloorsRange.Last:
                            return floor > floorsCount - onlyLastRange; 
                        
                        case FloorsRange.FirstAndLast:
                            return floor <= onlyFirstRange || floor > floorsCount - onlyLastRange;
                    }
                    break;
            }
            return false;
        }
      
    }
}