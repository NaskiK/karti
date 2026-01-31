using UnityEngine;

public enum UpgradeType
{
    Offence,
    Defence
}

public abstract class Upgrade : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;
    public UpgradeType type;

    public abstract void Apply(GameObject player);
}