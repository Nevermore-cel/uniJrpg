using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ActionSelector : MonoBehaviour
{
    public int currentAbilityIndex = 0;
    public int currentItemIndex = 0;
    public List<AbilityData> availableAbilities;
    public List<ItemData> availableItems;
    private ActionSelectorController actionSelectorController;
    private UnitData currentUnit;
    private CombatManager combatManager;
    public TargetSelector targetSelector;
    private bool _isSelectingAbility;

    void Start()
    {
        actionSelectorController = FindObjectOfType<ActionSelectorController>();
        combatManager = FindObjectOfType<CombatManager>();
        targetSelector = FindObjectOfType<TargetSelector>();
        if (combatManager != null)
        {
            combatManager.StartTurn();
        }
        else
        {
            Debug.LogError("CombatManager is null!");
        }
        if (actionSelectorController == null)
        {
            Debug.LogError("ActionSelectorController is null!");
        }
        if (targetSelector == null)
        {
            Debug.LogError("TargetSelector is null!");
        }
    }

    public void UpdateAbilitiesAndItems()
    {
        if (actionSelectorController != null)
        {
            currentUnit = GetCurrentUnitData(actionSelectorController.currentUnitID);
            if (currentUnit != null)
            {
                availableAbilities = currentUnit.GetAbilities();
                availableItems = currentUnit.GetItems();
            }
        }
        else
        {
            Debug.LogError("ActionSelectorController is null!");
        }
    }
    public void SelectNextAbility()
    {
        if (availableAbilities == null || availableAbilities.Count == 0) return;
        currentAbilityIndex = (currentAbilityIndex + 1) % availableAbilities.Count;
        actionSelectorController.HighlightSelectedAbility();
        Debug.Log($"Выбрана способность: {availableAbilities[currentAbilityIndex].abilityName}");
    }
    public void SelectPreviousAbility()
    {
        if (availableAbilities == null || availableAbilities.Count == 0) return;
        currentAbilityIndex--;
        if (currentAbilityIndex < 0)
        {
            currentAbilityIndex = availableAbilities.Count - 1;
        }
        actionSelectorController.HighlightSelectedAbility();
        Debug.Log($"Выбрана способность: {availableAbilities[currentAbilityIndex].abilityName}");
    }
    public void SelectNextItem()
    {
        if (availableItems == null || availableItems.Count == 0) return;
        currentItemIndex = (currentItemIndex + 1) % availableItems.Count;
        actionSelectorController.HighlightSelectedItem();
        Debug.Log($"Выбран предмет: {availableItems[currentItemIndex].itemName}");
    }

    public void SelectPreviousItem()
    {
        if (availableItems == null || availableItems.Count == 0) return;
        currentItemIndex--;
        if (currentItemIndex < 0)
        {
            currentItemIndex = availableItems.Count - 1;
        }
        actionSelectorController.HighlightSelectedItem();
        Debug.Log($"Выбран предмет: {availableItems[currentItemIndex].itemName}");
    }
    public void SelectArtifact()
    {
        Debug.Log("Artifact selected");
        actionSelectorController.ShowAbilities();
        UpdateAbilitiesAndItems();
        _isSelectingAbility = true;
    }
    public void SelectItem()
    {
        Debug.Log("Item selected");
        actionSelectorController.ShowItems();
        UpdateAbilitiesAndItems();
        _isSelectingAbility = true;
    }
     public ItemData GetSelectedItem()
    {
        if (availableItems != null && currentItemIndex >= 0 && currentItemIndex < availableItems.Count)
        {
            return availableItems[currentItemIndex];
        }
        return null;
    }
    public void TriggerAbilityButton()
    {
        if (targetSelector != null)
        {
            targetSelector.InitializeTargets(true);
            if (actionSelectorController.currentActionType == ActionSelectorController.ActionType.Ability)
            {
                if (availableAbilities != null && availableAbilities.Count > 0)
                {
                    if (currentAbilityIndex < availableAbilities.Count)
                    {
                        targetSelector.selectedAbility = availableAbilities[currentAbilityIndex];
                        targetSelector.selectedItem = null;
                        targetSelector.isSelectingTarget = true;
                        Debug.Log($"Передаем способность: {availableAbilities[currentAbilityIndex].abilityName}");
                    }
                    else
                    {
                        Debug.LogError("Такой способности нет");
                    }
                }
            }
            else if (actionSelectorController.currentActionType == ActionSelectorController.ActionType.Item)
            {
                if (availableItems != null && availableItems.Count > 0)
                {
                     if (currentItemIndex < availableItems.Count)
                    {
                        targetSelector.selectedItem = availableItems[currentItemIndex];
                        targetSelector.selectedAbility = null;
                        targetSelector.isSelectingTarget = true;
                        Debug.Log($"Передаем предмет: {availableItems[currentItemIndex].itemName}");
                    }
                    else
                    {
                        Debug.LogError("Такого предмета нет");
                    }
                }
            }
           
        }
        else
        {
            Debug.LogError("TargetSelector is null!");
        }
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