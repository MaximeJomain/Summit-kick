using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour
{
    public WeaponScriptableObject weaponData;

    private bool canBeUsed = true;
    
    public bool Hit(Rigidbody sandbagRb, Vector3 normalVec, RaycastHit hitInfo)
    {
        if (canBeUsed)
        {
            Debug.Log("HIT");
            if (weaponData.randDir > 0)
            {
                // Randomise la direction avec une composante aléatoire// Ajuste la force de la randomisation
                normalVec.x += Random.Range(-weaponData.randDir, weaponData.randDir);
                normalVec.y += Random.Range(-weaponData.randDir, weaponData.randDir);
            
                normalVec.Normalize(); // Normalise le vecteur pour conserver la direction principale
            }
            
            float appliedForce = weaponData.hitForce;
            if (weaponData.randFor > 0)
            {
                appliedForce += Random.Range(-weaponData.randFor * weaponData.hitForce, weaponData.randFor * weaponData.hitForce);
            }
            
            // Ajuste la force exercée vers le haut
            if (normalVec.y > 0)
            {
                normalVec.y *= 2;
            }
            
            Debug.DrawLine(hitInfo.point, hitInfo.point + normalVec * weaponData.hitForce, Color.blue, 1f);
            sandbagRb.AddForceAtPosition(normalVec * appliedForce, hitInfo.point, ForceMode.Impulse);
            
            if (weaponData.cooldown > 0) StartCoroutine(WeaponCooldownCoroutine(weaponData));

            return true;
        }

        return false;
    }
    
    private IEnumerator WeaponCooldownCoroutine(WeaponScriptableObject weapon)
    {
        canBeUsed = false;
        yield return new WaitForSeconds(weapon.cooldown);
        canBeUsed = true;
        Debug.Log("weapon.canBeUsed = true");
        yield return null;
    }
}
