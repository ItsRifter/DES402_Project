using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceSlot : MonoBehaviour
{
    [Header("Player Backgrounds")]

    [SerializeField] Color PlyOneBGColour = Color.white;
    [SerializeField] Color PlyTwoBGColour = Color.white;
    [SerializeField] Color PlyThreeBGColour = Color.white;
    [SerializeField] Color PlyFourBGColour = Color.white;

    [Space(2)]

    [Header("Player Icons")]

    [SerializeField] Color PlyOneIconColour = Color.white;
    [SerializeField] Color PlyTwoIconColour = Color.white;
    [SerializeField] Color PlyThreeIconColour = Color.white;
    [SerializeField] Color PlyFourIconColour = Color.white;

    [Space(16)]

    public Sprite PlyOneIcon;
    public Sprite PlyTwoIcon;
    public Sprite PlyThreeIcon;
    public Sprite PlyFourIcon;

    [Space(32)]

    public Sprite ClashIcon;

    [SerializeField] Color BackgroundColor = Color.grey;
    [SerializeField] Color ClashedColor = Color.red;

    [HideInInspector] public bool IsSelected;
    [HideInInspector] public int PlayerOwner;

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
        slotIcon.color = BackgroundColor;

        imgTop.SetActive(false);
        imgLeft.SetActive(false);
        imgRight.SetActive(false);
        imgBottom.SetActive(false);
        imgCenter.SetActive(false);

        IsSelected = false;
        PlayerOwner = -1;
    }

    public Color GetPlayerBackgroundColour(int ply)
    {
        switch(ply)
        {
            case 0: return PlyOneBGColour;
            case 1: return PlyTwoBGColour;
            case 2: return PlyThreeBGColour;
            case 3: return PlyFourBGColour;

            default: return Color.white;
        }
    } 
    
    public Color GetPlayerColour(int ply)
    {
        switch(ply)
        {
            case 0: return PlyOneIconColour;
            case 1: return PlyTwoIconColour;
            case 2: return PlyThreeIconColour;
            case 3: return PlyFourIconColour;

            default: return Color.white;
        }
    }


    public Sprite GetPlayerIcon(int ply)
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

    //Sets the corner icon of index
    public void SetCornerIcon(int cornerIndex, int player)
    {
        GameObject corner = GetCornerObject(cornerIndex);
        Image renderer = corner.GetComponentInChildren<Image>();

        renderer.color = GetPlayerColour(player);
        renderer.sprite = GetPlayerIcon(player);

        corner.SetActive(true);
    }

    public void SetCenterIcon(int player, bool hasClashed = false)
    {
        Image renderer = imgCenter.GetComponentInChildren<Image>();

        //If the slot hasn't clashed with multiple players
        if(!hasClashed)
        {
            renderer.color = GetPlayerColour(player);
            renderer.sprite = GetPlayerIcon(player);

            slotIcon.color = GetPlayerBackgroundColour(player);
        }
        else
        {
            renderer.sprite = ClashIcon;
            slotIcon.color = ClashedColor;
        }

        imgCenter.SetActive(true);

    }

    //When the slot is selected
    public void OnSelected(int player)
    {
        Color plyCol = GetPlayerBackgroundColour(player);
        slotIcon.color = plyCol;
    }
    
    //Assign the slot to that player
    public void Assign(int player)
    {
        IsSelected = true;
        PlayerOwner = player;
    }

    //Resets slot
    public void Reset()
    {
        imgTop.SetActive(false);
        imgLeft.SetActive(false);
        imgRight.SetActive(false);
        imgBottom.SetActive(false);
        imgCenter.SetActive(false);

        IsSelected = false;
        PlayerOwner = -1;

        slotIcon.color = BackgroundColor;
    }
}
