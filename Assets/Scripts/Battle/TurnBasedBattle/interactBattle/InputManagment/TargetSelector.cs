using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TargetSelector : MonoBehaviour
{
    private CombatManager combatManager;
    private List<UnitData> selectableTargets;
    private int currentTargetIndex = 0;
    private UnitData _selectedTarget;
    public UnitData SelectedTarget => _selectedTarget;
    public Attack attack;
    public bool isSelectingTarget = false;
    public AbilityData selectedAbility;
    public ItemData selectedItem; // Add this
    public ActionSelectorController actionSelectorController; // Добавляем ссылку на ActionSelectorController

    void Start()
    {
        combatManager = FindObjectOfType<CombatManager>();
        actionSelectorController = FindObjectOfType<ActionSelectorController>();// Находим ActionSelectorController
        if (combatManager == null)
        {
            Debug.LogError("CombatManager is null!");
        }
    }

    public void InitializeTargets(bool isSelectingEnemies)
    {
        selectableTargets = isSelectingEnemies ?
            combatManager.enemyTeam.Where(unit => unit.IsActive()).ToList() :
            combatManager.playerTeam.Where(unit => unit.IsActive()).ToList();

        if (selectableTargets.Count == 0)
        {
            Debug.LogWarning("No selectable targets available.");
            _selectedTarget = null;
            return;
        }

        currentTargetIndex = 0;
        HighlightTarget(selectableTargets[currentTargetIndex]);
        _selectedTarget = selectableTargets[currentTargetIndex];
        attack._selectedTarget = _selectedTarget;
    }

    public void SelectNextTarget()
    {
        if (selectableTargets == null || selectableTargets.Count == 0 || isSelectingTarget == false) return;

        RemoveHighlight(selectableTargets[currentTargetIndex]);
        currentTargetIndex = (currentTargetIndex + 1) % selectableTargets.Count;
        HighlightTarget(selectableTargets[currentTargetIndex]);
        _selectedTarget = selectableTargets[currentTargetIndex];
        attack._selectedTarget = _selectedTarget;
        Debug.Log($"Selected target: {selectableTargets[currentTargetIndex].unitName}, ID: {selectableTargets[currentTargetIndex].unitID}");
    }

    public void SelectPreviousTarget()
    {
        if (selectableTargets == null || selectableTargets.Count == 0 || isSelectingTarget == false) return;

        RemoveHighlight(selectableTargets[currentTargetIndex]);
        currentTargetIndex--;
        if (currentTargetIndex < 0)
        {
            currentTargetIndex = selectableTargets.Count - 1;
        }
        HighlightTarget(selectableTargets[currentTargetIndex]);
        _selectedTarget = selectableTargets[currentTargetIndex];
        attack._selectedTarget = _selectedTarget;
        Debug.Log($"Selected target: {selectableTargets[currentTargetIndex].unitName}, ID: {selectableTargets[currentTargetIndex].unitID}");
    }

      public void ConfirmTarget()
    {
        
         if (!isSelectingTarget)
        {
            Debug.LogWarning("Не в режиме выбора цели");
            return;
        }
        if (_selectedTarget == null ) // Added attack != null check
        {
            Debug.LogWarning("Нет выбранной цели или способности");
            return;
        }
              // Get current Unit
                UnitData currentUnit = GetCurrentUnitData(actionSelectorController.currentUnitID);
                if (currentUnit == null)
                {
                    Debug.LogWarning($"Unit with ID {actionSelectorController.currentUnitID} not found!");
                    return;
                }
                 if(selectedAbility != null){
                     if (currentUnit.CanUseAbility(selectedAbility))
                    {
                         currentUnit.DeductActionPoints(selectedAbility.cost);
                        bool isCrit = currentUnit.CalculateCrit(); // Calculate crit for ability use
                         _selectedTarget.ApplyAbilityEffect(selectedAbility, isCrit); // Pass isCrit

                     }
                }
                else if(selectedItem != null){
                       if (currentUnit.CanUseItem(selectedItem))
                    {
                           currentUnit.UseItem(selectedItem, _selectedTarget); 
                           bool isCrit = currentUnit.CalculateCrit(); // Calculate crit for item use
                           _selectedTarget.ApplyItemEffect(selectedItem, _selectedTarget, isCrit); // Pass isCrit
                    }
                }
            
            
             Debug.Log($"{currentUnit.unitName} Attack {_selectedTarget.unitName}, ID: {_selectedTarget.unitID}");
              isSelectingTarget = false;
              combatManager.EndTurn();
              actionSelectorController.HideActionSelector();
    }

    private void HighlightTarget(UnitData target)
    {
        //  Добавь визуальный эффект выделения (например, изменение цвета, контур)
        Debug.Log($"Highlighting target: {target.unitName}");
    }
    private void RemoveHighlight(UnitData target)
    {
        //  Удаление визуальный эффект выделения
        Debug.Log($"RemoveHighlight: {target.unitName}");
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
}