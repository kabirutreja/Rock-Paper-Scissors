using UnityEngine;
using UnityEngine.UI;

public enum PowerType { Rock, Paper, Scissors, None }

public class Playerpower : MonoBehaviour
{
     [Header("UI")]
    [SerializeField] private Image powerIcon;       // drag PowerIcon here
    [SerializeField] private Sprite rockSprite;
    [SerializeField] private Sprite paperSprite;
    [SerializeField] private Sprite scissorsSprite;

    private PowerType currentPower = PowerType.None;
    
    //[SerializeField] private Transform powerDisplay; // UI element to show current power

    public void AddRandomPower()
    {
         if (currentPower != PowerType.None)
        {
            Debug.Log("Already holding a power (" + currentPower + "), can't pick up another.");
            return;
        }
        int randomPower = Random.Range(0, 3);
        
        switch (randomPower)
        {
            case 0:
                currentPower = PowerType.Rock;
                
                break;
            case 1:
                currentPower = PowerType.Paper;
                
                break;
            case 2:
                currentPower = PowerType.Scissors;
                
                break;
        }
        
        UpdatePowerDisplay();
    }

    public PowerType GetCurrentPower()
    {
        return currentPower;
    }

    public bool HasPower()
    {
        return currentPower != PowerType.None;
    }

    public void UsePower()
    {
        if (currentPower == PowerType.None)
        {
           
            return;
        }

        
        currentPower = PowerType.None;
        UpdatePowerDisplay();
    }

    void UpdatePowerDisplay()
    {
        if (powerIcon == null) return;

        switch (currentPower)
        {
            case PowerType.Rock:
                powerIcon.sprite = rockSprite;
                powerIcon.enabled = true;
                break;
            case PowerType.Paper:
                powerIcon.sprite = paperSprite;
                powerIcon.enabled = true;
                break;
            case PowerType.Scissors:
                powerIcon.sprite = scissorsSprite;
                powerIcon.enabled = true;
                break;
            case PowerType.None:
                powerIcon.enabled = false; // hide icon when no power held
                break;
    }

    
}
}