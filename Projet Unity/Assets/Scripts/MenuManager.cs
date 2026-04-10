using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Configuration")]
    public string nomDeLaSceneIntro = "Intro"; // Mets le nom exact de ta scène avec le journal

    // Fonction pour le bouton Jouer
    public void JouerJeu()
    {
        SceneManager.LoadScene(nomDeLaSceneIntro);
    }

    // Fonction pour le bouton Quitter
    public void QuitterJeu()
    {
        Application.Quit();
        Debug.Log("Le jeu a été fermé");
    }
}