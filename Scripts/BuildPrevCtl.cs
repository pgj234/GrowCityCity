using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildPrevCtl : MonoBehaviour {

    [SerializeField] float camZ;
    [SerializeField] float gridCellSizeX;
    [SerializeField] float gridCellSizeY;

    SpriteRenderer spriteRenderer;
    Color buildAbleSpriteColor;
    Color buildUnableSpriteColor;

    void Start() {
        buildAbleSpriteColor.r = 100f / 255f;
        buildAbleSpriteColor.g = 230f / 255f;
        buildAbleSpriteColor.b = 90f / 255f;
        buildAbleSpriteColor.a = 200f / 255f;

        buildUnableSpriteColor.r = 255f / 255f;
        buildUnableSpriteColor.g = 50f / 255f;
        buildUnableSpriteColor.b = 50f / 255f;
        buildUnableSpriteColor.a = 200f / 255f;

        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer.color = buildAbleSpriteColor;
    }

    void OnTriggerStay2D(Collider2D col) {
        if (col.tag == "Object" || col.tag == "BuildUnAblePlatform") {
            GameManager.buildAble = false;
            spriteRenderer.color = buildUnableSpriteColor;
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.tag == "Object" || col.tag == "BuildUnAblePlatform") {
            GameManager.buildAble = true;
            spriteRenderer.color = buildAbleSpriteColor;
        }
    }

    void LateUpdate() {
        if (Input.touchCount > 0) {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                return;
            }
            else {
                if (Input.touchCount == 1) {
                    transform.position = ResultTouchGridPos();
                }
            }
        }
    }

    // 타일에 맞는 터치 포지션 리턴
    Vector3 ResultTouchGridPos() {
        Vector3 touchPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, camZ);
        Vector3 touchFixPos = Camera.main.ScreenToWorldPoint(touchPos);

        touchFixPos.x = touchFixPos.x / gridCellSizeX;
        touchFixPos.y = touchFixPos.y / gridCellSizeY;
        touchFixPos.x = Mathf.Round(touchFixPos.x) * gridCellSizeX;
        touchFixPos.y = Mathf.Round(touchFixPos.y) * gridCellSizeY;
        return touchFixPos;
    }
}