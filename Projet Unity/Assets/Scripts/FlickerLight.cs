using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    [Header("Lumière")]
    private Light lt;
    public float minIntensity = 0f;
    public float maxIntensity = 2f;
    public float flickerSpeed = 0.05f;

    [Header("Son")]
    public AudioSource audioSource;
    public AudioClip[] sonsFlicker; // plusieurs sons pour varier
    [Range(0f, 1f)]
    public float volume = 0.3f;
    public float chanceDeJouerSon = 0.3f; // 30% de chance de jouer un son

    void Start()
    {
        lt = GetComponent<Light>();

        // Crée automatiquement l'AudioSource si pas assignée
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f; // son 3D
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (Random.value < flickerSpeed)
        {
            lt.intensity = Random.Range(minIntensity, maxIntensity);

            Debug.Log("Flicker! Sons disponibles : " + sonsFlicker.Length + " | AudioSource : " + (audioSource != null));

            if (sonsFlicker.Length > 0 && Random.value < chanceDeJouerSon)
            {
                if (!audioSource.isPlaying)
                {
                    AudioClip clip = sonsFlicker[Random.Range(0, sonsFlicker.Length)];
                    Debug.Log("Joue le son : " + clip.name);
                    audioSource.PlayOneShot(clip, volume);
                }
            }
        }
    }
}