using UnityEngine;

[System.Serializable]
public class ResistanceData
{
    public ActionType resistanceType;
    [Range(0, 1)] public float value = 0f;
}