using UnityEngine;
using TMPro;
using System.Collections;

public class ScrollingText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    private string fullText = "";
    public float defaultScrollSpeed = 20f;
    public int defaultRepetitions = 5;
    private float scrollOffset = 0f;
    private Coroutine scrollCoroutine;
    private int textLength;
    private float textWidth;
    private float previousTextWidth;
    private Vector2 preferredValues = Vector2.zero;

    private void Awake()
    {
        if (textMeshPro == null)
       {
             textMeshPro = GetComponent<TextMeshProUGUI>();
            if (textMeshPro == null)
            {
                Debug.LogError("TextMeshProUGUI component not found!");
             }
       }
       preferredValues = textMeshPro.GetPreferredValues();
    }

    public void SetFullText(string newText, float newSpeed = 0f, int newRepetitions = 0)
    {
         if (string.IsNullOrEmpty(newText))
       {
           Debug.LogWarning("Attempting to set empty text!");
            return;
        }
       fullText = "";
        defaultScrollSpeed = newSpeed > 0 ? newSpeed : defaultScrollSpeed;
        defaultRepetitions = newRepetitions > 0 ? newRepetitions : defaultRepetitions;
        for (int i = 0; i < defaultRepetitions; i++) { fullText += newText + "  "; }
        textLength = fullText.Length;
        textWidth = preferredValues.x;
        StartScrolling();
         previousTextWidth = textMeshPro.rectTransform.rect.width;
    }
   public void StartScrolling()
    {
        if (textMeshPro == null || string.IsNullOrEmpty(fullText))
        {
           Debug.LogError("TextMeshProUGUI or fullText is null or empty!");
           return;
        }
       StopScrolling();
        scrollOffset = 0;
        textMeshPro.text = fullText;
        textMeshPro.ForceMeshUpdate();
        scrollCoroutine = StartCoroutine(ScrollText());
    }
    public void StopScrolling()
    {
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
             scrollCoroutine = null;
           scrollOffset = 0f;
            UpdateText();
        }
   }
    private IEnumerator ScrollText()
    {
         while (true)
        {
            scrollOffset += defaultScrollSpeed * Time.deltaTime;
            UpdateText();
            yield return null;
         }
   }
     private void UpdateText()
    {
        if (string.IsNullOrEmpty(fullText)) return;
         if (previousTextWidth != textMeshPro.rectTransform.rect.width)
       {
           previousTextWidth = textMeshPro.rectTransform.rect.width;
           preferredValues = textMeshPro.GetPreferredValues();
             textWidth = preferredValues.x;
       }
        int visibleChars = Mathf.CeilToInt(textWidth / (textMeshPro.fontSize / 2));
        if (visibleChars <= 0) return;
        int startIndex = Mathf.FloorToInt(scrollOffset) % textLength;

        string visibleText = "";
        for (int i = 0; i < visibleChars; i++)
        {
            int charIndex = (startIndex + i) % textLength;
            visibleText += fullText[charIndex];
        }
       textMeshPro.text = visibleText;
        textMeshPro.ForceMeshUpdate();
    }
}