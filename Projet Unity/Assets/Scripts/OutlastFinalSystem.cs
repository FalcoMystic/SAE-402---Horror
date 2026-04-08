using UnityEngine;

public class OutlastFinalSystem : MonoBehaviour
{
    [Header("L'Appareil 3D")]
    public GameObject dossierAppareil; // Ton modèle 3D "VRAIE_CAM"
    
    [Header("Interface & Effets")]
    public GameObject canvasREC;       // Ton Canvas avec le texte REC, la batterie, etc.
    public GameObject volumeNocturne;  // L'objet qui contient ton effet vert (Post-Processing Volume)

    [Header("Positions")]
    public Vector3 posPoche = new Vector3(0.5f, -1.5f, 0.5f);
    // On augmente le Z à 0.6 ou 0.8 pour que la caméra soit plus loin devant et ne rentre pas dans le crâne !
    public Vector3 posViseur = new Vector3(0f, -0.2f, 0.6f); 
    public float vitesseAnimation = 10f;

    private bool estSortie = false;
    private bool visionNocturneActive = false;

    void Start()
    {
        // On éteint les effets au début
        if(canvasREC != null) canvasREC.SetActive(false);
        if(volumeNocturne != null) volumeNocturne.SetActive(false);
        
        // On place l'appareil en bas et on s'assure qu'il est visible
        dossierAppareil.transform.localPosition = posPoche;
        dossierAppareil.SetActive(true);
    }

    void Update()
    {
        // 1. LE CLIC DROIT : Sortir ou Ranger la caméra
        if (Input.GetButtonDown("Fire2"))
        {
            estSortie = !estSortie;

            if (!estSortie) // Si on range la caméra...
            {
                // On éteint l'écran REC et la vision nocturne
                if(canvasREC != null) canvasREC.SetActive(false);
                visionNocturneActive = false;
                if(volumeNocturne != null) volumeNocturne.SetActive(false);
                
                // On réaffiche le modèle 3D pour le voir descendre dans la poche
                dossierAppareil.SetActive(true);
            }
        }

        // 2. L'ANIMATION DE MONTÉE/DESCENTE
        Vector3 cible = estSortie ? posViseur : posPoche;
        dossierAppareil.transform.localPosition = Vector3.Lerp(dossierAppareil.transform.localPosition, cible, Time.deltaTime * vitesseAnimation);

        // 3. LA MAGIE DE LA POV : Quand la caméra arrive devant les yeux
        // On calcule la distance. Si elle est presque arrivée (moins de 0.05 de distance)...
        if (estSortie && Vector3.Distance(dossierAppareil.transform.localPosition, posViseur) < 0.05f)
        {
            // On cache le modèle 3D pour qu'il ne bloque pas la vue
            dossierAppareil.SetActive(false);
            // On allume l'interface de la caméra (le REC)
            if(canvasREC != null) canvasREC.SetActive(true);
        }

        // 4. LA VISION NOCTURNE (Touche N)
        // On ne peut l'allumer que si la caméra est en mode "POV complète" (modèle caché)
        if (estSortie && dossierAppareil.activeSelf == false)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                visionNocturneActive = !visionNocturneActive;
                if(volumeNocturne != null) volumeNocturne.SetActive(visionNocturneActive);
            }
        }
    }
}