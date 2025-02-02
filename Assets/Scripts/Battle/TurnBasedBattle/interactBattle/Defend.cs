using UnityEngine;
using System.Collections.Generic;

public class Defense : MonoBehaviour
{
    public float damageReductionPercentage = 0.3f; // Процент уменьшения урона
    private CombatManager combatManager;
    private ActionSelectorController actionSelectorController;
    private UnitData currentUnit;
    public BattleInterfaceController battleInterfaceController;

    void Start()
    {
        combatManager = FindObjectOfType<CombatManager>();
        actionSelectorController = FindObjectOfType<ActionSelectorController>();
         if (combatManager == null)
        {
            Debug.LogError("CombatManager is null!");
        }
        if (actionSelectorController == null)
        {
            Debug.LogError("ActionSelectorController is null!");
        }
        if (battleInterfaceController == null)
        {
            Debug.LogError("BattleInterfaceController is null!");
        }
    }

    public void OnDefenseButtonClicked()
    {
         currentUnit = GetCurrentUnitData(actionSelectorController.currentUnitID);
        if (currentUnit != null)
        {
              ApplyDefense();
               EndTurn();
           }
        else
       {
          Debug.LogWarning($"Unit with ID {actionSelectorController.currentUnitID} not found!");
       }
    }
      private void ApplyDefense()
    {
           currentUnit.AddDamageReduction(damageReductionPercentage);
             Debug.Log($"Unit '{currentUnit.unitName}', ID {currentUnit.unitID} adds '{damageReductionPercentage * 100}%' damage reduction");
    }

    private UnitData GetCurrentUnitData(int unitId)
    {
        List<UnitData> allUnits = combatManager.GetAllUnits();
        if (allUnits != null)
       {
            foreach (UnitData unit in allUnits)
           {
                 if (unit != null && unit.unitID == unitId)
                {
                    return unit;
                }
            }
        }
      Debug.LogWarning($"No unit found with ID {unitId}");
       return null;
   }
    public void EndTurn()
   {
        if(combatManager != null)
        {
            combatManager.EndTurn();
             if (actionSelectorController != null)
           {
               actionSelectorController.HideActionSelector();
             }
        }
        else{
            Debug.LogError("CombatManager is null!");
        }
     }
}