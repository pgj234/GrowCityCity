using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.Linq;

public class EventManager : MonoBehaviour {

    static EventManager manager;

    // 싱글톤 프로퍼티
    public static EventManager Manager {
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

    OptionManager optionManager;
    [SerializeField] UIManager uIManager;

    [SerializeField] GameObject eventPanel;
    [SerializeField] float eventPanelCloseMinTime;

    List<EventData> eventDataList = new List<EventData>();
    List<EventData> tmpEDataList = new List<EventData>();

    Image imgEventSpace;
    Text txtEventTitle;
    Text txtEventContent;

    //uint variableMultiple = 1;

    #region 이벤트 넘버링 텍스트

    // 0 : 지진
    // 1 : 홍수

    #endregion

    void Awake() {
        imgEventSpace = eventPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        txtEventTitle = eventPanel.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
        txtEventContent = eventPanel.transform.GetChild(0).transform.GetChild(2).GetComponent<Text>();
    }

    void Start() {
        optionManager = GameObject.Find("OptionManager").GetComponent<OptionManager>();

        eventDataList = EventSetting.Instance.eventData;
    }

    // 이벤트 Set
    public void SetEvent() {
        tmpEDataList.Clear();
        tmpEDataList = eventDataList;

        int randNum = -1;

        for (int i=0; i< tmpEDataList.Count; i++) {
            randNum = RandomInt(0, tmpEDataList.Count -1);

            if (EventConditionCheck(tmpEDataList[randNum]) == true) {           //조건이 안맞을 시   임시 이벤트 리스트에서 지우고   다시 돌리기
                tmpEDataList.Remove(tmpEDataList[randNum]);
            }
            else {
                uIManager.TimeStop();
                eventPanel.SetActive(true);

                optionManager.SFX(tmpEDataList[randNum].eventAudioClip);

                imgEventSpace.sprite = eventDataList[randNum].eventSprite;
                txtEventTitle.text = eventDataList[randNum].eventName;
                txtEventTitle.color = eventDataList[randNum].eventNameColor;
                txtEventContent.text = eventDataList[randNum].content;

                StartCoroutine(EventEffect(tmpEDataList[randNum], tmpEDataList[randNum].effectDuration));

                StartCoroutine(CloseEventPanel());

                break;
            }
        }
    }

    // 이벤트 조건 체크
    bool EventConditionCheck(EventData eData) {
        if (eData.conditionGeneration > GameManager.generation) {        // 최소 시대 조건이 안맞는 경우        조건 추가 추가 추가 추가 추가 추가 추가 추가 추가 추가 추가 추가 추가 추가 추가
            return true;
        }

        return false;
    }

    // 이벤트로 인한 영향
    IEnumerator EventEffect(EventData tmpEData, uint effectDuration) {

        uint tmpWeek = 0;
        uint duration = effectDuration;

        // 약한 건물 내구도 영향
        if (tmpEData.absoluteWeakBuildingDurability != 0) {                       // 건물 내구도 영향 있을 시
            for (int i=0; i<GameManager.buildingObj.Count; i++) {
                GameManager.buildingObj[i].GetComponent<Building>().buildingData.durability += tmpEData.absoluteWeakBuildingDurability;
            }
        }

        // 강한 건물 내구도 영향
        // if (tmpEData.absoluteStrongBuildingDurability != 0) {                       // 건물 내구도 영향 있을 시
        //     for (int i=0; i<GameManager.buildingObj.Count; i++) {
        //         GameManager.buildingObj[i].GetComponent<Building>().buildingData.durability += tmpEData.absoluteStrongBuildingDurability;
        //     }
        // }

        // 약한 인구 퍼센트 영향
        GameManager.s_resource.Population = (int)(GameManager.s_resource.Population * (1 + (tmpEData.percentWeakPopulation * (decimal)0.01m)));

        // 강한 인구 퍼센트 영향
        // GameManager.s_resource.Population = (int)(GameManager.s_resource.Population * (1 + (tmpEData.percentStrongPopulation * (decimal)0.01m)));


        while (duration != 0) {

            tmpWeek = GameManager.Week;
            while (GameManager.Week != tmpWeek +1) {
                yield return null;
            }

            // 약한 행복도 영향
            if (tmpEData.absoluteWeakHappiness != 0) {
                GameManager.s_resource.Happiness += tmpEData.absoluteWeakHappiness;
            }

            // 강한 행복도 영향
            // if (tmpEData.absoluteStrongHappiness != 0) {
            //     GameManager.s_resource.Happiness += tmpEData.absoluteStrongHappiness;
            // }


            // 약한 절대값 영향
            // GameManager.s_resource.Population += tmpEData.absoluteWeakPopulation;

            // for (int i=0; i<tmpEData.absoluteWeakResources.Length; i++) {
            //     switch (i) {
            //         case 0 :
            //             GameManager.s_resource.Budget += tmpEData.absoluteWeakResources[i] / tmpEData.effectDuration;
            //             break;
            //         case 1 :
            //             GameManager.s_resource.Wood += tmpEData.absoluteWeakResources[i] / (int)tmpEData.effectDuration;
            //             break;
            //         case 2 :
            //             GameManager.s_resource.Stone += tmpEData.absoluteWeakResources[i] / (int)tmpEData.effectDuration;
            //             break;
            //         case 3 :
            //             GameManager.s_resource.Iron += tmpEData.absoluteWeakResources[i] / (int)tmpEData.effectDuration;
            //             break;
            //         case 4 :
            //             GameManager.s_resource.Food += tmpEData.absoluteWeakResources[i] / (int)tmpEData.effectDuration;
            //             break;
            //         case 5 :
            //             GameManager.s_resource.Electric += tmpEData.absoluteWeakResources[i];
            //             break;
            //     }
            // }

            // 강한 절대값 영향
            // GameManager.s_resource.Population += tmpEData.absoluteStrongPopulation;

            // for (int i=0; i<tmpEData.absoluteStrongResources.Length; i++) {
                // switch (i) {
                //     case 0 :
                //         GameManager.s_resource.Budget += tmpEData.absoluteStrongResources[i] / tmpEData.effectDuration;
                //         break;
                //     case 1 :
                //         GameManager.s_resource.Wood += tmpEData.absoluteStrongResources[i] / (int)tmpEData.effectDuration;
                //         break;
                //     case 2 :
                //         GameManager.s_resource.Stone += tmpEData.absoluteStrongResources[i] / (int)tmpEData.effectDuration;
                //         break;
                //     case 3 :
                //         GameManager.s_resource.Iron += tmpEData.absoluteStrongResources[i] / (int)tmpEData.effectDuration;
                //         break;
                //     case 4 :
                //         GameManager.s_resource.Food += tmpEData.absoluteStrongResources[i] / (int)tmpEData.effectDuration;
                //         break;
                //     case 5 :
                //         GameManager.s_resource.Electric += tmpEData.absoluteStrongResources[i];
                //         break;
                // }
            // }

            for (int i=0; i<tmpEData.percentWeakResources.Length; i++) {
                if (tmpEData.percentWeakResources[i] != 0) {                                        // 추가 추가
                    switch (i) {
                        case 0 :
                            GameManager.s_resource.Budget = (ulong)(GameManager.s_resource.Budget * (1 + ((tmpEData.percentWeakResources[i] * (decimal)0.01m)) / tmpEData.effectDuration));
                            break;
                        case 1 :
                            GameManager.s_resource.Wood  = (ulong)(GameManager.s_resource.Wood * (1 + ((tmpEData.percentWeakResources[i] * (decimal)0.01m)) / tmpEData.effectDuration));
                            break;
                        case 2 :
                            GameManager.s_resource.Stone  = (ulong)(GameManager.s_resource.Stone * (1 + ((tmpEData.percentWeakResources[i] * (decimal)0.01m)) / tmpEData.effectDuration));
                            break;
                        case 3 :
                            GameManager.s_resource.Iron  = (ulong)(GameManager.s_resource.Iron * (1 + ((tmpEData.percentWeakResources[i] * (decimal)0.01m)) / tmpEData.effectDuration));
                            break;
                        case 4 :
                            GameManager.s_resource.Food  = (ulong)(GameManager.s_resource.Food * (1 + ((tmpEData.percentWeakResources[i] * (decimal)0.01m)) / tmpEData.effectDuration));
                            break;
                        case 5 :
                            GameManager.s_resource.Electric  = (ulong)(GameManager.s_resource.Electric * (1 + ((tmpEData.percentWeakResources[i] * (decimal)0.01m)) / tmpEData.effectDuration));
                            break;
                    }
                }
            }

            // for (int i=0; i<tmpEData.percentStrongResources.Length; i++) {
            //     switch (i) {
            //         case 0 :
            //             GameManager.s_resource.Budget += tmpEData.percentStrongResources[i] / tmpEData.effectDuration;
            //             break;
            //         case 1 :
            //             GameManager.s_resource.Wood += tmpEData.percentStrongResources[i] / (int)tmpEData.effectDuration;
            //             break;
            //         case 2 :
            //             GameManager.s_resource.Stone += tmpEData.percentStrongResources[i] / (int)tmpEData.effectDuration;
            //             break;
            //         case 3 :
            //             GameManager.s_resource.Iron += tmpEData.percentStrongResources[i] / (int)tmpEData.effectDuration;
            //             break;
            //         case 4 :
            //             GameManager.s_resource.Food += tmpEData.percentStrongResources[i] / (int)tmpEData.effectDuration;
            //             break;
            //         case 5 :
            //             GameManager.s_resource.Electric += tmpEData.percentStrongResources[i];
            //             break;
            //     }
            // }

            duration -= 1;
        }
    }

    // 랜덤 int 값 리턴
    int RandomInt(int minNum, int maxNum) {
        return Random.Range(minNum, maxNum +1);
    }

    // 이벤트 패널 지연시간 두고 닫을 수 있게
    IEnumerator CloseEventPanel() {
        yield return new WaitForSecondsRealtime(eventPanelCloseMinTime);

        while (Input.GetMouseButtonDown(0) == false) {
            yield return null;
        }

        eventPanel.SetActive(false);
        uIManager.TimePlay();
    }
}