using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerUpManager : MonoBehaviour
{
    public PongManager PongManager;
    public GameObject playerPowerBarsPanel, opponentPowerBarsPanel, playerPowersPanel, opponentPowersPanel;
    public List<PowerUp> player1PowerUps, player2PowerUps, ownersPowerUps, ownersCurrentlyActivePowersUps;
    public PowerBar playerPowerBar, opponentPowerBar;
    private bool hasLoadedPowerUps, shouldShowInfoPanel = true;
    private PowerUpObject focusPowerUp;
    [SerializeField] private Sprite showIcon, hideIcon;
    [SerializeField] private GameObject powerUpPrefab, powerUpSelectionPrefab;
    [SerializeField] private GameObject powerUpSelectionScreen, selectedPowersPanel, powerDisplayPanel, powerInfoPanel, powerInfoAddButton,
        powerInfoRemoveButton;
    [SerializeField] private TMP_Text headerPowerText, headerCostText, bodyText;
    [SerializeField] private Image toggleShowHideImage;
    public void SetLocalPlayerPowers(List<PowerUp> powerUps, bool isOwner)
    {
        powerUps.Clear();
        if (ownersPowerUps.Count == 6 && isOwner)
            powerUps.AddRange(ownersPowerUps);
        else
            RandomPowerUpAdd(powerUps);
    }
    public void SetOnlinePlayerPowers(List<string> powerUpStrings, List<PowerUp> powerUps)
    {
        foreach (string powerUpString in powerUpStrings)
            powerUps.Add(Enum.Parse<PowerUp>(powerUpString));
    }    
    public List<string> SendOwnerStringData(List<PowerUp> powerUps)
    {
        List<string> returnList = new();
        foreach(PowerUp powerUp in powerUps)
            returnList.Add(powerUp.ToString());
        return returnList;
    }

    public void DamageCharge(int amount, bool playerTookDamage)
    {
        if (playerTookDamage)
        { 
            playerPowerBar.PowerPercentChange(amount * 30, true);
            opponentPowerBar.PowerPercentChange(amount * 10, true);
            return;
        }
        opponentPowerBar.PowerPercentChange(amount * 30, true);
        playerPowerBar.PowerPercentChange(amount * 10, true);

    }

    public void ToggleUI(bool shouldShow)
    {
        playerPowersPanel.SetActive(shouldShow);
        opponentPowersPanel.SetActive(shouldShow);
        playerPowerBarsPanel.SetActive(shouldShow);
        opponentPowerBarsPanel.SetActive(shouldShow);
    }
    public void RandomPowerUpAdd(List<PowerUp> powerUps)
    {
        List<PowerUp> powers = new();
        foreach (PowerUp powerUp in Enum.GetValues(typeof(PowerUp)))
            powers.Add(powerUp);
        powerUps.Clear();
        while (powerUps.Count < 6)
        {
            int x = UnityEngine.Random.Range(0, powers.Count);
            powerUps.Add(powers[x]);
            powers.RemoveAt(x);
        }
    }
    public void SetPowerUps(bool isPlayer)
    {
        if (isPlayer)
            SetPowerUps(playerPowersPanel, player1PowerUps);
        else
            SetPowerUps(opponentPowersPanel, player2PowerUps);
    }

    public void SetPowerUps(GameObject panel, List<PowerUp> powerUps)
    {
        panel.SetActive(true);
        while (panel.transform.childCount < 3)
        {
            if (powerUps.Count == 0)
                break;
            int x = UnityEngine.Random.Range(0, powerUps.Count);
            GameObject g = Instantiate(powerUpPrefab, panel.transform);
            PowerUpObject p = g.GetComponent<PowerUpObject>();
            p.PongManager = PongManager;
            p.PowerUpManager = this;
            p.isPlayer = panel == playerPowersPanel;
            if (!p.isPlayer && PongManager.gameType != GameType.VSLocal)
                g.GetComponent<Button>().enabled = false;
            p.SetPowerUp(powerUps[x]);
            if (p.isPlayer)
                ownersCurrentlyActivePowersUps.Add(powerUps[x]);
            powerUps.RemoveAt(x);
        }
    }
    public void PowerUpSelectionScreen()
    {
        powerUpSelectionScreen.SetActive(true);
        if (hasLoadedPowerUps)
            return;
        foreach (PowerUp powerUp in Enum.GetValues(typeof(PowerUp)))
        {
            GameObject p = Instantiate(powerUpSelectionPrefab, powerDisplayPanel.transform);
            PowerUpObject power = p.GetComponent<PowerUpObject>();
            power.PongManager = PongManager;
            power.PowerUpManager = this;
            power.SetPowerUp(powerUp);
        }
        hasLoadedPowerUps = true;
    }
    public void PowerUpAddOrRemove(PowerUpObject powerUp)
    {
        focusPowerUp = powerUp;
        if (shouldShowInfoPanel)
        {
            powerInfoPanel.SetActive(true);
            headerCostText.text = powerUp.powerBarCost.ToString();
            headerPowerText.text = powerUp.powerUp.ToString();
            bodyText.text = powerUp.powerUpInfo;
            powerInfoAddButton.SetActive(!ownersPowerUps.Contains(powerUp.powerUp));
            powerInfoRemoveButton.SetActive(ownersPowerUps.Contains(powerUp.powerUp));
        }
        else
            AddRemoveToggle();
    }

    public void AddRemoveToggle()
    {
        if (powerInfoPanel.activeInHierarchy)
            powerInfoPanel.SetActive(false);
        if (ownersPowerUps.Contains(focusPowerUp.powerUp))
        {
            ownersPowerUps.Remove(focusPowerUp.powerUp);
            focusPowerUp.transform.SetParent(powerDisplayPanel.transform);
            return;
        }
        if (ownersPowerUps.Count == 6)
            return;
        ownersPowerUps.Add(focusPowerUp.powerUp);
        focusPowerUp.transform.SetParent(selectedPowersPanel.transform);
    }

    //add warning if less than 6
    public void DisablePowerUpSelectionScreen()
    {
        powerUpSelectionScreen.SetActive(false);
    }
    public void ToggleShowHide()
    {
        toggleShowHideImage.sprite = shouldShowInfoPanel ? showIcon : hideIcon;
        shouldShowInfoPanel = !shouldShowInfoPanel;
    }

    public void PowerUpSetup()
    {
        playerPowerBar.SetPowerPercent(0);
        opponentPowerBar.SetPowerPercent(0);
        PongManager.player1Paddle.powerBar = playerPowerBar;
        PongManager.player2Paddle.powerBar = opponentPowerBar;
        if (PongManager.gameType == GameType.VSOnline)
            return;
        SetPowerUps(playerPowersPanel, player1PowerUps);
        SetPowerUps(opponentPowersPanel, player2PowerUps);
    }
}
