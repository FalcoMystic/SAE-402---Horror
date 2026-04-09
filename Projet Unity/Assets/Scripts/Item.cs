using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;

    public void Collect()
    {
        Debug.Log("Objet ramassé : " + itemName);
        // Ajouter du son ou mettre l'objet dans un inventaire
        Destroy(gameObject);
    }
}