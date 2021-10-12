using System.Collections.Generic;
using UnityEngine;
using System.IO;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.UI;

public class Load : MonoBehaviour {
    List<BuildingData> buildingDataList = new List<BuildingData>();
    List<ResourceData> resourceDataList = new List<ResourceData>();
    List<QuestData> questDataList = new List<QuestData>();

    string jsonData;

    [SerializeField] UIManager uIManager;

    [SerializeField] GameObject canvas;

    [SerializeField] GameObject cityName_Panel;

    [SerializeField] GameObject questContentObj;

    [SerializeField] GameObject cityHallObj;

    [SerializeField] Slider taxSlider;

    [Space(10f)]
    [SerializeField] GameObject bgmSettingGrp;
    [SerializeField] GameObject sfxSettingGrp;

    OptionManager optionManager;

    void Awake() {
        Time.timeScale = 0;

        optionManager = GameObject.Find("OptionManager").GetComponent<OptionManager>();
        optionManager.BGM();

        SavedGameCheck();

        buildingDataList.Clear();
        resourceDataList.Clear();
        questDataList.Clear();

        LoadCloud();

        SetSetting();

        canvas.SetActive(true);
        Time.timeScale = 1;
    }

    #region 클라우드 로드

    // 클라우드 데이터 로드
    public void LoadCloud() {
        GoogleManager.SavedGame().OpenWithAutomaticConflictResolution("buildingsave", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood, BuildingLoad);
        GoogleManager.SavedGame().OpenWithAutomaticConflictResolution("questsave", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood, QuestLoad);
        GoogleManager.SavedGame().OpenWithAutomaticConflictResolution("resourcesave", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood, ResourceLoad);
    }

    // 건물 로드
    void BuildingLoad(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if (status == SavedGameRequestStatus.Success) {
            GoogleManager.SavedGame().ReadBinaryData(game, LoadBuildingData);
        }
    }

    // 자원 로드
    void ResourceLoad(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if (status == SavedGameRequestStatus.Success) {
            GoogleManager.SavedGame().ReadBinaryData(game, LoadResourceData);
        }
    }

    // 퀘스트 로드
    void QuestLoad(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if (status == SavedGameRequestStatus.Success) {
            GoogleManager.SavedGame().ReadBinaryData(game, LoadQuestData);
        }
    }

    #endregion

    #region 건물 데이터 변환

    void LoadBuildingData(SavedGameRequestStatus status, byte[] LoadedData) {
        if (status == SavedGameRequestStatus.Success) {
            string code = System.Text.Encoding.UTF8.GetString(LoadedData);

            jsonData = Encoding(code);

            buildingDataList = JsonUtility.FromJson<Serialization<BuildingData>>(jsonData).data;

            GameManager.buildingObj.Add(cityHallObj);

            GameObject loadBuildingObj;
            for (int i=0; i<buildingDataList.Count; i++) {
                loadBuildingObj = Instantiate(Resources.Load(Path.Combine("Prefabs", buildingDataList[i].PrefabName)), buildingDataList[i].location, UnityEngine.Quaternion.identity) as GameObject;
                BuildManager.BuildCompleteAfter(loadBuildingObj);

                SumBuildingData(loadBuildingObj, buildingDataList[i]);
            }
        }
        else {
            // LogText.text = "클라우드 데이터 로드 실패";
        }
    }

    #endregion

    #region 자원 데이터 변환

    void LoadResourceData(SavedGameRequestStatus status, byte[] LoadedData) {
        if (status == SavedGameRequestStatus.Success) {
            string code = System.Text.Encoding.UTF8.GetString(LoadedData);

            jsonData = Encoding(code);

            resourceDataList = JsonUtility.FromJson<Serialization<ResourceData>>(jsonData).data;
            GetComponent<Resource>().resourceData = resourceDataList[0];

            SumResourceData(GetComponent<Resource>().resourceData);
        }
        else {
            // LogText.text = "클라우드 데이터 로드 실패";
        }
    }

    #endregion

    #region 퀘스트 데이터 변환

    void LoadQuestData(SavedGameRequestStatus status, byte[] LoadedData) {
        if (status == SavedGameRequestStatus.Success) {
            string code = System.Text.Encoding.UTF8.GetString(LoadedData);

            jsonData = Encoding(code);

            questDataList = JsonUtility.FromJson<Serialization<QuestData>>(jsonData).data;

            GameObject loadQuestObj;
            for (int i=0; i<questDataList.Count; i++) {
                loadQuestObj = questContentObj.transform.GetChild(i).gameObject;

                SumQuestData(loadQuestObj, questDataList[i]);
            }
        }
        else {
            // LogText.text = "클라우드 데이터 로드 실패";
        }
    }

