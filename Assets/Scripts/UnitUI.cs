using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    public Text healthPointsText;

    public GameObject HealthBar;
    private UIHealthBar uiHealthBar;

    void Awake()
    {
        uiHealthBar = HealthBar.GetComponent<UIHealthBar>();
        DisableUnitUI();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthPointsText.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
        HealthBar.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
    }

    public void EnableUnitUI(int playerID)
    {
        healthPointsText.enabled = true;
        HealthBar.SetActive(true);

        Transform fillObject = HealthBar.transform.GetChild(0);
        Image image = fillObject.GetComponent<Image>();
        image.color = playerID == 1 ? Color.red : Color.blue;
    }

    public void DisableUnitUI()
    {
        healthPointsText.enabled = false;
        HealthBar.SetActive(false);
    }

    public void SetUIHealth(int health)
    {
        uiHealthBar.SetCurrentHealth(health);
        healthPointsText.text = health.ToString();
    }

    public void SetUIMaxHealth(int health)
    {
        uiHealthBar.SetMaxHealth(health);
        healthPointsText.text = health.ToString();
    }
}
