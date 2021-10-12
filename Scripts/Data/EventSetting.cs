using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// 세팅 파일 생성
public class EventSetting : ScriptableObject {
    const string SettingFileDirectory = "Assets/Resources/Setting";
    const string EventSettingFilePath = "Assets/Resources/Setting/EventSetting.asset";

    static EventSetting _instance;
    public static EventSetting Instance {
        get {
            if (_instance != null) {
                return _instance;
            }

            _instance = Resources.Load<EventSetting>(path: "EventSetting");

    #if UNITY_EDITOR
            if (_instance == null) {
                if (!AssetDatabase.IsValidFolder(path: SettingFileDirectory)) {
                    AssetDatabase.CreateFolder(parentFolder: "Assets/Resources", newFolderName: "Setting");
                }

                _instance = AssetDatabase.LoadAssetAtPath<EventSetting>(EventSettingFilePath);

                if (_instance == null) {
                    _instance = CreateInstance<EventSetting>();
                    AssetDatabase.CreateAsset(_instance, EventSettingFilePath);
                }
            }
    #endif

            return _instance;
        }
    }

    public List<EventData> eventData = new List<EventData>();
}

// 세팅
[System.Serializable]
public class EventData {
    // [Header("이벤트 인덱스")]
    // public uint index;

    [Header("이벤트 이름")]
    public string eventName;

    [Header("이벤트 이름 색")]
    public Color eventNameColor;

    [Header("이벤트 스프라이트 이미지")]
    public Sprite eventSprite;

    [Header("이벤트 사운드")]
    public AudioClip eventAudioClip;

    [Header("이벤트 내용")]
    [TextArea(3, 5)] public string content;

    // [Header("재해 ID")]
    [Space(5f)]
    [Header("영향 (약)")]
    [Header("영향 (절대값)")]
    [Space(20f)]
    [Tooltip("인구 고정값 영향 (약)")] public int absoluteWeakPopulation;
    [Tooltip("행복도 고정값 영향 (약)")] public int absoluteWeakHappiness;
    [Tooltip("건물 내구도 고정값 영향 (약)")] public int absoluteWeakBuildingDurability;
    [Tooltip("자원 고정값 영향 (약)\n0:돈, 1:나무, 2:돌, 3:철, 4:식량, 5:전기")] public int[] absoluteWeakResources;
    
    [Header("영향 (강)")]
    [Space(5f)]
    [Tooltip("인구 고정값 영향 (강)")] public int absoluteStrongPopulation;
    [Tooltip("행복도 고정값 영향 (강)")] public int absoluteStrongHappiness;
    [Tooltip("건물 내구도 고정값 영향 (강)")] public int absoluteStrongBuildingDurability;
    [Tooltip("자원 고정값 영향 (강)\n0:돈, 1:나무, 2:돌, 3:철, 4:식량, 5:전기")] public int[] absoluteStrongResources;

    [Header("영향 (약)")]
    [Header("영향 (퍼센트)")]
    [Space(20f)]
    [Tooltip("인구 퍼센트 영향 (약)")] public int percentWeakPopulation;
    [Tooltip("자원 퍼센트 영향 (약)\n0:돈, 1:나무, 2:돌, 3:철, 4:식량, 5:전기")] public int[] percentWeakResources;
        
    [Header("영향 (강)")]
    [Space(5f)]
    [Tooltip("인구 퍼센트 영향 (강)")] public int percentStrongPopulation;
    [Tooltip("자원 퍼센트 영향 (강)\n0:돈, 1:나무, 2:돌, 3:철, 4:식량, 5:전기")] public int[] percentStrongResources;

    [Header("영향 지속시간 (게임 시간 Week단위)")]
    [Space(20f)]
    public uint effectDuration;

    [Header("발생하기 위한 최소 시대 (1 ~ 4)")]
    [Header("발생 조건")]
    [Space(15f)]
    public uint conditionGeneration;

    // [Header("발생하기 위한 최소 돈")]
    // public condition
}