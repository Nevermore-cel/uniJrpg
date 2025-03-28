using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButtonInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText = "Open Door";
    [SerializeField] private string closeInteractText = "Close Door"; // Text when door is open
    [SerializeField] private string actionText = "Press";
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private DoorController doorToOpen; // Ссылка на скрипт двери
    [SerializeField] private KeyData requiredKey; // Ключ для открытия двери
    [SerializeField] private string animationBoolName = "doorOpen"; // Название параметра в аниматоре двери

    public bool _isDoorOpen = false;
    
    public string GetInteractText()
    {
        return _isDoorOpen ? closeInteractText : interactText; // Change interaction text based on door state
    }

    public string GetActionText()
    {
        return actionText;
    }

    public float GetRangeInteraction()
    {
        return interactionRange;
    }

    public void Interact(Transform interactorTransform)
    {
        KeyInventory keyInventory = interactorTransform.GetComponent<KeyInventory>();
        if (keyInventory != null)
        {
            if (requiredKey == null || keyInventory.HasKey(requiredKey))
            {
                if (doorToOpen != null)
                {
                    if (_isDoorOpen)
                    {
                        doorToOpen.CloseDoor(animationBoolName);
                    }
                    else
                    {
                        doorToOpen.OpenDoor(animationBoolName);
                    }
                     _isDoorOpen = !_isDoorOpen;
                }
                else
                {
                    Debug.LogError("No door assigned to the button!");
                }
            }
            else
            {
                Debug.Log("You need a key to open this door!");
            }
        }
        else
        {
            Debug.LogWarning("No KeyInventory component found on the interactor!");
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}