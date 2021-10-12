using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    static UIManager manager;

    // 싱글톤 프로퍼티
    public static UIManager Manager {
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

    #region 자원 Text

    [SerializeField] Text txtBudget;
    [SerializeField] Text txtWood;
    [SerializeField] Text txtStone;
    [SerializeField] Text txtIron;
    [SerializeField] Text txtFood;
    [SerializeField] Text txtElectric;

    #endregion

    [Space(10f)]
    [SerializeField] Text txtWeek;
    [SerializeField] Text txtPopulation;
    [SerializeField] Text txtHappiness;
    [SerializeField] Text txtTaxRate;

    [SerializeField] Text txtCityName;
    [SerializeField] Text txtCityNameInput;

    [SerializeField] Text txtDurability;
    [SerializeField] Text txtBuildingName;

    [Space(10f)]
    [SerializeField] Image imgHappinessGauge;

    [SerializeField] Slider sliderTaxControl;

    [SerializeField] GameObject txtNoticeObj;
    [SerializeField] GameObject txtCityNameNoticeObj;

    [SerializeField] GameObject buildingInfoPanelObj;

    [Header("이펙트")]
    [Space(10f)]
    [SerializeField] GameObject coinEffect;

    [Space(10f)]
    [SerializeField] AudioClip coinClip;


    [Header("인포 프리팹")]
    [SerializeField] GameObject infoPrefab;

    [Header("자원 스프라이트")]
    [Space(10f)]
    [SerializeField] Sprite budgetSprite;
    [SerializeField] Sprite woodSprite;
    [SerializeField] Sprite stoneSprite;
    [SerializeField] Sprite ironSprite;
    [SerializeField] Sprite foodSprite;
    [SerializeField] Sprite electricSprite;

    [Header("퀘스트 버튼 색")]
    [Space(10f)]
    [SerializeField] Color questNotCompleteColor;
    [SerializeField] Color questCompleteColor;
    [SerializeField] Color questCompletedColor;

    [Space(10f)]
    [SerializeField] GameObject gameManagerObj;
    [SerializeField] GameObject questContentObj;
    [SerializeField] AudioClip sfx_OnClip;

    [Space(10f)]
    [SerializeField] GameObject cityName_Panel;
    [SerializeField] GameObject gameover_Panel;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] GameObject quitPanel;

    OptionManager optionManager;
    GoogleManager googleManager;

    List<GameObject> infoPrefabList = new List<GameObject>();

    GameObject imgInfoObj;

    BuildingData bData;

    uint k = (uint)Mathf.Pow(10, 3);
    uint m = (uint)Mathf.Pow(10, 6);
    uint b = (uint)Mathf.Pow(10, 9);
    ulong t = (ulong)Mathf.Pow(10, 12);
    ulong q = (ulong)Mathf.Pow(10, 15);
    
    void Awake() {
        Manager = this;
        optionManager = GameObject.Find("OptionManager").GetComponent<OptionManager>();
    }

    void Start() {
        imgInfoObj = buildingInfoPanelObj.transform.GetChild(0).transform.GetChild(2).gameObject;

        txtNoticeObj.SetActive(false);

        googleManager = GameObject.FindObjectOfType<GoogleManager>();
    }

    public void SceneLoad(string str) {
        Destroy(GameObject.Find("OptionManager"));
        Destroy(GameObject.Find("GoogleManager"));
        Time.timeScale = 1;
        SceneManager.LoadScene(str);
    }

    // 도시 이름 짓기
    public void SetCityName() {
        if (string.IsNullOrEmpty(txtCityNameInput.text) == false) {
            GameManager.CityName = txtCityNameInput.text;
            Save();

            cityName_Panel.SetActive(false);
            TimePlay();
        }
        else {
            txtCityNameNoticeObj.SetActive(false);
            txtCityNameNoticeObj.SetActive(true);
        }
    }

    // 행복도
    public void Happiness_View(int happinessNum) {
        imgHappinessGauge.fillAmount = happinessNum * 0.01f;
        txtHappiness.text = happinessNum.ToString() + "%";
    }

    #region 인포 관련

    // 인포 On
    public void Info_On() {

        if (infoPrefabList != null) {
            InfoPrefabDestroy();            // 풀링 가능하면 풀링  풀링 가능하면 풀링  풀링 가능하면 풀링  풀링 가능하면 풀링  풀링 가능하면 풀링  풀링 가능하면 풀링  풀링 가능하면 풀링
        }

        buildingInfoPanelObj.SetActive(true);
        
        bData = BuildingCtl.targetObj.GetComponent<Building>().buildingData;

        if (bData.durability > 0) {
            txtDurability.text = "내구도 : " + ((uint)(bData.durability)).ToString();
        }
        else {
            txtDurability.text = "파괴됨";
        }

        txtBuildingName.text = bData.buildingName;

        InfoCheck();
    }

    // 정보 체크
    void InfoCheck() {

            for (int i=0; i<bData.requireResource.Length; i++) {
                switch (i) {
                    case 0 :
                        if (bData.budget > 0) {
                            InfoPrefabCreate(budgetSprite, bData.budget);
                        }
                        break;
                    case 1 :
                        if (bData.wood > 0) {
                            InfoPrefabCreate(woodSprite, bData.wood);
                        }
                        break;
                    case 2 :
                        if (bData.stone > 0) {
                            InfoPrefabCreate(stoneSprite, bData.stone);
                        }
                        break;
                    case 3 : 
                        if (bData.iron > 0) {
                            InfoPrefabCreate(ironSprite, bData.iron);
                        }
                        break;
                    case 4 : 
                        if (bData.food > 0) {
                            InfoPrefabCreate(foodSprite, bData.food);
                        }
                        break;
                    case 5 : 
                        if (bData.electric > 0) {
                            InfoPrefabCreate(electricSprite, bData.electric);
                        }
                        break;
                }
            }
    }

    // 정보 프리팹 생성
    void InfoPrefabCreate(Sprite resourceSprite, int infoStr) {
        GameObject infoObj = Instantiate(infoPrefab, imgInfoObj.transform);

        infoObj.transform.GetChild(0).GetComponent<Image>().sprite = resourceSprite;
        infoObj.transform.GetChild(1).GetComponent<Text>().text = infoStr.ToString();

        infoPrefabList.Add(infoObj);
    }

    // 정보 프리팹 파괴
    void InfoPrefabDestroy() {
        for (int i=0; i<infoPrefabList.Count; i++) {
            Destroy(infoPrefabList[i]);     // 풀링 가능하면 풀링  풀링 가능하면 풀링  풀링 가능하면 풀링  풀링 가능하면 풀링  풀링 가능하면 풀링  풀링 가능하면 풀링  풀링 가능하면 풀링
        }

        infoPrefabList.Clear();
    }

    #endregion

    #region 퀘스트 관련

    // 퀘스트
    public void QuestReward_View(GameObject questObj, string str) {
        QuestData qData = questObj.GetComponent<Quest>().questData;
        Text txt = questObj.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
        Image questBtnImage = questObj.transform.GetChild(1).GetComponent<Image>();
        Button questBtn = questObj.transform.GetChild(1).GetComponent<Button>();

        if (qData.rewardReceiveChk == true) {               // 보상을 받았을 때
            // questBtnImage.color = questCompletedColor;
            // questBtn.enabled = false;
            // txt.text = "완료";
            questObj.SetActive(false);
        }
        else {
            if (qData.progress >= qData.rewardRequire) {        // 퀘스트 클리어 후 보상 버튼 활성화
                questBtnImage.color = questCompleteColor;
                questBtn.enabled = true;
                txt.text = "보상";
            }
            else {                                                  // 퀘스트 진행 중
                questBtnImage.color = questNotCompleteColor;
                questBtn.enabled = false;
                txt.text = str;
            }
        }
    }

    // 퀘스트 새로고침
    public void QuestRefresh() {
        if (questContentObj.transform.childCount > 0 == false) {
            return;
        }

        string str;

        for (int i=0; i<questContentObj.transform.childCount; i++) {
            uint conditionSatisfiedCnt = 0;
            QuestData qData = questContentObj.transform.GetChild(i).GetComponent<Quest>().questData;

            for (int j=0; j<GameManager.buildingObj.Count; j++) {
                if ((qData.conditionStr + "(Clone)") == GameManager.buildingObj[j].name) {
                    conditionSatisfiedCnt += 1;
                    qData.progress = conditionSatisfiedCnt;

                    if (qData.rewardRequire <= conditionSatisfiedCnt) {
                        break;
                    }
                }
            }

            str = string.Format("{0} / {1}", conditionSatisfiedCnt, qData.rewardRequire);
            QuestReward_View(questContentObj.transform.GetChild(i).gameObject, str);
        }
    }

    // 퀘스트 보상 받기
    public void QuestRewardGet() {
        GameObject questObj = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        QuestData qData = questObj.GetComponent<Quest>().questData;

        switch(qData.rewardIndex) {
            case 0 :
                GameManager.s_resource.Budget += (ulong)qData.rewardNum;
                qData.rewardReceiveChk = true;
                QuestReward_View(questObj, "");

                EffectInstantiate(coinEffect);
                optionManager.SFX(coinClip);
                break;
        }
    }

    #endregion

    #region TEXT 관련

    public void CityName_TXT(string str) {
        txtCityName.text = str;
    }

    public void Notice_TXT(string str) {
        txtNoticeObj.SetActive(false);
        txtNoticeObj.SetActive(true);
        txtNoticeObj.GetComponent<Text>().text = str;
    }

    public void TaxRate_TXT(string str) {
        txtTaxRate.text = str;
    }

    public void Week_View_TXT(string str) {
        txtWeek.text = str;
    }

    public void Population_View_TXT(int num) {
        txtPopulation.text = ConversionNum((ulong)num);
    }

    public void Budget_View_TXT(ulong bigInt) {
        txtBudget.text = ConversionNum(bigInt);
    }

    public void Wood_View_TXT(ulong bigInt) {
        txtWood.text = ConversionNum(bigInt);
    }

    public void Stone_View_TXT(ulong bigInt) {
        txtStone.text = ConversionNum(bigInt);
    }
    
    public void Iron_View_TXT(ulong bigInt) {
        txtIron.text = ConversionNum(bigInt);
    }

    public void Food_View_TXT(ulong bigInt) {
        txtFood.text = ConversionNum(bigInt);
    }

    public void Electric_View_TXT(ulong bigInt) {
        txtElectric.text = ConversionNum(bigInt);
    }

    #endregion

    #region 타임 관련

    public void TimeStop() {
        GameManager.timeStop = true;
    }

    public void TimePlay() {
        GameManager.timeStop = false;
        Time.timeScale = 1;
    }

    public void TimeTwice() {
        GameManager.timeStop = false;
        Time.timeScale = 2;
    }

    #endregion

    #region 옵션 관련

    public void BGM_On() {
        PlayerPrefs.SetInt("BGM_OnOff", 1);
        optionManager.BGM();
    }

    public void BGM_Off() {
        PlayerPrefs.SetInt("BGM_OnOff", 0);
        optionManager.BGM();
    }

    public void SFX_On() {
        PlayerPrefs.SetInt("SFX_OnOff", 1);
        optionManager.SFX(sfx_OnClip);
    }

    public void SFX_Off() {
        PlayerPrefs.SetInt("SFX_OnOff", 0);
    }

    #endregion

    // 세율 조정
    public void TaxControl() {
        GameManager.TaxRate = (decimal)sliderTaxControl.value;
    }

    public void Save() {
        googleManager.SaveCloud();
    }

    public void GameOverPanelOpen() {
        gameover_Panel.SetActive(true);
    }

    public void GameVictoryPanelOpen() {
        victoryPanel.SetActive(true);
    }

    // 숫자 단위 변환(string)
    string ConversionNum(ulong num) {

        if (q <= num) {
            return (num / q).ToString() + " Q";
        }
        else if (t <= num && num < q) {
            return (num / t).ToString() + " T";
        }
        else if (b <= num && num < t) {
            return (num / b).ToString() + " B";
        }
        else if (m <= num && num < b) {
            return (num / m).ToString() + " M";
        }
        else if (k <= num && num < m) {
            return (num / k).ToString() + " K";
        }

        return num.ToString();
    }

    void Update() {
        Quit();
    }

    // 기본 이펙트
    void EffectInstantiate(GameObject effect) {
        Instantiate(effect, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, +10)), Quaternion.identity);
    }

    // 휴대폰 뒤로가기 버튼 (종료 확인)
    void Quit() {
        if (Application.platform == RuntimePlatform.Android) {
            if (Input.GetKey(KeyCode.Escape)) {
                quitPanel.SetActive(true);
                TimeStop();
            }
        }
    }

    // 종료
    public void Quit_Yes() {
        Application.Quit();
    }
}