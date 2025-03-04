using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CombatUnitAction
{
    private CombatManager combatManager;

    public CombatUnitAction(CombatManager manager)
    {
        combatManager = manager;
    }

    public void SimulateAction(UnitData unit, List<UnitData> aliveUnits)
    {
        if (unit.abilitiesData == null || unit.GetAbilities().Count == 0)
        {
            Debug.LogWarning($"{unit.unitName} (ID: {unit.unitID}) has no ability to use.");
            return;
        }

        AbilityData ability = unit.GetAbilities()[0];
        Debug.Log($"{unit.unitName} (ID: {unit.unitID}) uses {ability.abilityName}.");
        unit.DeductActionPoints(ability.cost);

        UnitData targetUnit = FindTarget(unit, aliveUnits);

        if (targetUnit != null && targetUnit.gameObject.activeInHierarchy && targetUnit.currentHealth > 0)
        {
            bool isCrit = unit.CalculateCrit();
            targetUnit.ApplyAbilityEffect(ability, isCrit);
        }
        else
        {
            Debug.LogWarning($"{unit.unitName} (ID: {unit.unitID}) found no valid target.");
        }
    }

    private UnitData FindTarget(UnitData attacker, List<UnitData> aliveUnits)
    {
        List<UnitData> targetTeam;

        if (attacker.gameObject.CompareTag(combatManager.playerTag) || attacker.gameObject.CompareTag(combatManager.companionTag))
        {
            targetTeam = combatManager.enemyTeam;
        }
        else if (attacker.gameObject.CompareTag(combatManager.enemyTag))
        {
            targetTeam = combatManager.playerTeam;
        }
        else
        {
            return null;
        }

        if (aliveUnits != null && aliveUnits.Count > 0)
        {
            List<UnitData> validTargets = aliveUnits.Where(unit =>
                (attacker.gameObject.CompareTag(combatManager.playerTag) || attacker.gameObject.CompareTag(combatManager.companionTag)) ? targetTeam.Contains(unit) : targetTeam.Contains(unit)
                && unit.currentHealth > 0
            ).ToList();

            if (validTargets.Count > 0)
            {
                return validTargets[UnityEngine.Random.Range(0, validTargets.Count)];
            }
        }
        return null;
    }
}