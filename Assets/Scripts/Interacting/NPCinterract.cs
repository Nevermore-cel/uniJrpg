using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCinterract : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText;
    [SerializeField] private string actionText;
    [SerializeField] private float interactionRange;

    //текст интерфейса интерации 

    public string GetInteractText(){
        return interactText;
    }

    public string GetActionText(){
        return actionText;
    }
    
    public float GetRangeInteraction(){
        return interactionRange;
    }

    public void Interact(Transform interactorTransform)
    {
       Debug.Log("interact");
    }

    public Transform GetTransform(){
        return transform;
    }

    
}
