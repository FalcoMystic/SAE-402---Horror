using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FlashController : MonoBehaviour
{
    [Header("Les Rigs")]
    public Rig rigFlashAim;
    public Rig rigFlashIdle;
    public Light flashLight;

    [Header("Paramètres")]
    public KeyCode aimKey = KeyCode.F;
    public float transitionSpeed = 8f;

    private bool isAiming = false;

    void Start()
    {
        // On force l'état initial
        if (rigFlashAim != null) rigFlashAim.weight = 0f;
        if (rigFlashIdle != null) rigFlashIdle.weight = 1f;
        if (flashLight != null) flashLight.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(aimKey))
        {
            isAiming = !isAiming;

            // On active/désactive la lumière
            if (flashLight != null)
            {
                flashLight.enabled = isAiming;
                Debug.Log("État de la lampe : " + isAiming);
            }
            else
            {
                Debug.LogError("Attention : Aucune Light assignée dans l'inspecteur !");
            }
        }

        // Calcul des poids cibles
        float targetAimWeight = isAiming ? 1f : 0f;
        float targetIdleWeight = isAiming ? 0f : 1f;

        // Transition fluide des poids des Rigs
        if (rigFlashAim != null)
            rigFlashAim.weight = Mathf.Lerp(rigFlashAim.weight, targetAimWeight, Time.deltaTime * transitionSpeed);

        if (rigFlashIdle != null)
            rigFlashIdle.weight = Mathf.Lerp(rigFlashIdle.weight, targetIdleWeight, Time.deltaTime * transitionSpeed);
    }
}