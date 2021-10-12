using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour {

    static OptionManager manager;

    // 싱글톤 프로퍼티
    public static OptionManager Manager {
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

    public static AudioSource audioSource;
    [SerializeField] AudioClip mainBgmClip;
    [SerializeField] AudioClip playBgmClip;
    [SerializeField] AudioClip gameoverBgmClip;
    [SerializeField] AudioClip gameVictoryBgmClip;

    void Awake() {
        Manager = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        Init();
        BGM();
    }

    void Init() {
        audioSource = GetComponent<AudioSource>();
        
        // BGM
        PlayerPrefs.GetInt("BGM_OnOff", 1);

        // SFX
        PlayerPrefs.GetInt("SFX_OnOff", 1);
    }

    // BGM
    public void BGM() {
        if (PlayerPrefs.GetInt("BGM_OnOff", 1) == 1) {
            if (SceneManager.GetActiveScene().name == "Main") {
                audioSource.clip = mainBgmClip;
                audioSource.Play();
            }
            else if (GameManager.isGameover == true) {
                audioSource.clip = gameoverBgmClip;
                audioSource.Play();
            }
            else if (GameManager.isVictory == true) {
                audioSource.clip = gameVictoryBgmClip;
                audioSource.Play();
            }
            else {
                audioSource.clip = playBgmClip;
                audioSource.Play();
            }
        }
        else {
            audioSource.Stop();
        }
    }

    // 효과음
    public void SFX(AudioClip sfx_Clip) {
        if (sfx_Clip == null) {
            return;
        }

        if (PlayerPrefs.GetInt("SFX_OnOff", 1) == 1) {
            try {
                audioSource.PlayOneShot(sfx_Clip);
            }
            catch {
                // Debug.Log("효과음 NULL");
            }
        }
    }
}