using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct WinScreenInfo
{
    public GameObject Panel;
    public GameObject[] ScoreTexts;
}

public class PlayerWinScreen : MonoBehaviour
{
    [SerializeField] WinScreenInfo[] winScreens;

    public void SetPlayerStats(int[] scores)
    {
        int index = 0;

        foreach (var screen in winScreens)
        {
            foreach (var textpnl in screen.ScoreTexts)
            {
                textpnl.GetComponent<TMP_Text>().SetText($"- {scores[index]}");
                index++;
            }

            index = 0;
        }
    }

    public void ShowScreens()
    {
        foreach (var screen in winScreens)
            screen.Panel.SetActive(true);
    }

    public void HideScreens()
    {
        foreach (var screen in winScreens)
            screen.Panel.SetActive(false);
    }
}
