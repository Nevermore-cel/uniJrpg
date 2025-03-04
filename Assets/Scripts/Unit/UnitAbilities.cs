using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnitAbilities
{
    private UnitData unitData;

    public UnitAbilities(UnitData unit)
    {
        unitData = unit;
    }

    public List<AbilityData> GetAbilities()
    {
        List<AbilityData> selectedAbilities = new List<AbilityData>();
        foreach (int index in unitData.selectedAbilitiesIndexes)
        {
            if (index >= 0 && index < unitData.abilitiesData.abilities.Count)
            {
                selectedAbilities.Add(unitData.abilitiesData.abilities[index]);
            }
        }
        return selectedAbilities;
    }

    public void SetSelectedAbilities(List<AbilityData> newAbilities)
    {
        unitData.selectedAbilitiesIndexes.Clear();
        for (int i = 0; i < newAbilities.Count; i++)
        {
            if (unitData.abilitiesData != null && unitData.abilitiesData.abilities.Contains(newAbilities[i]))
            {
                unitData.selectedAbilitiesIndexes.Add(unitData.abilitiesData.abilities.IndexOf(newAbilities[i]));
            }
            else
            {
                Debug.LogWarning("Ability not found in AbilityData");
            }
        }
    }
}