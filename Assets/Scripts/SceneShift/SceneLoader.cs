using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public void LoadNextSceneAndTransferData(string objectID, EnemyData enemyData)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        SceneData.PlayerPositions[currentSceneName] = playerTransform.position;
        SceneData.PlayerRotations[currentSceneName] = playerTransform.rotation;
        SceneData.previousScene = currentSceneName;

        // Убедитесь, что статические поля очищены перед установкой новых значений
        StartScene.prefabToSpawn = enemyData.EnemyPrefab;
        StartScene.spawnCount = enemyData.EnemyCount;
        StartScene.SpawnTag = enemyData.SpawnTag;
        StartScene.nextSceneName = enemyData.nextSceneName;
        if (objectID != null)
        {
            SceneData.MarkObjectAsDestroyed(currentSceneName, objectID);
        }
        // Сначала подписываемся на событие, затем начинаем загрузку
        SceneManager.sceneLoaded += StartScene.OnSceneLoaded;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(enemyData.nextSceneName);

        // Убрать блокировку активации сцены, если она не нужна
        asyncLoad.allowSceneActivation = true;
    }
}