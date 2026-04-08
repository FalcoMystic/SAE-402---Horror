using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Déplacement (Ambiance Horreur)")]
    [Tooltip("Une vitesse réduite augmente la tension.")]
    public float walkSpeed = 3.0f; 
    public float gravity = -15.0f; // Gravité un peu plus forte pour un sentiment de lourdeur

    [Header("Effet de Caméra (Head Bobbing)")]
    public Camera playerCamera;
    [Tooltip("Vitesse du balancement (lent = pas prudents)")]
    public float bobSpeed = 10f; 
    [Tooltip("Amplitude du balancement (intensité du pas)")]
    public float bobAmount = 0.08f; 

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float defaultCameraY;
    private float timer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (playerCamera != null)
        {
            defaultCameraY = playerCamera.transform.localPosition.y;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleHeadBob();
    }

    private void HandleMovement()
    {
        // 1. Vérification du sol
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // 2. Mouvement ZQSD (Géré nativement par les axes "Horizontal" et "Vertical" de Unity)
        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");   

        // 3. Application du déplacement
        Vector3 move = transform.right * x + transform.forward * z;
        
        // On normalise le vecteur pour éviter que le joueur aille plus vite en diagonale
        if (move.magnitude > 1f) move.Normalize();

        controller.Move(move * walkSpeed * Time.deltaTime);

        // 4. Application de la gravité
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleHeadBob()
    {
        if (playerCamera == null) return;

        // On vérifie s'il y a une action sur les touches de déplacement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Si le joueur bouge et touche le sol
        if ((Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f) && isGrounded)
        {
            timer += Time.deltaTime * bobSpeed;
            
            // Le Mathf.Sin crée le mouvement de haut en bas réaliste
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultCameraY + Mathf.Sin(timer) * bobAmount,
                playerCamera.transform.localPosition.z
            );
        }
        else
        {
            // Retour doux à la position initiale quand on s'arrête (pour ne pas casser l'immersion avec un arrêt brutal)
            timer = 0;
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                Mathf.Lerp(playerCamera.transform.localPosition.y, defaultCameraY, Time.deltaTime * (bobSpeed / 2)),
                playerCamera.transform.localPosition.z
            );
        }
    }
}