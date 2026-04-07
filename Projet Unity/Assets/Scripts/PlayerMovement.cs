using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3.0f; 
    public float gravity = -15.0f; 

    private CharacterController controller;
    private Animator anim;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // On va chercher l'Animator sur le même objet (Player_Leonard)
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 1. Récupérer les touches (Z/S = Vertical, Q/D = Horizontal)
        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");   

        // 2. Envoyer ces valeurs à l'Animator (Le Blend Tree)
        if (anim != null)
        {
            anim.SetFloat("Horizontal", x);
            anim.SetFloat("Vertical", z);
        }

        // 3. Déplacer physiquement la capsule
        Vector3 move = transform.right * x + transform.forward * z;
        if (move.magnitude > 1f) move.Normalize(); // Évite de courir plus vite en diagonale

        controller.Move(move * walkSpeed * Time.deltaTime);

        // 4. Gérer la gravité (pour ne pas slove)
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}