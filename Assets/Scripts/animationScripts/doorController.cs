using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;

    private void Start()
    {
        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
            if(doorAnimator == null)
            {
                Debug.LogError("No Animator component found on the door!");
            }
        }
    }

    public void OpenDoor(string animationBoolName)
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool(animationBoolName, true);
        }
        else
        {
            Debug.LogError("Door Animator is not assigned!");
        }
    }

      public void CloseDoor(string animationBoolName)
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool(animationBoolName, false);
        }
        else
        {
            Debug.LogError("Door Animator is not assigned!");
        }
    }
}