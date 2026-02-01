using UnityEngine;
using UnityEngine.UI;

public class XPManager : MonoBehaviour
{
    public Image xpFill;
    private PlayerXP playerXP;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerXP = player.GetComponent<PlayerXP>();
        else
            Debug.LogError("Player not found! Make sure it has the Player tag.");
    }

    void Update()
    {
        if (playerXP == null || xpFill == null) return;

        xpFill.fillAmount =
            (float)playerXP.currentXP / playerXP.xpToNextLevel;
    }
}