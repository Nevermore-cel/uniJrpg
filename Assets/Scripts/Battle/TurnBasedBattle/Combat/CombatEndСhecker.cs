using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatEndChecker
{
    private readonly CombatManager _combatManager;

    public CombatEndChecker(CombatManager combatManager)
    {
        _combatManager = combatManager;
    }
    public bool CheckCombatEnd(List<UnitData> playerTeam, List<UnitData> enemyTeam, ActionSelectorController actionSelectorController, string previousSceneName) // Добавляем параметр
    {
        if (playerTeam.All(unit => unit.currentHealth <= 0))
        {
            _combatManager.EndCombat("Враги победили!");
            ReturnToPreviousScene(previousSceneName); // Передаем название сцены
            return true;
        }
        if (enemyTeam.All(unit => unit.currentHealth <= 0))
        {
            _combatManager.EndCombat("Игроки победили!");
            ReturnToPreviousScene(previousSceneName); // Передаем название сцены
            return true;
        }
        return false;
    }
    private void ReturnToPreviousScene(string nextScene)
    {
        // Сохраняем позицию и поворот игрока перед переходом
        string currentSceneName = SceneManager.GetActiveScene().name;
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        SceneData.PlayerPositions[currentSceneName] = playerTransform.position;
        SceneData.PlayerRotations[currentSceneName] = playerTransform.rotation;
        SceneData.previousScene = currentSceneName;
        SceneManager.LoadScene(nextScene);
    }
}