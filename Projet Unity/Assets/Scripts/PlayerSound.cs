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
        bool bougeSol = controller.isGrounded && controller.velocity.magnitude > 0.1f;

        if (bougeSol)
        {
            if (!pasSource.isPlaying)
                pasSource.Play(); // ← reprend où il s'était arrêté
        }
        else
        {
            if (pasSource.isPlaying)
                pasSource.Pause(); // ← pause pour reprendre au bon endroit
        }
    }
}