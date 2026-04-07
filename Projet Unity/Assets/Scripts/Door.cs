using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;
    
    [Header("Réglage de la rotation")]
    [Tooltip("Change ces valeurs (X, Y ou Z) si la porte tourne du mauvais côté.")]
    public Vector3 openRotation = new Vector3(0, 90, 0); 
    
    public float speed = 3f;

    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = transform.localRotation;
        openRot = Quaternion.Euler(openRotation) * closedRot;
    }

    void Update()
    {
        if (isOpen)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, openRot, Time.deltaTime * speed);
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, closedRot, Time.deltaTime * speed);
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}