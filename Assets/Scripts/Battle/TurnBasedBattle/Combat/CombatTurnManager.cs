using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CombatTurnManager
{
    private CombatManager combatManager;
    private int currentUnitIndex = 0;
    private int currentTeamIndex = 0;
    private int totalTurns = 0;
    private int playerTeamTurns;
    private int enemyTeamTurns;
    public CombatTurnManager(CombatManager manager)
    {
        combatManager = manager;
    }
    public void ResetTurns(int playerTurns, int enemyTurns)
    {
        totalTurns = 1;
        playerTeamTurns = playerTurns;
        enemyTeamTurns = enemyTurns;
        currentUnitIndex = 0;
        currentTeamIndex = 0;
    }
    public UnitData GetCurrentUnit()
    {
        List<UnitData> currentTeam = GetCurrentTeam();

        if (currentTeam.Count == 0)
        {
            Debug.Log($"---{GetCurrentTeamName()} team has no units---");
            return null;
        }
        UnitData currentUnit = null;
        int attempts = 0; // Prevent infinite loop
                while (attempts < 500 &&  (currentUnit == null || !currentUnit.gameObject.activeInHierarchy || !currentUnit.IsActive()))
        {   
                   currentUnit = currentTeam[currentUnitIndex];
                    if (currentUnit == null || !currentUnit.gameObject.activeInHierarchy || !currentUnit.IsActive())
            {
            currentUnitIndex++;
              
            }
               
                if (currentUnitIndex >= currentTeam.Count)
                {
                                currentUnitIndex = 0;
                }
                attempts++;
              if(attempts > 490)
                {
                    Debug.LogError("infinite loop");
                     SwitchToNextTeam();
                     return null;
                }
        }
         if (currentUnit == null || !currentUnit.gameObject.activeInHierarchy || !currentUnit.IsActive())
         {
             return null;
         }

        return currentUnit;
    }

    public void NextTurn(ActionSelectorController actionSelectorController, BattleInterfaceController battleInterfaceController)
    {
        List<UnitData> currentTeam = GetCurrentTeam();

            // Удаляем мертвых юнитов из текущей команды
            currentTeam.RemoveAll(unit => unit == null || !unit.gameObject.activeInHierarchy || !unit.IsActive());

        if (currentTeam.Count == 0)
        {
            Debug.Log($"---{GetCurrentTeamName()} team has no units---");
             SwitchToNextTeam();
            return;
        }
    //  UnitData currentUnit = GetCurrentUnitInternal();

        currentUnitIndex++;
        if (currentUnitIndex >= currentTeam.Count)
        {
            Debug.Log($"---{GetCurrentTeamName()}'s turn has ended---");
            SwitchToNextTeam();
            return;
        }
     NextUnit();

    }
           private void NextUnit()
    {
                 List<UnitData> currentTeam = GetCurrentTeam();

                    if(currentTeam.Count > 0){
                      UnitData currentUnit = GetCurrentUnit();
                            if ( currentUnit != null)
                    {
                        Unit unitComponent = GetCurrentUnit().GetComponent<Unit>();
                        if (unitComponent != null)
                        {
                            unitComponent.SetCanBeSelected(true);
                        }
                        if (combatManager.battleInterfaceController != null)
                        {
                            combatManager.battleInterfaceController.HideBattleInterface();
                        }
                        foreach (UnitData unit in combatManager.GetAllUnits())
                        {
                            Unit unitComponentAll = unit.GetComponent<Unit>();
                            if (unitComponentAll != null)
                            {
                                unitComponentAll.SetCanBeSelected(true);
                            }
                        }
                    }
            totalTurns++; // Увеличиваем счетчик ходов
       //   combatManager.totalTurns++;
        }
    }
        
    private void SwitchToNextTeam()
    {
       currentUnitIndex = 0;
           currentTeamIndex = (currentTeamIndex + 1) % 2;
             if (currentTeamIndex == 0)
            {
                combatManager.playerTeamTurns = combatManager.playerTeam.Count;
            }
            else
            {
                combatManager.enemyTeamTurns = combatManager.enemyTeam.Count;
            }
         combatManager.StartTurn();
    }
    private UnitData GetCurrentUnitInternal()
    {
        return GetCurrentTeam()[currentUnitIndex];
    }
    private void EndCombat(string message)
    {
        Debug.Log(message);
    }
    public string GetCurrentTeamName()
    {
        if (currentTeamIndex == 0)
        {
            return "Player team";
        }
        else
        {
            return "Enemy team";
        }
    }
    public List<UnitData> GetCurrentTeam()
    {
        if (currentTeamIndex == 0)
        {
            return combatManager.playerTeam;
        }
        else
        {
            return combatManager.enemyTeam;
        }
    }
    public int TotalTurns => totalTurns;
}