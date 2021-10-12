using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventSetting))]
public class EventSettingEditor : Editor {
    
    [MenuItem("Assets/Open EventSetting")]
    public static void OpenEventSettingInspector() {
        Selection.activeObject = EventSetting.Instance;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUI.changed) {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}