using UnityEngine;
using TMPro;
using UnityEngine.Events; // ← INDISPENSABLE pour les événements

public class CollectibleItem : MonoBehaviour
{
    [Header("Configuration")]
    public string nomDeLobjet = "Objet inconnu";
    public string messageInteraction = "Appuyer sur E pour ramasser";
    public string messageTrouve = "L'objet a été trouvé !";
    public float displayDuration = 3f;

    [Header("Action Spéciale (ex: Déverrouiller Porte)")]
    public UnityEvent onPickUpAction; // ← Cet événement apparaîtra dans l'inspecteur

    [Header("UI")]
    public TextMeshProUGUI uiText;

    [Header("Trigger de la pièce")]
    public TriggerMessage triggerPiece;

    private bool dejaRamasse = false;

    void Start()
    {
        if (uiText != null)
            uiText.gameObject.SetActive(false);
    }

    public void Ramasser()
    {
        if (dejaRamasse) return;
        dejaRamasse = true;

        Debug.Log(nomDeLobjet + " a été ajouté à l'inventaire.");

        // --- DÉCLENCHEMENT DE L'ACTION ---
        if (onPickUpAction != null)
            onPickUpAction.Invoke(); // Appelle tout ce qui est listé dans l'inspecteur

        if (triggerPiece != null)
            triggerPiece.ObjetTrouve(messageTrouve, displayDuration);

        if (GameManager.Instance != null)
            GameManager.Instance.ObjetRamasse();

        Destroy(gameObject);
    }

    // Garde tes méthodes OnTriggerEnter, OnTriggerExit et Fade telles quelles...
}