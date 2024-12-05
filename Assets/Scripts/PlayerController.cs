using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject rigParent;
    
    [SerializeField]
    private Rigidbody rb;
    
    [SerializeField]
    private WeaponScriptableObject[] weaponDataList;

    private new Collider mainCollider;
    private Rigidbody mainRb;
    private Camera mainCamera;
    private Vector3 vectorStart, vectorEnd, hitVector;
    private WeaponController weapon;
    private Collider[] rigColliderList;
    private Rigidbody[] rigRbList;
    private bool isRagdollEnabled, isHit, isMoving;
    private Quaternion startRotation, localRotation;
    private Vector3 localPosition;
    private Animator animator;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        weapon = gameObject.AddComponent<WeaponController>();
        
        for (int i = 0; i < weaponDataList.Length; i++)
        {
            weaponDataList[i] = Instantiate(weaponDataList[i]);
        }

        rigColliderList = rigParent.GetComponentsInChildren<Collider>();
        rigRbList = rigParent.GetComponentsInChildren<Rigidbody>();
        mainCollider = GetComponent<Collider>();
        mainRb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        startRotation = transform.rotation;
        localRotation = rb.gameObject.transform.localRotation;
        localPosition = rb.gameObject.transform.localPosition;
        
        DisableRagdollMode();
        SwitchWeapon(0);
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
                    EnableRagdollMode();
                    isHit = weapon.Hit(rb, normalVec, hit);
                }
            }
        }

        if (isRagdollEnabled)
        {
            mainRb.position = rb.position;
            mainRb.rotation = rb.rotation;
        }
        
        if (isHit && rb.velocity.magnitude > 0)
        {
            isMoving = true;
        }

        if (isMoving && rb.velocity.magnitude < 0.1f)
        {
            isMoving = false;
            isHit = false;
            DisableRagdollMode();
            animator.SetTrigger("Get Up");
            transform.rotation = startRotation;
            // rb.gameObject.transform.localPosition = localPosition;
            // rb.gameObject.transform.localRotation = localRotation;
            
            // rb.gameObject.transform.localRotation = localRotation;
            // rb.gameObject.transform.localPosition += new Vector3(0, 1f, 0f);
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

    private void DisableRagdollMode()
    {
        Debug.Log("DisableRagdollMode");
        isRagdollEnabled = false;
        
        foreach (Collider col in rigColliderList)
        {
            col.enabled = false;
        }
        foreach (Rigidbody rigBod in rigRbList)
        {
            rigBod.isKinematic = true;
        }
        
        mainCollider.enabled = true;
        mainRb.isKinematic = false;
        animator.enabled = true;
    }
    
    private void EnableRagdollMode()
    {
        Debug.Log("EnableRagdollMode");
        rb.isKinematic = false;
        animator.enabled = false;
        
        foreach (Collider col in rigColliderList)
        {
            col.enabled = true;
        }
        foreach (Rigidbody rigBod in rigRbList)
        {
            rigBod.isKinematic = false;
        }
        
        mainCollider.enabled = false;
        mainRb.isKinematic = true;
        isRagdollEnabled = true;
    }
}
