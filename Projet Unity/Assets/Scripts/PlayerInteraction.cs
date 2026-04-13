using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 3f;
    public Camera playerCam;
    public GameObject texteInteraction; // Glisse ton texte TMP ici

    void Update()
    {
        Debug.Log("Update tourne"); // ← toute première ligne

        // 1. DÉTECTION CONTINUE (pour afficher le message "Appuyer sur E")
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        bool aTrouveInteraction = false;

        if (Physics.Raycast(ray, out hit, interactRange))
        {

            Debug.Log("Raycast touche : " + hit.collider.name); // ← ajoute ça
            // On vérifie si c'est une porte OU un item
            if (hit.collider.GetComponentInParent<Door>() != null || hit.collider.GetComponentInParent<CollectibleItem>() != null)
            {
                aTrouveInteraction = true;
                Debug.Log("Interaction trouvée !"); // ← et ça
            }
            else
            {
                Debug.Log("Pas de Door/Item sur : " + hit.collider.name); // ← et ça
            }
        }
        else
        {
            Debug.Log("Raycast ne touche rien (range: " + interactRange + ")"); // ← et ça
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
            CollectibleItem item = hit.collider.GetComponentInParent<CollectibleItem>();

            if (item != null)
            {
                item.Ramasser();
                return; // On a fini
            }
        }
    }

}