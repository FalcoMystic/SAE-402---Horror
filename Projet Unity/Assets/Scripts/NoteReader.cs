using UnityEngine;

public class NoteReader : MonoBehaviour
{
    public GameObject panneauNote; // Glisse ton Panel "AffichageNote" ici
    private bool noteOuverte = false;

    public void OuvrirNote()
    {
        panneauNote.SetActive(true);
        noteOuverte = true;

        // Optionnel : On peut mettre le jeu en pause
        Time.timeScale = 0f;
        // Et libérer la souris si besoin
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Si la note est ouverte et qu'on appuie sur E ou Escape
        if (noteOuverte && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)))
        {
            FermerNote();
        }
    }

    public void FermerNote()
    {
        panneauNote.SetActive(false);
        noteOuverte = false;

        // On relance le temps et on cache la souris
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}