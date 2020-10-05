using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BuildingData : ScriptableObject {

    struct s_Buildings {
        
    }

    public GameObject[] BuildingObj;

    const string SettingFileDirectory = "Assets/Resources";
    const string SettingFilePath = "Assets/Resources/BuildingData.asset";

    static BuildingData _instance;
    public static BuildingData Instance {
        get {
            if (_instance != null) {
                return _instance;
            }

            _instance = Resources.Load<BuildingData>(path: "BuildingData");

        #if UNITY_EDITOR
            if (_instance == null) {
                if (!AssetDatabase.IsValidFolder(path: SettingFileDirectory)) {
                    AssetDatabase.CreateFolder(parentFolder: "Assets", newFolderName: "Resources");
                }

                _instance = AssetDatabase.LoadAssetAtPath<BuildingData>(SettingFilePath);

                if (_instance == null) {
                    _instance = CreateInstance<BuildingData>();
                    AssetDatabase.CreateAsset(_instance, SettingFilePath);
                }
            }
        #endif

            return _instance;
        }
    }

    public string buildingName;
    public uint pop;
}
