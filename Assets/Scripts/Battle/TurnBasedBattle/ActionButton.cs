using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ScrollingText scrollingText;
    public TextMeshProUGUI actionText;
    public BattleInterfaceController battleInterfaceController;
    private Button button;
    private string defaultActionText;

    void Start()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button component not found!");
        }
        if (scrollingText == null)
       {
            Debug.LogError("ScrollingText not assigned!");
       }
        if (battleInterfaceController == null)
       {
             Debug.LogError("BattleInterfaceController not assigned!");
        }
        if (actionText == null)
        {
           Debug.LogError("actionText not assigned!");
        }
        defaultActionText = actionText.text;

    }
   public void OnPointerEnter(PointerEventData eventData)
    {
       if(battleInterfaceController == null || scrollingText == null || actionText == null || button == null)
        {
             Debug.LogError("BattleInterfaceController, ScrollingText, actionText or Button component is missing");
            return;
        }
        string buttonText = button.GetComponentInChildren<TextMeshProUGUI>().text;

         if (scrollingText != null)
       {
          scrollingText.SetFullText(buttonText, 30f, 2);
        }

       if (actionText != null && battleInterfaceController != null)
      {
           actionText.text = buttonText;
      }
        else{
            Debug.LogError("actionText or battleInterfaceController is missing");
       }
    }
     public void OnPointerExit(PointerEventData eventData)
    {
        if(battleInterfaceController == null || scrollingText == null || actionText == null || button == null)
        {
             Debug.LogError("BattleInterfaceController, ScrollingText, actionText or Button component is missing");
            return;
        }
        if (scrollingText != null)
       {
             scrollingText.StopScrolling();
        }
       if (actionText != null && battleInterfaceController != null)
      {
          actionText.text = defaultActionText;
        }
       else{
            Debug.LogError("actionText or battleInterfaceController is missing");
        }
    }
}