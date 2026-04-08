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
        if (rigFlashAim != null) rigFlashAim.weight = 0f;
        if (rigFlashIdle != null) rigFlashIdle.weight = 1f;

        if (flashLight != null) flashLight.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(aimKey))
        {
            isAiming = !isAiming;
            if (flashLight != null) flashLight.enabled = isAiming;
        }

        float targetAimWeight = isAiming ? 1f : 0f;
        float targetIdleWeight = isAiming ? 0f : 1f;

        if (rigFlashAim != null && rigFlashIdle != null)
        {
            rigFlashAim.weight = Mathf.Lerp(rigFlashAim.weight, targetAimWeight, Time.deltaTime * transitionSpeed);
            rigFlashIdle.weight = Mathf.Lerp(rigFlashIdle.weight, targetIdleWeight, Time.deltaTime * transitionSpeed);
        }
    }
}