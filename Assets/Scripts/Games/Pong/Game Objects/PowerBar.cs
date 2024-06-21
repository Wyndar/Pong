using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class PowerBar : MonoBehaviour
{
    [SerializeField] private TMP_Text powerPercentText;
    [SerializeField] private Slider powerBar, overchargeBar;
    public float PowerPercent { get; private set; }

    public void SetPowerPercent(int amount)
    {
        PowerPercent = amount;
        PowerBarUpdate();
    }
    public void PowerPercentChange(float amount, bool add)
    {
        if (add)
            PowerPercent += amount;
        else
            PowerPercent -= amount;
        if(PowerPercent < 0)
            PowerPercent = 0;
        if(PowerPercent > 200)
            PowerPercent = 200;
        PowerBarUpdate();
    }
    public void PowerBarUpdate()
    {
        overchargeBar.gameObject.SetActive(PowerPercent > 100);
        powerPercentText.text = $"{Mathf.FloorToInt(PowerPercent)}%";
        if (PowerPercent <= 100)
            powerBar.value = PowerPercent/100;
        else
            overchargeBar.value = (PowerPercent - 100)/100;
    }
}
