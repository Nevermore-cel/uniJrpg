using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints; 

    // Метод для получения данных из предыдущей сцены 
    public void SetSpawnData(GameObject prefab, int count, string tag)
    {
        Debug.Log("Получены данные для спавна:");
        Debug.Log("Prefab: " + prefab.name);
        Debug.Log("Count: " + count);
        Debug.Log("SpawnTag: " + tag);

        Debug.Log("Найдены точки спавна: " + GameObject.FindGameObjectsWithTag(tag).Length);
        spawnPoints = GameObject.FindGameObjectsWithTag(tag).Select(go => go.transform).ToArray(); 

        if (spawnPoints.Length > 0)
        {
            // Сохраняем данные для спавна
            StartScene.prefabToSpawn = prefab;
            StartScene.spawnCount = count;
            StartScene.SpawnTag = tag;
        }
    }

    // Функция для спавна врагов
    public void SpawnEnemies()
    {
        if (spawnPoints.Length > 0)
        {
            for (int i = 0; i < StartScene.spawnCount; i++)
            {
                Instantiate(StartScene.prefabToSpawn, spawnPoints[i % spawnPoints.Length].position, spawnPoints[i % spawnPoints.Length].rotation);
            }
        }
    }
}