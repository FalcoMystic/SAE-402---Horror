using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;
    public bool isLocked = false;
    
    [Header("Réglage de la rotation")]
    [Tooltip("Angle d'ouverture en degrés.")]
    public float openAngle = 90f;

    [Tooltip("Axe local de rotation de la porte. Par défaut: Z.")]
    public Vector3 rotationAxis = Vector3.forward;

    [Tooltip("Nom de l'objet caméra recherché automatiquement en scène si nécessaire.")]
    public string fallbackCameraObjectName = "main_camera";

    [Tooltip("Axe local servant à décider le côté gauche/droite de la porte.")]
    public Vector3 lookReferenceAxis = Vector3.right;

    [Tooltip("Si activé, la porte s'ouvre à l'opposé du joueur/caméra (loin de toi).")]
    public bool openAwayFromViewer = true;
    
    public float speed = 3f;

    private Quaternion closedRot;
    private Quaternion targetRot;
    private float currentOpenDirection = 1f;
    private float nextOpenDirection = 1f;
    private bool hasOpenedOnce = false;

    void Start()
    {
        if (rotationAxis.sqrMagnitude < 0.0001f)
        {
            rotationAxis = Vector3.forward;
        }

        closedRot = transform.localRotation;
        targetRot = closedRot;
    }

    void Update()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * speed);
    }

    public void ToggleDoor()
    {
        ToggleDoor(null);
    }

    public void ToggleDoor(Transform interactor)
    {
        if (isLocked)
        {
            return;
        }

        if (isOpen)
        {
            isOpen = false;
            targetRot = closedRot;
            nextOpenDirection = -currentOpenDirection;
            return;
        }

        float direction = nextOpenDirection;
        Transform lookSource = ResolveLookSource(interactor);

        if (lookSource != null)
        {
            Vector3 axisLocal = rotationAxis.normalized;
            Vector3 referenceOnPlane = Vector3.ProjectOnPlane(lookReferenceAxis, axisLocal);

            if (referenceOnPlane.sqrMagnitude < 0.0001f)
            {
                referenceOnPlane = Vector3.ProjectOnPlane(Vector3.right, axisLocal);
            }

            if (referenceOnPlane.sqrMagnitude > 0.0001f)
            {
                Vector3 sourceLocalPos = transform.InverseTransformPoint(lookSource.position);
                Vector3 sourcePosOnPlane = Vector3.ProjectOnPlane(sourceLocalPos, axisLocal);

                if (sourcePosOnPlane.sqrMagnitude > 0.0001f)
                {
                    float side = Vector3.Dot(sourcePosOnPlane.normalized, referenceOnPlane.normalized);

                    if (!Mathf.Approximately(side, 0f))
                    {
                        float sideSign = side > 0f ? 1f : -1f;
                        // On first open, use viewer side; afterward, keep alternating after each close.
                        if (!hasOpenedOnce)
                        {
                            direction = openAwayFromViewer ? -sideSign : sideSign;
                        }
                    }
                }
            }
        }

        Quaternion openRot = Quaternion.AngleAxis(openAngle * direction, rotationAxis.normalized) * closedRot;
        targetRot = openRot;
        currentOpenDirection = direction;
        hasOpenedOnce = true;
        isOpen = !isOpen;
    }

    private Transform ResolveLookSource(Transform interactor)
    {
        if (interactor != null)
        {
            return interactor;
        }

        if (Camera.main != null)
        {
            return Camera.main.transform;
        }

        GameObject namedMainCamera = GameObject.Find(fallbackCameraObjectName);
        if (namedMainCamera != null)
        {
            return namedMainCamera.transform;
        }

        return null;
    }
}