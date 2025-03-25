using UnityEngine;

[CreateAssetMenu(fileName = "New Key Data", menuName = "Key Data")]
public class KeyData : ScriptableObject
{
    public string keyName = "New Key";
    public string description = "A key";
    public string keyColor = "white"; // Теперь это строка
}