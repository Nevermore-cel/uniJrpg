using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CompanionMovement : MonoBehaviour
{
    public Transform playerTransform; // Ссылка на трансформ игрока
    public float speed = 3.0f; // Скорость движения
    public float stoppingDistance = 0.5f; // Дистанция остановки
    public float patrolRange = 2.0f; // Радиус патрулирования вокруг игрока
    public float companionSpacing = 1f; // Расстояние между компаньонами

    public NavMeshAgent navMeshAgent;
    private bool hasReachedFirstPatrolPoint = false; // Флаг достижения первой точки патрулирования
    private bool hasPatrolPoint = false; // Флаг наличия активной точки патрулирования
    private const int MAX_RANDOM_POINT_ATTEMPTS = 10;

    private Vector3 lastPlayerPosition; // Последняя зафиксированная позиция игрока
    private bool playerStopped = false; // Флаг остановки игрока
    private bool _isInitialized = false;

    private int _companionIndex; // Индекс текущего компаньона

    void Awake()
    {
        // Вычисление индекса компаньона из имени объекта
        string[] nameParts = gameObject.name.Split('(');
        if(nameParts.Length > 1)
        {
            string indexPart = nameParts[1].TrimEnd(')');
            if(int.TryParse(indexPart, out int index))
            {
                _companionIndex = index;
            }
        }
    }

    void Start()
    {
        if(_isInitialized) return;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.stoppingDistance = stoppingDistance;

        string currentSceneName = SceneManager.GetActiveScene().name;
        // Проверка возврата на предыдущую сцену
        if (SceneData.PlayerPositions.ContainsKey(currentSceneName) && SceneData.previousScene != "")
        {
            // Расчет позиции телепортации со смещением
            Vector3 offsetPosition = SceneData.PlayerPositions[currentSceneName] +
                              (Quaternion.Euler(0, 45f*_companionIndex, 0) * Vector3.left* companionSpacing);
            // Телепортация к позиции игрока
            transform.position = offsetPosition;

            // Сброс состояния компаньона
            ResetCompanionState();
        }
        lastPlayerPosition = playerTransform.position;
        _isInitialized = true;
    }

    private void ResetCompanionState()
    {
        hasReachedFirstPatrolPoint = false;
        hasPatrolPoint = false;
        playerStopped = false;
        navMeshAgent.isStopped = false;
        navMeshAgent.ResetPath();
        Debug.Log("Companion state reset");
    }

    void Update()
    {
        // Проверка движения игрока
        if (Vector3.Distance(playerTransform.position, lastPlayerPosition) < 0.1f)
        {
            playerStopped = true;
        }
        else
        {
            playerStopped = false;
            lastPlayerPosition = playerTransform.position;
        }

        // Обработка достижения точки назначения
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!hasReachedFirstPatrolPoint)
            {
                hasReachedFirstPatrolPoint = true;
                navMeshAgent.isStopped = true; // Остановка у первой точки
                Debug.Log("Companion has reached the first patrol point");
            }
            else
            {
                hasPatrolPoint = false; // Сброс флага точки патрулирования
            }
        }

        // Проверка дистанции для начала патрулирования
        if (!playerStopped && Vector3.Distance(playerTransform.position, transform.position) > patrolRange && hasReachedFirstPatrolPoint)
        {
            Patrol();
        }
    }

    // Логика патрулирования
    private void Patrol()
    {
        if (!hasPatrolPoint)
        {
            Vector3 point;
            if (RandomPoint(playerTransform.position, patrolRange, out point))
            {
                NavMeshPath path = new NavMeshPath();
                if (navMeshAgent.isOnNavMesh && NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, path))
                {
                    navMeshAgent.SetDestination(point);
                    navMeshAgent.isStopped = false;
                    hasPatrolPoint = true;
                    Debug.Log("Companion is patrolling");
                }
                else
                {
                    Debug.Log("Point is not accessible for NavMeshAgent");
                }
            }
            else
            {
                Debug.Log("No valid point found");
            }
        }
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < MAX_RANDOM_POINT_ATTEMPTS; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                Debug.Log("Valid point found: " + result);
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}