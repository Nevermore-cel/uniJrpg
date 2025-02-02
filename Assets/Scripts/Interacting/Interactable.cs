using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {

    //интерфейс-буфер 

    void Interact(Transform interactorTransform);
    string GetInteractText();
    string GetActionText();
    float GetRangeInteraction();
    Transform GetTransform();
}

