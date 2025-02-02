using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

[System.Serializable]
public class UnitData : MonoBehaviour
{
    public string unitName;
    public int maxHealth;
    public int currentHealth;
    public int maxActionPoints;
    public int currentActionPoints;
    public bool isActive;
    public int unitID;
    public int attackDamage;
    public ActionType attackType = ActionType.piercing; // Default attack type
    [SerializeField] public AbilitiesData abilitiesData;
    [SerializeField] public List<int> selectedAbilitiesIndexes;
    [SerializeField] public ItemsData itemsData;
    [SerializeField] public List<ItemQuantity> itemQuantities; // New list to store individual item quantities
      [SerializeField] public List<ResistanceData> resistances = new List<ResistanceData>(); // New List for resistances
     [SerializeField] public List<WeaknessData> weaknesses = new List<WeaknessData>(); // New List for weaknesses
    public Vector2Int startGridPosition;
    public float damageReductionPercentage = 0f; // Добавлено поле для уменьшения урона

    private static HashSet<int> usedIDs = new HashSet<int>();
    private float damageReduction = 0f; // Local damage reduction

    // Event for position changes
    public delegate void PositionChangedHandler(Vector3 newPosition);
    public event PositionChangedHandler OnPositionChanged;

    private Vector3 _previousPosition;

    void Awake()
    {
        Initialize();
    }

