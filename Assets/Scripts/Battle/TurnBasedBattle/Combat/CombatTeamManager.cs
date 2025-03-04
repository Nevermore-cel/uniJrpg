using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CombatTeamManager
{
    private CombatManager combatManager;

    public CombatTeamManager(CombatManager manager)
    {
        combatManager = manager;
    }

    public void FindAndPopulateTeams(string playerTag, string companionTag, string enemyTag)
    {
        combatManager.allUnits.Clear();
        combatManager.playerTeam.Clear();
        combatManager.enemyTeam.Clear();

        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag(playerTag);
        GameObject[] companionObjects = GameObject.FindGameObjectsWithTag(companionTag);
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag(enemyTag);

        AddUnitsToList(playerObjects, combatManager.playerTeam);
        AddUnitsToList(companionObjects, combatManager.playerTeam);
        AddUnitsToList(enemyObjects, combatManager.enemyTeam);
    }

    private void AddUnitsToList(GameObject[] objects, List<UnitData> team)
    {
        foreach (GameObject obj in objects)
        {
            UnitData unit = obj.GetComponent<UnitData>();
            if (unit != null)
            {
                combatManager.allUnits.Add(unit);
                team.Add(unit);
            }
        }
    }
    public void ResetDamageReductionForPlayerTeam(List<UnitData> playerTeam)
    {
        foreach (UnitData unit in playerTeam)
        {
            if (unit != null && unit.gameObject.activeInHierarchy)
            {
                unit.ApplyDamageReduction(0f);
            }
        }
        Debug.Log("Damage reduction reset for the player team.");
    }
    public bool IsPlayerOrCompanion(UnitData unit)
    {
        return unit.gameObject.CompareTag(combatManager.playerTag) || unit.gameObject.CompareTag(combatManager.companionTag);
    }
}