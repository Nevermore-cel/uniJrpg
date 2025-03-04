using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitInterface : MonoBehaviour
{
    private Image healthBar;
    private Image actionPointsBar;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI actionPointsText;
    private UnitData unitData;
    private int previousHealth;
    private int previousActionPoints;
    private Transform uiCanvasTransform;

    void Start()
    {
        unitData = GetComponent<UnitData>();
        if (unitData == null)
        {
            Debug.LogError($"No UnitData component found on {gameObject.name}!");
            enabled = false;
            return;
        }

         uiCanvasTransform = transform.Find("Canvas"); // Get the Transform of the Canvas
        if (uiCanvasTransform == null)
        {
            Debug.LogError($"No Canvas object found on {gameObject.name}!");
           enabled = false;
            return;
       }
       healthBar = uiCanvasTransform.Find("HealthBar")?.GetComponent<Image>();
        actionPointsBar = uiCanvasTransform.Find("ActionPointsBar")?.GetComponent<Image>();
        healthText = uiCanvasTransform.Find("HealthText")?.GetComponent<TextMeshProUGUI>();
         actionPointsText = uiCanvasTransform.Find("ActionPointsText")?.GetComponent<TextMeshProUGUI>();

        if (healthBar == null || actionPointsBar == null || healthText == null || actionPointsText == null)
       {
             Debug.LogError($"Health or Action point bars or texts are missing in UI Prefab, please check the UI configuration for {gameObject.name}!");
            enabled = false;
            return;
        }
        previousHealth = unitData.currentHealth;
        previousActionPoints = unitData.currentActionPoints;

        UpdateUI();
    }
    void LateUpdate()
    {
        if (unitData != null)
        {
             //Check if there are changes in health or action points, and updates only if there is a change.
            if (unitData.currentHealth != previousHealth || unitData.currentActionPoints != previousActionPoints)
            {
                UpdateUI();
                previousHealth = unitData.currentHealth;
                previousActionPoints = unitData.currentActionPoints;
            }
         }

          FaceCamera();
    }
    void UpdateUI()
    {
        // Calculate health bar fill amount
        float healthFill = (float)unitData.currentHealth / unitData.maxHealth;
        healthFill = Mathf.Clamp01(healthFill);

        // Calculate action points bar fill amount
        float actionPointsFill = (float)unitData.currentActionPoints / unitData.maxActionPoints;
        actionPointsFill = Mathf.Clamp01(actionPointsFill);

        // Update the fill amount for the bars, and set the colors.
        healthBar.fillAmount = healthFill;
        healthBar.color = Color.red;
        actionPointsBar.fillAmount = actionPointsFill;
        actionPointsBar.color = Color.yellow;

        //Updates the health and action points text.
        healthText.text = $"{unitData.currentHealth} / {unitData.maxHealth}";
        actionPointsText.text = $"{unitData.currentActionPoints} / {unitData.maxActionPoints}";
    }
    private void FaceCamera()
    {
        if (Camera.main == null || uiCanvasTransform == null) return;
        uiCanvasTransform.forward = Camera.main.transform.forward;
   }
}