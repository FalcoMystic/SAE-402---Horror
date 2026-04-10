using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Configuration")]
    public string nomDeLobjet = "Objet inconnu";
    public string messageInteraction = "Appuyer sur E pour ramasser";

    public void Ramasser()
    {
        Debug.Log(nomDeLobjet + " a été ajouté à l'inventaire.");

        // futur script d'inventaire

        Destroy(gameObject); // L'objet disparaît de la scène
    }
}