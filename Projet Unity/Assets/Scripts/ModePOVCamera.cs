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
    public Camera yeuxJoueur;  // Ta Main Camera
    public Camera camLentille; // La caméra enfant du modèle 3D

    [Header("Rendu")]
    public RenderTexture textureEcran;
    public GameObject uiCamera; // Ton Canvas (REC, batterie...)

    [Header("Effets")]
    public GameObject flashUI; // Panel blanc pour l'effet flash

    [Header("Paramètres de Détection")]
    public float distancePhoto = 10f; // Portée du clic photo

    private bool enModePOV = false;
    private bool cameraSortie = false;

    void Start()
    {
        if (uiCamera != null) uiCamera.SetActive(false);
        if (flashUI != null) flashUI.SetActive(false);
    }

    void Update()
    {
        // 1. Sortir ou Ranger la caméra
        if (Input.GetKeyDown(toucheRangeCam))
        {
            if (enModePOV) TogglePOV();
            cameraSortie = !cameraSortie;
        }

        // 2. Regarder dans la lentille
        if (Input.GetKeyDown(touchePOV) && cameraSortie)
        {
            TogglePOV();
        }

        // 3. Prendre la photo
        if (enModePOV && Input.GetKeyDown(touchePhoto))
        {
            StartCoroutine(PrendrePhotoPropre());
        }
    }

    IEnumerator PrendrePhotoPropre()
    {
        // --- DETECTION DE L'OBJET (RAYCAST) ---
        // On tire un rayon depuis le centre de la vue (0.5, 0.5)
        Ray ray = camLentille.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distancePhoto))
        {
            PhotoTarget cible = hit.collider.GetComponent<PhotoTarget>();

            if (cible != null)
            {
                // Appelle la fonction qui fera le Debug.Log("photo take : " + name)
                cible.TriggerPhotoEffect();
            }
        }

        // --- EFFETS VISUELS ET CAPTURE ---
        if (uiCamera != null) uiCamera.SetActive(false);
        if (flashUI != null) flashUI.SetActive(true);

        yield return new WaitForEndOfFrame();

        // Enregistrement du fichier PNG
        string nomFichier = "Photo_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string cheminComplet = Path.Combine(Application.persistentDataPath, nomFichier);
        ScreenCapture.CaptureScreenshot(cheminComplet);

        yield return new WaitForSeconds(0.05f);

        if (uiCamera != null) uiCamera.SetActive(true);
        if (flashUI != null) flashUI.SetActive(false);

        // Note : J'ai retiré le Debug.Log ici pour que tu ne vois que celui de l'objet
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