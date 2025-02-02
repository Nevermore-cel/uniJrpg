using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText;
    [SerializeField] private string actionText;
    [SerializeField] public string nextScene = "";
    [SerializeField] private float interactionRange;
    [SerializeField] public bool _active = true;

    private bool _interactionProcessedThisFrame = false;

    public void AcceptExit()
    {
        // Сохраняем позицию и поворот игрока перед переходом
        string currentSceneName = SceneManager.GetActiveScene().name;
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        SceneData.PlayerPositions[currentSceneName] = playerTransform.position;
        SceneData.PlayerRotations[currentSceneName] = playerTransform.rotation;
        SceneData.previousScene = currentSceneName;
        SceneManager.LoadScene(nextScene);
    }

    //текст интерфейса интерации 
    public string GetInteractText()
    {
        return interactText;
    }

    public string GetActionText()
    {
        return actionText;
    }

    public float GetRangeInteraction()
    {
        return interactionRange;
    }

    public void Interact(Transform interactorTransform)
    {
         if (_interactionProcessedThisFrame)
        {
            return; // Игнорируем повторный вызов в этом кадре
        }
        if (_active)
        {
            AcceptExit();
             _interactionProcessedThisFrame = true; // Устанавливаем флаг, что взаимодействие было обработано
        }
        else
        {
            Debug.Log("Exit is inactive!");
             _interactionProcessedThisFrame = true; // Устанавливаем флаг, что взаимодействие было обработано
        }

    }

    public Transform GetTransform()
    {
        return transform;
    }
     private void LateUpdate()
    {
        _interactionProcessedThisFrame = false; // Сбрасываем флаг в конце кадра
    }
}