using UnityEditor;

namespace UIExtension.ScrollRectExt.Editor
{
    [CustomEditor(typeof(UiCircularScrollView))]
    public class UiCircularScrollViewEditor : UnityEditor.Editor
    {
        UiCircularScrollView list;
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
