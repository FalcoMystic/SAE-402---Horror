using UnityEngine;
public class DoubleDoor : MonoBehaviour

{
    private bool estVerrouillee = true;

    public void Deverrouiller()
    {
        estVerrouillee = false;
        Debug.Log("La double porte est maintenant déverrouillée !");
        // Tu peux ajouter ici un son de verrou qui saute
    }

    public void OpenDoor()
    {
        if(!estVerrouillee) {
            // Logique d'ouverture (animation)
        } else {
            Debug.Log("C'est fermé à clé...");
        }
    }
}