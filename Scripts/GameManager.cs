using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Serialization<T> {
    public Serialization(List<T> _target) => data = _target;
    public List<T> data;
}

public class GameManager : MonoBehaviour {

    static GameManager manager;

    // 싱글톤 프로퍼티
    public static GameManager Manager {
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

    public static List<GameObject> buildingObj = new List<GameObject>();

    static string cityName;
    public static string CityName {
        get {
            return cityName;
        }

        set {
            cityName = value;
            uiManager.CityName_TXT(cityName);
        }
    }

    // 자원
    static int population;          // 인구
    static float happiness;           // 행복도
    
    static ulong budget;       // 도시예산
    static ulong wood;                // 나무
    static ulong stone;               // 돌
    static ulong iron;                // 철
    static ulong food;                // 식량
    static ulong electric;            // 전기

    // 자원 구조체 프로퍼티
    public struct s_Resource {
        public int Population {
            get {
                return population;
            }
            set {
                population = value;
                uiManager.Population_View_TXT(population);
            }
        }

        public float Happiness {
            get {
                return happiness;
            }
            set {
                happiness = value;

                if (happiness < 0) {
                    happiness = 0;
                }
                else if (happiness >= 100) {
                    happiness = 100;
                }

                uiManager.Happiness_View((int)happiness);
            }
        }

        public ulong Budget {
            get {
                return budget;
            }
            set {
                budget = value;
                uiManager.Budget_View_TXT(budget);
            }
        }

        public ulong Wood {
            get {
                return wood;
            }
            set {
                wood = value;
                uiManager.Wood_View_TXT(wood);
            }
        }
        
        public ulong Stone {
            get {
                return stone;
            }
            set {
                stone = value;
                uiManager.Stone_View_TXT(stone);
            }
        }
        
        public ulong Iron {
            get {
                return iron;
            }
            set {
                iron = value;
                uiManager.Iron_View_TXT(iron);
            }
        }

        public ulong Food {
            get {
                return food;
            }
            set {
                food = value;
                uiManager.Food_View_TXT(food);
            }
        }
        
        public ulong Electric {
            get {
                return electric;
            }
            set {
                electric = value;
                uiManager.Electric_View_TXT(electric);
            }
        }
    }

    public static s_Resource s_resource;

    [SerializeField] EventManager eventManager;

    GoogleManager googleManager;

    static UIManager uiManager;

    OptionManager optionManager;

    [SerializeField] GameObject cityNamePanel;

    [Space(10f)]
    [Tooltip("게임시간 1Week = 현실시간 몇초?")] [SerializeField] uint realTime;
    [Tooltip("이벤트 주기(게임시간 Week 단위)")] [SerializeField] uint eventCycleTime;
    //[Tooltip("이벤트 주기 오차")] [SerializeField] uint eventError;
    [Tooltip("자동세이브 주기(현실시간 초 단위)")] [SerializeField] uint autoSaveCycleTime;

    public static uint generation;

    public static bool buildPrev;

    public static bool buildAble;

    public static bool selectBuilding;

    public static bool timeStop;

    public static bool isGameover;
    public static bool isVictory;

    static uint week;
    public static uint Week {
        get {
            return week;
        }

        set {
            week = value;
            uiManager.Week_View_TXT(Week.ToString() + " Week");
        }
    }

    static decimal taxRate;             // 세율
    public static decimal TaxRate {
        get {
            return taxRate;
        }

        set {
            taxRate = (decimal)(Mathf.Round((float)value * 1000) * 0.001f);
            uiManager.TaxRate_TXT(taxRate.ToString() + " %");
        }
    }

    void Awake() {
        Init();
    }

    void Init() {
        buildingObj.Clear();
        uiManager = null;
        cityName = null;
        
        generation = 1;
        population = 100;
        happiness = 25;
        budget = 1000000;
        wood = 30000;
        stone = 30000;
        iron = 30000;
        food = 30000;
        electric = 10000;
        week = 0;
        taxRate = 0.3m;

        buildPrev = false;
        buildAble = true;
        selectBuilding = false;
        timeStop = false;
        isGameover = false;

        Manager = this;
        buildPrev = false;
        timeStop = false;
    }

    void Start() {
        uiManager = GameObject.FindObjectOfType<UIManager>();
        optionManager = GameObject.FindObjectOfType<OptionManager>();
        googleManager = GameObject.FindObjectOfType<GoogleManager>();

        StartCoroutine(SetTime());
        StartCoroutine(AutoSave());
    }

    // 날짜 Set
    IEnumerator SetTime() {
        while(true) {
            yield return new WaitForSeconds(realTime);

            if (timeStop == false) {
                Week += 1;

                BuildingCirculation();

                // 이벤트 주기별 발생
                if (buildingObj.Count > 0) {
                    // if (Week % RandomInt(eventCycleTime -eventError, eventCycleTime +eventError) == 0) {
                    //     eventManager.SetEvent();
                    // }
                    if (Week > 0 && Week % eventCycleTime == 0) {
                        eventManager.SetEvent();
                    }
                }

                // 게임오버 조건 체크
                GameOverConditionChk();
            }
        }
    }

    // 건물 순회 체크
    void BuildingCirculation() {
        if (buildingObj.Count > 0) {
            
            decimal tmpElectric = s_resource.Electric;
            for (int i=0; i<buildingObj.Count; i++) {
                BuildingData building_Data = buildingObj[i].GetComponent<Building>().buildingData;

                if (building_Data.durability > 0) {
                    // 자원 Set
                    SetResources(building_Data);

                    // 전기 Set
                    if (building_Data.electric > 0) {
                        tmpElectric *= (1 + ((building_Data.electric * (decimal)0.01m)));//       여기 전기를 어케하야 하는겨
                    }

                    // 건물 순회 내구도 수리
                    DurabilityCheck(building_Data);
                }
            }

            s_resource.Electric = (ulong)tmpElectric;
        }
    }

    // 자원 Set
    void SetResources(BuildingData bData) {
        s_resource.Population += bData.population;
        happiness += bData.happiness;

        s_resource.Budget += (ulong)((int.Parse(bData.requireResource[0]) * 0.01m) * TaxRate);
        s_resource.Wood += (ulong)bData.wood;
        s_resource.Stone += (ulong)bData.stone;
        s_resource.Iron += (ulong)bData.iron;
        s_resource.Food += (ulong)bData.food;
    }

    // 건물 순회 내구도 체크와 수리
    void DurabilityCheck(BuildingData bData) {
        // if (bData.durability <= 0) {
        //     bData.
        // }

        if (bData.durability > 0 && bData.durability < 100) {
            bData.durability += (decimal)bData.durabilityRecoveryNum;

            if (bData.durability > 100) {
                bData.durability = 100m;
            }
        }
    }

    // 게임오버 조건 체크
    void GameOverConditionChk() {
        // 행복도 게임오버 체크
        if (happiness <= 0) {            // 원래는 회생 기회를 줘야함    원래는 회생 기회를 줘야함   원래는 회생 기회를 줘야함   원래는 회생 기회를 줘야함 (테스트테스트테스트)
            GameOver();
        }
        // 인구 0 게임 오버 체크
        else if (population <= 0) {     // 인구 0은 즉시 게임 오버
            GameOver();
        }
    }

    // 게임오버
    void GameOver() {
        uiManager.TimeStop();
        uiManager.GameOverPanelOpen();
        isGameover = true;
        optionManager.BGM();

        Destroy(optionManager);
        optionManager = null;
    }

    // 승리
    public void Victory() {
        uiManager.TimeStop();
        uiManager.GameVictoryPanelOpen();
        isVictory = true;
        optionManager.BGM();

        Destroy(optionManager);
        optionManager = null;
    }

    // 오토 세이브
    IEnumerator AutoSave() {
        while(true) {
            if (cityNamePanel.activeSelf == false) {
                yield return new WaitForSecondsRealtime(autoSaveCycleTime);

                googleManager.SaveCloud();
            }
            else {
                yield return new WaitForSecondsRealtime(10);
            }
        }
    }

    // 랜덤 int 값 리턴
    int RandomInt(uint minNum, uint maxNum) {
        return (int)Random.Range(minNum, maxNum +1);
    }
}