using System;

namespace Cozy_House_Generator.Scripts.Core.DataTypes
{
    //////////////////////////////////////////////
    /// <summary>  Pipe UI metadata  </summary>
    ///////////////////////////////////////////// 
    [Serializable]
    public class PipeUIData
    {
        public int  pipeId;
        public bool isEnabled = true;
        public int  useCount  = 1;

        public UseOnFloorResolver floors = new UseOnFloorResolver();
    }

}