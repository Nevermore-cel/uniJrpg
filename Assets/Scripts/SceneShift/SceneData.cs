using UnityEngine;
using System.Collections.Generic;

public static class SceneData
{
    public static Dictionary<string, Vector3> PlayerPositions { get; set; } = new Dictionary<string, Vector3>();
    public static Dictionary<string, Quaternion> PlayerRotations { get; set; } = new Dictionary<string, Quaternion>();
    public static string previousScene { get; set; } = "";

    public static Dictionary<string, HashSet<string>> DestroyedObjects { get; set; } = new Dictionary<string, HashSet<string>>();

    // Добавлено: состояние дверей
    public static Dictionary<string, Dictionary<string, bool>> DoorStates { get; set; } = new Dictionary<string, Dictionary<string, bool>>();

    // Добавлено: инвентарь ключей
    public static Dictionary<string, List<string>> KeyInventories { get; set; } = new Dictionary<string, List<string>>();

    public static void MarkObjectAsDestroyed(string sceneName, string objectID)
    {
        if (!DestroyedObjects.ContainsKey(sceneName))
        {
            DestroyedObjects[sceneName] = new HashSet<string>();
        }
        DestroyedObjects[sceneName].Add(objectID);
    }

    public static bool IsObjectDestroyed(string sceneName, string objectID)
    {
        return DestroyedObjects.ContainsKey(sceneName) && DestroyedObjects[sceneName].Contains(objectID);
    }
}