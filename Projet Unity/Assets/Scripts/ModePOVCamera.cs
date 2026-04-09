using UnityEngine;
using System.IO;
using System.Collections;

public class ModePOVCamera : MonoBehaviour
{
    [Header("Touches")]
    public KeyCode touchePOV = KeyCode.V; 
    public KeyCode toucheRangeCam = KeyCode.C;
    public KeyCode touchePhoto = KeyCode.Mouse0; // Clic gauche pour prendre la photo

    [Header("Caméras")]
    public Camera yeuxJoueur;  // Ta Main Camera
    public Camera camLentille; // La caméra enfant du modèle 3D

    [Header("Rendu")]
    public RenderTexture textureEcran;
    public GameObject uiCamera; // Ton Canvas avec le REC, la batterie, etc.

    [Header("Effets")]
    public GameObject flashUI; // Un simple Panel blanc pour l'effet flash

    private bool enModePOV = false;
    private bool cameraSortie = false; // la caméra est entre les mains du joueur ?

    void Start()
    {
        // On s'assure que tout est bien éteint au début
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
        // 1. MASQUER l'interface de la caméra (le REC, etc.)
        if (uiCamera != null) uiCamera.SetActive(false);

        // 2. ACTIVER le flash blanc pour le feedback visuel
        if (flashUI != null) flashUI.SetActive(true);

        // 3. ATTENDRE la fin de la frame pour que Unity valide le masquage de l'UI
        yield return new WaitForEndOfFrame();

        // 4. CRÉER LE FICHIER
        string nomFichier = "Photo_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        // Enregistre dans le dossier persistant du PC (AppData/LocalLow/TonProjet)
        string cheminComplet = Path.Combine(Application.persistentDataPath, nomFichier);

        // 5. CAPTURE D'ÉCRAN (L'écran est "propre" ici car l'UI est désactivée)
        ScreenCapture.CaptureScreenshot(cheminComplet);

        // On attend un tout petit peu (0.05s) pour que le flash soit visible par le joueur
        yield return new WaitForSeconds(0.05f);

        // 6. RÉACTIVER l'interface et éteindre le flash
        if (uiCamera != null) uiCamera.SetActive(true);
        if (flashUI != null) flashUI.SetActive(false);

        Debug.Log("PHOTO PROPRE ENREGISTRÉE : " + cheminComplet);
    }

    void TogglePOV()
    {
        enModePOV = !enModePOV;

        if (enModePOV)
        {
            // --- ON ENTRE DANS LA LENTILLE ---
            yeuxJoueur.enabled = false;
            camLentille.targetTexture = null;
            if (uiCamera != null) uiCamera.SetActive(true);
        }
        else
        {
            // --- ON REVIENT AUX YEUX DU JOUEUR ---
            yeuxJoueur.enabled = true;
            camLentille.targetTexture = textureEcran;
            if (uiCamera != null) uiCamera.SetActive(false);
        }
    }

    void LateUpdate()
    {
        // Aligne la caméra 3D sur le regard du joueur (pour regarder en haut/bas)
        if (enModePOV && yeuxJoueur != null && camLentille != null)
        {
            camLentille.transform.rotation = yeuxJoueur.transform.rotation;
        }
    }
}