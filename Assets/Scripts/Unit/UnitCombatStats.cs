using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnitCombatStats
{
    private UnitData unitData;

    public UnitCombatStats(UnitData unit)
    {
        unitData = unit;
    }

    public bool CanUseAbility(AbilityData ability)
    {
        return unitData.currentActionPoints >= ability.cost;
    }

    public bool CanUseItem(ItemData item)
    {
        return item != null && unitData.GetItemQuantity(item) > 0;
    }

    public void DeductActionPoints(int cost)
    {
        if (unitData.currentActionPoints >= cost)
        {
            unitData.currentActionPoints -= cost;
            Debug.Log($"{unitData.unitName} spent {cost} action points. Current AP: {unitData.currentActionPoints}");
        }
        else
        {
            Debug.LogWarning($"{unitData.unitName} does not have enough action points to use this ability. Current AP: {unitData.currentActionPoints}, cost: {cost}");
        }
    }

    public void ApplyAbilityEffect(AbilityData ability, bool isCrit)
    {
        if (ability == null)
        {
            Debug.LogError("Ability is null. Cannot apply effect.");
            return;
        }
        TakeDamage(ability.damage, ability.typeAction, ability.abilityName, isCrit);
    }

    public void ApplyItemEffect(ItemData item, UnitData targetUnit, bool isCrit)
    {
        if (item == null)
        {
            Debug.LogError("Item is null. Cannot apply effect.");
            return;
        }
        if (targetUnit != null)
        {
            targetUnit.TakeDamage(item.damage, item.typeAction, item.itemName, isCrit);
        }
        else
        {
            Debug.LogWarning($"{unitData.unitName} (ID: {unitData.unitID}) found no valid target.");
        }
    }

    public void TakeDamage(int damage, ActionType damageType, string damageSourceName, bool isCrit)
    {
        if (isCrit)
        {
            damage *= 2;
            Debug.Log($"{unitData.unitName} performed a CRITICAL HIT!");
        }

        float modifiedDamage = CalculateDamage(damage, damageType);
       int finalHealth = unitData.currentHealth - (int)modifiedDamage;
    if (finalHealth <= 0)
        {
                Die();
        }
    unitData.currentHealth = Mathf.Clamp(finalHealth, 0, unitData.maxHealth);
        Debug.Log($"{unitData.unitName} took {modifiedDamage} damage from '{damageSourceName}'. Current health: {unitData.currentHealth}");
    }
    private float CalculateDamage(int damage, ActionType attackType)
    {
        float modifiedDamage = damage;

        foreach (var resistance in unitData.resistances)
        {
            if (resistance.resistanceType == attackType)
            {
                modifiedDamage *= (1f - resistance.value);
                Debug.Log($"{unitData.unitName} has resistance to {attackType}, damage reduced by {resistance.value * 100}%");
                break;
            }
        }

        foreach (var weakness in unitData.weaknesses)
        {
            if (weakness.weaknessType == attackType)
            {
                modifiedDamage *= (1f + weakness.value);
                Debug.Log($"{unitData.unitName} has weakness to {attackType}, damage increased by {weakness.value * 100}%");
                break;
            }
        }

        modifiedDamage *= (1f - unitData.damageReductionPercentage);

        return modifiedDamage;
    }

    public void AddDamageReduction(float reductionPercentage)
    {
        unitData.damageReductionPercentage = Mathf.Clamp01(unitData.damageReductionPercentage + reductionPercentage);
        Debug.Log($"{unitData.unitName} damage reduction increased by: '{reductionPercentage * 100}%', total: {unitData.damageReductionPercentage * 100}%");
    }

    public void ApplyDamageReduction(float percentage)
    {
        unitData.damageReductionPercentage = Mathf.Clamp01(percentage);
        Debug.Log($"{unitData.unitName} damage reduction is set to : '{unitData.damageReductionPercentage * 100}%'");
    }

    public bool CalculateCrit()
    {
        float randomNumber = UnityEngine.Random.value;
        unitData._isCrit = randomNumber <= unitData.currentCritChance;

        if (unitData._isCrit)
        {
            unitData.currentCritChance = unitData.baseCritChance;
        }
        else
        {
            unitData.currentCritChance = Mathf.Min(unitData.currentCritChance + unitData.critChanceIncreasePerFailedAttempt, unitData.maxCritChance);
        }

        return unitData._isCrit;
    }

    public string GetCritInfo()
    {
        return unitData._isCrit ?
               $"{unitData.unitName} performed a CRITICAL HIT! (Chance was {unitData.currentCritChance * 100}%)" :
               $"{unitData.unitName} did not perform a critical hit.";
    }
     private void Die()
    {
        Debug.Log($"{unitData.unitName} has reached 0 health, but will not die because it is a {unitData.gameObject.tag}");
    
         if (unitData.gameObject.CompareTag("Enemy"))
        {
            CombatManager combatManager = Object.FindObjectOfType<CombatManager>();
            if (combatManager != null)
            {
                combatManager.RemoveUnit(unitData);
                Object.Destroy(unitData.gameObject);
                return;
            }
        }
        unitData.isActive = false;
    }
}