using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    public static GameObject prefabToSpawn;
    public static int spawnCount;
    public static string SpawnTag;
    public static string nextSceneName;
    private static bool sceneLoaded = false;

    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == nextSceneName)
        {
            sceneLoaded = true;
            SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.SetSpawnData(prefabToSpawn, spawnCount, SpawnTag);
            }
           
            //вызываем один раз при загрузке сцены, что бы запустился первый ход.
           CombatManager combatManager = FindObjectOfType<CombatManager>();
             if (combatManager != null)
           {
             combatManager.InitializeCombat();
           }

           SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}