using System;

namespace Cozy_House_Generator.Scripts.Core.DataTypes.Enums
{
    /////////////////////////////////////////////////
    /// <summary>  Type of a near cell  </summary>
    ////////////////////////////////////////////////
    [Serializable]
    public enum NearCellType
    {
        Void,
        SameRoom,
        AnotherRoom
    }
}