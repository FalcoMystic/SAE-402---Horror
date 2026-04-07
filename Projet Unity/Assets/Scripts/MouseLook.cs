using UnityEngine;

public class SimpleMouseLook : MonoBehaviour
{
    public float sensitivity = 2f;
    public Transform playerBody; // Glisse "Player_Leonard" ici dans l'Inspector
    
    private float rotationX = 0f;

    void Start()
    {
        // Bloque et cache la souris pour ne pas être gêné
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. Récupère les mouvements de la souris
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // 2. Rotation Verticale (Haut/Bas) : On fait tourner UNIQUEMENT la caméra
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f); // Empêche de faire un salto arrière
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // 3. Rotation Horizontale (Gauche/Droite) : On fait tourner TOUT le corps (le parent)
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}