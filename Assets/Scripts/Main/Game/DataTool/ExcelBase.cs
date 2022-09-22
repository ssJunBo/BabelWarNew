
namespace Main.Game.DataTool
{
    [System.Serializable]
    public class ExcelBase 
    {
#if UNITY_EDITOR
        public virtual void Construction() { }
#endif

        public virtual void Init() { }
    }
}
