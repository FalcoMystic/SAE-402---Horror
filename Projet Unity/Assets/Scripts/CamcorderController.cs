using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CamcorderController : MonoBehaviour
{
    [Header("Les Rigs")]
    public Rig rigCamAim;
    public Rig rigCamIdle;
    public Camera camLentille;

    [Header("Paramètres")]
    public KeyCode aimKey = KeyCode.C;
    public float transitionSpeed = 8f;

    private bool isAiming = false;

    void Start()
    {
        if (rigCamAim != null) rigCamAim.weight = 0f;
        if (rigCamIdle != null) rigCamIdle.weight = 1f;

        if (camLentille != null) camLentille.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(aimKey))
        {
            isAiming = !isAiming;
            if (camLentille != null) camLentille.enabled = isAiming;
        }

        float targetAimWeight = isAiming ? 1f : 0f;
        float targetIdleWeight = isAiming ? 0f : 1f;

        if (rigCamAim != null && rigCamIdle != null)
        {
            rigCamAim.weight = Mathf.Lerp(rigCamAim.weight, targetAimWeight, Time.deltaTime * transitionSpeed);
            rigCamIdle.weight = Mathf.Lerp(rigCamIdle.weight, targetIdleWeight, Time.deltaTime * transitionSpeed);
        }
    }
}