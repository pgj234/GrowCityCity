using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrevUI : MonoBehaviour {
    BuildManager buildManager;

    void Start() {
        buildManager = GameObject.Find("BuildManager").GetComponent<BuildManager>();
    }

    public void Confirm() {
        buildManager.SetBuild();
    }

    public void Cancel() {
        buildManager.BuildModeExit();
    }

    public void PrevUIDestroy() {
        if (GameManager.buildAble == true) {
            Destroy(gameObject);
        }
    }
}