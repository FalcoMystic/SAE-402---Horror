using UnityEngine;
using UnityEngine.Events;

public class PhotoTarget : MonoBehaviour
{
    [Header("Paramètres")]
    [Tooltip("Le nom qui apparaîtra dans la console")]
    public string objectName; 
    public bool canBePhotographedOnlyOnce = true;
    
    [Header("Audio (Optionnel)")]
    [Tooltip("Le composant AudioSource sur cet objet")]
    public AudioSource audioSource;
    [Tooltip("Le clip audio à jouer lors de la photo")]
    public AudioClip soundEffect;

    [Header("Événement")]
    public UnityEvent onPhotoTaken; 

    private bool hasBeenPhotographed = false;

    public void TriggerPhotoEffect()
    {
        // 1. Vérification si déjà pris
        if (canBePhotographedOnlyOnce && hasBeenPhotographed) return;

        // 2. Message console
        Debug.Log("photo take : " + objectName);
        
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