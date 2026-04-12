using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 3f; 
    public Camera playerCam;
    public GameObject texteInteraction; // Glisse ton texte TMP ici

    void Update()
    {
        // 1. DÉTECTION CONTINUE (pour afficher le message "Appuyer sur E")
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        bool aTrouveInteraction = false;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // On vérifie si c'est une porte OU un item
            if (hit.collider.GetComponent<Door>() != null || hit.collider.GetComponent<CollectibleItem>() != null)
            {
                aTrouveInteraction = true;
            }
        }

        // Affiche ou cache le texte "Appuyer sur E"
        if (texteInteraction != null) texteInteraction.SetActive(aTrouveInteraction);


        // 2. ACTION (quand on appuie sur E)
        if (Input.GetKeyDown(KeyCode.E) && aTrouveInteraction)
        {
            // Vérification Porte
            Door door = hit.collider.GetComponent<Door>();
            if (door != null)
            {
                door.ToggleDoor(playerCam.transform);
                return; // On a fini
            }

            // Vérification Item
            CollectibleItem item = hit.collider.GetComponent<CollectibleItem>();

            if (item != null)
            {
                item.Ramasser();
                return; // On a fini
            }
        }
    }
}