using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Attack : MonoBehaviour
{
    public int attackDamage;
    private CombatManager combatManager;
    private ActionSelectorController actionSelectorController;
    private UnitData currentUnit;
    private UnitData targetUnitData;
    private bool isSelectingTarget = false;
    private List<UnitData> allUnits;
    public BattleInterfaceController battleInterfaceController;
    public UnitData _selectedTarget;

    public TargetSelector targetSelector;

    void Start()
    {
        combatManager = FindObjectOfType<CombatManager>();
        actionSelectorController = FindObjectOfType<ActionSelectorController>();
        targetSelector = FindObjectOfType<TargetSelector>();
        if (combatManager != null)
        {
            allUnits = combatManager.GetAllUnits();
        }
        else
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

    private void OnEnable()
    {
        CancelAttack();
    }

    public void OnAttackButtonClicked()
    {
        if (targetSelector != null)
        {
            targetSelector.InitializeTargets(true);
            currentUnit = GetCurrentUnitData(actionSelectorController.currentUnitID);
            targetSelector.isSelectingTarget = true;
            targetSelector.selectedAbility = null;
            targetSelector.selectedItem = null;
            Debug.Log($"Attack selected, selecting target");
        }
        else
        {
            Debug.LogError("TargetSelector is null");
        }
    }

    private UnitData GetCurrentUnitData(int unitId)
    {
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

    public void CancelAttack()
    {
        isSelectingTarget = false;
    }
}