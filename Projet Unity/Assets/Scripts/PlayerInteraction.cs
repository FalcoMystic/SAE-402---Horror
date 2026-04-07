using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 3f; 
    public Camera playerCam;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactRange))
            {
                Debug.Log("Le rayon a touché : " + hit.collider.gameObject.name);

                Door door = hit.collider.GetComponent<Door>();
                if (door != null)
                {
                    Debug.Log("Super, l'objet a bien le script Door ! Ouverture...");
                    door.ToggleDoor();
                }
                else
                {
                    Debug.Log("Aïe, l'objet touché n'a PAS le script Door d'attaché !");
                }
            }
            else
            {
                Debug.Log("Le rayon n'a absolument rien touché (tu es trop loin ou l'objet n'a pas de Collider).");
            }
        }
    }
}