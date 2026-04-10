using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [Header("Destination")]
    [Tooltip("Écris le nom exact de la scène cible (ex: A2)")]
    public string nomDeLaSceneCible;

    private void OnTriggerEnter(Collider other)
    {
        // 1. On vérifie que c'est bien le joueur qui entre dans le cube
        // Ton objet "Player_Leonard" DOIT avoir le Tag "Player" dans l'Inspector
        if (other.CompareTag("Player"))
        {
            // 2. On vérifie si le TransitionManager existe dans la scène
            if (TransitionManager.Instance != null)
            {
                Debug.Log("Déclenchement du changement de scène vers : " + nomDeLaSceneCible);
                
                // 3. On demande au Manager de lancer la transition
                TransitionManager.Instance.ChargerScene(nomDeLaSceneCible);
            }
            else
            {
                // Si ce message s'affiche, c'est que tu n'as pas lancé le jeu depuis 
                // la scène où se trouve l'objet SYSTEM_TRANSITION (ton Menu).
                Debug.LogError("Erreur : Le TransitionManager est introuvable dans cette scène !");
            }
        }
    }
}