using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Abilities Data", menuName = "Abilities Data")]
public class AbilitiesData : ScriptableObject
{
    public List<AbilityData> abilities = new List<AbilityData>();
}

[System.Serializable]
public class AbilityData
{
    public string abilityName;
    public int rangeX;
    public int rangeY; 
    public int cost;
    public int damage;
    public ActionType typeAction;
    public string description;
}

public enum ActionType
{
  attack,
  heal,
  fire,
  ice,
  lightning,
  light,
  dark,
  pure,
  piercing,
  slashing,
  wind,
}