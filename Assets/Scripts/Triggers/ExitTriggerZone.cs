using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ExitTriggerZone : MonoBehaviour
{
    [SerializeField] private string nextScene = "";
 private void OnTriggerEnter (Collider other)
	{
        
		if (other.CompareTag("Player")){
			SceneManager.LoadScene(nextScene);
		}
	}
}
