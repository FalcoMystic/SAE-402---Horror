using UnityEngine;

// Force l'ajout d'un AudioSource sur l'objet
[RequireComponent(typeof(AudioSource))]
public class Door : MonoBehaviour
{
    public bool isOpen = false;
    public bool isLocked = false;
    
    [Header("Réglage de la rotation")]
    public float openAngle = 90f;
    public Vector3 rotationAxis = Vector3.forward;
    public string fallbackCameraObjectName = "main_camera";
    public Vector3 lookReferenceAxis = Vector3.right;
    public bool openAwayFromViewer = true;
    public float speed = 3f;

    [Header("Sons")]
    public AudioSource audioSource;
    [Tooltip("Son quand la porte se ferme.")]
    public AudioClip closeSound;
    [Tooltip("Son quand on essaie d'ouvrir une porte verrouillée.")]
    public AudioClip lockedSound;

    private Quaternion closedRot;
    private Quaternion targetRot;
    private float currentOpenDirection = 1f;
    private float nextOpenDirection = 1f;
    private bool hasOpenedOnce = false;

    void Start()
    {
        if (audioSource == null) 
            audioSource = GetComponent<AudioSource>();

        if (rotationAxis.sqrMagnitude < 0.0001f)
            rotationAxis = Vector3.forward;

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
        // --- 1. CAS : PORTE VERROUILLÉE ---
        if (isLocked)
        {
            if (audioSource != null && lockedSound != null)
            {
                audioSource.PlayOneShot(lockedSound);
            }
            // On s'arrête ici : la porte ne s'ouvre pas
            return; 
        }

        // --- 2. CAS : FERMETURE ---
        if (isOpen)
        {
            isOpen = false;
            targetRot = closedRot;
            nextOpenDirection = -currentOpenDirection;

            if (audioSource != null && closeSound != null)
            {
                audioSource.PlayOneShot(closeSound);
            }
            return;
        }

        // --- 3. CAS : OUVERTURE ---
        isOpen = true; 
        float direction = nextOpenDirection;
        Transform lookSource = ResolveLookSource(interactor);

        if (lookSource != null)
        {
            Vector3 axisLocal = rotationAxis.normalized;
            Vector3 referenceOnPlane = Vector3.ProjectOnPlane(lookReferenceAxis, axisLocal);

            if (referenceOnPlane.sqrMagnitude < 0.0001f)
                referenceOnPlane = Vector3.ProjectOnPlane(Vector3.right, axisLocal);

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
                        if (!hasOpenedOnce)
                            direction = openAwayFromViewer ? -sideSign : sideSign;
                    }
                }
            }
        }

        Quaternion openRot = Quaternion.AngleAxis(openAngle * direction, rotationAxis.normalized) * closedRot;
        targetRot = openRot;
        currentOpenDirection = direction;
        hasOpenedOnce = true;
    }

    private Transform ResolveLookSource(Transform interactor)
    {
        if (interactor != null) return interactor;
        if (Camera.main != null) return Camera.main.transform;
        GameObject namedMainCamera = GameObject.Find(fallbackCameraObjectName);
        if (namedMainCamera != null) return namedMainCamera.transform;
        return null;
    }
}