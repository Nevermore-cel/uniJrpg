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
    private UnitData _selectedTarget;

    void Start()
    {
        combatManager = FindObjectOfType<CombatManager>();
        actionSelectorController = FindObjectOfType<ActionSelectorController>();
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
        if (combatManager != null && actionSelectorController != null)
        {
            isSelectingTarget = true;
            Debug.Log($"Attack selected, selecting target");
            foreach (Unit unit in FindObjectsOfType<Unit>())
            {
                unit.SetCanBeSelected(false);
            }
        }
        else
        {
            Debug.LogError("CombatManager or ActionSelectorController is null");
        }
    }
    private void Update()
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
            _selectedTarget = target.GetComponent<UnitData>();
            if (_selectedTarget != null)
            {
                // Get current Unit
                currentUnit = GetCurrentUnitData(actionSelectorController.currentUnitID);
                if (currentUnit != null)
                {
                    // Apply damage
                    ApplyDamageToTarget();
                    // End the turn, передавая цель
                    EndTurn(_selectedTarget);
                }
                else
                {
                    CancelAttack();
                    Debug.LogWarning($"Unit with ID {actionSelectorController.currentUnitID} not found!");
                }
            }
            else
            {
                CancelAttack();
                Debug.LogWarning("Target clicked has no UnitData Component");
            }
        }
        else
        {
            CancelAttack();
            Debug.Log("Clicked on nothing, target selection cancelled");
        }
        isSelectingTarget = false;
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.SetCanBeSelected(true);
        }
    }
    private void ApplyDamageToTarget()
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
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.SetCanBeSelected(true);
        }
    }
}