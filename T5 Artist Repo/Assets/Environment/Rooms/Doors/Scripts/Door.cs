using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour
{
    // The door should be unlocked by respective door key if it has any
    [SerializeField] DoorKeys.Key doorKey;
    // The door animator should have DoorAnimator Controller attached
    Animator doorAnimator;
    // Should the door be open as soon as player goes near it?
    [SerializeField] bool triggerBased;
    // Should the door be closed automatically when the player leaves the door range
    [SerializeField] bool shouldCloseAuto;

    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        GetComponent<Collider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(triggerBased)
                if(doorKey == DoorKeys.Key.None /*|| Player has the door key*/)
                    OpenDoor();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!triggerBased /* && Player presses the unlock button*/)
                if (doorKey == DoorKeys.Key.None /*|| Player has the door key*/)
                    OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
                if (shouldCloseAuto)
                    CloseDoor();
        }
    }

    void OpenDoor()
    {
        // Play Animation for Door Open
        // if(!doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        if (!doorAnimator.GetBool("Open"))
                doorAnimator.SetBool("Open", true);
    }

    void CloseDoor()
    {
        // Play Animation for Door Close
        // if (!doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        if (doorAnimator.GetBool("Open"))
            doorAnimator.SetBool("Open", false);
    }

    void LockDoor()
    {
        // Door will be locked if player doesn't have a keycard
        // Locked Door
    }

    void UnlockDoor()
    {
        // Door will be unlocked if player has a keycard
        // Unlock Door
    }
}
