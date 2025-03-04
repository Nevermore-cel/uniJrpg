 using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ActionSelectorController : MonoBehaviour
{
    public GameObject actionSelectorInterface;
    public BattleInterfaceController battleInterfaceController;
    public Color selectedColor = Color.yellow;
    public Color defaultColor = Color.white;
    public Button abilityButton;
    public Button itemButton;

    private bool interfaceIsActive = false;
    private enum ActionType { Ability, Item, None }
    private ActionType currentActionType = ActionType.None;
    public int currentUnitID;
    private bool isSelectingTarget = false;
    private AbilityData selectedAbility;
    private ItemData selectedItem;
    private UnitData targetUnit;

    private CombatManager combatManager;
    private List<UnitData> allUnits;

    [Header("Action Panel Settings")]
    public Transform abilityPanel;
    public GameObject abilityInfoPrefab;
    public float verticalSpacing = 30f;
    private AbilityItemCreator _abilityItemCreator;

    void Start()
    {
        _abilityItemCreator = new AbilityItemCreator(
            abilityPanel,
            abilityInfoPrefab,
            verticalSpacing,
            defaultColor,
            selectedColor,
            (prefab, parent) => Instantiate(prefab, parent),
            (obj) => Destroy(obj)
        );

        combatManager = FindObjectOfType<CombatManager>();
        if (combatManager != null)
        {
            allUnits = combatManager.GetAllUnits();
        }
        else
        {
            Debug.LogError("CombatManager is null!");
        }

        actionSelectorInterface.SetActive(false);
        interfaceIsActive = false;
        if (abilityButton == null)
        {
            Debug.LogError("Ability Button not found");
        }
        if (itemButton == null)
        {
            Debug.LogError("Item Button not found");
        }
        abilityButton.onClick.AddListener(ShowAbilities);
        itemButton.onClick.AddListener(ShowItems);
    }

    public void ShowAbilities()
    {
        ResetSelection();
        currentActionType = ActionType.Ability;
        _abilityItemCreator.HideItems();
        ShowActionSelector();
    }

    public void ShowItems()
    {
        ResetSelection();
        currentActionType = ActionType.Item;
        _abilityItemCreator.HideAbilities();
        ShowActionSelector();
    }

    private void ResetSelection()
    {
        isSelectingTarget = false;
        selectedAbility = null;
        selectedItem = null;
        targetUnit = null;
        _abilityItemCreator.ResetSelection();
    }

    public void SetCurrentUnitId(int unitId)
    {
        currentUnitID = unitId;
    }

    public void SetUnitAbilities(int unitId, List<AbilityData> abilities)
    {
        _abilityItemCreator.SetUnitAbilities(abilities, unitId, OnAbilityButtonClicked);
    }
    public void SetUnitItems(int unitId, List<ItemData> items)
    {
        UnitData unitData = GetUnitData(unitId);
        _abilityItemCreator.SetUnitItems(items, unitId, unitData, OnItemButtonClicked);
    }
    public void ShowActionSelector()
    {
        if (!interfaceIsActive && (_abilityItemCreator.HasAbilities() || _abilityItemCreator.HasItems()))
        {
            actionSelectorInterface.SetActive(true);
            interfaceIsActive = true;
            if (currentActionType == ActionType.Ability)
            {
                _abilityItemCreator.ShowAbilitiesInternal();
            }
            else if (currentActionType == ActionType.Item)
            {
                _abilityItemCreator.ShowItemsInternal();
            }
        }
    }
      public void ClearAllAbilityInfo()
    {
        _abilityItemCreator.ClearAllAbilityInfoElements();
    }

    public void HideActionSelector()
    {
        if (interfaceIsActive)
        {
            _abilityItemCreator.HideAbilities();
            _abilityItemCreator.HideItems();
            actionSelectorInterface.SetActive(false);
            interfaceIsActive = false;
            currentActionType = ActionType.None;
            ResetSelection();
        }
    }

    private void OnAbilityButtonClicked(AbilityData ability, GameObject clickedButton)
    {
        _abilityItemCreator.SelectAbility(ability, clickedButton);
        isSelectingTarget = true;
        selectedAbility = ability;
        selectedItem = null;
        Debug.Log($"Ability '{ability.abilityName}' selected, selecting target");
    }

    private void OnItemButtonClicked(ItemData item, GameObject clickedButton)
    {
        _abilityItemCreator.SelectItem(item, clickedButton);
        isSelectingTarget = true;
        selectedItem = item;
        selectedAbility = null;
        Debug.Log($"Item '{item.itemName}' selected, selecting target");
    }

    void Update()
    {
        if (isSelectingTarget && Input.GetMouseButtonDown(0))
        {
            HandleTargetSelection();
            isSelectingTarget = false;
        }
    }

    private void HandleTargetSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject target = hit.collider.gameObject;
            targetUnit = target.GetComponent<UnitData>(); // Get target unit data

            if (targetUnit != null)
            {
                UnitData currentUnit = GetUnitData(currentUnitID); // Get the correct currentUnit

                if (selectedAbility != null)
                {
                    if (currentUnit.CanUseAbility(selectedAbility))
                    {
                        currentUnit.DeductActionPoints(selectedAbility.cost);
                        bool isCrit = currentUnit.CalculateCrit(); // Calculate crit for ability use
                        targetUnit.ApplyAbilityEffect(selectedAbility, isCrit); // Pass isCrit
                        Debug.Log($"Ability '{selectedAbility.abilityName}' Target Unit '{targetUnit.unitName}', ID {targetUnit.unitID}, Type: {selectedAbility.typeAction}");
                         HideActionSelector();
                        combatManager.EndTurn();
                    }
                    else
                    {
                        Debug.LogWarning($"Unit with ID {currentUnitID} has not enough Action Points to use {selectedAbility.abilityName}");
                    }
                }
                else if (selectedItem != null)
                {
                    if (currentUnit != null && currentUnit.gameObject.CompareTag("Companion") && battleInterfaceController != null)
                    {
                        UnitData playerUnit = battleInterfaceController.GetPlayerUnit();
                        if (playerUnit.CanUseItem(selectedItem))
                        {
                            playerUnit.UseItem(selectedItem, currentUnit);
                            bool isCrit = currentUnit.CalculateCrit(); // Calculate crit for item use
                            targetUnit.ApplyItemEffect(selectedItem, targetUnit, isCrit); // Pass isCrit
                            Debug.Log($"Item '{selectedItem.itemName}' Target Unit '{targetUnit.unitName}', ID {targetUnit.unitID}, Type: {selectedItem.typeAction}, used from player's inventory");
                             HideActionSelector();
                           
                        }
                        else
                        {
                            Debug.LogWarning($"Unit with ID {currentUnitID} has not enough  {selectedItem.itemName}");
                        }
                    }
                    else if (currentUnit != null && currentUnit.CanUseItem(selectedItem))
                    {
                        currentUnit.UseItem(selectedItem);
                        bool isCrit = currentUnit.CalculateCrit(); // Calculate crit for item use
                        targetUnit.ApplyItemEffect(selectedItem, targetUnit, isCrit); // Pass isCrit
                        Debug.Log($"Item '{selectedItem.itemName}' Target Unit '{targetUnit.unitName}', ID {targetUnit.unitID}, Type: {selectedItem.typeAction}");
                           HideActionSelector();
                         
                    }
                    else
                    {
                        Debug.LogWarning($"Unit with ID {currentUnitID} has not enough  {selectedItem.itemName}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("Target clicked has no UnitData Component");
            }
        }
        else
        {
            Debug.Log("Clicked on nothing, target selection cancelled");
        }
                if (combatManager.CheckCombatEnd()) // Check if combat ended
               {
                  return; // Exit if combat ended
               }
                   combatManager.EndTurn(); // Guarantee EndTurn() is called
        isSelectingTarget = false;
        selectedAbility = null;
        selectedItem = null;
        targetUnit = null;
    }

    private UnitData GetUnitData(int unitId)
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

    public bool IsInterfaceActive()
    {
        return interfaceIsActive;
    }
}