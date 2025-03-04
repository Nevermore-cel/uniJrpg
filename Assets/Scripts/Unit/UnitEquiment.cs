using UnityEngine;
using System.Collections.Generic;

public class UnitEquipment
{
    private UnitData unitData;

    public UnitEquipment(UnitData unit)
    {
        unitData = unit;
    }

    public void AssignUniqueEnemyID()
    {
        UnitIDAssigner.AssignUniqueEnemyID(unitData);
    }

    public void SetSelectedResistances(List<ResistanceData> newResistances)
    {
        unitData.resistances.Clear();
        foreach (var resistance in newResistances)
        {
            unitData.resistances.Add(new ResistanceData { resistanceType = resistance.resistanceType, value = resistance.value });
        }
    }

    public void SetSelectedWeaknesses(List<WeaknessData> newWeaknesses)
    {
        unitData.weaknesses.Clear();
        foreach (var weakness in newWeaknesses)
        {
            unitData.weaknesses.Add(new WeaknessData { weaknessType = weakness.weaknessType, value = weakness.value });
        }
    }
}