using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    Light lt;
    public float minIntensity = 0f;
    public float maxIntensity = 2f;
    public float flickerSpeed = 0.05f;

    void Start() => lt = GetComponent<Light>();

    void Update()
    {
        if (Random.value < flickerSpeed)
            lt.intensity = Random.Range(minIntensity, maxIntensity);
    }
}
