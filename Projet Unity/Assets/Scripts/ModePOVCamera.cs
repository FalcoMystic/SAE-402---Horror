using UnityEngine;
using System.IO;
using System.Collections;

public class ModePOVCamera : MonoBehaviour
{
    [Header("Touches")]
    public KeyCode touchePOV = KeyCode.V; 
    public KeyCode toucheRangeCam = KeyCode.C;
    public KeyCode touchePhoto = KeyCode.Mouse0;

    [Header("Caméras")]
    public Camera yeuxJoueur;
    public Camera camLentille;

    [Header("Rendu")]
    public RenderTexture textureEcran;
    public GameObject uiCamera;

    [Header("Effets")]
    public GameObject flashUI;

    [Header("Détection Photo")]
    public float distanceDetection = 10f; 
    public LayerMask layerCible; 

    private bool enModePOV = false;
    private bool cameraSortie = false; // la caméra est entre les mains du joueur ?

    void Start()
    {
        if (uiCamera != null) uiCamera.SetActive(false);
        if (flashUI != null) flashUI.SetActive(false);
    }

    void Update()
        {
            // 1. GESTION DE LA TOUCHE C (Sortir/Ranger l'objet caméra)
            if (Input.GetKeyDown(toucheRangeCam))
            {
                if (enModePOV)
                {
                    // Si on range alors qu'on regardait dedans, on quitte le mode POV d'abord
                    TogglePOV();
                }

                cameraSortie = !cameraSortie;
                Debug.Log(cameraSortie ? "Caméra sortie" : "Caméra rangée");

            }

            // 2. GESTION DE LA TOUCHE V (Regarder dans la lentille)
            // On ajoute la condition : il faut que cameraSortie soit TRUE
            if (Input.GetKeyDown(touchePOV) && cameraSortie)
            {
                TogglePOV();
            }

            // 3. PHOTO (Seulement en POV)
            if (enModePOV && Input.GetKeyDown(touchePhoto))
            {
                StartCoroutine(PrendrePhotoPropre());
            }
        }

    IEnumerator PrendrePhotoPropre()
    {
        // --- LOGIQUE DE DETECTION AVEC DEBUG ---
        Ray ray = camLentille.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanceDetection, layerCible))
        {
            PhotoTarget target = hit.collider.GetComponent<PhotoTarget>();
            if (target != null)
            {
                // MESSAGE DE SUCCÈS
                Debug.Log("<color=green>SUCCÈS :</color> Photo prise de " + hit.collider.gameObject.name);
                target.TriggerPhotoEffect();
            }
            else
            {
                // MESSAGE D'OBJET SANS SCRIPT
                Debug.Log("<color=yellow>INFO :</color> Tu as photographié '" + hit.collider.gameObject.name + "' mais il n'a pas de script PhotoTarget.");
            }
        }
        else
        {
            // MESSAGE DE VIDE
            Debug.Log("<color=red>ECHEC :</color> Tu as pris une photo du vide.");
        }

        // --- RESTE DU CODE (Flash et Capture) ---
        if (uiCamera != null) uiCamera.SetActive(false);
        if (flashUI != null) flashUI.SetActive(true);

        yield return new WaitForEndOfFrame();

        string nomFichier = "Photo_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string cheminComplet = Path.Combine(Application.persistentDataPath, nomFichier);
        ScreenCapture.CaptureScreenshot(cheminComplet);

        yield return new WaitForSeconds(0.05f);

        if (uiCamera != null) uiCamera.SetActive(true);
        if (flashUI != null) flashUI.SetActive(false);
    }

    void TogglePOV()
    {
        enModePOV = !enModePOV;
        if (enModePOV)
        {
            yeuxJoueur.enabled = false;
            camLentille.targetTexture = null;
            if (uiCamera != null) uiCamera.SetActive(true);
        }
        else
        {
            yeuxJoueur.enabled = true;
            camLentille.targetTexture = textureEcran;
            if (uiCamera != null) uiCamera.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (enModePOV && yeuxJoueur != null && camLentille != null)
        {
            camLentille.transform.rotation = yeuxJoueur.transform.rotation;
        }
    }
}