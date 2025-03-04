using UnityEngine;
using System.Collections;

public class EnemyAttackInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText;
    [SerializeField] private string actionText;
    [SerializeField] private float interactionRange = 10f;

    public void Interact(Transform interactorTransform) // Реализация интерфейса
    {
        Debug.Log("EnemyInteract");

        PlayerAttackHandler playerAttackHandler = interactorTransform.GetComponent<PlayerAttackHandler>();

        if (playerAttackHandler != null)
        {
            // Запускаем корутину загрузки сцены
            StartCoroutine(InteractCoroutine(playerAttackHandler));
        }
        else
        {
            Debug.LogError("PlayerAttackHandler component not found on the interactorTransform!");
            LoadNextScene(); // Загружаем сцену сразу, если нет обработчика атаки
        }
    }

    // Корутина для воспроизведения атаки и загрузки сцены
    private IEnumerator InteractCoroutine(PlayerAttackHandler playerAttackHandler)
    {
        playerAttackHandler.HandleAttack(); // Запускаем анимацию атаки

        // Даем анимации воспроизвестись (длительность анимации определена в PlayerAttackHandler)
        yield return new WaitUntil(() => !playerAttackHandler.IsAttacking());

        LoadNextScene(); // Загружаем сцену после завершения атаки
    }

    private void LoadNextScene()
    {
        enemyRandomMovement enemyMovement = GetComponent<enemyRandomMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.LoadNextSceneAndTransferData(); // Загружаем следующую сцену
        }
        else
        {
            Debug.LogError("enemyRandomMovement component not found on this GameObject!");
        }
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

    public Transform GetTransform()
    {
        return transform;
    }
}