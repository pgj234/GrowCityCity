using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;

public class BuildManager : MonoBehaviour {

    static BuildManager manager;

    // 싱글톤 프로퍼티
    public static BuildManager Manager {
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
    
    [SerializeField] GameManager gameManager;
    [SerializeField] UIManager uIManager;
    [SerializeField] GameObject[] buildingPrefab;
    [SerializeField] GameObject buildCompleteEffectPrefab;

    [SerializeField] AudioClip buildCompleteClip;

    OptionManager optionManager;

    BuildPrevCtl buildPrevCtl;
    BuildingData building_Data;
    GameObject BuildModeObj;

    static int sortingOrder;

    int buildingNum;

    void Awake() {
        Manager = this;
    }

    void Start() {
        Init();

        optionManager = GameObject.Find("OptionManager").GetComponent<OptionManager>();
    }

    void Init() {
        buildPrevCtl = null;
        building_Data = null;
        BuildModeObj = null;
        buildingNum = -1;
    }

    void ComponentReSetting() {
        buildPrevCtl = BuildModeObj.GetComponent<BuildPrevCtl>();
        building_Data = BuildModeObj.GetComponent<Building>().buildingData;
    }

    // 건설모드
    public void BuildMode(int buildingNumber) {
        if (BuildModeObj == null) {
            buildingNum = buildingNumber;
            BuildModeObj = Instantiate(buildingPrefab[buildingNum]);
            BuildModeObj.SetActive(false);

            if (ResourceCheck(BuildModeObj.GetComponent<Building>().buildingData.requireResource) == true) {            // 자원 검사
                GameManager.buildPrev = true;
                BuildModeObj.SetActive(true);
                
                ComponentReSetting();
            }
            else {
                Destroy(BuildModeObj);
                Init();
                uIManager.Notice_TXT("자원이 부족합니다");
            }
        }
        else {
            try {
                
            }
            catch {
                // Debug.LogError("BuildModeObj 있음");
            }
        }
    }

    // 건설모드 취소
    public void BuildModeExit() {
        if (BuildModeObj != null) {
            GameManager.buildPrev = false;
            Destroy(BuildModeObj);
            Init();
        }
        else {
            try {

            }
            catch {
                // Debug.LogError("BuildModeObj 없음");
            }
        }
    }

    // 건설
    public void SetBuild() {
        if (BuildModeObj != null) {
            Building building = BuildModeObj.GetComponent<Building>();

            if (GameManager.buildAble == true) {                    // 건설 가능 체크
                if (ResourceCheck(building.buildingData.requireResource) == true) {         // 자원 체크
                    UseResource(building.buildingData.requireResource);            // 자원 소모

                    // 건설 완료 후 처리
                    BuildCompleteAfter(BuildModeObj);

                    // 빌딩 데이터 세팅
                    SetBuildingData(building.buildingData);

                    //건설 이펙트
                    Instantiate(buildCompleteEffectPrefab, BuildModeObj.transform);
                    //건설 효과음
                    Play_SFX(buildCompleteClip, 3, 0.5f);

                    BuildModeObj = Instantiate(buildingPrefab[buildingNum], BuildModeObj.transform.position, UnityEngine.Quaternion.identity);
                    ComponentReSetting();

                    if (building.buildingData.PrefabName == "LANDMARK") {                           // 랜드마크 건설  승리!
                        gameManager.Victory();
                    }
                }
                else {
                    Destroy(BuildModeObj);
                    Init();
                    uIManager.Notice_TXT("자원이 부족합니다");
                }
            }
            else {
                uIManager.Notice_TXT("건설 가능한 위치가 아닙니다");
            }
        }
        else {
            try {

            }
            catch {
                // Debug.LogError("BuildModeObj 없음");
            }
        }
    }

    // 자원 검사
    bool ResourceCheck(string[] requireRes) {
        for (int i=0; i<requireRes.Length; i++) {

            if (string.IsNullOrEmpty(requireRes[i]) == false) {
                switch(i) {
                    // 도시예산
                    case 0 :
                        if (BigInteger.Parse(requireRes[i]) > GameManager.s_resource.Budget) {
                            return false;
                        }
                        break;
                    // 나무
                    case 1 :
                        if (BigInteger.Parse(requireRes[i]) > GameManager.s_resource.Wood) {
                            return false;
                        }
                        break;
                    // 돌
                    case 2 :
                        if (BigInteger.Parse(requireRes[i]) > GameManager.s_resource.Stone) {
                            return false;
                        }
                        break;
                    // 철
                    case 3 :
                        if (BigInteger.Parse(requireRes[i]) > GameManager.s_resource.Iron) {
                            return false;
                        }
                        break;
                    // 식량
                    case 4 :
                        if (BigInteger.Parse(requireRes[i]) > GameManager.s_resource.Food) {
                            return false;
                        }
                        break;
                    // 전기
                    case 5 :
                        if (BigInteger.Parse(requireRes[i]) > GameManager.s_resource.Electric) {
                            return false;
                        }
                        break;
                }
            }
        }

        return true;
    }

    // 자원 소모
    void UseResource(string[] requireRes) {
        for (int i=0; i<requireRes.Length; i++) {

            if (string.IsNullOrEmpty(requireRes[i]) == false) {
                switch(i) {
                    // 도시예산 소모
                    case 0 :
                        GameManager.s_resource.Budget -= ulong.Parse(requireRes[i]);
                        break;
                    // 나무 소모
                    case 1 :
                        GameManager.s_resource.Wood -= ulong.Parse(requireRes[i]);
                        break;
                    // 돌 소모
                    case 2 :
                        GameManager.s_resource.Stone -= ulong.Parse(requireRes[i]);
                        break;
                    // 철 소모
                    case 3 :
                        GameManager.s_resource.Iron -= ulong.Parse(requireRes[i]);
                        break;
                    // 식량 소모
                    case 4 :
                        GameManager.s_resource.Food -= ulong.Parse(requireRes[i]);
                        break;
                    // 전기 소모
                    case 5 :
                        GameManager.s_resource.Electric -= ulong.Parse(requireRes[i]);
                        break;
                }
            }
        }
    }

    // 건설 완료 후 처리
    public static void BuildCompleteAfter(GameObject BuildObj) {
        GameObject childObj = BuildObj.transform.GetChild(0).gameObject;

        childObj.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        BuildObj.GetComponent<CapsuleCollider2D>().enabled = true;
        BuildObj.transform.GetChild(1).GetComponent<Button>().enabled = true;
        Destroy(BuildObj.transform.GetChild(1).transform.GetChild(0).gameObject);
        Destroy(BuildObj.GetComponent<EdgeCollider2D>());
        Destroy(BuildObj.GetComponent<BuildPrevCtl>());

        SpriteRenderer childSpriteRenderer = childObj.GetComponent<SpriteRenderer>();
        childSpriteRenderer.sortingLayerName = "Object";
        childSpriteRenderer.sortingOrder = (int)(BuildObj.transform.position.y * -4);
        
        sortingOrder = childObj.GetComponent<SpriteRenderer>().sortingOrder;
    }

    // 빌딩 데이터 세팅
    public void SetBuildingData(BuildingData data) {
        data.location = BuildModeObj.transform.position;
        data.orderInLayer = sortingOrder;

        GameManager.buildingObj.Add(BuildModeObj);
    }

    //효과음 n번 재생
    IEnumerator Play_SFX(AudioClip clip, uint playCnt, float playInterval) {
        for (int i=0; i<playCnt; i++) {
            optionManager.SFX(clip);

            yield return new WaitForSecondsRealtime(playInterval);
        }
    }
}