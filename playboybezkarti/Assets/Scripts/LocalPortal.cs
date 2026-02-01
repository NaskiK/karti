using UnityEngine;

public class LocalPortal : MonoBehaviour
{
    [Header("Targeting")]
    public Transform destination; // Where do you want to go?

    [Header("State")]
    public bool isLocked = true;
    public float cooldown = 1.0f;
    private static float lastTeleportTime;

    void Start()
    {
        // Hide the portal visually if it's locked
        GetComponent<SpriteRenderer>().enabled = !isLocked;
        GetComponent<Collider2D>().enabled = !isLocked;
    }

    public void UnlockPortal()
    {
        isLocked = false;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;

        // Optional: Play a sound or particle effect here!
        Debug.Log("The portal has appeared!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLocked) return;

        if (other.CompareTag("Player") && Time.time > lastTeleportTime + cooldown)
        {
            if (destination != null)
            {
                lastTeleportTime = Time.time;
                other.transform.position = destination.position;
            }
        }
    }
}