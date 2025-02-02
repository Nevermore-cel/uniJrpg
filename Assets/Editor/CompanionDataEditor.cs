using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(UnitData))]
public class UnitDataEditor : Editor
{
    private SerializedProperty abilitiesDataProp;
    private SerializedProperty selectedAbilitiesIndexesProp;
    private SerializedProperty isActiveProp;
    private SerializedProperty unitNameProp;
    private SerializedProperty maxHealthProp;
    private SerializedProperty currentHealthProp;
    private SerializedProperty maxActionPointsProp;
    private SerializedProperty currentActionPointsProp;
    private SerializedProperty unitIDProp;
    private SerializedProperty startGridPositionProp;
    private SerializedProperty attackDamageProp;
    private SerializedProperty attackTypeProp; // Added
    private SerializedProperty damageReductionPercentageProp;
    private SerializedProperty itemsDataProp;
    private SerializedProperty itemQuantitiesProp;
    private SerializedProperty resistancesProp;
    private SerializedProperty weaknessesProp;


    private void OnEnable()
    {
        abilitiesDataProp = serializedObject.FindProperty("abilitiesData");
        selectedAbilitiesIndexesProp = serializedObject.FindProperty("selectedAbilitiesIndexes");
        isActiveProp = serializedObject.FindProperty("isActive");
        unitNameProp = serializedObject.FindProperty("unitName");
        maxHealthProp = serializedObject.FindProperty("maxHealth");
        currentHealthProp = serializedObject.FindProperty("currentHealth");
        unitIDProp = serializedObject.FindProperty("unitID");
        maxActionPointsProp = serializedObject.FindProperty("maxActionPoints");
        currentActionPointsProp = serializedObject.FindProperty("currentActionPoints");
        startGridPositionProp = serializedObject.FindProperty("startGridPosition");
        attackDamageProp = serializedObject.FindProperty("attackDamage");
        attackTypeProp = serializedObject.FindProperty("attackType"); // Added
        damageReductionPercentageProp = serializedObject.FindProperty("damageReductionPercentage");
        itemsDataProp = serializedObject.FindProperty("itemsData");
        itemQuantitiesProp = serializedObject.FindProperty("itemQuantities");
        resistancesProp = serializedObject.FindProperty("resistances");
        weaknessesProp = serializedObject.FindProperty("weaknesses");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(unitNameProp);
        EditorGUILayout.PropertyField(maxHealthProp);
        EditorGUILayout.PropertyField(currentHealthProp);
        EditorGUILayout.PropertyField(unitIDProp);
        EditorGUILayout.PropertyField(startGridPositionProp);
        EditorGUILayout.PropertyField(attackDamageProp);
        EditorGUILayout.PropertyField(attackTypeProp); // Added
        EditorGUILayout.PropertyField(damageReductionPercentageProp);
        EditorGUILayout.PropertyField(abilitiesDataProp);
        EditorGUILayout.PropertyField(itemsDataProp);

        EditorGUILayout.PropertyField(maxActionPointsProp);
        EditorGUILayout.PropertyField(currentActionPointsProp);
        EditorGUILayout.PropertyField(isActiveProp);

        // Отображение списка сопротивлений
        for (int i = 0; i < resistancesProp.arraySize; i++)
        {
            SerializedProperty elementProp = resistancesProp.GetArrayElementAtIndex(i);
            SerializedProperty resistanceTypeProp = elementProp.FindPropertyRelative("resistanceType");
            SerializedProperty valueProp = elementProp.FindPropertyRelative("value");


            ActionType selectedActionType = (ActionType)resistanceTypeProp.enumValueIndex;
            ActionType newActionType = (ActionType)EditorGUILayout.EnumPopup("Resistance Type", selectedActionType);

            if (newActionType != selectedActionType)
            {
                resistanceTypeProp.enumValueIndex = (int)newActionType;
            }

            valueProp.floatValue = EditorGUILayout.Slider("Resistance Value", valueProp.floatValue, 0f, 1f);
        }
        if (GUILayout.Button("Add Resistance"))
        {
            resistancesProp.arraySize++;
        }
        if (resistancesProp.arraySize > 0 && GUILayout.Button("Remove Resistance"))
        {
            resistancesProp.arraySize--;
        }

        // Отображение списка слабостей
        for (int i = 0; i < weaknessesProp.arraySize; i++)
        {
            SerializedProperty elementProp = weaknessesProp.GetArrayElementAtIndex(i);
            SerializedProperty weaknessTypeProp = elementProp.FindPropertyRelative("weaknessType");
            SerializedProperty valueProp = elementProp.FindPropertyRelative("value");

            ActionType selectedActionType = (ActionType)weaknessTypeProp.enumValueIndex;
            ActionType newActionType = (ActionType)EditorGUILayout.EnumPopup("Weakness Type", selectedActionType);

            if (newActionType != selectedActionType)
            {
                weaknessTypeProp.enumValueIndex = (int)newActionType;
            }
            valueProp.floatValue = EditorGUILayout.Slider("Weakness Value", valueProp.floatValue, 0f, 1f);
        }
        if (GUILayout.Button("Add Weakness"))
        {
            weaknessesProp.arraySize++;
        }
        if (weaknessesProp.arraySize > 0 && GUILayout.Button("Remove Weakness"))
        {
            weaknessesProp.arraySize--;
        }

        // Отображение списка предметов
        for (int i = 0; i < itemQuantitiesProp.arraySize; i++)
        {
            SerializedProperty elementProp = itemQuantitiesProp.GetArrayElementAtIndex(i);
            SerializedProperty itemIndexProp = elementProp.FindPropertyRelative("itemIndex");
            SerializedProperty quantityProp = elementProp.FindPropertyRelative("quantity");


            int selectedIndex = itemIndexProp.intValue;
            int newIndex = EditorGUILayout.Popup("Item", selectedIndex, GetItemNames());

            if (newIndex != selectedIndex)
            {
                itemIndexProp.intValue = newIndex;
            }

            quantityProp.intValue = EditorGUILayout.IntField("Quantity", quantityProp.intValue);
        }

        // Кнопки "Add" и "Remove" для предметов
        if (GUILayout.Button("Add Item"))
        {
            itemQuantitiesProp.arraySize++;
        }
        if (itemQuantitiesProp.arraySize > 0 && GUILayout.Button("Remove Item"))
        {
            itemQuantitiesProp.arraySize--;
        }

        // Отображение списка индексов способностей
        for (int i = 0; i < selectedAbilitiesIndexesProp.arraySize; i++)
        {
            SerializedProperty elementProp = selectedAbilitiesIndexesProp.GetArrayElementAtIndex(i);
            int selectedIndex = elementProp.intValue;

            // Создаем выпадающий список
            int newIndex = EditorGUILayout.Popup("Способность", selectedIndex, GetAbilityNames());
            if (newIndex != selectedIndex)
            {
                elementProp.intValue = newIndex;
            }
        }

        // Кнопки "Add" и "Remove" для способностей
        if (GUILayout.Button("Add Ability"))
        {
            selectedAbilitiesIndexesProp.arraySize++;
        }
        if (selectedAbilitiesIndexesProp.arraySize > 0 && GUILayout.Button("Remove Ability"))
        {
            selectedAbilitiesIndexesProp.arraySize--;
        }

        serializedObject.ApplyModifiedProperties();
    }

    // Метод для получения названий способностей из AbilitiesData
    private string[] GetAbilityNames()
    {
        AbilitiesData abilitiesData = (AbilitiesData)serializedObject.FindProperty("abilitiesData").objectReferenceValue;
        if (abilitiesData != null)
        {
            string[] abilityNames = new string[abilitiesData.abilities.Count];
            for (int i = 0; i < abilitiesData.abilities.Count; i++)
            {
                abilityNames[i] = abilitiesData.abilities[i].abilityName + " - " + abilitiesData.abilities[i].typeAction; // Displaying type action
            }
            return abilityNames;
        }
        return new string[0]; // Возвращаем пустой массив, если abilitiesData null
    }
    // Метод для получения названий предметов из ItemsData
    private string[] GetItemNames()
    {
        ItemsData itemsData = (ItemsData)serializedObject.FindProperty("itemsData").objectReferenceValue;
        if (itemsData != null)
        {
            string[] itemNames = new string[itemsData.items.Count];
            for (int i = 0; i < itemsData.items.Count; i++)
            {
                itemNames[i] = itemsData.items[i].itemName + " - " + itemsData.items[i].typeAction; // Displaying type action
            }
            return itemNames;
        }
        return new string[0];
    }
}