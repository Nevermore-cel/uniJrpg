using UnityEngine;
using System.Collections.Generic;

public static class SceneData
{
    public static Dictionary<string, Vector3> PlayerPositions { get; set; } = new Dictionary<string, Vector3>();
    public static Dictionary<string, Quaternion> PlayerRotations { get; set; } = new Dictionary<string, Quaternion>();
    public static string previousScene { get; set; } = "";
    // Добавлено: словарь для отслеживания уничтоженных объектов
    public static Dictionary<string, HashSet<string>> DestroyedObjects { get; set; } = new Dictionary<string, HashSet<string>>();


    // Метод для добавления идентификатора уничтоженного объекта
    public static void MarkObjectAsDestroyed(string sceneName, string objectID)
    {
         if (!DestroyedObjects.ContainsKey(sceneName))
        {
            DestroyedObjects[sceneName] = new HashSet<string>();
        }
        DestroyedObjects[sceneName].Add(objectID);
    }

        // Метод для проверки, был ли объект уничтожен
     public static bool IsObjectDestroyed(string sceneName, string objectID)
    {
        return DestroyedObjects.ContainsKey(sceneName) && DestroyedObjects[sceneName].Contains(objectID);
    }
}