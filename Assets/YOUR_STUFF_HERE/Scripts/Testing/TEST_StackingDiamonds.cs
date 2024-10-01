using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEST_StackingDiamonds : MonoBehaviour
{
    [SerializeField] GameObject slotPrefab;

    [SerializeField] GameObject[] grids;

    GameObject[,] slotArray;

    [SerializeField] int maxRoundsTest;
   
    int roundsLeft = -1;

    // Start is called before the first frame update
    void Start()
    {
        roundsLeft = maxRoundsTest-1;

        for (int i = 0; i < 4; i++)    
            MakeSlots(i);
    }

    void MakeSlots(int playerIndex)
    {
        slotArray = new GameObject[maxRoundsTest, 4];

        //Create rows for each round
        for (int m = 0; m < slotArray.GetLength(0); m++)
        {
            //Create the panels on that row
            for (int p = 0; p < slotArray.GetLength(1); p++)
            {
                var panel = Instantiate(slotPrefab, grids[playerIndex].transform);
                slotArray[m, p] = panel;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (roundsLeft < 0) return;

        DetermineInput();
        UpdateSlot();
        
        if (Input.GetKeyDown(KeyCode.Space))
            ChangeSlot();
    }

    int choice = -1;
    bool locked = false;

    void DetermineInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            choice = 0;

        if (Input.GetKeyDown(KeyCode.A))
            choice = 1;

        if (Input.GetKeyDown(KeyCode.D))
            choice = 2;

        if (Input.GetKeyDown(KeyCode.S))
            choice = 3;
    }

    GameObject lastSelected = null;

    void UpdateSlot()
    {
        if (choice == -1) return;

        GameObject slot = slotArray[roundsLeft, choice];

        if (lastSelected != slot)
        {
            if(lastSelected != null)
                lastSelected.GetComponent<Image>().color = Color.grey;

            slot.GetComponent<Image>().color = Color.blue;

            lastSelected = slot;
        }
    }

    void ChangeSlot()
    {
        GameObject slot = slotArray[roundsLeft, choice];
        slot.GetComponent<Image>().color = Color.green;

        lastSelected = null;

        choice = -1;
        locked = false;
        roundsLeft--;
    }
}
