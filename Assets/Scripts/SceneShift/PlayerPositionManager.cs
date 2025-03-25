using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionManager : MonoBehaviour
{
    private bool _isInitialized = false;
  void Awake()
    {
      if (_isInitialized) return;
        string currentSceneName = SceneManager.GetActiveScene().name;
         if(SceneData.previousScene != "" && SceneData.previousScene != currentSceneName){
            // Если позиция игрока была сохранена для этой сцены
            if(SceneData.PlayerPositions.ContainsKey(currentSceneName)){
                transform.position = SceneData.PlayerPositions[currentSceneName];
                transform.rotation = SceneData.PlayerRotations[currentSceneName];
                 Debug.Log($"Player position restored from SceneData for scene: {currentSceneName}");
            }
            else{
                 Debug.Log($"No player position found in SceneData for scene: {currentSceneName}");
            }
           
        }
         else{
             if (SceneData.previousScene == "")
            {
                 Debug.Log($"No previous Scene in SceneData: {currentSceneName}");
            }
             else if (SceneData.previousScene == currentSceneName)
            {
                  Debug.Log($"Player is in same scene: {currentSceneName}");
            }
        }
       _isInitialized = true;
    }
    void Start()
    {
         if (SceneData.previousScene != ""){
             SceneData.previousScene = "";
         }

         
    }
}