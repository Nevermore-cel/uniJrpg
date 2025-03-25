using UnityEngine;
using System.Collections;

public class EnemyAttackInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private EnemyData enemyData; // Ссылка на EnemyData
    public SceneLoader sceneLoader; // Ссылка на SceneLoader
    private string _objectID; // Уникальный ID объекта

    void Start()
    {
        _objectID = gameObject.name;
    }
     public string GetInteractText()
    {
        return enemyData.interactText;
    }

    public string GetActionText()
    {
        return enemyData.actionText;
    }

    public float GetRangeInteraction()
    {
        return enemyData.interactionRange;
    }
    public void Interact(Transform interactorTransform)
    {
        Debug.Log("EnemyInteract");
        PlayerAttackHandler playerAttackHandler = interactorTransform.GetComponent<PlayerAttackHandler>();
        if (playerAttackHandler != null)
        {
            StartCoroutine(InteractCoroutine(playerAttackHandler));
        }
        else
        {
            Debug.LogError("PlayerAttackHandler component not found on the interactorTransform!");
            LoadNextScene(); // Загружаем сцену сразу, если нет обработчика атаки
        }
    }

    private IEnumerator InteractCoroutine(PlayerAttackHandler playerAttackHandler)
    {
        playerAttackHandler.HandleAttack();
        yield return new WaitUntil(() => !playerAttackHandler.IsAttacking());
        LoadNextScene(); // Загружаем сцену после завершения атаки
    }
    private void LoadNextScene()
    {
        enemyRandomMovement enemyMovement = GetComponent<enemyRandomMovement>();
        if (sceneLoader == null)
        {
            Debug.LogError("SceneLoader не прикреплен к этому GameObject!");
            return;
        }
        gameObject.SetActive(false);
        sceneLoader.LoadNextSceneAndTransferData(_objectID, enemyData); // Вызываем метод загрузки сцены
    }

    public Transform GetTransform()
    {
        return transform;
    }
}