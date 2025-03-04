using UnityEngine;

[System.Serializable]
public class WeaknessData
{
    public ActionType weaknessType;
    [Range(0, 1)] public float value = 0f;
}