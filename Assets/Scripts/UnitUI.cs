using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    public Text movementPointsText;
    // Start is called before the first frame update
    void Start()
    {
        // movementPointsText = gameObject.GetComponentInChildren<Text>();
        DisableUnitUI();
    }

    // Update is called once per frame
    void Update()
    {
        movementPointsText.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
    }

    public void EnableUnitUI()
    {
        movementPointsText.enabled = true;
    }

    public void DisableUnitUI()
    {
        movementPointsText.enabled = false;
    }
}
