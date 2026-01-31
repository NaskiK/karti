using UnityEngine;
using UnityEngine.InputSystem; // Required for Keyboard.current
using TMPro;

public class MovementTutorialUnity6 : MonoBehaviour
{
    public GameObject objectiveTextMovement;
    public GameObject objectiveTextMask;
    public GameObject objectiveText;

    private bool w, a, s, d, ice, fire;
    private bool isCompleteMovement = false;
    private bool isCompleteMask = false;

    // Update runs every single frame
    void Update()
    {
        // 1. Don't do anything if we already finished
        //if (isCompleteMovement) return;

        // 2. Direct Hardware Check (New Input System style)
        // We use Keyboard.current to check the physical keys
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) w = true;
            if (Keyboard.current.sKey.isPressed) s = true;
            if (Keyboard.current.dKey.isPressed) d = true;
            if (Keyboard.current.aKey.isPressed) a = true;
            if (Keyboard.current.digit2Key.isPressed) ice = true;
            if (Keyboard.current.digit1Key.isPressed) fire = true;
        }

        // 3. Check if the "Win Condition" is met
        if (w && a && s && d)
        {
            isCompleteMovement = true;
            objectiveTextMovement.SetActive(false); // Show the "Objective Complete" text!
            Debug.Log("MovementFinished");
            objectiveTextMask.SetActive(true);
        }
        if (fire && ice) { 
            isCompleteMask= true;

            objectiveTextMask.SetActive(false); // Show the "Objective Complete" text!
            Debug.Log("MaskFinished");
        }
    }
}