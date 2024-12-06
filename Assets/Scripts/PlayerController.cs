using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody handleRigidbody;
    [SerializeField] private float followSpeed = 10f;
    
    [SerializeField]
    private GameObject rigParent, baseballBat;
    
    [SerializeField]
    private Rigidbody rb;
    
    [SerializeField]
    private WeaponScriptableObject[] weaponDataList;

    [SerializeField]
    private ShakyCame shakyCame;

    [SerializeField]
    private AudioClip[] weaponSounds, characterSounds; 

    private new Collider mainCollider;
    private Rigidbody mainRb;
    private Camera mainCamera;
    private Vector3 vectorStart, vectorEnd, hitVector;
    private WeaponController weapon;
    private Collider[] rigColliderList;
    private Rigidbody[] rigRbList;
    public bool isRagdollEnabled, isHit, isMoving;
    private Quaternion startRotation, localRotation;
    private Vector3 localPosition;
    private Animator animator;
    private AudioSource audioSourceCam, audioSource;

    [SerializeField]
    private float maxHoldTime = 5f;
    
    private float holdTime, powerMultiplier, powerPercentage = 1f;
    
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
        audioSource = GetComponent<AudioSource>();
        audioSourceCam = mainCamera!.GetComponent<AudioSource>();
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
        // baseballBat.transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // baseballBat.transform.position = new Vector3(baseballBat.transform.position.x, baseballBat.transform.position.y, -35f);

        if (Input.GetKeyDown(KeyCode.R))
        {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            ResetPosition();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            vectorStart = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            vectorStart.z = 0;

            holdTime = 1f;
        }

        if (Input.GetMouseButton(0))
        {
            holdTime += Time.deltaTime;
            holdTime = Mathf.Clamp(holdTime, 1f, maxHoldTime);
            powerMultiplier = holdTime;
            
            // Calcul de la puissance (optionnel : pour affichage visuel ou UI)
            powerPercentage = holdTime / maxHoldTime;
            Debug.Log($"Puissance : {powerPercentage * 100}%");
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            vectorEnd = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            vectorEnd.z = 0;

            hitVector = vectorEnd - vectorStart;
            if (hitVector.y > 0)
            {
                hitVector.y *= 1.5f;
            }
            // Vector3 normalVec = hitVector.normalized;
            Vector3 normalVec = hitVector;
            normalVec *= 0.05f;
            
            Debug.DrawLine(vectorStart, vectorEnd, Color.red, 1f);
            if (Physics.Raycast(vectorStart, normalVec, out RaycastHit hit, hitVector.magnitude))
            {
                if (hit.collider.CompareTag("Sandbag"))
                {
                    EnableRagdollMode();
                    isHit = weapon.Hit(rb, normalVec * powerMultiplier, hit);
                    if (isHit)
                    {
                        // shakyCame.ShakyCameCustom(1f, 1f);
                        AudioClip hitSound = GetRandomAudioClip(weaponSounds);
                        if (hitSound)
                        {
                            audioSourceCam.volume = 1f;
                            audioSourceCam.PlayOneShot(hitSound);
                        }
                        
                        AudioClip charSound = GetRandomAudioClip(characterSounds);
                        if (charSound && GetRandomChance(66))
                        {
                            audioSource.volume = Random.Range(0.1f, 0.5f);
                            audioSource.PlayOneShot(charSound);
                        }

                        
                        
                    }
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

        if (isMoving && rb.velocity.magnitude < 0.075f)
        {
            ResetPosition();
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
    
    private void FixedUpdate()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 45f; // Profondeur pour la caméra
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(mousePosition);
    
        handleRigidbody.MovePosition(Vector3.Lerp(handleRigidbody.position, targetPosition, Time.fixedDeltaTime * followSpeed));
    }

    private void ResetPosition()
    {
        isMoving = false;
        isHit = false;
        animator.SetTrigger("Get Up");
        DisableRagdollMode();
        transform.rotation = startRotation;
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
        isRagdollEnabled = false;
        animator.enabled = true;
        
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
    }
    
    private void EnableRagdollMode()
    {
        isRagdollEnabled = true;
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
    }

    private static AudioClip GetRandomAudioClip(AudioClip[] array)
    {
        if (array == null || array.Length == 0)
        {
            Debug.LogWarning("Le tableau d'AudioClip est vide ou null");
            return null;
        }
        
        int rand = UnityEngine.Random.Range(0, array.Length);
        return array[rand];
    }
    
    private bool GetRandomChance(int percentage)
    {
        percentage = Math.Clamp(percentage, 0, 100);

        int randomValue = Random.Range(0, 100);

        return randomValue < percentage;
    }
}
