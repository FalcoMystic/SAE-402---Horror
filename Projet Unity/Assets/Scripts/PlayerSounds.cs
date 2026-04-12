using UnityEngine;

public class PlayerSounds : MonoBehaviour
{

    [Header("Respiration")]
    public AudioSource respirationSource;
    public AudioClip respirationClip;

    [Header("Bruits de pas")]
    public AudioSource pasSource;
    public AudioClip bruitsDePas; // ← un seul clip

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Respiration en boucle
        if (respirationSource != null && respirationClip != null)
        {
            respirationSource.clip = respirationClip;
            respirationSource.loop = true;
            respirationSource.volume = 0.3f;
            respirationSource.Play();
        }

        // Configure les pas
        if (pasSource != null && bruitsDePas != null)
        {
            pasSource.clip = bruitsDePas;
            pasSource.loop = true; // ← boucle le clip
        }
    }

    void Update()
    {
        GererBruitsDePas();
    }

    private void GererBruitsDePas()
    {
        bool bougeSol = controller.isGrounded &&
    (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);

        if (bougeSol)
        {
            if (!pasSource.isPlaying)
                pasSource.Play();
        }
        else
        {
            if (pasSource.isPlaying)
                pasSource.Pause();
        }
    }
}