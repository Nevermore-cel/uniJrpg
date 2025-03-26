using UnityEngine;
using System.Collections.Generic;

public class SceneDoorManager : MonoBehaviour
{
    private DoorController[] doors; // Массив всех дверей на сцене

    void Start()
    {
        doors = FindObjectsOfType<DoorController>();
        LoadDoorStates();
    }

    private void LoadDoorStates()
    {
        string sceneName = gameObject.scene.name;
        if (SceneData.DoorStates.ContainsKey(sceneName))
        {
            Dictionary<string, bool> doorStates = SceneData.DoorStates[sceneName];
            foreach (var door in doors)
            {
                if (doorStates.ContainsKey(door.gameObject.name))
                {
                    // Устанавливаем состояние двери из SceneData
                    if (doorStates[door.gameObject.name])
                    {
                        door.OpenDoor("doorOpen"); // Убедитесь, что "doorOpen" - это название параметра в аниматоре
                    }
                    else
                    {
                        door.CloseDoor("doorOpen");
                    }
                }
            }
        }
    }

     void OnDestroy()
    {
        SaveDoorStates();
    }
    private void SaveDoorStates()
    {
        string sceneName = gameObject.scene.name;
        Dictionary<string, bool> doorStates = new Dictionary<string, bool>();

        // Записываем состояние каждой двери в словарь
        foreach (var door in doors)
        {
            //Проверка состояния аниматора здесь
            Animator animator = door.GetComponent<Animator>();
             if (animator != null)
            {
                bool isOpen = animator.GetBool("doorOpen");
                doorStates[door.gameObject.name] = isOpen;
            }
            else{
                Debug.LogWarning($"No Animator on Door: {door.gameObject.name} saving its state");
                doorStates[door.gameObject.name] = false;
            }
           
        }

        SceneData.DoorStates[sceneName] = doorStates;
    }
}