using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private Movement _movement;

    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator component not found!");
        }

        _movement = GetComponent<Movement>();
        if (_movement == null)
        {
            Debug.LogError("Movement component not found!");
        }
    }

    void Update()
    {
        if (_animator == null || _movement == null) return;

        // Check if there is any input
        bool isMoving = Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f;

        // Set the "isMoving" parameter in the Animator
        _animator.SetBool("isMoving", isMoving);
    }

    // Этот метод будет вызываться из EnemyAttackInteract
    public void TriggerAttackAnimation()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Attack"); // Триггерим параметр "Attack"
        }
    }
}