using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingCtl : MonoBehaviour {
    public static GameObject targetObj;

    void Awake() {
        targetObj = null;
    }

    void LateUpdate() {
        if (targetObj != null && Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                StartCoroutine(MiniUIOff(targetObj));
                return;
            }
            else {
                targetObj.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
                targetObj = null;
            }
        }

        // if (GameManager.buildPrev == false && Input.GetMouseButtonDown(0)) {
        //     if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
        //         return;
        //     }
        //     else {
        //         CastRay();
        //     }
        // }
    }
    IEnumerator MiniUIOff(GameObject targetObj) {
        yield return new WaitForSecondsRealtime(Time.deltaTime * 5);

        targetObj.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
    }

    // 건물 선택 빔
    // void CastRay() {
    //     targetObj = null;

    //     Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

    //     if (hit.collider != null) {
    //         Building building = hit.transform.GetComponent<Building>();

    //         if (building != null) {
    //             targetObj = hit.collider.gameObject;
    //             targetObj.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
    //         }
    //     }
    // }
}
