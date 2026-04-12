using UnityEngine;
using TMPro;

public class CollectibleItem : MonoBehaviour
{

    void Start()
    {
        uiText.gameObject.SetActive(false);
    }

    [Header("Configuration")]
    public string nomDeLobjet = "Objet inconnu";
    public string messageInteraction = "Appuyer sur E pour ramasser";
    public string messageTrouve = "L'objet a été trouvé !";
    public float displayDuration = 3f;
    

    [Header("UI")]
    public TextMeshProUGUI uiText;

    [Header("Trigger de la pièce")]
    public TriggerMessage triggerPiece; // relie le trigger de la pièce ici

    private bool dejaRamasse = false;

    // Appelé quand le joueur appuie sur E
    public void Ramasser()
    {
        if (dejaRamasse) return;
        dejaRamasse = true;

        Debug.Log(nomDeLobjet + " a été ajouté à l'inventaire.");

        // Prévient le trigger de la pièce que l'objet est trouvé
        if (triggerPiece != null)
            triggerPiece.ObjetTrouve(messageTrouve, displayDuration);

        // futur script d'inventaire

        Destroy(gameObject);
    }

    // Affiche le message d'interaction quand le joueur est proche
    private void OnTriggerEnter(Collider other)
    {
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

    private System.Collections.IEnumerator FadeEtCacher()
    {
        yield return StartCoroutine(Fade(1f, 0f, 0.3f));
        uiText.gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator Fade(float depart, float arrivee, float duree)
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