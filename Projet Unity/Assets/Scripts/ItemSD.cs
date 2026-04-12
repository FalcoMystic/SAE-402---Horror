using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public partial class CollectibleItem : MonoBehaviour
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

    private bool dejaRamasse = false;

    void Start()
    {
        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);

        }
    }

    // --- CETTE FONCTION EST APPELÉE PAR LE SCRIPT DE LA CAMÉRA ---
    public void Ramasser()
    {
        if (dejaRamasse) return;
        dejaRamasse = true;

        Debug.Log(nomDeLobjet + " a été ajouté à l'inventaire.");

        if (onPickUpAction != null)
            onPickUpAction.Invoke();

        if (triggerPiece != null)
            triggerPiece.ObjetTrouve(messageTrouve, displayDuration);

        if (GameManager.Instance != null)
            GameManager.Instance.ObjetRamasse();

        // On cache le texte avant de détruire l'objet
        if (uiText != null) uiText.gameObject.SetActive(false);

        Destroy(gameObject);
    }

    // --- GESTION DE L'AFFICHAGE DU TEXTE ---
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Quelque chose est entré dans la zone : " + other.name);
        // Vérifie si c'est bien le joueur et si l'UI est assignée
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