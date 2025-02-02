using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TriggerObjectWorld : MonoBehaviour

{
	public Animator anim;
	[SerializeField] private Animator ObjectToTrigger = null;

	[SerializeField] private string Trigger = "";
	[SerializeField] private string TriggerExit = "";

    private void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag("Player")){
			anim.SetTrigger(Trigger);
		}
	}

	private void OnTriggerExit (Collider other)
	{
		if (other.CompareTag("Player")){
			anim.SetTrigger(TriggerExit);
		}
	}

	// public void AcceptExit (){
	// 	SceneManager.LoadScene(nextScene);
	// }
}
