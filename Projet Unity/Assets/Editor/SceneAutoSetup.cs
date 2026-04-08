using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SceneAutoSetup
{
    [MenuItem("Tools/Horror/Setup Leonard + All Door Groups")]
    public static void SetupScene()
    {
        Camera mainCam = FindMainCamera();
        GameObject leonard = FindByNameContains("leonard");

        if (leonard == null)
        {
            Debug.LogError("Setup interrompu: impossible de trouver un objet contenant 'leonard' dans la scene.");
            return;
        }

        ConfigureLeonard(leonard, mainCam);

        List<GameObject> allGroups = FindAllDoorGroupsInScene();
        List<GameObject> processedDoors = ProcessDoorGroups(allGroups);

        Selection.objects = processedDoors.ToArray();
        Debug.Log($"Setup termine. Leonard configure, groupes traites: {allGroups.Count}, portes configurees: {processedDoors.Count}.");
    }

    [MenuItem("Tools/Horror/Setup Selected Door Groups")]
    public static void SetupSelectedGroups()
    {
        List<GameObject> selectedGroups = new List<GameObject>();

        foreach (Object selected in Selection.objects)
        {
            if (selected is GameObject go && IsDoorGroupName(go.name))
            {
                selectedGroups.Add(go);
            }
        }

        if (selectedGroups.Count == 0)
        {
            Debug.LogWarning("Aucun Door_Group selectionne. Selectionne les groupes puis relance Tools/Horror/Setup Selected Door Groups.");
            return;
        }

        List<GameObject> processedDoors = ProcessDoorGroups(selectedGroups);
        Selection.objects = processedDoors.ToArray();

        Debug.Log($"Groupes selectionnes traites: {selectedGroups.Count}, portes configurees: {processedDoors.Count}.");
    }

    private static void ConfigureLeonard(GameObject leonard, Camera mainCam)
    {
        Undo.RegisterFullObjectHierarchyUndo(leonard, "Configure Leonard Interaction");

        PlayerInteraction interaction = leonard.GetComponent<PlayerInteraction>();
        if (interaction == null)
        {
            interaction = Undo.AddComponent<PlayerInteraction>(leonard);
        }

        if (mainCam != null)
        {
            interaction.playerCam = mainCam;
            EditorUtility.SetDirty(interaction);
        }
        else
        {
            Debug.LogWarning("Main Camera introuvable (tag MainCamera et/ou nom main camera). Player Cam n'a pas ete assignee automatiquement.");
        }
    }

    private static List<GameObject> ProcessDoorGroups(List<GameObject> groups)
    {
        List<GameObject> processedDoors = new List<GameObject>();

        foreach (GameObject group in groups)
        {
            if (group == null)
            {
                continue;
            }

            List<Transform> doorTransforms = FindDoorChildren(group.transform);
            if (doorTransforms.Count == 0)
            {
                Debug.LogWarning($"Aucune porte detectee dans le groupe {group.name}.");
                continue;
            }

            foreach (Transform doorTransform in doorTransforms)
            {
                GameObject doorObject = doorTransform.gameObject;
                Undo.RegisterFullObjectHierarchyUndo(doorObject, "Configure Door Object");

                BoxCollider boxCollider = doorObject.GetComponent<BoxCollider>();
                if (boxCollider == null)
                {
                    boxCollider = Undo.AddComponent<BoxCollider>(doorObject);
                }

                if (boxCollider != null)
                {
                    boxCollider.isTrigger = false;
                    EditorUtility.SetDirty(boxCollider);
                }

                Door door = doorObject.GetComponent<Door>();
                if (door == null)
                {
                    door = Undo.AddComponent<Door>(doorObject);
                }

                if (door != null)
                {
                    door.rotationAxis = Vector3.forward;
                    EditorUtility.SetDirty(door);
                }

                if (!processedDoors.Contains(doorObject))
                {
                    processedDoors.Add(doorObject);
                }
            }
        }

        return processedDoors;
    }

    private static List<GameObject> FindAllDoorGroupsInScene()
    {
        List<GameObject> groups = new List<GameObject>();
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject go in allObjects)
        {
            if (!go.scene.IsValid())
            {
                continue;
            }

            if (IsDoorGroupName(go.name))
            {
                groups.Add(go);
            }
        }

        return groups;
    }

    private static bool IsDoorGroupName(string objectName)
    {
        string lower = objectName.ToLowerInvariant();
        return lower.Contains("door_group") || lower.Contains("group_door");
    }

    private static Camera FindMainCamera()
    {
        if (Camera.main != null)
        {
            return Camera.main;
        }

        GameObject byName = FindByNameContains("main camera");
        if (byName == null)
        {
            byName = FindByNameContains("main_camera");
        }

        return byName != null ? byName.GetComponent<Camera>() : null;
    }

    private static GameObject FindByNameContains(string token)
    {
        string lowerToken = token.ToLowerInvariant();
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject go in allObjects)
        {
            if (!go.scene.IsValid())
            {
                continue;
            }

            if (go.name.ToLowerInvariant().Contains(lowerToken))
            {
                return go;
            }
        }

        return null;
    }

    private static List<Transform> FindDoorChildren(Transform parent)
    {
        List<Transform> results = new List<Transform>();
        CollectDoorChildren(parent, results);
        return results;
    }

    private static void CollectDoorChildren(Transform parent, List<Transform> results)
    {
        if (IsDoorObjectName(parent.name))
        {
            results.Add(parent);
        }

        foreach (Transform child in parent)
        {
            CollectDoorChildren(child, results);
        }
    }

    private static bool IsDoorObjectName(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return false;
        }

        // Accept only: Door or Door.<digits> (e.g. Door.001)
        if (objectName == "Door")
        {
            return true;
        }

        if (!objectName.StartsWith("Door."))
        {
            return false;
        }

        string suffix = objectName.Substring("Door.".Length);
        if (suffix.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < suffix.Length; i++)
        {
            if (!char.IsDigit(suffix[i]))
            {
                return false;
            }
        }

        return true;
    }
}
