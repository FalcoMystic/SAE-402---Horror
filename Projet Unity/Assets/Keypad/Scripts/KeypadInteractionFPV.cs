using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavKeypad { 
public class KeypadInteractionFPV : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Camera interactionCamera;
    [SerializeField] private float interactionDistance = 4f;
    [SerializeField] private LayerMask interactionMask = ~0;
    [SerializeField] private KeyCode interactKey = KeyCode.Mouse0;

    private Camera cam;
    private bool warnedMissingCamera;

    private void Awake()
    {
        ResolveCamera();
    }

    private void OnEnable()
    {
        ResolveCamera();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(interactKey))
        {
            return;
        }

        if (cam == null)
        {
            ResolveCamera();
        }

        if (cam == null)
        {
            if (!warnedMissingCamera)
            {
                Debug.LogWarning("KeypadInteractionFPV: no camera found. Assign Interaction Camera in the inspector.", this);
                warnedMissingCamera = true;
            }

            return;
        }

        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, interactionDistance, interactionMask, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.TryGetComponent(out KeypadButton keypadButton))
            {
                keypadButton.PressButton();
            }
        }
    }

    private void ResolveCamera()
    {
        cam = interactionCamera;

        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }

        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            cam = FindObjectOfType<Camera>();
        }
    }
}
}