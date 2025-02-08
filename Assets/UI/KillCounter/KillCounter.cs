using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class KillCounter : MonoBehaviour
{
    public static int killCount = 0;
    TextMeshProUGUI killCounter;

    // Start is called before the first frame update
    void Start()
    {
        killCounter = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        killCounter.text = "Kills: " + killCount;
    }

    // To string method
    public override string ToString()
    {
        return killCount.ToString();
    }
}
