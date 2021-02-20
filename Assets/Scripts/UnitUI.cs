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

    public void EnableUnitUI()
    {
        healthPointsText.enabled = true;
        HealthBar.SetActive(true);
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
