using System;

namespace Cozy_House_Generator.Scripts.Core.DataTypes.Enums
{   ///////////////////////////////////////////////
    /// <summary>  The type of props  </summary>
    //////////////////////////////////////////////
    [Serializable]
    public enum PropsType
    {
        None , // Custom name for "Nothing" option
        Chair               = 1 << 0 ,
        Sofa                = 1 << 1,
        Table               = 1 << 2,
        Toilet              = 1 << 3,
        Fridge              = 1 << 4,
        KitchenWall         = 1 << 5,
        Chandelier          = 1 << 6,
        Stove               = 1 << 7,
        Bed                 = 1 << 8,
        Cupboard            = 1 << 9,
        Nightstand          = 1 << 10,
        SofaCorner          = 1 << 11,
        MedicineChest       = 1 << 12,
        ShowerBath          = 1 << 13,
        WashBasin           = 1 << 14,
        Fireplace           = 1 << 15,
        KitchenWallCentral  = 1 << 16,
        KitchenWallLeft     = 1 << 17,
        KitchenWallRight    = 1 << 18,
        TrainingApparatus   = 1 << 19,
        Picture             = 1 << 20
    }
}