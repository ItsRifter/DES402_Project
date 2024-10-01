using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceSlot : MonoBehaviour
{
    public Color PlyOneColour = Color.white;
    public Color PlyTwoColour = Color.white;
    public Color PlyThreeColour = Color.white;
    public Color PlyFourColour = Color.white;

    public Color HoverColour = Color.grey;

    Color curColor;
    Image slotIcon;

    void Start()
    {
        slotIcon = GetComponent<Image>();
    }

    Color GetSelectingPlayerColour(int ply)
    {
        switch(ply)
        {
            case 0: return PlyOneColour;
            case 1: return PlyTwoColour;
            case 2: return PlyThreeColour;
            case 3: return HoverColour;

            default: return Color.white;
        }
    }

    public void OnSelected(int player)
    {
        Color plyCol = GetSelectingPlayerColour(player);

        slotIcon.color = plyCol;
    }

    public void OnLockedIn(int player)
    {

    }


}
