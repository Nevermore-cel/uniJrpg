using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAnimationTriggersCloseOpen : MonoBehaviour,IInteractable
{
    public Animator anim;
    [SerializeField] private string interactText = "";
    [SerializeField] private string actionText = "";
    [SerializeField] private float interactionRange;
    [SerializeField] private Animator ObjectToTrigger = null;
    [SerializeField] private string TriggerClose = "";
	[SerializeField] private string TriggerOpen = "";

    
    public void Interact(){
        anim.SetTrigger(TriggerOpen);
    }
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
       Interact();
    }

    public Transform GetTransform(){
        return transform;
    }

    
}
