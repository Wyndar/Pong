using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public PongManager PongManager;
    public GameObject powerUpPrefab, powerUpSelectionPrefab;
    public GameObject player1PowerBarsPanel, player2PowerBarsPanel, player1PowersPanel, player2PowersPanel, powerUpSelectionScreen,
        selectedPowersPanel, powerDisplayPanel;
    public List<PowerUp> player1PowerUps, player2PowerUps, ownersPowerUps, ownersCurrentlyActivePowersUps;
    public PowerBar player1PowerBar, player2PowerBar;
    private bool hasLoadedPowerUps;

    public void SetLocalPlayerPowers(List<PowerUp> powerUps, bool isOwner)
    {
        if (ownersPowerUps.Count == 6 && isOwner)
            powerUps = new(ownersPowerUps);
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

    public void DamageCharge(int amount, bool player1TookDamage)
    {
        if (player1TookDamage)
        {
            player1PowerBar.PowerPercentChange(amount * 30, true);
            player2PowerBar.PowerPercentChange(amount * 10, true);
            return;
        }
        player2PowerBar.PowerPercentChange(amount * 30, true);
        player1PowerBar.PowerPercentChange(amount * 10, true);

    }

    public void ToggleUI(bool shouldShow)
    {
        player1PowersPanel.SetActive(shouldShow);
        player2PowersPanel.SetActive(shouldShow);
        player1PowerBarsPanel.SetActive(shouldShow);
        player2PowerBarsPanel.SetActive(shouldShow);
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
    public void SetPowerUps(bool isPlayer1)
    {
        if (isPlayer1)
            SetPowerUps(player1PowersPanel, player1PowerUps);
        else
            SetPowerUps(player2PowersPanel, player2PowerUps);
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
            p.isPlayer1 = powerUps == player1PowerUps;
            if (!p.isPlayer1 && PongManager.gameType == GameType.VSCOM)
                g.GetComponent<Button>().enabled = false;
            p.SetPowerUp(powerUps[x]);
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
        if (ownersPowerUps.Contains(powerUp.powerUp))
        {
            ownersPowerUps.Remove(powerUp.powerUp);
            powerUp.transform.SetParent(powerDisplayPanel.transform);
            return;
        }
        if (ownersPowerUps.Count == 6)
            return;
        ownersPowerUps.Add(powerUp.powerUp);
        powerUp.transform.SetParent(selectedPowersPanel.transform);
    }

    //add warning if less than 6
    public void DisablePowerUpSelectionScreen()
    {
        powerUpSelectionScreen.SetActive(false);
    }

    public void PowerUpSetup()
    {
        player1PowerBar.SetPowerPercent(0);
        player2PowerBar.SetPowerPercent(0);
        PongManager.player1Paddle.powerBar = player1PowerBar;
        PongManager.player2Paddle.powerBar = player2PowerBar;
        if (PongManager.gameType != GameType.VSOnline)
        {
            SetPowerUps(player1PowersPanel, player1PowerUps);
            SetPowerUps(player2PowersPanel, player2PowerUps);
        }
    }
}