    void OnEnable()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            gameObject.tag = "Enemy";
        }
    }

    void Start()
    {
        _previousPosition = transform.position;
        currentHealth = maxHealth;
        currentActionPoints = maxActionPoints;
    }

    void Update()
    {
        if (transform.position != _previousPosition)
        {
            OnPositionChanged?.Invoke(transform.position);
            _previousPosition = transform.position;
        }
    }

    public void Initialize()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            AssignUniqueEnemyID();
        }
        if (itemQuantities.Count == 0 && itemsData != null) // Initialize if empty
        {
            InitializeItemQuantities();
        }
    }

    private void InitializeItemQuantities()
    {
        itemQuantities.Clear(); // Clear old itemQuantities
        if (itemsData != null)
        {
            foreach (var item in itemsData.items)
            {
                itemQuantities.Add(new ItemQuantity { itemIndex = itemsData.items.IndexOf(item), quantity = item.quantity }); // Initial quantity based on ItemsData
            }
        }
        else
        {
            Debug.LogError("ItemsData is null");
        }
    }

    public void SetSelectedAbilities(List<AbilityData> newAbilities)
    {
        selectedAbilitiesIndexes.Clear();
        for (int i = 0; i < newAbilities.Count; i++)
        {
            if (abilitiesData != null && abilitiesData.abilities.Contains(newAbilities[i]))
            {
                selectedAbilitiesIndexes.Add(abilitiesData.abilities.IndexOf(newAbilities[i]));
            }
            else
            {
                Debug.LogWarning("Ability not found in AbilityData");
            }
        }
    }
   public void SetSelectedResistances(List<ResistanceData> newResistances)
    {
        resistances.Clear();
          foreach (var resistance in newResistances)
        {
                 resistances.Add(new ResistanceData { resistanceType = resistance.resistanceType, value = resistance.value });
        }
    }
    public void SetSelectedWeaknesses(List<WeaknessData> newWeaknesses)
    {
         weaknesses.Clear();
        foreach (var weakness in newWeaknesses)
        {
             weaknesses.Add(new WeaknessData { weaknessType = weakness.weaknessType, value = weakness.value });
        }
    }
    public void SetSelectedItems(List<ItemData> newItems)
    {
        itemQuantities.Clear(); // Clear old itemQuantities
        if (itemsData == null)
        {
            Debug.LogWarning("ItemsData is null, can't set items");
            return;
        }
        foreach (var item in newItems)
        {
            if (itemsData != null && itemsData.items.Contains(item))
            {
                itemQuantities.Add(new ItemQuantity { itemIndex = itemsData.items.IndexOf(item), quantity = item.quantity }); //Initial quantity
            }
            else
            {
                Debug.LogWarning($"Item '{item.itemName}' not found in ItemsData");
            }
        }
        selectedAbilitiesIndexes.Clear();

    }

    public List<ItemData> GetItems()
    {
        List<ItemData> selectedItems = new List<ItemData>();
        if (itemsData == null)
        {
            Debug.LogWarning("ItemsData is null, can't get items");
            return selectedItems;
        }
        foreach (var itemQuantity in itemQuantities)
        {
            if (itemQuantity.itemIndex >= 0 && itemQuantity.itemIndex < itemsData.items.Count && itemQuantity.quantity > 0)
            {
                selectedItems.Add(itemsData.items[itemQuantity.itemIndex]);
            }
        }
        return selectedItems;
    }
    public int GetItemQuantity(ItemData item)
    {
        if (itemsData == null || item == null) return 0;
        int itemIndex = itemsData.items.IndexOf(item);
        foreach (var itemQuantity in itemQuantities)
        {
            if (itemQuantity.itemIndex == itemIndex)
            {
                return itemQuantity.quantity;
            }
        }
        return 0; // Return 0 if item is not found
    }
    public void SetItemQuantity(ItemData item, int quantity)
    {
        if (itemsData == null || item == null) return;
        int itemIndex = itemsData.items.IndexOf(item);
        foreach (var itemQuantity in itemQuantities)
        {
            if (itemQuantity.itemIndex == itemIndex)
            {
                itemQuantity.quantity = quantity;
                return;
            }
        }
        // Add new item if doesn't exist
        itemQuantities.Add(new ItemQuantity { itemIndex = itemIndex, quantity = quantity });
    }
    public List<AbilityData> GetAbilities()
    {
        List<AbilityData> selectedAbilities = new List<AbilityData>();
        foreach (int index in selectedAbilitiesIndexes)
        {
            if (index >= 0 && index < abilitiesData.abilities.Count)
            {
                selectedAbilities.Add(abilitiesData.abilities[index]);
            }
        }
        return selectedAbilities;
    }

    private void AssignUniqueEnemyID()
    {
        int newID;
        do
        {
            newID = UnityEngine.Random.Range(1000, 10000);
        } while (usedIDs.Contains(newID));
        unitID = newID;
        usedIDs.Add(newID);
    }

    public bool CanUseAbility(AbilityData ability)
    {
        return currentActionPoints >= ability.cost;
    }
    public bool CanUseItem(ItemData item)
    {
        if (item != null && GetItemQuantity(item) > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DeductActionPoints(int cost)
    {
        if (currentActionPoints >= cost)
        {
            currentActionPoints -= cost;
            Debug.Log($"{unitName} spent {cost} action points. Current AP: {currentActionPoints}");
        }
        else
        {
            Debug.LogWarning($"{unitName} does not have enough action points to use this ability. Current AP: {currentActionPoints}, cost: {cost}");
        }
    }
    public void UseItem(ItemData item, UnitData playerUnit = null)
    {
        if (item != null)
        {
            if (playerUnit != null && gameObject.CompareTag("Companion"))
            {
                if (playerUnit.GetItemQuantity(item) > 0)
                {
                    int quantity = playerUnit.GetItemQuantity(item) - 1;
                    playerUnit.SetItemQuantity(item, quantity);
                    Debug.Log($"{unitName} used {item.itemName}. Remaining quantity: {quantity} on Player's inventory");
                    if (quantity <= 0)
                    {
                        Debug.Log($"{item.itemName} has 0 quantity, item removed from Player's inventory");
                    }
                }
                else
                {
                    Debug.LogWarning($"{unitName} cant use {item.itemName} because quantity is 0");
                }
            }
            else
            {
                if (GetItemQuantity(item) > 0)
                {
                    int quantity = GetItemQuantity(item) - 1;
                    SetItemQuantity(item, quantity);
                    Debug.Log($"{unitName} used {item.itemName}. Remaining quantity: {quantity}");
                    if (quantity <= 0)
                    {
                        Debug.Log($"{item.itemName} has 0 quantity, item removed from inventory");
                    }
                }
                else
                {
                    Debug.LogWarning($"{unitName} cant use {item.itemName} because quantity is {GetItemQuantity(item)}");
                }
            }
        }
        else
        {
            Debug.LogWarning("Item is null, can't use item");
        }
    }
    public void ApplyAbilityEffect(AbilityData ability)
    {
          if (ability == null)
        {
            Debug.LogError("Ability is null. Cannot apply effect.");
            return;
        }
        int damageAmount = ability.damage;
           if (ability.typeAction == ActionType.attack || ability.typeAction == ActionType.fire || ability.typeAction == ActionType.ice || ability.typeAction == ActionType.lightning || ability.typeAction == ActionType.light || ability.typeAction == ActionType.dark || ability.typeAction == ActionType.pure || ability.typeAction == ActionType.piercing || ability.typeAction == ActionType.slashing || ability.typeAction == ActionType.bludgeoning || ability.typeAction == ActionType.wind) // Added wind
        {
            CombatManager combatManager = FindObjectOfType<CombatManager>();
            // Уменьшение урона перед применением
           float modifiedDamage = CalculateDamage(damageAmount,ability.typeAction);
            currentHealth -= Mathf.RoundToInt(modifiedDamage);
            Debug.Log($"{unitName} took {modifiedDamage} damage from '{ability.abilityName}'. Current health: {currentHealth}");
           if (currentHealth <= 0)
            {
               if (combatManager != null)
               {
                    combatManager.RemoveUnit(this); // Удаляем юнита из списков CombatManager
               }
               if (gameObject.CompareTag("Enemy"))
                {
                    Die(); // Call Die function if the unit died and is an enemy.
                   return;
               }
              else
               {
                   currentHealth = 0; // Sets the health to 0 if it is a companion or a player unit
                    Debug.Log($"{unitName} has reached 0 health, but will not die because it is a {gameObject.tag}");
                }
           }
        }
       else if (ability.typeAction == ActionType.heal)
        {
            int healAmount = Mathf.Min(damageAmount, maxHealth - currentHealth);
            currentHealth += healAmount;
            Debug.Log($"{unitName} healed {healAmount} health from '{ability.abilityName}'. Current health: {currentHealth}");
        }
        else
       {
          Debug.LogWarning($"Unknown action type '{ability.typeAction}' for ability '{ability.abilityName}'.");
       }
        // Make sure that the health does not go below 0, or over maxHealth
       currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
     public void ApplyItemEffect(ItemData item, UnitData targetUnit)
    {
         if (item == null)
        {
            Debug.LogError("Item is null. Cannot apply effect.");
            return;
        }
        if (item.typeAction == ActionType.attack || item.typeAction == ActionType.fire || item.typeAction == ActionType.ice || item.typeAction == ActionType.lightning || item.typeAction == ActionType.light || item.typeAction == ActionType.dark || item.typeAction == ActionType.pure || item.typeAction == ActionType.piercing || item.typeAction == ActionType.slashing || item.typeAction == ActionType.bludgeoning || item.typeAction == ActionType.wind) // Added wind
        {
            if (targetUnit != null)
            {
                // Уменьшение урона перед применением
               float modifiedDamage = CalculateDamage(item.damage, item.typeAction);
                targetUnit.currentHealth -= Mathf.RoundToInt(modifiedDamage);
                Debug.Log($"{unitName} attacked {targetUnit.unitName} with {item.itemName} and dealt {modifiedDamage} damage. Current health of target: {targetUnit.currentHealth}");
                if (targetUnit.currentHealth <= 0)
                {
                    CombatManager combatManager = FindObjectOfType<CombatManager>();
                    if (combatManager != null)
                    {
                        combatManager.RemoveUnit(targetUnit);
                    }
                    if (targetUnit.gameObject.CompareTag("Enemy"))
                    {
                        targetUnit.Die();
                    }
                    else
                    {
                        targetUnit.currentHealth = 0; // Sets the health to 0 if it is a companion or a player unit
                        Debug.Log($"{targetUnit.unitName} has reached 0 health, but will not die because it is a {targetUnit.gameObject.tag}");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"{unitName} (ID: {unitID}) found no valid target.");
            }

        }
        else if (item.typeAction == ActionType.heal)
        {
            int healAmount = Mathf.Min(item.damage, maxHealth - currentHealth);
            currentHealth += healAmount;
            Debug.Log($"{unitName} healed {healAmount} health from '{item.itemName}'. Current health: {currentHealth}");
        }
        else
        {
            Debug.LogWarning($"Unknown action type '{item.typeAction}' for item '{item.itemName}'.");
        }
        // Make sure that the health does not go below 0, or over maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
      private float CalculateDamage(int damage, ActionType attackType)
     {
         float modifiedDamage = damage;
        // Check for resistances
         foreach (var resistance in resistances)
        {
              if (resistance.resistanceType == attackType)
            {
               modifiedDamage *= (1f - resistance.value);
                 Debug.Log($"{unitName} has resistance to {attackType}, damage reduced by {resistance.value*100}%");
                break; // Only apply one resistance
            }
        }
          foreach (var weakness in weaknesses)
        {
            if (weakness.weaknessType == attackType)
            {
                 modifiedDamage *= (1f + weakness.value);
               Debug.Log($"{unitName} has weakness to {attackType}, damage increased by {weakness.value*100}%");
              break; // Only apply one weakness
            }
        }
        // Уменьшение урона перед применением
        modifiedDamage *= (1f - damageReduction);

         return modifiedDamage;
     }
     private UnitData FindTarget(UnitData attacker, List<UnitData> aliveUnits)
    {
         List<UnitData> targetTeam;

        if (attacker.gameObject.CompareTag("Player") || attacker.gameObject.CompareTag("Companion"))
        {
            targetTeam = FindObjectOfType<CombatManager>().enemyTeam;
        }
       else if (attacker.gameObject.CompareTag("Enemy"))
       {
            targetTeam = FindObjectOfType<CombatManager>().playerTeam;
        }
        else
        {
            return null; // Или выбросить исключение, если метка не определена.
        }

         if (aliveUnits != null && aliveUnits.Count > 0)
        {
             // Фильтруем список живых юнитов, чтобы убедиться, что они принадлежат к нужной команде для выбора цели
             List<UnitData> validTargets = aliveUnits.Where(unit =>
                (attacker.gameObject.CompareTag("Player") || attacker.gameObject.CompareTag("Companion")) ? targetTeam.Contains(unit) : targetTeam.Contains(unit)
                && unit.currentHealth > 0 // Убедимся, что цель имеет положительное здоровье
            ).ToList();
            if (validTargets.Count > 0)
            {
                return validTargets[UnityEngine.Random.Range(0, validTargets.Count)];
            }
        }
        return null;
    }

    public void AddDamageReduction(float reductionPercentage)
    {
        damageReduction = Mathf.Clamp01(damageReduction + reductionPercentage);
        Debug.Log($"{unitName} damage reduction increased by: '{reductionPercentage * 100}%', total: {damageReduction * 100}%");
    }
    // Новый метод для применения процентов
    public void ApplyDamageReduction(float percentage)
    {
        damageReduction = Mathf.Clamp01(percentage);
        Debug.Log($"{unitName} damage reduction is set to : '{damageReduction * 100}%'");
    }
    private void Die()
    {
        Debug.Log($"{unitName} has died!");
        CombatManager combatManager = FindObjectOfType<CombatManager>();
         if (combatManager != null)
        {
            combatManager.RemoveUnit(this); // Удаляем юнита из списков CombatManager
        }
        Destroy(gameObject); // Уничтожаем GameObject
    }
}
[System.Serializable]
public class ResistanceData
{
    public ActionType resistanceType;
    [Range(0,1)] public float value = 0f;
}
[System.Serializable]
public class WeaknessData
{
  public ActionType weaknessType;
   [Range(0,1)] public float value = 0f;
}

[System.Serializable]
public class ItemQuantity
{
    public int itemIndex;
    public int quantity;
}