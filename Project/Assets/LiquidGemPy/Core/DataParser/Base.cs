namespace GemPlay.Core.Data.LiquidEarth
{
    public enum LiquidEarthDataType
    {
        User,
        Project,
        Volume,
        Wells,
        StaticMesh,
        WellTubes
    }
    public abstract class LiquidEarthBase
    {
        public readonly string DataId;
        public abstract LiquidEarthDataType DataType { get; }
        
        protected LiquidEarthBase(string dataId)
        {
            DataId = dataId;
        }
        
    }
}