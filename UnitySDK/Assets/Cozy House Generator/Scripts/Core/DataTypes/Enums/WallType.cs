using System;

namespace Cozy_House_Generator.Scripts.Core.DataTypes.Enums
{
    //////////////////////////////////////////////
    /// <summary>  Type of the wall  </summary>
    /////////////////////////////////////////////
    [Serializable]
    public enum WallType
    {
        Void,
        ExternalWall,
        InternalWall,
        InternalDoor,
        ExternalDoor,
        Window
    }
}