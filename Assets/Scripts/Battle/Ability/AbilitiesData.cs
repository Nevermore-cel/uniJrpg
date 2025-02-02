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
    public int range;
    public int cost;
    public int damage;
    public ActionType typeAction;  // Changed from string to enum
    public string description;
}
// enum for type action
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
  bludgeoning,
  wind,
}