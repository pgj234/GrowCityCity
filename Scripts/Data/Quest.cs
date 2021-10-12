using UnityEngine;

public class Quest : MonoBehaviour {
    public QuestData questData;
}

[System.Serializable]
public class QuestData {

    [Header("보상 수령 체크")]
    [HideInInspector] public bool rewardReceiveChk;

    [Header("퀘스트 프리팹 이름")]
    public string conditionStr;

    [Header("보상 인덱스")]
    public uint rewardIndex;

    [Header("보상 양")]
    public int rewardNum;

    [Header("보상 조건 수")]
    public uint rewardRequire;

    [Header("진행도")]
    public uint progress;
}