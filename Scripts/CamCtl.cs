using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamCtl : MonoBehaviour {
    [SerializeField] float xCamLimit;
    [SerializeField] float yCamLimit;
    [SerializeField] float zCamLimit;

    [Space (20f)]
    [SerializeField] float minZoom;
    [SerializeField] float maxZoom;

    Vector2?[] touchPrevPos = {
        null,
        null
    };
    Vector2 touchPrevVector;
    float touchPrevDist;

    void LateUpdate() {
        if (Input.touchCount > 0) {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {          
                return;
            }
        }

        if (Input.touchCount == 0) {
            touchPrevPos[0] = null;
            touchPrevPos[1] = null;
        }
        else if (Input.touchCount == 1) {
            // 오브젝트 선택 예외처리
            if (GameManager.buildPrev == true) {
                return;
            }

            if (touchPrevPos[0] == null || touchPrevPos[1] != null) {
                touchPrevPos[0] = Input.GetTouch(0).position;
                touchPrevPos[1] = null;
            }
            else {
                Vector2 touchNewPos = Input.GetTouch(0).position;
                transform.position += transform.TransformDirection((Vector3)((touchPrevPos[0] - touchNewPos) * Camera.main.orthographicSize / Camera.main.pixelHeight * 2f));

                MoveLimit();

                touchPrevPos[0] = touchNewPos;
            }
        }
        else if (Input.touchCount == 2) {                           // 손가락 2개로 확대, 축소 해야하는 부분
            if (touchPrevPos[1] == null) {
                touchPrevPos[0] = Input.GetTouch(0).position;
                touchPrevPos[1] = Input.GetTouch(1).position;
                touchPrevVector = (Vector2)(touchPrevPos[0] - touchPrevPos[1]);
                touchPrevDist = touchPrevVector.magnitude;
            }
            else {
                Vector2 screen = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

                Vector2[] touchNewPos = { Input.GetTouch(0).position, Input.GetTouch(1).position };
                Vector2 touchNewVector = touchNewPos[0] - touchNewPos[1];
                float touchNewDist = touchNewVector.magnitude;

                transform.position += transform.TransformDirection((Vector3)((touchPrevPos[0] + touchPrevPos[1] - screen) * Camera.main.orthographicSize / screen.y));

                Camera.main.orthographicSize *= touchPrevDist / touchNewDist;

                transform.position -= transform.TransformDirection((touchNewPos[0] + touchNewPos[1] - screen) * Camera.main.orthographicSize / screen.y);

                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, minZoom);
                Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, maxZoom);

                touchPrevPos[0] = touchNewPos[0];
                touchPrevPos[1] = touchNewPos[1];
                touchPrevVector = touchNewVector;
                touchPrevDist = touchNewDist;
            }
        }
        else {
            return;
        }
    }

    void MoveLimit() {
        Vector3 tmp;
        tmp.x = Mathf.Clamp(transform.position.x, -xCamLimit, xCamLimit);
        tmp.y = Mathf.Clamp(transform.position.y, -yCamLimit, yCamLimit);
        tmp.z = Mathf.Clamp(transform.position.z, zCamLimit, zCamLimit);

        transform.position = tmp;
    }
}