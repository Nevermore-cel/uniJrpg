using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Animation")]
    public string walkAnimationTrigger = "Walking";
    public string idleAnimationTrigger = "Idling";
    public string attackAnimationTrigger = "Attack";

    [Header("Scene Transition")]
    public string nextSceneName = "dungeonNormalFightScene";
    public GameObject EnemyPrefab;
    public int EnemyCount = 1;
    public string SpawnTag = "enemySpawnPoint";

    [Header("Interaction")]
    public string interactText = "Враг";
    public string actionText = "Атаковать!";
    public float interactionRange = 10f;

    [Header("AI")]
    public float range = 10f;
    public float chaseRange = 10f;
    public float idleTime = 0.3f;
    public float attackDistance = 4f;
    public float fieldOfView = 45f;
    public float hitDelay = 2f;
}