namespace Cozy_House_Generator.Scripts.Core.DataTypes
{
    public class ColumnsData
    {
        public ColumnInfo forwardRight  = new ColumnInfo();
        public ColumnInfo forwardLeft   = new ColumnInfo();
        public ColumnInfo backwardRight = new ColumnInfo();
        public ColumnInfo backwardLeft  = new ColumnInfo();
        
        public ColumnsData(){}

        public ColumnsData(ColumnsData copySource)
        {
            forwardRight    = new ColumnInfo(copySource.forwardRight);
            forwardLeft     = new ColumnInfo(copySource.forwardLeft);
            backwardRight   = new ColumnInfo(copySource.backwardRight);
            backwardLeft    = new ColumnInfo(copySource.backwardLeft);
        }
    }
}