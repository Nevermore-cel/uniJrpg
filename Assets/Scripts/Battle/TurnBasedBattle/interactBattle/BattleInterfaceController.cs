using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleInterfaceController : MonoBehaviour
{
    public GameObject battleInterface;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI actionText2;
    public ScrollingText scrollingText;
    public TextMeshProUGUI actionText;
    public string defaultActionText = "Выберите действие";
    public ActionSelectorController actionSelectorController;
    private CombatManager combatManager;
    public Attack attack;
    private void Start()
    {
        HideBattleInterface();
        actionText.text = defaultActionText;
        combatManager = FindObjectOfType<CombatManager>();
    }

    public void ShowBattleInterface(int unitId, GameObject unit)
    {
        Debug.Log("ShowBattleInterface called!");
        if (battleInterface != null && unit != null)
        {
            Debug.Log("battleInterface and unit are NOT null");
            battleInterface.SetActive(true);
            //Retrieve data BEFORE updating the interface
            if (actionSelectorController != null)
            {
                UnitData unitData = unit.GetComponent<UnitData>();
                if (unitData != null)
                {
                    List<AbilityData> abilities = unitData.GetAbilities();
                     List<ItemData> items;
                    if(unit.CompareTag("Companion"))
                    {
                       UnitData playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitData>();
                       if(playerUnit != null){
                             items = playerUnit.GetItems();
                       }
                        else
                        {
                            Debug.LogError("PlayerUnit is null in BattleInterfaceController!");
                             items = new List<ItemData>();
                         }
                    }
                   else
                   {
                      items = unitData.GetItems();
                    }
                    actionSelectorController.SetUnitAbilities(unitId, abilities);
                     actionSelectorController.SetUnitItems(unitId, items);

                    if (scrollingText != null)
                    {
                        scrollingText.SetFullText("Действие");
                        scrollingText.StartScrolling();
                    }
                }
                else
                {
                    Debug.LogError("UnitData component not found on selected unit!");
                }
            }
            else
            {
                Debug.LogError("ActionSelectorController is not assigned!");
            }
            UpdateInterface(unit);
            if (attack != null)
            {
                attack.CancelAttack();
            }
        }
        else
        {
            Debug.LogError("battleInterface or unit is null!");
        }
    }

    public void HideBattleInterface()
    {
        if (battleInterface != null)
        {
            battleInterface.SetActive(false);
            actionText.text = defaultActionText;
            scrollingText?.StopScrolling();
        }
    }

    private void UpdateInterface(GameObject unit)
    {
        if (unit == null)
        {
            Debug.LogError("Unit GameObject is null in UpdateInterface!");
            return;
        }

        UnitData unitStats = unit.GetComponent<UnitData>();
        if (unitStats == null)
        {
            Debug.LogError("UnitData component not found on selected unit!");
            HideBattleInterface();
        }
        if (actionText2 != null)
        {
            actionText2.text = actionText.text;
        }
    }
    public void OnNextTurnClicked()
    {
        if (combatManager != null)
        {
            List<UnitData> currentTeam = combatManager.GetCurrentTeam();
            int currentUnitIndex = currentTeam.FindIndex(unit => unit != null && unit.unitID == actionSelectorController.currentUnitID);

            if (currentUnitIndex != -1)
            {
                int nextUnitIndex = (currentUnitIndex + 1) % currentTeam.Count;
                UnitData nextUnit = currentTeam[nextUnitIndex];

                if (nextUnit != null)
                {
                    ShowBattleInterface(nextUnit.unitID, nextUnit.gameObject);
                    Debug.Log("Button Next Turn Was Clicked - next unit is " + nextUnit.unitName);
                }
                else
                {
                    Debug.LogWarning("nextUnit  is null, check this");
                }
            }
            else
            {
                Debug.LogWarning("currentUnit  is not found, check this");
            }
        }
        else
        {
            Debug.LogError("CombatManager is null!");
        }
    }
}