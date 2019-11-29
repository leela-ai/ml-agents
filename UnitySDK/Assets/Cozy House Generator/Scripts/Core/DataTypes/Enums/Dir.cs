using System;

namespace Cozy_House_Generator.Scripts.Core.DataTypes.Enums
{   
    //////////////////////////////////////////////
    /// <summary>  Object direction  </summary>
    /////////////////////////////////////////////
    [Serializable]
    public enum Dir
    {
        Forward,
        Backward,
        Right,
        Left,
        Null,
        ForwardLeft,
        ForwardRight,
        BackwardLeft,
        BackwardRight
    }
}