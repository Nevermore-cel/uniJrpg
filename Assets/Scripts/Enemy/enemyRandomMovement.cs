using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.SceneManagement;

public class enemyRandomMovement : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData; // Ссылка на EnemyData
    public SceneLoader sceneLoader; // Ссылка на SceneLoader
    [SerializeField] private Animator anim; // Ссылка на Animator
    public NavMeshAgent agent;
    public Transform centrePoint;
    public Transform player;
    [SerializeField] private BoxCollider attackCollider;
    private bool isChasing = false;
    private bool isIdle = false;
    private bool isAttacking = false;
    private bool isHitting = false;
    private const int MAX_RANDOM_POINT_ATTEMPTS = 10;
    private bool hasDestination = false;

    private string _objectID; // Уникальный ID объекта
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private bool _isInitialized = false;

    void Awake()
    {
        _objectID = gameObject.name;
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    void Start()
    {
        if (_isInitialized) return;
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        if (anim == null)
        {
            Debug.LogError("Animator не найден на этом GameObject!");
        }
        agent = GetComponent<NavMeshAgent>();
        // Проверяем, не был ли враг уже уничтожен
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (SceneData.IsObjectDestroyed(currentSceneName, _objectID))
        {
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
            gameObject.SetActive(false); // Отключаем объект
            return;
        }
        if (centrePoint != null)
        {
            Patrol();
        }
        else
        {
            agent.isStopped = true;
            anim.SetTrigger(enemyData.idleAnimationTrigger);
        }
        _isInitialized = true;
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;
        HandleChasing();
        HandleAttacking();
        HandlePatrolling();
        HandleAnimation();
        if (agent.remainingDistance <= agent.stoppingDistance && hasDestination)
        {
            hasDestination = false;
        }
    }

    private void HandleChasing()
    {
        if (Vector3.Distance(transform.position, player.position) <= GetChaseRange())
        {
            isChasing = true;
            agent.SetDestination(player.position);
        }
        else
        {
            isChasing = false;
        }
    }

    private void HandleAttacking()
    {
        if (Vector3.Distance(transform.position, player.position) <= GetAttackDistance() &&
            !isAttacking &&
            IsInFieldOfView(player.position))
        {
            isAttacking = true;
            anim.SetTrigger(enemyData.attackAnimationTrigger);
            agent.isStopped = true;
            StartCoroutine(ResumeMovementAfterAttack(1f));
        }
    }

    private void HandlePatrolling()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !isIdle && !isChasing && !isAttacking)
        {
            isIdle = true;
            agent.isStopped = true;
            anim.SetTrigger(enemyData.idleAnimationTrigger);
            StartCoroutine(ResumeMovement(GetIdleTime()));
        }
        else if (!isChasing && !isAttacking)
        {
            agent.isStopped = false;
            Patrol();
        }
    }

    private void HandleAnimation()
    {
        if (isChasing || (!isIdle && agent.velocity.magnitude > 0.1f))
        {
            anim.SetTrigger(enemyData.walkAnimationTrigger);
        }
    }

    private bool IsInFieldOfView(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        return angle <= GetFieldOfView() / 2f;
    }

    private void Patrol()
    {
        if (!hasDestination && !isChasing)
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, GetRange(), out point))
            {
                NavMeshPath path = new NavMeshPath();
                if (agent.isOnNavMesh && NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, path))
                {
                    Debug.DrawRay(point, Vector3.up, Color.red, 1.0f);
                    agent.SetDestination(point);
                    hasDestination = true;
                }
            }
        }
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        result = Vector3.zero; //  Инициализируем result
        for (int i = 0; i < MAX_RANDOM_POINT_ATTEMPTS; i++)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        return false;
    }

    private IEnumerator ResumeMovement(float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetTrigger(enemyData.walkAnimationTrigger);
        agent.isStopped = false;
        isIdle = false;
        Patrol();
    }

    private IEnumerator ResumeMovementAfterAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
        yield return new WaitForSeconds(GetHitDelay());
        if (IsInFieldOfView(player.position))
        {
            isHitting = true;
            Debug.Log("Попадание!");
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneData.MarkObjectAsDestroyed(currentSceneName, _objectID);
            // Сохраняем позицию и поворот врага и деактивируем его
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
            gameObject.SetActive(false);
            if (sceneLoader == null)
            {
                Debug.LogError("SceneLoader не прикреплен к этому GameObject!");
                yield break; // Используем yield break вместо return
            }

            sceneLoader.LoadNextSceneAndTransferData(_objectID, enemyData); // Вызываем метод загрузки сцены
        }
        else
        {
            isHitting = false;
            Debug.Log("Промах!");
        }

        if (gameObject.activeSelf)
        {
            agent.isStopped = false;
            Patrol();
        }
    }
    public float GetRange()
    {
        return enemyData.range;
    }
    public float GetChaseRange()
    {
        return enemyData.chaseRange;
    }
    public float GetIdleTime()
    {
        return enemyData.idleTime;
    }
    public float GetAttackDistance()
    {
        return enemyData.attackDistance;
    }
    public float GetFieldOfView()
    {
        return enemyData.fieldOfView;
    }
    public float GetHitDelay()
    {
        return enemyData.hitDelay;
    }
}