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

    void Start()
    {
        if (uiCamera != null) uiCamera.SetActive(false);
        if (flashUI != null) flashUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(touchePOV)) TogglePOV();
        if (Input.GetKeyDown(toucheRangeCam) && enModePOV) TogglePOV();

        // On dessine le rayon dans la fenêtre "Scene" pour t'aider à débugger
        // La ligne sera rouge si tu es en mode POV
        if (enModePOV)
        {
            Debug.DrawRay(camLentille.transform.position, camLentille.transform.forward * distanceDetection, Color.red);
        }

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