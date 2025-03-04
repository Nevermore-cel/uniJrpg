using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CombatEndChecker
{
    private CombatManager combatManager;

    public CombatEndChecker(CombatManager manager)
    {
        combatManager = manager;
    }

    public bool CheckCombatEnd(List<UnitData> playerTeam, List<UnitData> enemyTeam, ActionSelectorController actionSelectorController)
    {
        // Проверяем, что вражеская команда пуста
        if (enemyTeam.Count == 0)
        {
            EndCombat("Player team wins!");
            return true;
        }

        // Проверяем, что все юниты игрока неактивны (мертвы или выведены из строя)
        if (playerTeam.All(unit => unit == null || !unit.gameObject.activeInHierarchy || !unit.IsActive()))
        {
            EndCombat("Enemy team wins!");
            return true;
        }

        return false;
    }

    private void EndCombat(string winnerMessage)
    {
        combatManager.EndCombat(winnerMessage);
    }
}