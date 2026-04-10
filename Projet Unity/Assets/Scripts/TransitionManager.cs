using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("Réglages")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeSpeed = 1.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // On s'assure que l'objet racine ne meurt jamais
            DontDestroyOnLoad(gameObject.transform.root.gameObject);
            if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChargerScene(string nomScene)
    {
        StartCoroutine(SequenceTransition(nomScene));
    }

    private IEnumerator SequenceTransition(string nomScene)
    {
        if (fadeCanvasGroup == null)
        {
            SceneManager.LoadScene(nomScene);
            yield break;
        }

        // 1. Fondu au noir
        float timer = 0;
        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeSpeed;
            fadeCanvasGroup.alpha = timer;
            yield return null;
        }

        // 2. Chargement de la scène
        AsyncOperation op = SceneManager.LoadSceneAsync(nomScene);
        while (!op.isDone) yield return null;

        yield return new WaitForSeconds(0.3f);

        // 3. Retour à la lumière
        while (timer > 0f)
        {
            timer -= Time.deltaTime * fadeSpeed;
            fadeCanvasGroup.alpha = timer;
            yield return null;
        }
        fadeCanvasGroup.alpha = 0;
    }
}