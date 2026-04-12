using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("Éléments UI")]
    public GameObject introPanel;      // Ton RawImage (le journal)
    public TextMeshProUGUI introText;  // Ton texte (TMP)

    [Header("Réglages Timing")]
    public float delaiAvantTexte = 2f;  // Temps d'attente avant que le texte commence
    public float delaiApresTexte = 4f;  // Temps de lecture une fois le texte fini
    public float vitesseEcriture = 0.05f; // Temps entre chaque lettre (plus petit = plus vite)
    
    [Header("Configuration Scène")]
    public string nomDeLaSceneDeJeu = "Jeu"; // Nom de ta scène de début de jeu

    void Start()
    {
        // On cache tout au début
        if (introPanel != null) introPanel.SetActive(false);
        if (introText != null) 
        {
            introText.text = ""; // On vide le texte par défaut
            introText.gameObject.SetActive(false);
        }

        // Lance la séquence automatiquement
        StartCoroutine(SequenceIntro());
    }

    IEnumerator SequenceIntro()
    {
        // 1. Afficher le journal
        if (introPanel != null) introPanel.SetActive(true);

        // 2. Pause dramatique
        yield return new WaitForSeconds(delaiAvantTexte);

        // 3. Afficher le texte lettre par lettre
        if (introText != null) 
        {
            introText.gameObject.SetActive(true);
            yield return StartCoroutine(EcrireTextePetitAPetit("Je vais te retrouver Sylvain..."));
        }

        // 4. Temps de lecture une fois le texte complet
        yield return new WaitForSeconds(delaiApresTexte);

        // 5. Charger le jeu
        SceneManager.LoadScene(nomDeLaSceneDeJeu);
    }

    // Coroutine pour l'effet machine à écrire
    IEnumerator EcrireTextePetitAPetit(string texteComplet)
    {
        introText.text = ""; // On s'assure qu'il est vide
        foreach (char lettre in texteComplet.ToCharArray())
        {
            introText.text += lettre; // Ajoute une lettre
            yield return new WaitForSeconds(vitesseEcriture); // Petite pause
        }
    }
}