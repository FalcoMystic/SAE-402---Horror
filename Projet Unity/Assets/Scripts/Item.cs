using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class CollectibleItem : MonoBehaviour
{
    [Header("Configuration")]
    public string nomDeLobjet = "Objet inconnu";
    public string messageInteraction = "Appuyer sur E pour ramasser";
    public string messageTrouve = "L'objet a été trouvé !";
    public float displayDuration = 3f;

    [Header("Action Spéciale (ex: Déverrouiller Porte)")]
    public UnityEvent onPickUpAction;

    [Header("UI")]
    public TextMeshProUGUI uiText;

    [Header("Trigger de la pièce")]
    public TriggerMessage triggerPiece;

    [Header("Activation d'objets")]
    public GameObject objetAActiver; // ← glisse le bloc SceneTrigger ici

    private bool dejaRamasse = false;

    void Start()
    {
        // Fusion des deux Start() pour éviter l'erreur CS0111
        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);

            // On s'assure que le texte est transparent au début pour le Fade
            Color couleur = uiText.color;
            couleur.a = 0f;
            uiText.color = couleur;
        }
        else
        {
            // Message d'alerte pratique pour le débuggage en SAE
            Debug.LogWarning("uiText non assigné sur : " + gameObject.name);
        }
    }

    // --- CETTE FONCTION EST APPELÉE PAR LE SCRIPT DE LA CAMÉRA ---
    public void Ramasser()
    {
        if (dejaRamasse) return;
        dejaRamasse = true;

        // Activation de l'objet lié (ex: déclencheur de scène)
        if (objetAActiver != null)
            objetAActiver.SetActive(true);

        Debug.Log(nomDeLobjet + " a été ajouté à l'inventaire.");

        // Déclenche l'événement Unity (ouverture de porte, etc.)
        if (onPickUpAction != null)
            onPickUpAction.Invoke();

        // Envoie le message au trigger de la pièce
        if (triggerPiece != null)
            triggerPiece.ObjetTrouve(messageTrouve, displayDuration);

        // Notifie le GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.ObjetRamasse();

        // On cache le texte proprement avant de détruire l'objet
        if (uiText != null) uiText.gameObject.SetActive(false);

        Destroy(gameObject);
    }

    // --- GESTION DE L'AFFICHAGE DU TEXTE (Trigger Zone) ---
    private void OnTriggerEnter(Collider other)
    {
        // Vérifie si c'est bien le joueur qui entre dans la zone
        if (other.CompareTag("Player") && uiText != null)
        {
            uiText.text = messageInteraction;
            uiText.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(Fade(0f, 1f, 0.3f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && uiText != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeEtCacher());
        }
    }

    private IEnumerator FadeEtCacher()
    {
        yield return StartCoroutine(Fade(1f, 0f, 0.3f));
        uiText.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float depart, float arrivee, float duree)
    {
        float elapsed = 0f;
        Color couleur = uiText.color;
        while (elapsed < duree)
        {
            elapsed += Time.deltaTime;
            couleur.a = Mathf.Lerp(depart, arrivee, elapsed / duree);
            uiText.color = couleur;
            yield return null;
        }
        couleur.a = arrivee;
        uiText.color = couleur;
    }
}