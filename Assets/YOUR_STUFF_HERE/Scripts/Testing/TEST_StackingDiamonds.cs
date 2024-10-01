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

        DeterminePlayerInput();
        DetermineInput();

        UpdateSlot();
        
        if (Input.GetKeyDown(KeyCode.Space))
            ChangeSlot();
    }

    int choice = -1;
    int lastPlayer = -1;

    int playerOneChoice = -1;
    int playerTwoChoice = -1;

    bool playerOneLocked = false;
    bool playerTwolocked = false;

    //TESTING ONLY
    //Gets the last player who inputted
    void DeterminePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
        Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            lastPlayer = 0;

        if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.G) ||
        Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.J))
            lastPlayer = 1;
    }

    void DetermineInput()
    {
        //Messy way of doing inputs, this is just for testing
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Y))
            choice = 0;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.G))
            choice = 1;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.J))
            choice = 2;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.H))
            choice = 3;

        if(lastPlayer != -1)
        {
            switch(lastPlayer)
            {
                case 0: playerOneChoice = choice; break;
                case 1: playerTwoChoice = choice; break;
            }
        }
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

            slot.GetComponent<ChoiceSlot>().OnSelected(choice);
            
            //slot.GetComponent<Image>().color = Color.blue;

            lastSelected = slot;
        }
    }

    void ChangeSlot()
    {
        GameObject slot = slotArray[roundsLeft, choice];
        slot.GetComponent<Image>().color = Color.green;

        lastSelected = null;

        playerOneChoice = -1;
        playerTwoChoice = -1;

        roundsLeft--;
    }
}
