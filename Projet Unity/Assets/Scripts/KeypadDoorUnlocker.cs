using UnityEngine;

public class KeypadDoorUnlocker : MonoBehaviour
{
    [Header("Preferred: assign doors directly")]
    [SerializeField] private Door[] doorsToUnlock;

    [Header("Door object names in scene")]
    [SerializeField] private string firstDoorName = "Door.021";
    [SerializeField] private string secondDoorName = "Door.022";

    [Header("Optional direct references (priority over names)")]
    [SerializeField] private Door firstDoor;
    [SerializeField] private Door secondDoor;

    [Header("Behavior")]
    [SerializeField] private bool openAfterUnlock = false;

    public void UnlockConfiguredDoors()
    {
        int unlockedCount = 0;

        if (doorsToUnlock != null && doorsToUnlock.Length > 0)
        {
            for (int i = 0; i < doorsToUnlock.Length; i++)
            {
                Door door = doorsToUnlock[i];
                if (door == null)
                {
                    continue;
                }

                UnlockDoorInstance(door);
                unlockedCount++;
            }
        }

        if (unlockedCount == 0)
        {
            unlockedCount += UnlockDoorByReferenceOrName(ref firstDoor, firstDoorName);
            unlockedCount += UnlockDoorByReferenceOrName(ref secondDoor, secondDoorName);
        }

        if (unlockedCount == 0)
        {
            Debug.LogWarning("KeypadDoorUnlocker: no Door unlocked. Assign doorsToUnlock in inspector or verify door names.", this);
        }
    }

    private int UnlockDoorByReferenceOrName(ref Door doorRef, string fallbackName)
    {
        if (doorRef == null)
        {
            GameObject found = GameObject.Find(fallbackName);
            if (found != null)
            {
                doorRef = found.GetComponent<Door>();
            }
        }

        if (doorRef == null)
        {
            Debug.LogWarning($"KeypadDoorUnlocker: Door not found: {fallbackName}", this);
            return 0;
        }

        UnlockDoorInstance(doorRef);
        return 1;
    }

    private void UnlockDoorInstance(Door door)
    {
        if (door == null)
        {
            return;
        }

        door.isLocked = false;

        if (openAfterUnlock && !door.isOpen)
        {
            door.ToggleDoor();
        }
    }
}
