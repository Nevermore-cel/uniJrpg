using UnityEngine;

public class CompanionAnimationController : MonoBehaviour
{
    public CompanionMovement companionMovement; // Ссылка на скрипт передвижения компаньона
    public Animator animator; // Ссылка на компонент Animator

    private static readonly int _isMoving = Animator.StringToHash("isMoving");
    void Update()
    {
        // Проверяем, существует ли ссылка на скрипт передвижения и компонент Animator
        if (companionMovement == null || animator == null)
        {
            Debug.LogError("CompanionAnimationController: Не назначены ссылки на CompanionMovement или Animator!");
            return;
        }

        // Получаем скорость компаньона из компонента NavMeshAgent
        bool isMoving = companionMovement.navMeshAgent.velocity.magnitude > 0.1f; // Используем небольшое значение для фильтрации

        // Устанавливаем параметр аниматора "isMoving"
        animator.SetBool(_isMoving, isMoving);
    }
}