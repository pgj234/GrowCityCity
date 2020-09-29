using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    // 휴대폰 뒤로가기 버튼 (종료)
    void Update() {
        if (Application.platform == RuntimePlatform.Android) {
            if (Input.GetKey(KeyCode.Escape)) {
                Application.Quit();
                return;
            }
        }
    }

    // 씬 로드
    public void SceneLoad(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
