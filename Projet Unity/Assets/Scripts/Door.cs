using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip lockedSound;
    [Range(0f, 1f)]
    public float volume = 1f;
    public bool autoSetupAudio = true;

    private Quaternion closedRot;
    private Quaternion targetRot;
    private float currentOpenDirection = 1f;
    private float nextOpenDirection = 1f;
    private bool hasOpenedOnce = false;

    // --- NOUVELLE FONCTION POUR LA PHOTO ---
    public void UnlockDoor()
    {
        isLocked = false; 
        Debug.Log("SANTÉ : La porte " + gameObject.name + " a été déverrouillée par la photo !");
    }
    // ---------------------------------------

    void Start()
    {
        EnsureAudioSetup();
        if (rotationAxis.sqrMagnitude < 0.0001f) rotationAxis = Vector3.forward;
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
            PlaySound(lockedSound);
            return; 
        }

        if (isOpen)
        {
            isOpen = false;
            targetRot = closedRot;
            nextOpenDirection = -currentOpenDirection;
            PlaySound(closeSound);
            return;
        }

        isOpen = true; 
        float direction = nextOpenDirection;
        Transform lookSource = ResolveLookSource(interactor);

        if (lookSource != null)
        {
            Vector3 axisLocal = rotationAxis.normalized;
            Vector3 referenceOnPlane = Vector3.ProjectOnPlane(lookReferenceAxis, axisLocal);
            if (referenceOnPlane.sqrMagnitude < 0.0001f) referenceOnPlane = Vector3.ProjectOnPlane(Vector3.right, axisLocal);

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
                        if (!hasOpenedOnce) direction = openAwayFromViewer ? -sideSign : sideSign;
                    }
                }
            }
        }

        Quaternion openRot = Quaternion.AngleAxis(openAngle * direction, rotationAxis.normalized) * closedRot;
        targetRot = openRot;
        currentOpenDirection = direction;
        hasOpenedOnce = true;
        PlaySound(openSound);
    }

    private void EnsureAudioSetup()
    {
        if (!autoSetupAudio) return;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1.5f;
        audioSource.maxDistance = 12f;

#if UNITY_EDITOR
        AutoAssignClipIfMissing(ref openSound, "door_open", "door-open", "open_door", "open", "creak");
        AutoAssignClipIfMissing(ref closeSound, "door_close", "door-close", "close_door", "close", "slam");
        AutoAssignClipIfMissing(ref lockedSound, "door_locked", "door-lock", "locked", "lock", "metal");
        AutoAssignAnyClipIfMissing(ref openSound);
        AutoAssignAnyClipIfMissing(ref closeSound);
        AutoAssignAnyClipIfMissing(ref lockedSound);
#endif

        if (openSound == null) openSound = closeSound;
        if (closeSound == null) closeSound = openSound;
        if (lockedSound == null) lockedSound = closeSound != null ? closeSound : openSound;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

#if UNITY_EDITOR
    private void OnValidate() { EnsureAudioSetup(); }

    private void AutoAssignClipIfMissing(ref AudioClip target, params string[] keywords)
    {
        if (target != null) return;
        string[] guids = AssetDatabase.FindAssets("t:AudioClip");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
            for (int i = 0; i < keywords.Length; i++)
            {
                if (!fileName.Contains(keywords[i])) continue;
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip != null) { target = clip; EditorUtility.SetDirty(this); return; }
            }
        }
    }

    private void AutoAssignAnyClipIfMissing(ref AudioClip target)
    {
        if (target != null) return;
        string[] guids = AssetDatabase.FindAssets("t:AudioClip");
        if (guids.Length == 0) return;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        if (clip != null) { target = clip; EditorUtility.SetDirty(this); }
    }
#endif

    private Transform ResolveLookSource(Transform interactor)
    {
        if (interactor != null) return interactor;
        if (Camera.main != null) return Camera.main.transform;
        GameObject namedMainCamera = GameObject.Find(fallbackCameraObjectName);
        if (namedMainCamera != null) return namedMainCamera.transform;
        return null;
    }
}