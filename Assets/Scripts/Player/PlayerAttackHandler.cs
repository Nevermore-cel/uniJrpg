using UnityEngine;
using System.Collections;

public class PlayerAttackHandler : MonoBehaviour
{
    private Animator _animator;
    private AnimationClip _attackAnimationClip; // Ссылка на анимацию атаки
    private bool _isAttacking = false;
    private int _attackStateHash; // Hash состояния анимации атаки

    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator component not found!");
            return;
        }

        _attackStateHash = Animator.StringToHash("attackMelee"); // Хэш состояния анимации атаки

        // Ищем AnimationClip в AnimatorOverrideController
        AnimatorOverrideController overrideController = _animator.runtimeAnimatorController as AnimatorOverrideController;
        if (overrideController != null)
        {
            // Перебираем все AnimationClip-ы в AnimatorOverrideController
            AnimationClip[] clips = overrideController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == "attackMelee")
                {
                    _attackAnimationClip = clip;
                    break;
                }
            }
        }

        // Если AnimationOverrideController не используется или анимация не найдена, пробуем получить из стандартного контроллера
        if (_attackAnimationClip == null)
        {
            foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "attackMelee")
                {
                    _attackAnimationClip = clip;
                    break;
                }
            }
        }

        if (_attackAnimationClip == null)
        {
            Debug.LogError("Attack animation clip not found!");
        }
    }

    public void HandleAttack()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        _isAttacking = true;
        _animator.SetTrigger("Attack"); // Запускаем анимацию атаки

        // Ждем, пока анимация не перейдет в состояние attackMelee
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == _attackStateHash);

        float animationSpeed = _animator.GetCurrentAnimatorStateInfo(0).speed; // Получаем скорость анимации из AnimatorStateInfo
        float actualAnimationLength = _attackAnimationClip != null ? _attackAnimationClip.length / animationSpeed : 1f; // Вычисляем реальную длительность

        if (_attackAnimationClip != null)
        {
            yield return new WaitForSeconds(actualAnimationLength); // Ждем длительность анимации
        }
        else
        {
            Debug.LogError("Attack animation clip is null! Using default 1 second delay.");
            yield return new WaitForSeconds(1f); // Запасной вариант
        }

        _isAttacking = false;
        // Сбрасываем триггер Attack (если не используете StateMachineBehavior или AnimationEvent)
        //_animator.ResetTrigger("Attack");
    }

    public bool IsAttacking()
    {
        return _isAttacking;
    }

    // (Можно использовать, если используете Animation Event)
    public void ResetAttackTrigger()
    {
        if (_animator != null)
        {
            _animator.ResetTrigger("Attack");
        }
    }
}