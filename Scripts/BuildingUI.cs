using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingUI : MonoBehaviour {
    BuildingData bData;

    UIManager uIManager;

    void Start() {
        uIManager = GameObject.FindObjectOfType<UIManager>();
    }

    // 미니 UI 활성화
    public void BuildingUIOn() {
        if (GameManager.buildPrev == false) {
            gameObject.SetActive(true);
        }
    }

    // 철거
    public void Demolition() {
        bData = BuildingCtl.targetObj.GetComponent<Building>().buildingData;
        
        // 자원 반환        
        for (int i=0; i<bData.requireResource.Length; i++) {

            if (string.IsNullOrEmpty(bData.requireResource[i]) == false) {
                if (ulong.Parse(bData.requireResource[i]) > 0) {                 // 반환 자원은 바뀔 수 있음 (식이랑) 반환 자원은 바뀔 수 있음 (식이랑) 반환 자원은 바뀔 수 있음 (식이랑) 
                    switch (i) {
                        case 0 :
                            GameManager.s_resource.Budget += RatioCalculation(bData.requireResource[i], bData.durability);
                            break;
                        case 1 :
                            GameManager.s_resource.Wood += RatioCalculation(bData.requireResource[i], bData.durability);
                            break;
                        case 2 :
                            GameManager.s_resource.Stone += RatioCalculation(bData.requireResource[i], bData.durability);
                            break;
                        case 3 : 
                            GameManager.s_resource.Iron += RatioCalculation(bData.requireResource[i], bData.durability);
                            break;
                        case 4 : 
                            GameManager.s_resource.Food += RatioCalculation(bData.requireResource[i], bData.durability);
                            break;
                        case 5 : 
                            GameManager.s_resource.Electric += ulong.Parse(bData.requireResource[i]);
                            break;
                    }
                }
            }
        }

        // 게임매니저 리스트에서 건물빼고 Destroy
        for (int i=0; i<GameManager.buildingObj.Count; i++) {
            if (GameManager.buildingObj[i] == BuildingCtl.targetObj) {
                GameManager.buildingObj.RemoveAt(i);
                Destroy(BuildingCtl.targetObj);
            }
        }
    }

    // 내구도에 따른 결과값 계산
    ulong RatioCalculation(string res, decimal durability) {                 // 반환 자원은 바뀔 수 있음 (식이랑) 반환 자원은 바뀔 수 있음 (식이랑) 반환 자원은 바뀔 수 있음 (식이랑) 
        return (ulong)((ulong.Parse(res) * (decimal)0.5m) * (durability * (decimal)0.01m));
    }

    // 정보 보기
    public void Info_View() {
        uIManager.Info_On();
    }

    // 건물 선택 정보 BuildingCtl에 넘기기
    public void SelectBuildingPass() {
        BuildingCtl.targetObj = gameObject;
    }
}
