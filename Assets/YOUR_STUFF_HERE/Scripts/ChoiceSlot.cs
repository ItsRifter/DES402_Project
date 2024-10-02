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

    public Sprite PlyOneIcon;
    public Sprite PlyTwoIcon;
    public Sprite PlyThreeIcon;
    public Sprite PlyFourIcon;

    public bool IsSelected;
    public int PlayerOwner;

    [SerializeField] GameObject imgTop;
    [SerializeField] GameObject imgLeft;
    [SerializeField] GameObject imgRight;
    [SerializeField] GameObject imgBottom;
    [SerializeField] GameObject imgCenter;

    Color curColor;
    Image slotIcon;

    public int RowIndex;

    void Start()
    {
        slotIcon = GetComponent<Image>();

        imgTop.SetActive(false);
        imgLeft.SetActive(false);
        imgRight.SetActive(false);
        imgBottom.SetActive(false);
        imgCenter.SetActive(false);

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

    GameObject GetCornerObject(int corner)
    {
        switch (corner)
        {
            case 0: return imgLeft;
            case 1: return imgRight;
            case 2: return imgTop;
            case 3: return imgBottom;
            
            default: return imgCenter;
        }
    }

    public Sprite GetSelectingPlayerIcon(int ply)
    {
        switch (ply)
        {
            case 0: return PlyOneIcon;
            case 1: return PlyTwoIcon;
            case 2: return PlyThreeIcon;
            case 3: return PlyFourIcon;

            default: return null;
        }
    }

    public void SetCornerIcon(int cornerIndex, int player)
    {
        GameObject corner = GetCornerObject(cornerIndex);

        corner.GetComponent<Image>().sprite = GetSelectingPlayerIcon(player);
        corner.SetActive(true);
    }

    public void SetCenterIcon(int player)
    {
        Sprite icon = GetSelectingPlayerIcon(player);
        imgCenter.GetComponent<Image>().sprite = icon;
        imgCenter.SetActive(true);

        slotIcon.color = GetSelectingPlayerColour(player);
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
        imgTop.SetActive(false);
        imgLeft.SetActive(false);
        imgRight.SetActive(false);
        imgBottom.SetActive(false);
        imgCenter.SetActive(false);

        IsSelected = false;
        PlayerOwner = -1;
    }
}
