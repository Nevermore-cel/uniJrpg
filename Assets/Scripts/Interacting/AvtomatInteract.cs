using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvtomatInteract : MonoBehaviour,  IInteractable
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
        
    }

    // Корутина для воспроизведения атаки и загрузки сцены
    private IEnumerator InteractCoroutine(PlayerAttackHandler playerAttackHandler)
    {
        playerAttackHandler.HandleAttack(); // Запускаем анимацию атаки

        // Даем анимации воспроизвестись (длительность анимации определена в PlayerAttackHandler)
        yield return new WaitUntil(() => !playerAttackHandler.IsAttacking());
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