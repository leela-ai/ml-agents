using Cozy_House_Generator.Scripts.Core.DataTypes.Enums;
using UnityEngine;

namespace Cozy_House_Generator.Scripts.Core.DataTypes
{
    public class ColumnInfo
    {
        public ColumnType   type;
        public Material     material;

        public ColumnInfo(){}

        public ColumnInfo(ColumnInfo copySource)
        {
            type     = copySource.type;
            material = copySource.material;
        }
    }
}