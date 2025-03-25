using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class UnitData : MonoBehaviour
{
    public string unitName;
    public int maxHealth;
    public int currentHealth;
    public int maxActionPoints;
    public int currentActionPoints;
    public bool isActive = true;
    public int unitID;
    public int attackDamage;
    public ActionType attackType = ActionType.piercing;
    [SerializeField] public AbilitiesData abilitiesData;
    [SerializeField] public List<int> selectedAbilitiesIndexes;
    [SerializeField] public ItemsData itemsData;
    [SerializeField] public List<ItemQuantity> itemQuantities;
    [SerializeField] public List<ResistanceData> resistances = new List<ResistanceData>();
    [SerializeField] public List<WeaknessData> weaknesses = new List<WeaknessData>();
    public Vector2Int startGridPosition;
    public float damageReductionPercentage = 0f;

    [Range(0f, 1f)] public float baseCritChance = 0.05f;
    public float critChanceIncreasePerFailedAttempt = 0.02f;
    public float maxCritChance = 0.5f;
    public float currentCritChance;
    public bool _isCrit;

    //Event for position changes
    public delegate void PositionChangedHandler(Vector3 newPosition);
    public event PositionChangedHandler OnPositionChanged;
    private Vector3 _previousPosition;

    private UnitCombatStats combatStats;
    private UnitAbilities abilities;
    private UnitEquipment equipment;

    [Tooltip("Set to player UnitData if this is a companion")]
    public UnitData playerUnitData; // Ссылка на UnitData игрока

    private UnitInventory inventory; // Только для игрока

    void Awake()
    {
        combatStats = new UnitCombatStats(this);
        abilities = new UnitAbilities(this);
        equipment = new UnitEquipment(this);

        // UnitInventory только для игрока
        if (gameObject.CompareTag("Player"))
        {
            inventory = new UnitInventory(this);
        }

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
        currentCritChance = baseCritChance;
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
            equipment.AssignUniqueEnemyID();
        }

        // Inventory initialization только для игрока
        if (gameObject.CompareTag("Player") && itemQuantities.Count == 0 && itemsData != null)
        {
            InitializeItemQuantities();
        }

        if (currentHealth == 0)
        {
            currentHealth = maxHealth;
        }
        if (currentActionPoints == 0)
        {
            currentActionPoints = maxActionPoints;
        }
    }

    public bool IsActive()
    {
        return isActive;
    }

    // Inventory Methods (Only for Player)
    public void InitializeItemQuantities()
    {
        itemQuantities.Clear();
        if (itemsData != null)
        {
            foreach (var item in itemsData.items)
            {
                itemQuantities.Add(new ItemQuantity { itemIndex = itemsData.items.IndexOf(item), quantity = item.quantity });
            }
        }
        else
        {
            Debug.LogError("ItemsData is null");
        }
    }

    public List<ItemData> GetItems()
    {
        if (gameObject.CompareTag("Player"))
        {
            if (itemsData == null)
            {
                Debug.LogWarning("ItemsData is null, can't get items");
                return new List<ItemData>();
            }
            List<ItemData> selectedItems = new List<ItemData>();
            foreach (var itemQuantity in itemQuantities)
            {
                if (itemQuantity.itemIndex >= 0 && itemQuantity.itemIndex < itemsData.items.Count && itemQuantity.quantity > 0)
                {
                    selectedItems.Add(itemsData.items[itemQuantity.itemIndex]);
                }
            }
            return selectedItems;
        }
        else if (playerUnitData != null)
        {
            return playerUnitData.GetItems();
        }
        else
        {
            Debug.LogWarning("No player UnitData is setted");
            return new List<ItemData>();
        }
    }

    public int GetItemQuantity(ItemData item)
    {
      if (item == null) return 0;
         if (gameObject.CompareTag("Player"))
        {
             if (itemsData == null) return 0;
            int itemIndex = itemsData.items.IndexOf(item);
            foreach (var itemQuantity in itemQuantities)
            {
                if (itemQuantity.itemIndex == itemIndex)
                {
                    return itemQuantity.quantity;
                }
            }
            return 0;
        }
        else if (playerUnitData != null && gameObject.CompareTag("Companion"))
        {
           if (playerUnitData.itemsData == null) return 0;
             int itemIndex = playerUnitData.itemsData.items.IndexOf(item);
            foreach (var itemQuantity in playerUnitData.itemQuantities)
            {
                if (itemQuantity.itemIndex == itemIndex)
                {
                    return itemQuantity.quantity;
                }
            }
            return 0;
        }
       
        else
        {
             if (itemsData == null) return 0;
            int itemIndex = itemsData.items.IndexOf(item);
            foreach (var itemQuantity in itemQuantities)
            {
                if (itemQuantity.itemIndex == itemIndex)
                {
                    return itemQuantity.quantity;
                }
            }
            return 0;
        }
    }

    public void SetItemQuantity(ItemData item, int quantity)
    {
        if (gameObject.CompareTag("Player"))
        {
            if (itemsData == null) return;
            int itemIndex = itemsData.items.IndexOf(item);
            foreach (var itemQuantity in itemQuantities)
            {
                if (itemQuantity.itemIndex == itemIndex)
                {
                    itemQuantity.quantity = quantity;
                    return;
                }
            }

            itemQuantities.Add(new ItemQuantity { itemIndex = itemIndex, quantity = quantity });
        }
        else if (playerUnitData != null)
        {
            playerUnitData.SetItemQuantity(item, quantity);
        }
        else
        {
            Debug.LogWarning("No player UnitData setted!");
        }
    }

    public void UseItem(ItemData item, UnitData targetUnit = null)
    {
        if (gameObject.CompareTag("Player"))
        {
            if (GetItemQuantity(item) > 0)
            {
                int quantity = GetItemQuantity(item) - 1;
                SetItemQuantity(item, quantity);
                  
                 if(targetUnit != null)
                    {
                      bool isCrit = CalculateCrit();
                      targetUnit.ApplyItemEffect(item,targetUnit, isCrit);
                      
                       Debug.Log("targetUnit = " + targetUnit);
                    }
                    else
                    {
                         Debug.Log("targetUnit = NULL ");
                    }
            }
            else
            {
                Debug.LogWarning($"{unitName} cant use {item.itemName} because quantity is {GetItemQuantity(item)}");
            }
        }
        else if (playerUnitData != null)
        {
            if (playerUnitData.GetItemQuantity(item) > 0)
            {
                int quantity = playerUnitData.GetItemQuantity(item) - 1;
                playerUnitData.SetItemQuantity(item, quantity);
                   if(targetUnit != null)
                    {
                      bool isCrit = CalculateCrit();
                      targetUnit.ApplyItemEffect(item,targetUnit, isCrit);
                    }
                    else
                    {
                         Debug.Log("targetUnit = NULL ");
                    }
            }
            else
            {
                Debug.LogWarning($"{unitName} cant use {item.itemName} because quantity is 0");
            }
        }
        else
        {
            Debug.LogWarning("No player UnitData setted!");
        }
    }

    public List<AbilityData> GetAbilities() => abilities.GetAbilities();
    public bool CanUseAbility(AbilityData ability) => combatStats.CanUseAbility(ability);
    public bool CanUseItem(ItemData item)
    {
         if (gameObject.CompareTag("Player"))
        {
             if (GetItemQuantity(item) > 0)
            {
                return true;
            }
        }
         else if (playerUnitData != null)
        {
             if (playerUnitData.GetItemQuantity(item) > 0)
            {
                return true;
            }
        }
        return false;
    }
    public void DeductActionPoints(int cost) => combatStats.DeductActionPoints(cost);
    public void ApplyAbilityEffect(AbilityData ability, bool isCrit) => combatStats.ApplyAbilityEffect(ability, isCrit);
    public void TakeDamage(int damage, ActionType damageType, string damageSourceName, bool isCrit) => combatStats.TakeDamage(damage, damageType, damageSourceName, isCrit);
    public void AddDamageReduction(float reductionPercentage) => combatStats.AddDamageReduction(reductionPercentage);
    public void ApplyDamageReduction(float percentage) => combatStats.ApplyDamageReduction(percentage);
    public bool CalculateCrit() => combatStats.CalculateCrit();
    public string GetCritInfo() => combatStats.GetCritInfo();

    public void SetSelectedAbilities(List<AbilityData> newAbilities) => abilities.SetSelectedAbilities(newAbilities);
    public void SetSelectedResistances(List<ResistanceData> newResistances) => equipment.SetSelectedResistances(newResistances);
    public void SetSelectedWeaknesses(List<WeaknessData> newWeaknesses) => equipment.SetSelectedWeaknesses(newWeaknesses);

    public void SetSelectedItems(List<ItemData> newItems)
    {
                if (gameObject.CompareTag("Player"))
        {
              itemQuantities.Clear();
        if (itemsData == null)
        {
            Debug.LogWarning("ItemsData is null, can't set items");
            return;
        }
        foreach (var item in newItems)
        {
            if (itemsData != null && itemsData.items.Contains(item))
            {
                itemQuantities.Add(new ItemQuantity { itemIndex = itemsData.items.IndexOf(item), quantity = item.quantity });
            }
            else
            {
                Debug.LogWarning($"Item '{item.itemName}' not found in ItemsData");
            }
        }
        }
       
    }
       public void ApplyItemEffect(ItemData item, UnitData targetUnit, bool isCrit) => combatStats.ApplyItemEffect(item, targetUnit, isCrit);
    private void Die()
    {
          Debug.Log($"{unitName} has reached 0 health, but will not die because it is a {gameObject.tag}");
         if (gameObject.CompareTag("Enemy"))
        {
            CombatManager combatManager = FindObjectOfType<CombatManager>();
            if (combatManager != null)
            {
                combatManager.RemoveUnit(this);
            }
            Destroy(gameObject);
            return;
        }
        isActive = false;
    }
}