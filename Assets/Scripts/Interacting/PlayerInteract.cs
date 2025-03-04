using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
   private void Update()
{
    if (Input.GetButtonDown("Interact"))
    {
        Debug.Log("Interact button pressed"); // Проверяем, сколько раз нажимается кнопка

        IInteractable interactable = GetInterractbleObject();
        if (interactable != null)
        {
            Debug.Log("Interactable found: " + interactable.GetType().Name); // Узнаем тип объекта
            interactable.Interact(transform);
        }
        else
        {
            Debug.Log("No interactable found");
        }
    }
}

public IInteractable GetInterractbleObject()
{
    List<IInteractable> interactableList = new List<IInteractable>();

    Collider[] colliderArray = Physics.OverlapSphere(transform.position, 4f);

    foreach (Collider collider in colliderArray)
    {
        Debug.Log("Collider found: " + collider.gameObject.name); // Смотрим, какие коллайдеры находятся
        if (collider.TryGetComponent(out IInteractable interactable))
        {
            float interactRange = interactable.GetRangeInteraction();

            if (Vector3.Distance(transform.position, interactable.GetTransform().position) <= interactRange)
            {
                Debug.Log("Added interactable: " + interactable.GetType().Name + " on object: " + collider.gameObject.name);
                interactableList.Add(interactable);
            }
        }
    }

    IInteractable closestInteractable = null;
    foreach (IInteractable interactable in interactableList)
    {
        if (closestInteractable == null)
        {
            closestInteractable = interactable;
        }
        else
        {
            if (Vector3.Distance(transform.position, interactable.GetTransform().position) < Vector3.Distance(transform.position, closestInteractable.GetTransform().position))
            {
                closestInteractable = interactable;
            }
        }
    }

    if (closestInteractable != null)
    {
        Debug.Log("Closest interactable: " + closestInteractable.GetType().Name + " on object: " + closestInteractable.GetTransform().gameObject.name);
    }

    return closestInteractable;
}
}
