using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Attack : MonoBehaviour
{
    public int attackDamage; // Урон, который будет нанесен
    private CombatManager combatManager;
    private ActionSelectorController actionSelectorController;
    private UnitData currentUnit;
    private UnitData targetUnitData;
    private bool isSelectingTarget = false;
    private List<UnitData> allUnits;
    public BattleInterfaceController battleInterfaceController;
    public UnitData _selectedTarget;

    public TargetSelector targetSelector; // Добавлена ссылка на TargetSelector

    void Start()
    {
        combatManager = FindObjectOfType<CombatManager>();
        actionSelectorController = FindObjectOfType<ActionSelectorController>();
        targetSelector = FindObjectOfType<TargetSelector>(); // Находим TargetSelector
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
    public void OnAttackButtonClicked() // Теперь public и не static
    {
        if (targetSelector != null)
        {
            targetSelector.InitializeTargets(true);
              currentUnit = GetCurrentUnitData(actionSelectorController.currentUnitID);
             targetSelector.isSelectingTarget = true;
           Debug.Log($"Attack selected, selecting target");
        }
        else
        {
            Debug.LogError("TargetSelector is null");
        }
    }
     public void ApplyDamageToTarget()
    {
        // Apply damage to the target
        if (_selectedTarget != null && currentUnit != null)
        {
            bool isCrit = currentUnit.CalculateCrit();
            _selectedTarget.TakeDamage(currentUnit.attackDamage, currentUnit.attackType, currentUnit.unitName, isCrit);
            Debug.Log($"Attack Target Unit '{_selectedTarget.unitName}', ID {_selectedTarget.unitID}, Type: {currentUnit.attackType}");
        }
        else
        {
            Debug.LogWarning("Target Unit or Current Unit is null!");
        }
    }
    private void Update()
    {
        if (isSelectingTarget && Input.GetKeyDown(KeyCode.Return))
        {
             HandleTargetSelection();
              isSelectingTarget = false;
        }
    }
    private void HandleTargetSelection()
    {
          if (targetSelector != null && targetSelector.SelectedTarget != null)
        {
            // Мы уже вызываем ApplyDamageToTarget в TargetSelector.ConfirmTarget
              _selectedTarget = targetSelector.SelectedTarget;
           }
           else
           {
               CancelAttack();
               Debug.LogWarning("Target clicked has no UnitData Component");
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
    public void EndTurn(UnitData target = null)
    {
        if (combatManager != null)
        {
            combatManager.EndTurn(target);
            if (actionSelectorController != null)
            {
                actionSelectorController.HideActionSelector();
            }
        }
        else
        {
            Debug.LogError("CombatManager is null!");
        }
    }
    public void CancelAttack()
    {
        isSelectingTarget = false;
    }
}