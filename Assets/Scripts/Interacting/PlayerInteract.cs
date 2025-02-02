using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private void Update()
    {   
        if (Input.GetKeyDown(KeyCode.E))
        {
            IInteractable interactable = GetInterractbleObject();
            if (interactable != null)
            {
                interactable.Interact(transform);
            }
        }
    }

    public IInteractable GetInterractbleObject()
    {
        List<IInteractable> interactableList = new List<IInteractable>();

        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 4f); // Радиус поиска (можете поменять, если нужно)

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                // Получаем радиус взаимодействия из объекта
                float interactRange = interactable.GetRangeInteraction();

                // Проверка нахождения объекта в радиусе
                if (Vector3.Distance(transform.position, interactable.GetTransform().position) <= interactRange)
                {
                    interactableList.Add(interactable);
                }
            }
        }

        // Поиск ближайшего объекта интеракции
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

        return closestInteractable;
    }
} 
