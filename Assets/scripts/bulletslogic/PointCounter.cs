using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCounter : MonoBehaviour
{
    [SerializeField] Counter PointHUD;
    private void Start()
    {
        StartCoroutine(CountPoints());
    }
    private IEnumerator CountPoints()
    {
        while (true)
        {
            PointHUD.Points += 1;
            yield return new WaitForSeconds(1);
        }
    }
}
