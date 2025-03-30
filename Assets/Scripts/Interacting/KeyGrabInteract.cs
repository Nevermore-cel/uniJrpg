using UnityEngine;

public class KeyGrabInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText = "Pick Up Key";
    [SerializeField] private string actionText = "Grab";
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] public KeyData keyToGrab;

    public string GetInteractText()
    {
        return interactText;
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
            keyInventory.AddKey(keyToGrab);
            // Сохраняем информацию о подобранном ключе
            SceneData.MarkObjectAsDestroyed(gameObject.scene.name, keyToGrab.keyColor);
            Destroy(gameObject); // Уничтожаем объект ключа после подбора
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