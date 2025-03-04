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
    public bool isActive = true; // По умолчанию активны
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
    public bool _isCrit; //  Сделано public

    //Event for position changes
    public delegate void PositionChangedHandler(Vector3 newPosition);
    public event PositionChangedHandler OnPositionChanged;
    private Vector3 _previousPosition;

    private UnitInventory inventory;
    private UnitCombatStats combatStats;
    private UnitAbilities abilities;
    private UnitEquipment equipment;

    void Awake()
    {
        inventory = new UnitInventory(this);
        combatStats = new UnitCombatStats(this);
        abilities = new UnitAbilities(this);
        equipment = new UnitEquipment(this);

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

        if (itemQuantities.Count == 0 && itemsData != null)
        {
            inventory.InitializeItemQuantities();
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
    public List<ItemData> GetItems() => inventory.GetItems();
    public int GetItemQuantity(ItemData item) => inventory.GetItemQuantity(item);
    public void SetItemQuantity(ItemData item, int quantity) => inventory.SetItemQuantity(item, quantity);
    public List<AbilityData> GetAbilities() => abilities.GetAbilities();
    public bool CanUseAbility(AbilityData ability) => combatStats.CanUseAbility(ability);
    public bool CanUseItem(ItemData item) => combatStats.CanUseItem(item);
    public void DeductActionPoints(int cost) => combatStats.DeductActionPoints(cost);
    public void UseItem(ItemData item, UnitData playerUnit = null) => inventory.UseItem(item, playerUnit);
    public void ApplyAbilityEffect(AbilityData ability, bool isCrit) => combatStats.ApplyAbilityEffect(ability, isCrit);
    public void ApplyItemEffect(ItemData item, UnitData targetUnit, bool isCrit) => combatStats.ApplyItemEffect(item, targetUnit, isCrit);
    public void TakeDamage(int damage, ActionType damageType, string damageSourceName, bool isCrit) => combatStats.TakeDamage(damage, damageType, damageSourceName, isCrit);
    public void AddDamageReduction(float reductionPercentage) => combatStats.AddDamageReduction(reductionPercentage);
    public void ApplyDamageReduction(float percentage) => combatStats.ApplyDamageReduction(percentage);
    public bool CalculateCrit() => combatStats.CalculateCrit();
    public string GetCritInfo() => combatStats.GetCritInfo();

    public void SetSelectedAbilities(List<AbilityData> newAbilities) => abilities.SetSelectedAbilities(newAbilities);
    public void SetSelectedResistances(List<ResistanceData> newResistances) => equipment.SetSelectedResistances(newResistances);
    public void SetSelectedWeaknesses(List<WeaknessData> newWeaknesses) => equipment.SetSelectedWeaknesses(newWeaknesses);
    public void SetSelectedItems(List<ItemData> newItems) => inventory.SetSelectedItems(newItems);

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