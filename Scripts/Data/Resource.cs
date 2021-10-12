using UnityEngine;

public class Resource : MonoBehaviour {
    public ResourceData resourceData;
}

[System.Serializable]
public class ResourceData {
    [Header("도시명")]
    public string cityName;

    [Header("시대 (1 ~ 4)")]
    public uint generation;

    [Header("행복도")]
    public float happiness;

    [Header("도시 예산")]
    public string budget;

    [Header("나무")]
    public string wood;

    [Header("돌")]
    public string stone;

    [Header("철")]
    public string iron;

    [Header("식량")]
    public string food;

    [Header("전기")]
    public string electric;
    

    [Space(15f)]
    public int population;


    [HideInInspector] public uint week;

    [HideInInspector] public decimal taxRate;

    // [HideInInspector] public decimal taxRate;
}