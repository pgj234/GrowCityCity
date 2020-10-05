using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingData))]

public class BuildingDataSetting : Editor
{
    [MenuItem("Assets/Open Building Setting")]
    public static void OpenInspector() {
        Selection.activeObject = BuildingData.Instance;
    }
}
