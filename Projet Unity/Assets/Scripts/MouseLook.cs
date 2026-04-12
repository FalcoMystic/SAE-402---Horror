using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleMouseLook : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensitivity = 2f;
    public Transform playerBody;

    [Header("Crosshair")]
    public bool showCrosshair = true;
    [Range(2f, 16f)]
    public float crosshairSize = 6f;
    public Color crosshairColor = Color.white;

    [Header("Puzzle UI")]
    public Canvas puzzleCanvas;
    public KeyCode puzzleToggleKey = KeyCode.R;
    
    private float rotationX = 0f;
    private bool isMouseLocked = false;
    private bool isPuzzleOpen = false;

    void Start()
    {
        if (puzzleCanvas != null)
        {
            puzzleCanvas.enabled = false;
        }
        isMouseLocked = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        // Clic gauche pour verrouiller la souris SEULEMENT sur le monde 3D (pas sur UI)
        if (Input.GetMouseButtonDown(0) && !isPuzzleOpen && !IsPointerOverUI())
        {
            LockMouse(true);
        }

        // Touche dédiée: appui 1 ouvre l'épreuve, appui 2 referme et reverrouille la souris
        if (Input.GetKeyDown(puzzleToggleKey))
        {
            if (isPuzzleOpen)
            {
                ClosePuzzle(true);
            }
            else
            {
                OpenPuzzle();
            }
        }

        // Echap pour fermer le puzzle ou déverrouiller la souris
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPuzzleOpen)
            {
                ClosePuzzle(true);
            }
            else if (isMouseLocked)
            {
                LockMouse(false);
            }
        }

        // Rotation caméra seulement si souris verrouillée et puzzle fermé
        if (!isMouseLocked || isPuzzleOpen)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void OpenPuzzle()
    {
        if (isPuzzleOpen)
            return;

        isPuzzleOpen = true;
        if (puzzleCanvas != null)
        {
            puzzleCanvas.enabled = true;
        }
        LockMouse(false);
        Debug.Log("Puzzle ouvert - Appuie sur la touche d'interaction pour fermer");
    }

    void ClosePuzzle(bool relockMouse)
    {
        if (!isPuzzleOpen)
            return;

        isPuzzleOpen = false;
        if (puzzleCanvas != null)
        {
            puzzleCanvas.enabled = false;
        }
        if (relockMouse)
        {
            LockMouse(true);
        }
        Debug.Log("Puzzle fermé");
    }

    void LockMouse(bool locked)
    {
        isMouseLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    void OnGUI()
    {
        if (!showCrosshair)
        {
            return;
        }

        Color previousColor = GUI.color;
        GUI.color = crosshairColor;

        float x = (Screen.width - crosshairSize) * 0.5f;
        float y = (Screen.height - crosshairSize) * 0.5f;
        GUI.DrawTexture(new Rect(x, y, crosshairSize, crosshairSize), Texture2D.whiteTexture);

        GUI.color = previousColor;
    }
}