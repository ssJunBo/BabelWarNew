using UnityEditor;

namespace HotFix.UIExtension.ScrollRectExt.Editor
{
    [CustomEditor(typeof(UiCircularSv))]
    public class UiCircularScrollViewEditor : UnityEditor.Editor
    {
        UiCircularSv list;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
           
            //list = (UiCircularScrollView)target;
            // list.spacing = EditorGUILayout.FloatField("Spacing: ", list.spacing);
            // list.moveType = (MoveType)EditorGUILayout.EnumPopup("Direction: ", list.moveType);
            // list.m_Row = EditorGUILayout.IntField("Row Or Column: ", list.m_Row);
            // list.m_CellGameObject = (GameObject)EditorGUILayout.ObjectField("Cell: ", list.m_CellGameObject, typeof(GameObject), true);
            // list.isNeedOffset = EditorGUILayout.ToggleLeft(" IsNeedOffset", list.isNeedOffset);
        }
    }
}
