using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartLoading : MonoBehaviour {
    [SerializeField] string nextSceneName;
    [SerializeField] GameObject txtStart;
    [SerializeField] GameObject loadingObj;
    [SerializeField] Image img_LoadingBar;
    [SerializeField] GameObject startBtnObj;
    [SerializeField] GameObject imgStartObj;
    AsyncOperation async;

    bool startClick = false;

    bool loadingComplete = false;

    void OnEnable() {
        if (loadingComplete == true) {
            StartButtonEnable();
        }
        else {
            loadingObj.SetActive(true);
            StartCoroutine(Loading());
        }
    }

    // 휴대폰 뒤로가기 버튼 (종료)
    void Update() {
        if (Application.platform == RuntimePlatform.Android) {
            if (Input.GetKey(KeyCode.Escape)) {
                Application.Quit();
                return;
            }
        }
    }

    // 비동기 로딩
    IEnumerator Loading() {
        yield return null;

        async = SceneManager.LoadSceneAsync(nextSceneName);
        async.allowSceneActivation = false;
        float timer = 0.0f;

        while (!async.isDone) {
            yield return null;
            timer += Time.deltaTime;

            if (async.progress < 0.9f) {
                img_LoadingBar.fillAmount = Mathf.Lerp(img_LoadingBar.fillAmount, async.progress, timer);

                if (img_LoadingBar.fillAmount >= async.progress) {
                    timer = 0f;
                }
            }
            else {
                img_LoadingBar.fillAmount = Mathf.Lerp(img_LoadingBar.fillAmount, 1f, timer);

                if (img_LoadingBar.fillAmount == 1.0f) {
                    img_LoadingBar.transform.parent.gameObject.SetActive(false);

                    loadingComplete = true;
                    StartButtonEnable();

                    yield break;
                }
            }
        }
    }

    // 스타트 버튼 활성화
    void StartButtonEnable() {
        txtStart.SetActive(true);
        imgStartObj.SetActive(true);
        StartCoroutine(LoadScene());
        startBtnObj.SetActive(true);
    }

    // 씬 로드
    IEnumerator LoadScene() {
        while(true) {
            yield return null;

            if (startClick == true) {
                startClick = false;
                async.allowSceneActivation = true;
                break;
            }
        }
    }

    // 시작 클릭 OR 터치
    public void StartClick() {
        startClick = true;
    }
}