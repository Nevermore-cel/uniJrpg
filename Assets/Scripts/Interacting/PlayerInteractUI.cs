using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI interactTextMeshProUGUI;
    [SerializeField] private TextMeshProUGUI actionTextMeshProUGUI;

    private void Update(){
        if (playerInteract.GetInterractbleObject() !=null ) {
            Show(playerInteract.GetInterractbleObject());
            
        } else {
            Hide();
        }
    }
    //показать интерфейс интеракции
    private void Show(IInteractable interactable){
        containerGameObject.SetActive(true);
        actionTextMeshProUGUI.text = interactable.GetActionText();
        interactTextMeshProUGUI.text = interactable.GetInteractText();
        
    }
    //спрятать интерфейс интеракции
    private void Hide(){
        containerGameObject.SetActive(false);
        
    }
}
