using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class CombatEndChecker
{
    private readonly CombatManager _combatManager;

    public CombatEndChecker(CombatManager combatManager)
    {
        _combatManager = combatManager;
    }

    public bool CheckCombatEnd(List<UnitData> playerTeam, List<UnitData> enemyTeam, ActionSelectorController actionSelectorController, string previousSceneName)
    {
        if (TeamDefeated(playerTeam))
        {
            EndCombat("Враги победили!", previousSceneName);
            return true;
        }
        if (TeamDefeated(enemyTeam))
        {
            EndCombat("Игроки победили!", previousSceneName);
            return true;
        }
        return false;
    }

    private bool TeamDefeated(List<UnitData> team)
    {
        return team.All(unit => unit.currentHealth <= 0);
    }

    private void EndCombat(string message, string nextScene)
    {
        _combatManager.EndCombat(message);
        ReturnToPreviousScene(nextScene);
    }

    private void ReturnToPreviousScene(string nextScene)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Transform playerTransform = player.transform;
            SceneData.SavePlayerTransform(SceneManager.GetActiveScene().name, playerTransform);
        }
        else
        {
            Debug.LogWarning("No Player object found in the scene.");
        }

        SceneManager.LoadScene(nextScene);
    }
}