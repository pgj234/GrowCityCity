using UnityEngine;

public class Building : MonoBehaviour {
    public BuildingData buildingData;
}

[System.Serializable]
public class BuildingData {

    [Header("건물 이름")]
    public string buildingName;

    [Header("건설 소요 시간")]
    public uint buildTime;

    [Header("1 ~ 4 : 농업시대 ~ 근미래시대")]
    public uint level;

    [Header("건물 내구도")]
    [HideInInspector] public decimal durability = 100m;

    [Header("내구도 회복량")]
    public float durabilityRecoveryNum;


    [Header("세금 (세율 100% 기준)")]

    [Header("생산 자원")]
    public int budget;

    [Header("나무")]
    public int wood;

    [Header("돌")]
    public int stone;

    [Header("철")]
    public int iron;

    [Header("식량")]
    public int food;

    [Header("전기(%)")]
    public int electric;
    

    [Header("인구")]
    public int population;

    [Header("행복도")]
    public float happiness;



    [Header("원본 프리팹 이름")]
    [Space(10f)]
    public string PrefabName;

    [Header("건설 좌표")]
    public Vector2 location;

    [Header("Order in Layer")]
    public int orderInLayer;

    [Header("건설 남은 시간 (Week)")]
    public uint remainingTime;



    [Header("건설 및 업그레이드에 소요되는 자원들")]
    [Space(10f)]
    [Tooltip("0:돈, 1:나무, 2:돌, 3:철, 4:식량, 5:전기")]public string[] requireResource;
}