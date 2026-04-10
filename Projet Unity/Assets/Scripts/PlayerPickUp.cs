using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    [Header("Paramètres")]
    public float distanceInteraction = 3f; // Portée du bras du joueur
    public KeyCode toucheRamasser = KeyCode.E;

    [Header("UI (text)")]
    public GameObject texteInteraction; // "Appuyer sur E"

    void Update()
    {
        // On crée un rayon qui part du centre de la vue
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // On vérifie si le rayon touche quelque chose
        if (Physics.Raycast(ray, out hit, distanceInteraction))
        {
            // On essaie de récupérer le script CollectibleItem sur l'objet touché
            CollectibleItem item = hit.collider.GetComponent<CollectibleItem>();

            if (item != null)
            {
                // On affiche le message d'aide si tu as configuré l'UI
                if (texteInteraction != null) texteInteraction.SetActive(true);

                // Si on appuie sur E
                if (Input.GetKeyDown(toucheRamasser))
                {
                    item.Ramasser();
                }
            }
            else
            {
                // Si l'objet n'est pas ramassable, on cache l'UI
                if (texteInteraction != null) texteInteraction.SetActive(false);
            }
        }
        else
        {
            // Si on ne regarde rien, on cache l'UI
            if (texteInteraction != null) texteInteraction.SetActive(false);
        }
    }
}