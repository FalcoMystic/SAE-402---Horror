using UnityEngine;
using TMPro;

public class TriggerMessage : MonoBehaviour
{
    void Start()
    {
        uiText.gameObject.SetActive(false);
    }

    [Header("Message de la pièce")]
    public string messageEntree = "Un objet est présent dans la pièce...";
    public float displayDuration = 3f;

    [Header("UI")]
    public TextMeshProUGUI uiText;

    private bool messageAffiche = false;
    private bool objetTrouve = false;
    private Coroutine coroutineEnCours; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !messageAffiche && !objetTrouve)
        {
            messageAffiche = true;

            if (coroutineEnCours != null) 
                StopCoroutine(coroutineEnCours);

            coroutineEnCours = StartCoroutine(ShowMessage(messageEntree, displayDuration)); // ← MODIFIÉ
        }
    }

    public void ObjetTrouve(string message, float duration)
    {
        objetTrouve = true;

        if (coroutineEnCours != null) 
            StopCoroutine(coroutineEnCours);

        coroutineEnCours = StartCoroutine(ShowMessage(message, duration)); // ← MODIFIÉ
    }

    private System.Collections.IEnumerator ShowMessage(string msg, float duration)
    {
        uiText.text = msg;
        uiText.gameObject.SetActive(true);

        yield return StartCoroutine(Fade(0f, 1f, 0.5f));
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(Fade(1f, 0f, 0.5f));

        uiText.gameObject.SetActive(false);
        coroutineEnCours = null; 
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