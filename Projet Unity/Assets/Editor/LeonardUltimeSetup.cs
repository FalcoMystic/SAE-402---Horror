using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public static class LeonardUltimeSetup
{
    private const string LeonardPrefabPath = "Assets/3rd-party/LE_JOUEUR.prefab";
    private const string LeonardObjectName = "Leonard";
    private const string LeonardRenderTextureGuid = "79e1653da2701714e833f83458275652";

    [MenuItem("Tools/Horror/Setup Leonard Ultime")]
    public static void SetupLeonardUltime()
    {
        GameObject leonard = FindLeonardInScene();
        if (leonard == null)
        {
            leonard = InstantiateLeonardPrefab();
            if (leonard == null)
            {
                return;
            }
        }

        Undo.RegisterFullObjectHierarchyUndo(leonard, "Setup Leonard Ultime");

        leonard.name = LeonardObjectName;
        leonard.tag = "Player";

        EnsureComponent<CharacterController>(leonard);
        EnsureComponent<PlayerMovement>(leonard);

        CamcorderController camcorder = EnsureComponent<CamcorderController>(leonard);
        ModePOVCamera povCamera = EnsureComponent<ModePOVCamera>(leonard);
        PlayerInteraction interaction = EnsureComponent<PlayerInteraction>(leonard);

        Camera mainCamera = FindCamera("main camera", "main_camera", "cammain");
        Camera lentilleCamera = FindCamera("cam_lentille", "cam lentille", "camlentille");
        GameObject uiCamera = FindObject("canvas_camcorder", "canvas camcorder", "uicamera");
        GameObject flashUi = FindObject("flashui", "flash ui", "flash");
        GameObject texteInteraction = FindObject("texteinteraction", "texte interaction", "textinteraction", "appuyer", "interaction");

        if (mainCamera != null)
        {
            interaction.playerCam = mainCamera;
            povCamera.yeuxJoueur = mainCamera;
        }

        if (lentilleCamera != null)
        {
            camcorder.camLentille = lentilleCamera;
            povCamera.camLentille = lentilleCamera;
        }

        if (uiCamera != null)
        {
            povCamera.uiCamera = uiCamera;
        }

        if (flashUi != null)
        {
            povCamera.flashUI = flashUi;
        }

        if (texteInteraction != null)
        {
            interaction.texteInteraction = texteInteraction;
        }

        RenderTexture textureEcran = LoadLeonardRenderTexture();
        if (textureEcran != null)
        {
            povCamera.textureEcran = textureEcran;
        }

        AssignMouseLookPlayerBody(leonard.transform);
        RefreshRigReferences(leonard, camcorder);

        EditorUtility.SetDirty(leonard);
        EditorUtility.SetDirty(camcorder);
        EditorUtility.SetDirty(povCamera);
        EditorUtility.SetDirty(interaction);

        Selection.activeGameObject = leonard;
        Debug.Log("Leonard Ultime configure sur la branche leonard.");
    }

    private static GameObject InstantiateLeonardPrefab()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(LeonardPrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Prefab Leonard introuvable: {LeonardPrefabPath}");
            return null;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (instance == null)
        {
            Debug.LogError("Impossible d'instancier le prefab Leonard.");
            return null;
        }

        Undo.RegisterCreatedObjectUndo(instance, "Instantiate Leonard Ultime");
        return instance;
    }

    private static GameObject FindLeonardInScene()
    {
        GameObject[] objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject go in objects)
        {
            if (!go.scene.IsValid())
            {
                continue;
            }

            if (go.name.ToLowerInvariant().Contains("leonard"))
            {
                return go;
            }
        }

        return null;
    }

    private static Camera FindCamera(params string[] tokens)
    {
        Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera camera in cameras)
        {
            if (camera == null || !camera.gameObject.scene.IsValid())
            {
                continue;
            }

            string lowerName = camera.name.ToLowerInvariant();
            foreach (string token in tokens)
            {
                if (!string.IsNullOrWhiteSpace(token) && lowerName.Contains(token.ToLowerInvariant()))
                {
                    return camera;
                }
            }
        }

        return Camera.main;
    }

    private static GameObject FindObject(params string[] tokens)
    {
        GameObject[] objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject go in objects)
        {
            if (!go.scene.IsValid())
            {
                continue;
            }

            string lowerName = go.name.ToLowerInvariant();
            foreach (string token in tokens)
            {
                if (!string.IsNullOrWhiteSpace(token) && lowerName.Contains(token.ToLowerInvariant()))
                {
                    return go;
                }
            }
        }

        return null;
    }

    private static T EnsureComponent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        if (component == null)
        {
            component = Undo.AddComponent<T>(target);
        }

        return component;
    }

    private static void AssignMouseLookPlayerBody(Transform leonardTransform)
    {
        SimpleMouseLook[] looks = Object.FindObjectsByType<SimpleMouseLook>(FindObjectsSortMode.None);
        foreach (SimpleMouseLook look in looks)
        {
            if (look == null || !look.gameObject.scene.IsValid())
            {
                continue;
            }

            if (look.playerBody == null)
            {
                look.playerBody = leonardTransform;
                EditorUtility.SetDirty(look);
            }
        }
    }

    private static void RefreshRigReferences(GameObject leonard, CamcorderController camcorder)
    {
        Rig[] rigs = leonard.GetComponentsInChildren<Rig>(true);
        if (rigs == null || rigs.Length == 0 || camcorder == null)
        {
            return;
        }

        foreach (Rig rig in rigs)
        {
            if (rig == null)
            {
                continue;
            }

            string lowerName = rig.name.ToLowerInvariant();
            if (camcorder.rigCamAim == null && lowerName.Contains("aim"))
            {
                camcorder.rigCamAim = rig;
            }
            else if (camcorder.rigCamIdle == null && lowerName.Contains("idle"))
            {
                camcorder.rigCamIdle = rig;
            }
        }

        EditorUtility.SetDirty(camcorder);
    }

    private static RenderTexture LoadLeonardRenderTexture()
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(LeonardRenderTextureGuid);
        if (string.IsNullOrWhiteSpace(assetPath))
        {
            return null;
        }

        return AssetDatabase.LoadAssetAtPath<RenderTexture>(assetPath);
    }
}