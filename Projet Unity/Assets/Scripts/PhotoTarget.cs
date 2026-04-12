using UnityEngine;
using UnityEngine.Events;

public class PhotoTarget : MonoBehaviour
{
    [Header("Paramètres")]
    public string objectName; // Nom pour le debug
    public bool canBePhotographedOnlyOnce = true;

    [Header("Audio (Optionnel)")]
    public AudioSource audioSource;
    public AudioClip soundEffect;

    [Header("Événement")]
    public UnityEvent onPhotoTaken; // Ce qui se passe quand on prend la photo

    private bool hasBeenPhotographed = false;

    public void TriggerPhotoEffect()
    {
        // 1. Vérification si déjà pris
        if (canBePhotographedOnlyOnce && hasBeenPhotographed) return;

        // 2. Message console
        Debug.Log("Photo réussie sur : " + objectName);

        hasBeenPhotographed = true;

        // 3. Gestion du Son
        if (audioSource != null && soundEffect != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
        
        // 4. Déclenchement de l'événement Unity (actions supplémentaires)
        if (onPhotoTaken != null)
        {
            onPhotoTaken.Invoke();
        }
    }
}