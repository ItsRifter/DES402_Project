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

    public bool IsSelected;
    public int PlayerOwner;

    Color curColor;
    Image slotIcon;

    public int RowIndex;

    void Start()
    {
        slotIcon = GetComponent<Image>();

        IsSelected = false;
        PlayerOwner = -1;
    }

    public Color GetSelectingPlayerColour(int ply)
    {
        switch(ply)
        {
            case 0: return PlyOneColour;
            case 1: return PlyTwoColour;
            case 2: return PlyThreeColour;
            case 3: return PlyFourColour;

            default: return Color.white;
        }
    }

    public void OnSelected(int player)
    {
        Color plyCol = GetSelectingPlayerColour(player);

        slotIcon.color = plyCol;
    }

    public void Assign(int player)
    {
        IsSelected = true;
        PlayerOwner = player;
    }

    public void Reset()
    {
        IsSelected = false;
        PlayerOwner = -1;
    }
}
