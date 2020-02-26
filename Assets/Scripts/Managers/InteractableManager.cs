﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using TMPro;

public class InteractableManager : Singleton<InteractableManager>
{
    public delegate bool OnWeaponBoostActivateCheck();
    public static event OnWeaponBoostActivateCheck OnWeaponBoostActivateCheckEvent;

    public delegate bool OnShieldActiveCheck();
    public static event OnShieldActiveCheck OnShieldActiveCheckEvent;

    public delegate void OnShieldActivate();
    public static event OnShieldActivate OnShieldActivateEvent;

    public delegate int OnTeamPlayerCheck();
    public static event OnTeamPlayerCheck GetPlayerTeamEvent;


    [SerializeField] private Transform playerTransform;
    private void Start()
    {
        //bombText.SetText(bomb.ToString());
        //stunText.SetText(stun.ToString());
        //shieldText.SetText(shield.ToString());
    }

    [SerializeField] private int bomb;
    public int Bomb
    {
        get { return bomb; }
        set { bomb = value; if(bombText != null) bombText.SetText(bomb.ToString()); }
    }

    [SerializeField] private int stun;
    public int Stun
    {
        get { return stun; }
        set { stun = value; if(stunText != null) stunText.SetText(stun.ToString()); }
    }

    [SerializeField] private int shield;
    public int Shield
    {
        get { return shield; }
        set { shield = value; if(shieldText != null) shieldText.SetText(shield.ToString()); }
    }

    [SerializeField] private TextMeshProUGUI bombText;
    [SerializeField] private TextMeshProUGUI stunText;
    [SerializeField] private TextMeshProUGUI shieldText;

    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject stunPrefab;


    private void AddWeapon(int index)
    {
        //If there player has Weapon boost => Receives 2 random weapon. If player has weapon boost and its active reveice all three weapons
        switch (index)
        {
            case 0:
                Bomb++;
                //Debug.Log("Got bomb!");
                break;
            case 1:
                Stun++;
                //Debug.Log("Got Stun Projectile!");
                break;
            case 2:
                Shield++;
                //Debug.Log("Got Shield!");
                break;
            default:
                break;
        }
    }

    public void AddInteractable(int index, int amount)
    {
        //Adds Random First Weapon
        AddWeapon(index);
        //If player has weapon boost actiuvated => Add Random Second Weapon

        if ((bool)OnWeaponBoostActivateCheckEvent?.Invoke())
        {             
            AddWeapon(Random.Range(0, 3));
        }
        
    }

    /// <summary>
    /// Assigned in the inspector in Interactable UI on Button Click
    /// </summary>
    /// <param name="index"></param>
    public void OnInteractableItemClicked(int index)
    {
        switch (index)
        {
            case 0:
                if (Bomb > 0)
                {
                    //Retrieve bomb from pooler
                    Bomb--;
                    GameObject bombGO = Instantiate(bombPrefab, playerTransform.position, Quaternion.identity);
                    bombGO.SetActive(true);
                }
                break;
            case 1:
                if (Stun > 0)
                {
                    Stun--;
                    GameObject stunGO = Instantiate(stunPrefab, playerTransform.position, Quaternion.identity);
                    int tempTeam = (int)GetPlayerTeamEvent?.Invoke();

                    for (int i = 0; i < 4; i++)
                    {
                        Debug.LogWarning("Dodat argument Vector2.zero");
                        stunGO.transform.GetChild(i).GetComponent<Projectile>().SetUpProjectile(tempTeam, Vector2.zero);
                    }
                    stunGO.SetActive(true);


                }
                break;
            case 2:
                if (Shield > 0)
                {
                    if (OnShieldActiveCheckEvent != null)
                    {
                        if (!OnShieldActiveCheckEvent.Invoke())
                        {
                            OnShieldActivateEvent?.Invoke();
                            Shield--;
                        }
                    }

                    //Shield--;
                }
                break;
            default:
                break;
        }
    }
}
