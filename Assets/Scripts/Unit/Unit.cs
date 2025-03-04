using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public BattleInterfaceController battleInterfaceController;
    private UnitData unitData;
    public int unitId;

    private bool canBeSelected = true;

    private void Start()
    {
        unitData = GetComponent<UnitData>();
        if (unitData == null)
        {
            Debug.LogError("UnitData component not found on this unit!");
        }

        if (battleInterfaceController == null)
        {
            battleInterfaceController = FindObjectOfType<BattleInterfaceController>();
            if (battleInterfaceController == null)
            {
                Debug.LogError("BattleInterfaceController not found in the scene!");
            }
        }
    }
    private void OnMouseDown()
    {
       if (canBeSelected && battleInterfaceController != null && unitData != null)
       {
             battleInterfaceController.ShowBattleInterface(unitId, gameObject);
       }
    }
   public void SetCanBeSelected(bool value){
        canBeSelected = value;
    }
    public void SetAbilities(List<AbilityData> abilities)
    {
       unitData.SetSelectedAbilities(abilities);
    }
}