    #endregion

    // 건물 로드값 대입
    void SumBuildingData(GameObject buildingObj, BuildingData buildingData) {
        BuildingData buildingObjData = buildingObj.GetComponent<Building>().buildingData;

        buildingObjData.buildingName = buildingData.buildingName;
        buildingObjData.buildTime = buildingData.buildTime;
        buildingObjData.level = buildingData.level;
        // buildingObjData.product = buildingData.product;
        // buildingObjData.productNum = buildingData.productNum;
        
        buildingObjData.PrefabName = buildingData.PrefabName;
        buildingObjData.location = buildingData.location;
        buildingObjData.orderInLayer = buildingData.orderInLayer;
        buildingObjData.remainingTime = buildingData.remainingTime;
        buildingObjData.requireResource = buildingData.requireResource;
        buildingObjData.durability = buildingData.durability;
        buildingObjData.durabilityRecoveryNum = buildingData.durabilityRecoveryNum;
        
        // 생산 자원
        buildingObjData.budget = buildingData.budget;
        buildingObjData.wood = buildingData.wood;
        buildingObjData.stone = buildingData.stone;
        buildingObjData.iron = buildingData.iron;
        buildingObjData.food = buildingData.food;
        buildingObjData.electric = buildingData.electric;

        buildingObjData.population = buildingData.population;
        buildingObjData.happiness = buildingData.happiness;

        GameManager.buildingObj.Add(buildingObj);
    }

    // 자원 로드값 대입
    void SumResourceData(ResourceData resourceData) {
        GameManager.CityName = resourceData.cityName;
        GameManager.generation = resourceData.generation;
        
        GameManager.Week = resourceData.week;
        GameManager.TaxRate = resourceData.taxRate;
        taxSlider.value = (float)GameManager.TaxRate;

        GameManager.s_resource.Population = resourceData.population;
        GameManager.s_resource.Happiness = resourceData.happiness;

        GameManager.s_resource.Budget = ulong.Parse(resourceData.budget);
        GameManager.s_resource.Wood = ulong.Parse(resourceData.wood);
        GameManager.s_resource.Stone = ulong.Parse(resourceData.stone);
        GameManager.s_resource.Iron = ulong.Parse(resourceData.iron);
        GameManager.s_resource.Food = ulong.Parse(resourceData.food);
        GameManager.s_resource.Electric = ulong.Parse(resourceData.electric);
    }

    //저장된 게임 체크 (처음이면 도시이름 짓기 On)
    void SavedGameCheck() {
        GoogleManager.SavedGame().OpenWithAutomaticConflictResolution("resourcesave", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, CityNamePanelOn);
    }

    //도시이름 짓기 On
    void CityNamePanelOn(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if ((status == SavedGameRequestStatus.Success) == false) {
            uIManager.TimeStop();

            cityName_Panel.SetActive(true);
        }
    }

    // 퀘스트 로드값 대입
    void SumQuestData(GameObject questObj, QuestData questData) {
        QuestData questObjData = questObj.GetComponent<Quest>().questData;

        questObjData.rewardReceiveChk = questData.rewardReceiveChk;
        questObjData.conditionStr = questData.conditionStr;
        questObjData.rewardIndex = questData.rewardIndex;
        questObjData.rewardNum = questData.rewardNum;
        questObjData.rewardRequire = questData.rewardRequire;
        questObjData.progress = questData.progress;

        string str = string.Format("{0} / {1}", questObjData.progress, questObjData.rewardRequire);
        uIManager.QuestReward_View(questObj, str);
    }

    // 세팅 Set
    void SetSetting() {
        optionManager.BGM();

        if (PlayerPrefs.GetInt("BGM_OnOff", 1) == 1) {
            bgmSettingGrp.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        }
        else {
            bgmSettingGrp.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
        }

        if (PlayerPrefs.GetInt("SFX_OnOff", 1) == 1) {
            sfxSettingGrp.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
        }
        else {
            sfxSettingGrp.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    // 인코딩
    string Encoding(string code) {
        byte[] bytes;

        bytes = System.Convert.FromBase64String(code);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}