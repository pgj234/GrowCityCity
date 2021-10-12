using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SceneManagement;

public class GoogleManager : MonoBehaviour {

    static GoogleManager manager;

    // 싱글톤 프로퍼티
    public static GoogleManager Manager {
        get {
            return manager;
        }

        private set {
            if (manager == null) {
                manager = value;
            }
            else {
                Destroy(value.gameObject);
            }
        }
    }

    [SerializeField] GameObject startWaitPanel;
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject imgStartObj;
    [SerializeField] GameObject startBtnObj;
    [SerializeField] GameObject loadingManagerObj;
    [SerializeField] GameObject loadingObj;

    [SerializeField] Text txtLog;
    [SerializeField] Text txtStart;

    GameObject gameManagerObj;
    GameObject questContentObj;

    UIManager uIManager;

    List<BuildingData> buildingDataList = new List<BuildingData>();
    List<ResourceData> resourceDataList = new List<ResourceData>();
    List<QuestData> questDataList = new List<QuestData>();

    string jsonData = null;
    byte[] bytes = null;
    string code = null;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }    

    void Start() {
        var config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        Login();

        StartCoroutine(FindGameManager());
    }

    // 오브젝트들 찾기
    IEnumerator FindGameManager() {
        while (true) {
            yield return null;

            if (SceneManager.GetActiveScene().name == "Play") {
                gameManagerObj = GameObject.Find("GameManager").gameObject;
                uIManager = GameObject.FindObjectOfType<UIManager>();
                questContentObj = GameObject.Find("Main_Panel").transform.GetChild(1).transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).gameObject;

                break;
            }
        }
    }

    public void Login() {
        loginPanel.SetActive(false);
        startWaitPanel.SetActive(true);

        Social.localUser.Authenticate((bool success) => {
            if (success) {
                startWaitPanel.SetActive(false);
                loadingManagerObj.SetActive(true);
            }
            else {
                startWaitPanel.SetActive(false);
                txtLog.text = "구글 로그인 실패";
            }
        });
    }

    public void Logout() {
        ((PlayGamesPlatform)Social.Active).SignOut();
        startBtnObj.SetActive(false);
        loadingObj.SetActive(false);
        imgStartObj.SetActive(false);
        loginPanel.SetActive(true);
        loadingManagerObj.SetActive(false);
    }

    #region 클라우드 저장

    public static ISavedGameClient SavedGame() {
        return PlayGamesPlatform.Instance.SavedGame;
    }

    // 클라우드 데이터 세이브
    public void SaveCloud() {
         if (GameManager.buildingObj.Count > 0 == false) {           // 건물이 하나도 없으면 리턴 (저장 할 필요 없음)
             return;
         }

        SavedGame().OpenWithAutomaticConflictResolution("buildingsave", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood, BuildingSave);
        SavedGame().OpenWithAutomaticConflictResolution("resourcesave", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood, ResourceSave);
        SavedGame().OpenWithAutomaticConflictResolution("questsave", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood, QuestSave);

        uIManager.Notice_TXT("저장 되었습니다");
    }

    #region  건물 세이브

    void BuildingSave(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if (status == SavedGameRequestStatus.Success) {
            var update = new SavedGameMetadataUpdate.Builder().Build();

            buildingDataList.Clear();
            for (int i=0; i<GameManager.buildingObj.Count; i++) {
                buildingDataList.Add(GameManager.buildingObj[i].GetComponent<Building>().buildingData);
            }

            jsonData = JsonUtility.ToJson(new Serialization<BuildingData>(buildingDataList));
            bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            code = System.Convert.ToBase64String(bytes);

            byte[] codeBytes = System.Text.Encoding.UTF8.GetBytes(code);
            SavedGame().CommitUpdate(game, update, codeBytes, SaveData);
        }
    }

    #endregion

    #region  시간, 자원 세이브

    void ResourceSave(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if (status == SavedGameRequestStatus.Success) {
            var update = new SavedGameMetadataUpdate.Builder().Build();

            ResourceData resourceData = gameManagerObj.GetComponent<Resource>().resourceData;

            SumResourceData(resourceData);
            resourceDataList.Clear();
            resourceDataList.Add(gameManagerObj.GetComponent<Resource>().resourceData);

            jsonData = JsonUtility.ToJson(new Serialization<ResourceData>(resourceDataList), true);
            bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            code = System.Convert.ToBase64String(bytes);
            
            byte[] codeBytes = System.Text.Encoding.UTF8.GetBytes(code);
            SavedGame().CommitUpdate(game, update, codeBytes, SaveData);
        }
    }

    #endregion

    #region  퀘스트 세이브

    void QuestSave(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if (status == SavedGameRequestStatus.Success) {
            var update = new SavedGameMetadataUpdate.Builder().Build();

            questDataList.Clear();
            for (int i=0; i<questContentObj.transform.childCount; i++) {
                questDataList.Add(questContentObj.transform.GetChild(i).GetComponent<Quest>().questData);
            }

            jsonData = JsonUtility.ToJson(new Serialization<QuestData>(questDataList), true);
            bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            code = System.Convert.ToBase64String(bytes);

            byte[] codeBytes = System.Text.Encoding.UTF8.GetBytes(code);
            SavedGame().CommitUpdate(game, update, codeBytes, SaveData);
        }
    }

    #endregion

    // 저장할 자원 값 대입
    void SumResourceData(ResourceData resourceData) {
        resourceData.week = GameManager.Week;

        resourceData.cityName = GameManager.CityName;
        resourceData.generation = GameManager.generation;

        resourceData.population = GameManager.s_resource.Population;
        resourceData.happiness = GameManager.s_resource.Happiness;

        resourceData.budget = GameManager.s_resource.Budget.ToString();
        resourceData.wood = GameManager.s_resource.Wood.ToString();
        resourceData.stone = GameManager.s_resource.Stone.ToString();
        resourceData.iron = GameManager.s_resource.Iron.ToString();
        resourceData.food = GameManager.s_resource.Food.ToString();
        resourceData.electric = GameManager.s_resource.Electric.ToString();

        resourceData.taxRate = GameManager.TaxRate;
    }

    void SaveData(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if (status == SavedGameRequestStatus.Success) {
            // LogText.text = "클라우드 데이터 저장 성공";
        }
        else {
            // LogText.text = "클라우드 데이터 저장 실패";
        }
    }

    // 클라우드 데이터 삭제
    public void DeleteCloud() {
        SavedGame().OpenWithAutomaticConflictResolution("buildingsave", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, DeleteGame);
        SavedGame().OpenWithAutomaticConflictResolution("resourcesave", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, DeleteGame);
        SavedGame().OpenWithAutomaticConflictResolution("questsave", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, DeleteGame);
    }

    void DeleteGame(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if (status == SavedGameRequestStatus.Success) {
            SavedGame().Delete(game);
            txtStart.text = "클라우드 데이터 삭제 성공";
        }
        else {
            txtStart.text = "클라우드 데이터 삭제 실패";
        }
    }

    #endregion
}