using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuration")]
    public int nombreObjetsTotal = 2;
    private int objetsRamasses = 0;

    [Header("Portes à débloquer")]
    public GameObject[] portesADebloquer;

    void Awake()
    {
        Instance = this;
    }

    public void ObjetRamasse()
    {
        objetsRamasses++;
        Debug.Log("Objets ramassés : " + objetsRamasses + "/" + nombreObjetsTotal);

        if (objetsRamasses >= nombreObjetsTotal)
            DebloquePortes();
    }

    private void DebloquePortes()
    {
        foreach (GameObject porte in portesADebloquer)
        {
            if (porte != null)
            {
                Door door = porte.GetComponentInChildren<Door>(); // ← cherche dans les enfants
                if (door != null)
                    door.UnlockDoor();
            }
        }
    }
}