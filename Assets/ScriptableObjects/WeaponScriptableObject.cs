using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponScriptableObject", order = 1)]
public class WeaponScriptableObject : ScriptableObject
{
    public float hitForce;
    public float cooldown;
    public bool canBeUsed;
    public float randDir;
    public float randFor;

    private void OnEnable()
    {
        canBeUsed = true;
    }
}
