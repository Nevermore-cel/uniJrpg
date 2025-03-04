using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.SceneManagement;

public class enemyRandomMovement : MonoBehaviour
{
    [SerializeField] private Animator ObjectToTrigger = null;
    [SerializeField] private string walkAnimationTrigger = "";
    [SerializeField] private string idleAnimationTrigger = "";
    [SerializeField] private string attackAnimationTrigger = "";
    [SerializeField] private string nextSceneName;
    public Animator anim;
    public NavMeshAgent agent;
    public float range;
    public float chaseRange;
    public Transform centrePoint;
    public Transform player;
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private BoxCollider attackCollider;
    [SerializeField] private float attackDistance = 1f;
    [SerializeField] private float fieldOfView = 45f;
    [SerializeField] private float hitDelay = 0.5f;
    [SerializeField] public GameObject EnemyPrefab;
    [SerializeField] public int EnemyCount;
    public string SpawnTag = "SpawnPoint";
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
            anim.SetTrigger(idleAnimationTrigger);
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
        if (Vector3.Distance(transform.position, player.position) <= chaseRange)
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
        if (Vector3.Distance(transform.position, player.position) <= attackDistance &&
            !isAttacking &&
            IsInFieldOfView(player.position))
        {
            isAttacking = true;
            anim.SetTrigger(attackAnimationTrigger);
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
            anim.SetTrigger(idleAnimationTrigger);
            StartCoroutine(ResumeMovement(idleTime));
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
            anim.SetTrigger(walkAnimationTrigger);
        }
    }

    private bool IsInFieldOfView(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        return angle <= fieldOfView / 2f;
    }

    private void Patrol()
    {
        if (!hasDestination && !isChasing)
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point))
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
        result = Vector3.zero;
        return false;
    }

    private IEnumerator ResumeMovement(float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetTrigger(walkAnimationTrigger);
        agent.isStopped = false;
        isIdle = false;
        Patrol();
    }

    private IEnumerator ResumeMovementAfterAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
        yield return new WaitForSeconds(hitDelay);
        if (IsInFieldOfView(player.position))
        {
            isHitting = true;
            Debug.Log("Попадание!");
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneData.MarkObjectAsDestroyed(currentSceneName,_objectID);
             // Сохраняем позицию и поворот врага и деактивируем его
             transform.position = _initialPosition;
              transform.rotation = _initialRotation;
             gameObject.SetActive(false);
           LoadNextSceneAndTransferData();
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

    public void LoadNextSceneAndTransferData()
{   
    string currentSceneName = SceneManager.GetActiveScene().name;
    Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    SceneData.PlayerPositions[currentSceneName] = playerTransform.position;
    SceneData.PlayerRotations[currentSceneName] = playerTransform.rotation;
    SceneData.previousScene = currentSceneName;
    
    // Убедитесь, что статические поля очищены перед установкой новых значений
    StartScene.prefabToSpawn = EnemyPrefab;
    StartScene.spawnCount = EnemyCount;
    StartScene.SpawnTag = SpawnTag;
    StartScene.nextSceneName = nextSceneName;

    // Сначала подписываемся на событие, затем начинаем загрузку
    SceneManager.sceneLoaded += StartScene.OnSceneLoaded;
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(StartScene.nextSceneName);
    
    // Убрать блокировку активации сцены, если она не нужна
    asyncLoad.allowSceneActivation = true; 
    
}
}