using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class TEST_StackingDiamonds : MonoBehaviour
{
    [SerializeField] GameObject slotPrefab;

    [SerializeField] GameObject[] uiGrids;
    [SerializeField] GameObject[] uiGridsModulo;

    [SerializeField] int maxRoundsTest;
    
    int curRound = -1;

    public float GridPosOffsetX = 48;
    public float GridPosOffsetY = 64;

    [Tooltip("The horizontal positioning for the diagonal offset style every 2 rows")]
    public float GridModuloOffset = 68;

    GameObject[,] gridOne;
    GameObject[,] gridTwo;
    GameObject[,] gridThree;
    GameObject[,] gridFour;

    int playersLeft = 4;

    // Start is called before the first frame update
    void Start()
    {
        curRound = maxRoundsTest-1;

        for (int i = 0; i < uiGrids.Length; i++)
        {
            GameObject[,] newGrid = MakeSlots(i);

            switch(i)
            {
                case 0: gridOne = newGrid; break;
                case 1: gridTwo = newGrid; break;
                case 2: gridThree = newGrid; break;
                case 3: gridFour = newGrid; break;
            }
        }

        curGrid = gridOne;
    }

    GameObject[,] MakeSlots(int playerIndex)
    {
        GameObject[,] slotArray = new GameObject[maxRoundsTest, 4];
        
        //Create rows for each round
        for (int m = 0; m < slotArray.GetLength(0); m++)
        {
            //Create the panels on that row
            for (int p = 0; p < slotArray.GetLength(1); p++)
            {
                Transform parentTransform = uiGrids[playerIndex].transform;

                if (m % 2 > 0)
                    parentTransform = uiGridsModulo[playerIndex].transform;

                GameObject panel = Instantiate(slotPrefab, parentTransform);
                
                panel.GetComponent<ChoiceSlot>().RowIndex = p;

                slotArray[m, p] = panel;
            }
        }

        return slotArray;
    }

    // Update is called once per frame
    void Update()
    {
        if (curRound > maxRoundsTest) return;

        DetermineInput();

        UpdateSlot();

        if (Input.GetKeyDown(KeyCode.Space) && choice != -1)
            AssignSlot();
    }

    int choice = -1;

    void DetermineInput()
    {
        //Messy way of doing inputs, this is just for testing
        if (Input.GetKeyDown(KeyCode.W))
            choice = 0;

        if (Input.GetKeyDown(KeyCode.A))
            choice = 1;

        if (Input.GetKeyDown(KeyCode.S))
            choice = 2;

        if (Input.GetKeyDown(KeyCode.D))
            choice = 3;
    }

    ChoiceSlot lastSelected = null;

    void UpdateSlot()
    {
        if (choice == -1) return;

        ChoiceSlot slot = curGrid[curRound, choice].GetComponent<ChoiceSlot>();

        if (lastSelected != slot)
        {
            if(lastSelected != null)
                lastSelected.GetComponent<Image>().color = Color.grey;

            slot.OnSelected(grid);
            
            lastSelected = slot;
        }
    }

    int grid = 0;
    GameObject[,] curGrid;

    void IncrementGridSelect()
    {
        grid++;

        if (grid > 3)
            grid = 0;

        if (lastSelected != null)
            lastSelected.GetComponent<Image>().color = Color.grey;

        lastSelected = null;

        switch (grid)
        {
            case 0: curGrid = gridOne; break;
            case 1: curGrid = gridTwo; break;
            case 2: curGrid = gridThree; break;
            case 3: curGrid = gridFour; break;
        }
    }

    void AssignSlot()
    {
        ChoiceSlot slot = curGrid[curRound, choice].GetComponent<ChoiceSlot>();
        slot.Assign(grid);

        lastSelected = null;

        choice = -1;
        playersLeft--;

        if (playersLeft <= 0)
            EndRoundResults();
        else
            IncrementGridSelect();
    }

    GameObject[,] GetGrid(int grid)
    {
        switch (grid)
        {
            case 0: return gridOne;
            case 1: return gridTwo;
            case 2: return gridThree;
            case 3: return gridFour;

            default: return null;
        }
    }

    void EndRoundResults()
    {
        IncrementGridSelect();

        List<ChoiceSlot> slots = GetSlotsFromGrid(true);

        DetermineScore(slots);

        playersLeft = 4;
        curRound--;
    }

    //Gets selected slots from each grid
    List<ChoiceSlot> GetSlotsFromGrid(bool areSelected, int rowIndex = -1)
    {
        List<ChoiceSlot> slots = new List<ChoiceSlot>();

        for (int i = 0; i < uiGrids.Length; i++)
        {
            GameObject[,] grid = GetGrid(i);

            for (int p = 0; p < 4; p++)
            {
                ChoiceSlot slot = grid[curRound, p].GetComponent<ChoiceSlot>();

                //If we're looking for selected slots
                if (areSelected && slot.IsSelected) slots.Add(slot);

                //Otherwise if not
                else if (!areSelected)
                {
                    if (slot.RowIndex == rowIndex)
                        slots.Add(slot);
                }
            }
        }

        return slots;
    }

    int CountMatchingSlotOwners(ChoiceSlot curSlot, List<ChoiceSlot> slots)
    {
        //Counts how many players have selected that slot from row index
        //EXAMPLE: if 2 players selected row 3, it'll return 2 otherwise 1
        int owners = slots.Where(s => s.RowIndex == curSlot.RowIndex).Count(c => c.IsSelected);

        return owners;
    }
   
    /// <summary>
    /// Gets every PLAYER owning that slot as an integer into a list
    /// </summary>
    List<int> GetOwningPlayers(ChoiceSlot curSlot, List<ChoiceSlot> slots) 
        => slots.Where(s => s.RowIndex == curSlot.RowIndex).Select(p => p.PlayerOwner).ToList();
 

    /// <summary>
    /// Gets every GRID owning that slot
    /// </summary>
    List<ChoiceSlot> GetOwningGrids(ChoiceSlot curSlot, List<ChoiceSlot> slots)
        => slots.Where(s => s.RowIndex == curSlot.RowIndex).ToList();

    void SetClashSlotIcons(ChoiceSlot curSlot, List<ChoiceSlot> slots)
    {
        List<int> players = GetOwningPlayers(curSlot, slots);

        for (int i = 0; i < 4; i++)
        {
            GameObject[,] grid = GetGrid(i);

            ChoiceSlot slot = grid[curRound, curSlot.RowIndex].GetComponent<ChoiceSlot>();

            slot.SetCenterIcon(-1, true);

            for (int p = 0; p < players.Count; p++)
                slot.SetCornerIcon(p, players[p]);
        }  
    }

    //Determine score from that round for each player
    void DetermineScore(List<ChoiceSlot> slots)
    {
        foreach (ChoiceSlot slot in slots)
        {
            int totalOwners = CountMatchingSlotOwners(slot, slots);

            //Multiple players has that slot
            if (totalOwners > 1)
                SetClashSlotIcons(slot, slots);
            
            //Only one player has that slot
            else if (totalOwners == 1)
            {
                int player = slot.PlayerOwner;
                List<ChoiceSlot> otherSlots = GetSlotsFromGrid(false, slot.RowIndex);

                foreach (var other in otherSlots)
                    other.SetCenterIcon(player);
            }
        }
    }
}
