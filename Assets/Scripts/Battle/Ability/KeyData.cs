using UnityEngine;

[CreateAssetMenu(fileName = "New Key Data", menuName = "Key Data")]
public class KeyData : ScriptableObject
{
    public string keyName = "New Key";
    public string description = "A key";
    public string keyColor = "white"; // Теперь это строка
     private void OnEnable()
    {
        // Выполняем проверку, чтобы убедиться, что keyColor и keyName не совпадают.
        // Это необходимо для того, чтобы избежать конфликтов в системе сохранения/загрузки.
        if (keyColor == keyName)
        {
            Debug.LogError($"KeyColor не должен быть равен KeyName для {name}. Пожалуйста, измените значения в инспекторе.");
        }
    }
}