using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public TargetSelector targetSelector;
    public ActionSelector actionSelector;
    public ActionSelectorController actionSelectorController; // Ссылка на ActionSelectorController
    public Defense defense; // Ссылка на скрипт защиты
    public Attack attack; // Ссылка на скрипт атаки
     private bool isAbilitySelected = false; // Флаг, выбрана ли способность
    private void Update()
    {
        HandleTargetSelection();
        HandleActionSelection();
    }

    private void HandleTargetSelection()
    {
        //  Выбор цели (Врага или союзника)
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            targetSelector.SelectPreviousTarget();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            targetSelector.SelectNextTarget();
        }

     if (Input.GetKeyDown(KeyCode.Return)) // Подтверждение цели (Enter)
     {
          targetSelector.ConfirmTarget();
     }
    }
    private void HandleActionSelection()
    {
        // Выбор действия
        if (Input.GetKeyDown(KeyCode.W)) // Атака
        {
            Debug.Log("Attack");
            attack.OnAttackButtonClicked();
          
        }
        else if (Input.GetKeyDown(KeyCode.E)) // Защита
        {
            defense.OnDefenseButtonClicked();
        }
        else if (Input.GetKeyDown(KeyCode.Q)) // Артефакт
        {
            actionSelector.SelectArtifact();
        }
        else if (Input.GetKeyDown(KeyCode.R)) // Предметы
        {
            actionSelector.SelectItem();
        }
        // Навигация по списку способностей/предметов
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (actionSelectorController.currentActionType == ActionSelectorController.ActionType.Ability)
            {
                actionSelector.SelectPreviousAbility();
            }
            else if (actionSelectorController.currentActionType == ActionSelectorController.ActionType.Item)
            {
                actionSelector.SelectPreviousItem();
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (actionSelectorController.currentActionType == ActionSelectorController.ActionType.Ability)
            {
                actionSelector.SelectNextAbility();
            }
            else if (actionSelectorController.currentActionType == ActionSelectorController.ActionType.Item)
            {
                actionSelector.SelectNextItem();
            }
        }
        if (Input.GetKeyDown(KeyCode.Return)) // Enter - "нажимаем" на выбранную способность
        {
           targetSelector.ConfirmTarget();
        }
    }
    private void UpdateAbilityButtons()
    {
        actionSelector.UpdateAbilitiesAndItems();
    }
}