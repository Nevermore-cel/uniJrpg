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

        combatManager.playerTeam = FindUnitsByTag(playerTag, companionTag);
        combatManager.enemyTeam = FindUnitsByTag(enemyTag);

        combatManager.allUnits.AddRange(combatManager.playerTeam);
        combatManager.allUnits.AddRange(combatManager.enemyTeam);
    }

    private List<UnitData> FindUnitsByTag(params string[] tags)
    {
        List<UnitData> units = new List<UnitData>();
        foreach (string tag in tags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                UnitData unit = obj.GetComponent<UnitData>();
                if (unit != null)
                {
                    units.Add(unit);
                }
            }
        }
        return units;
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