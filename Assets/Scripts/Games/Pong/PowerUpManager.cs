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
    public List<PowerUpData> powerUpsList;
    public PowerBar playerPowerBar, opponentPowerBar;

    private bool hasLoadedPowerUps, shouldShowInfoPanel = true;
    private PowerUpObject focusPowerUp;
    [SerializeField] private Sprite showIcon, hideIcon;
    [SerializeField] private GameObject powerUpPrefab, powerUpSelectionPrefab, lockedPrefab;
    [SerializeField] private GameObject powerUpSelectionScreen, selectedPowersPanel, powerDisplayPanel, powerInfoPanel, powerInfoAddButton,
        powerInfoRemoveButton;
    [SerializeField] private TMP_Text headerPowerText, headerCostText, bodyText;
    [SerializeField] private Image toggleShowHideImage, powerUpInfoImage;
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
        foreach (PowerUpData powerUp in powerUpsList)
            if (!powerUp.IsLocked)
                powers.Add(powerUp.PowerUpID);
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
            int powerUpIndex = UnityEngine.Random.Range(0, powerUps.Count);
            GameObject spawnedPowerUp = Instantiate(powerUpPrefab, panel.transform);
            PowerUpObject powerUp = spawnedPowerUp.GetComponent<PowerUpObject>();
            powerUp.PongManager = PongManager;
            powerUp.PowerUpManager = this;
            powerUp.isPlayer = panel == playerPowersPanel;
            if (!powerUp.isPlayer && PongManager.gameType != GameType.VSLocal)
                spawnedPowerUp.GetComponent<Button>().enabled = false;
            powerUp.SetPowerUp(powerUpsList.Find(p => p.PowerUpID == powerUps[powerUpIndex]));
            if (powerUp.isPlayer)
                ownersCurrentlyActivePowersUps.Add(powerUps[powerUpIndex]);
            powerUps.RemoveAt(powerUpIndex);
        }
    }
    public void PowerUpSelectionScreen()
    {
        powerUpSelectionScreen.SetActive(true);
        if (hasLoadedPowerUps)
            return;
        foreach (PowerUpData powerUp in powerUpsList)
        {
            GameObject p = Instantiate(powerUpSelectionPrefab, powerDisplayPanel.transform);
            PowerUpObject power = p.GetComponent<PowerUpObject>();
            power.PongManager = PongManager;
            power.PowerUpManager = this;
            power.SetPowerUp(powerUp);
            if (powerUp.IsLocked)
                Instantiate(lockedPrefab, p.transform);
        }
        hasLoadedPowerUps = true;
    }
    public void PowerUpAddOrRemove(PowerUpObject powerUp)
    {
        //add warning????
        if (powerUp.isLocked)
            return;
        focusPowerUp = powerUp;
        if (shouldShowInfoPanel)
        {
            powerInfoPanel.SetActive(true);
            headerCostText.text = powerUp.PowerBarCost.ToString();
            headerPowerText.text = powerUp.PowerUpName.ToString();
            bodyText.text = powerUp.PowerUpInfo;
            powerInfoAddButton.SetActive(!ownersPowerUps.Contains(powerUp.PowerUpID));
            powerInfoRemoveButton.SetActive(ownersPowerUps.Contains(powerUp.PowerUpID));
            powerUpInfoImage.sprite = powerUp.powerUpImage.sprite;
        }
        else
            AddRemoveToggle();
    }

    public void AddRemoveToggle()
    {
        if (powerInfoPanel.activeInHierarchy)
            powerInfoPanel.SetActive(false);
        if (ownersPowerUps.Contains(focusPowerUp.PowerUpID))
        {
            ownersPowerUps.Remove(focusPowerUp.PowerUpID);
            focusPowerUp.transform.SetParent(powerDisplayPanel.transform);
            return;
        }
        if (ownersPowerUps.Count == 6)
            return;
        ownersPowerUps.Add(focusPowerUp.PowerUpID);
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
        if (PongManager.gameType == GameType.VSOnline)
            return;
        PongManager.player1Paddle.powerBar = playerPowerBar;
        PongManager.player2Paddle.powerBar = opponentPowerBar;
        SetPowerUps(playerPowersPanel, player1PowerUps);
        SetPowerUps(opponentPowersPanel, player2PowerUps);
    }
}
