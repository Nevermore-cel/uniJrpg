using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class SceneLoader : MonoBehaviour
{

    public void LoadNextSceneAndTransferData(string objectID, EnemyData enemyData)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        SceneData.PlayerPositions[currentSceneName] = playerTransform.position;
        SceneData.PlayerRotations[currentSceneName] = playerTransform.rotation;
        SceneData.previousScene = currentSceneName;

        
        StartScene.prefabToSpawn = enemyData.EnemyPrefab;
        StartScene.spawnCount = enemyData.EnemyCount;
        StartScene.SpawnTag = enemyData.SpawnTag;
        StartScene.nextSceneName = enemyData.nextSceneName;
        if (objectID != null)
        {
            SceneData.MarkObjectAsDestroyed(currentSceneName, objectID);
        }
        // Сохраняем состояние подобранных предметов (ключей)
        KeyInventory keyInventory = playerTransform.GetComponent<KeyInventory>();
        if (keyInventory != null)
        {
             SavePickedUpKeysState(currentSceneName,keyInventory.keys);
        }
        // Сохраняем состояние уничтоженных объектов
         SaveDestroyedObjectsState(currentSceneName);
        // Сохраняем состояние дверей
         // SaveDoorStates(currentSceneName);  <--  ручной вызов OnDestroy()

        // Сначала подписываемся на событие, затем начинаем загрузку
        SceneManager.sceneLoaded += StartScene.OnSceneLoaded;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(enemyData.nextSceneName);

        // Убрать блокировку активации сцены, если она уже не нужна
        asyncLoad.allowSceneActivation = true;
    }
       private void SavePickedUpKeysState(string sceneName, List<KeyData> collectedKeys)
    {
        if (!SceneData.KeyInventories.ContainsKey(sceneName))
        {
            SceneData.KeyInventories[sceneName] = new List<string>();
        }
           SceneData.KeyInventories[sceneName] = collectedKeys.Select(key => key.keyName).ToList();
    }
    private void SaveDestroyedObjectsState(string sceneName)
    {
        // Здесь можно добавить логику для сохранения списка уничтоженных объектов
        // Например, можно сохранить их идентификаторы в SceneData.DestroyedObjects
    }

}