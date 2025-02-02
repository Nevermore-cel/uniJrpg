using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CompanionMovement : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's transform
    public float speed = 3.0f; // Movement speed
    public float stoppingDistance = 0.5f; // Distance at which to stop moving
    public float patrolRange = 2.0f; // Patrol range around the player
    public float companionSpacing = 1f; // Spacing between companions

    private NavMeshAgent navMeshAgent;
    private bool hasReachedFirstPatrolPoint = false; // Flag to indicate if the companion has reached the first patrol point
    private bool hasPatrolPoint = false; // Flag to indicate if the companion has a patrol point assigned
    private const int MAX_RANDOM_POINT_ATTEMPTS = 10;

    private Vector3 lastPlayerPosition; // Last known position of the player
    private bool playerStopped = false; // Flag to indicate if the player has stopped moving
     private bool _isInitialized = false;

        private int _companionIndex; // Index of this companion
    void Awake()
    {
    // Вычисляем индекс компаньона на основе имени объекта
         string[] nameParts = gameObject.name.Split('(');
          if(nameParts.Length > 1){
             string indexPart = nameParts[1].TrimEnd(')');
              if(int.TryParse(indexPart, out int index)){
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
        // Проверяем, является ли это возвратом на сцену
        if (SceneData.PlayerPositions.ContainsKey(currentSceneName) && SceneData.previousScene != "")
        {
            // Вычисляем позицию для телепортации со смещением
                Vector3 offsetPosition = SceneData.PlayerPositions[currentSceneName] +
                                  (Quaternion.Euler(0, 45f*_companionIndex, 0) * Vector3.left* companionSpacing) ;
                // Телепортируем компаньона к позиции игрока со смещением
                transform.position = offsetPosition;


             // Сбрасываем флаги и состояние компаньона
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
    // Check if the player has stopped moving
    if (Vector3.Distance(playerTransform.position, lastPlayerPosition) < 0.1f)
    {
        playerStopped = true;
    }
    else
    {
        playerStopped = false;
        lastPlayerPosition = playerTransform.position;
    }

    // Check if the companion has reached its current destination
    if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
    {
        // Check if the companion has reached the first patrol point
        if (!hasReachedFirstPatrolPoint)
        {
            hasReachedFirstPatrolPoint = true;
            navMeshAgent.isStopped = true; // Stop the companion at the first patrol point
            Debug.Log("Companion has reached the first patrol point");
        }
        else
        {
            // Companion has reached a patrol point, reset the flag
            hasPatrolPoint = false;
        }
    }

    // Check if the player has moved out of the patrol range and the companion has reached the first patrol point
    if (!playerStopped && Vector3.Distance(playerTransform.position, transform.position) > patrolRange && hasReachedFirstPatrolPoint)
    {
        // Start patrolling around the player
        Patrol();
    }
}

    // Patrol around the player
    private void Patrol()
{
    if (!hasPatrolPoint)
    {
        Vector3 point;
        if (RandomPoint(playerTransform.position, patrolRange, out point))
        {
            // Check if the point is accessible for NavMeshAgent
            NavMeshPath path = new NavMeshPath();
            if (navMeshAgent.isOnNavMesh && NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, path))
            {
                navMeshAgent.SetDestination(point);
                navMeshAgent.isStopped = false; // Ensure the NavMeshAgent is not stopped
                hasPatrolPoint = true; // Set the flag to indicate that the companion has a patrol point assigned
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