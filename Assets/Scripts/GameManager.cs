using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public struct s_Select {
        public bool selectObj;
    }
    public static s_Select s_select;

    void Awake() {
        s_select.selectObj = false;
    }
}
