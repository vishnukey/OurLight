﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{

    const string WOOD_LEADER = "Wood: ";
    const string ARROW_LEADER = "Arrows: ";
    const string TORCH_LEADER = "Torches: ";
    
    public TextMeshProUGUI arrowsText;
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI torchesText;
    public TextMeshProUGUI deathText;
    public Slider healthBar;
    public string deathMessage;

    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        Player.Instance.OnDeath += PlayerDeathCB;
    }

    // Update is called once per frame
    void Update()
    {
        arrowsText.text = $"{WOOD_LEADER}{player.wood}";
        woodText.text = $"{ARROW_LEADER}{player.arrows}/{player.arrowCapacity}";
        torchesText.text = $"{TORCH_LEADER}{player.torches}";
        healthBar.value = (float)player.health / player.max_health;
    }

    void PlayerDeathCB(string reason)
    {
        deathText.text = $"{deathMessage}\n{reason}";
        deathText.gameObject.SetActive(true);
    }


}
