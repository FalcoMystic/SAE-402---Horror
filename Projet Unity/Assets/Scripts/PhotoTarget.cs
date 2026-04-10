using UnityEngine;
using UnityEngine.Events;

public class PhotoTarget : MonoBehaviour
{
    [Header("Paramètres")]
    public string objectName; // Nom pour le debug
    public bool canBePhotographedOnlyOnce = true;
    
    [Header("Événement")]
    public UnityEvent onPhotoTaken; // Ce qui se passe quand on prend la photo

    private bool hasBeenPhotographed = false;

    public void TriggerPhotoEffect()
    {
        if (canBePhotographedOnlyOnce && hasBeenPhotographed) return;

        Debug.Log("Photo réussie sur : " + objectName);
        hasBeenPhotographed = true;
        
        // Déclenche l'événement configuré dans l'Inspector Unity
        if (onPhotoTaken != null)
            onPhotoTaken.Invoke();
    }
}