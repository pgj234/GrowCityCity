using UnityEngine;

public class EffectDestroy : MonoBehaviour {

    float lifeTime;

    void Awake() {
        lifeTime = GetComponent<ParticleSystem>().duration;
    }

    void Start() {
        Destroy(gameObject, lifeTime);
    }
}