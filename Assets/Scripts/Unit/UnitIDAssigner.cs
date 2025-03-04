using UnityEngine;
using System.Collections.Generic;

public static class UnitIDAssigner
{
    private static HashSet<int> usedIDs = new HashSet<int>();

    public static void AssignUniqueEnemyID(UnitData unitData)
    {
        int newID;
        do
        {
            newID = UnityEngine.Random.Range(1000, 10000);
        } while (usedIDs.Contains(newID));
        unitData.unitID = newID;
        usedIDs.Add(newID);
    }
}