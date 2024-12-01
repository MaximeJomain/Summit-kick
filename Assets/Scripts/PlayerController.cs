using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 vectorStart, vectorEnd, hitVector;
    private Quaternion startRotation;
    private GameObject sandbag;
    private WeaponController weapon;
    
    [SerializeField]
    private Rigidbody sandbagRB;
    
    [SerializeField]
    private WeaponScriptableObject[] weaponDataList;

    private void Awake()
    {
        mainCamera = Camera.main;
        sandbag = GameObject.Find("Sandbag");
        weapon = gameObject.AddComponent<WeaponController>();
        
        for (int i = 0; i < weaponDataList.Length; i++)
        {
            weaponDataList[i] = Instantiate(weaponDataList[i]); // Crée une instance unique pour cette session
        }
    }

    private void Start()
    {
        SwitchWeapon(0);
        startRotation = sandbag.transform.rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            vectorStart = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            vectorStart.z = 0;
        }
        if (Input.GetMouseButtonUp(0))
        {
            vectorEnd = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            vectorEnd.z = 0;

            hitVector = vectorEnd - vectorStart;
            Vector3 normalVec = hitVector.normalized;
            Debug.DrawLine(vectorStart, vectorEnd, Color.red, 1f);
            if (Physics.Raycast(vectorStart, normalVec, out RaycastHit hit, hitVector.magnitude))
            {
                if (hit.collider.CompareTag("Sandbag"))
                {
                    weapon.Hit(sandbagRB, normalVec, hit);
                }
            }
        }

        if (sandbagRB.velocity.magnitude == 0f)
        {
            sandbag.transform.rotation = startRotation;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);

        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(3);
        }
    }

    private void SwitchWeapon(int index)
    {
        if (index >= 0 && index < weaponDataList.Length)
        {
            weapon.weaponData = weaponDataList[index];
            Debug.Log("selectedWeapon " + weapon.weaponData.name);
        }
        else
        {
            Debug.Log("SwitchWeapon index does not exist " + index);
        }
    }
}
