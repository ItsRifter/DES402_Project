using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public struct PlayerChoice
{
    public int Choice;
    public bool LockIn;
}

public class NumberClashGame : MinigameBase
{
    public int MaxRounds = 7;

    public float GridPosOffsetX = 48;
    public float GridPosOffsetY = 64;

    [Tooltip("The horizontal positioning for the diagonal offset style every 2 rows")]
    public float GridModuloOffset = 68;

    [SerializeField] GameObject slotPrefab;

    [SerializeField] GameObject[] uiGrids;
    [SerializeField] GameObject[] uiGridsModulo;

    GameObject[,] slotArray;

    int curRound;

    PlayerChoice[] playerList;

    GameObject[,] gridOne;
    GameObject[,] gridTwo;
    GameObject[,] gridThree;
    GameObject[,] gridFour;

    public void Initialise()
    {
        Debug.Log("NUMBER CLASH: Loaded");

        //Treat curRound as the index for the array
        curRound = 0;

        playerList = new PlayerChoice[3];
        ResetPlayersChoice();

        for (int i = 0; i < uiGrids.Length; i++)
        {
            GameObject[,] newGrid = MakeSlots(i);

            switch (i)
            {
                case 0: gridOne = newGrid; break;
                case 1: gridTwo = newGrid; break;
                case 2: gridThree = newGrid; break;
                case 3: gridFour = newGrid; break;
            }
        }
    }

    GameObject[,] MakeSlots(int playerIndex)
    {
        GameObject[,] slotArray = new GameObject[MaxRounds, 4];
        Vector3 pos = uiGrids[playerIndex].transform.position;

        //Create rows for each round
        for (int m = 0; m < slotArray.GetLength(0); m++)
        {
            //Create the panels on that row
            for (int p = 0; p < slotArray.GetLength(1); p++)
            {
                var panel = Instantiate(slotPrefab, uiGrids[playerIndex].transform);
                panel.transform.position = new Vector3(pos.x - 92 + (p * GridPosOffsetX), pos.y - 128 + (m * GridPosOffsetY), 0);

                if (m % 2 == 0)
                    panel.transform.localPosition += Vector3.right * GridModuloOffset;

                panel.GetComponent<ChoiceSlot>().RowIndex = p;

                slotArray[m, p] = panel;
            }
        }

        return slotArray;
    }

    public void StartRound()
    {
        Debug.Log("Round Start");
    }

    public void EndRound()
    {
        Debug.Log("Round end");
        curRound++;
    }

    public override GameScoreData GetScoreData()
    {
        throw new System.NotImplementedException();
    }

    //Converts DPad direction (Vector2) into integer choice
    int VectorToInt(Vector2 dir)
    {
        //Up button
        if (dir == Vector2.up)
            return 0;

        //Left button
        if (dir == Vector2.left)
            return 1;

        //Right button
        if (dir == Vector2.right)
            return 2;

        //Down button
        if (dir == Vector2.down)
            return 3;

        //Nothing was pressed/touched
        return -1;
    }

    public override void OnDirectionalInput(int playerIndex, Vector2 direction)
    {
        //Player has locked in their choice
        if (playerList[playerIndex].LockIn) return;
        
        //Convert vector to choice on DPad
        int choice = VectorToInt(direction);

        //If no choice was made
        if (choice == -1) return;

        //Set players choice to their selection
        playerList[playerIndex].Choice = choice;
    }

    //Locks that specific player from choosing
    void LockPlayerChoice(int playerIndex)
    {
        if (playerList[playerIndex].LockIn) return;

        playerList[playerIndex].LockIn = true;
    }

    //Resets all players choice and lock in
    void ResetPlayersChoice()
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i].Choice = -1;
            playerList[i].LockIn = false;
        }
    }

    //Locks players who have not locked in themselves
    void LockPlayersChoice()
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            if (!playerList[i].LockIn)
                playerList[i].LockIn = true;
        }
    }

    public override void OnPrimaryFire(int playerIndex)
    {
        ///Function for confirming the selected panel

        LockPlayerChoice(playerIndex);
    }

    public override void OnSecondaryFire(int playerIndex)
    {
        ///Function for confirming the selected panel

        LockPlayerChoice(playerIndex);
    }

    public override void TimeUp()
    {
        ///End the selection phase

    }

    protected override void OnResetGame()
    {
        Debug.Log("RESET");
        ResetPlayersChoice();
    }

    //Checks all players if they locked in their choice
    bool IsPlayersLocked() => playerList.Where(c => !c.LockIn).Any();

    protected override void OnUpdate()
    {
        ///Timer function here

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

    //Gets selected slots from each grid
    List<ChoiceSlot> GetSelectedFromGrids()
    {
        List<ChoiceSlot> slots = new List<ChoiceSlot>();

        for (int i = 0; i < uiGrids.Length; i++)
        {
            GameObject[,] grid = GetGrid(i);

            for (int p = 0; p < 4; p++)
            {
                ChoiceSlot slot = grid[curRound, p].GetComponent<ChoiceSlot>();

                //That slot is selected by a player
                if (slot.IsSelected)
                    slots.Add(slot);
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

    List<int> GetOwningPlayers(ChoiceSlot curSlot, List<ChoiceSlot> slots)
    {
        //Gets every player owning that slot as an integer into a list
        return slots.Where(s => s.RowIndex == curSlot.RowIndex).Select(p => p.PlayerOwner).ToList();
    }

    void SetSlotIcons(ChoiceSlot curSlot, List<ChoiceSlot> slots)
    {
        List<int> players = GetOwningPlayers(curSlot, slots);

        for (int i = 0; i < players.Count; i++)
            curSlot.SetCornerIcon(i, players[i]);
    }

    //Determine score from that round for each player
    void DetermineScore(List<ChoiceSlot> slots)
    {
        foreach (ChoiceSlot slot in slots)
        {
            int totalOwners = CountMatchingSlotOwners(slot, slots);

            //Multiple players has that slot
            if (totalOwners > 1)
            {
                slot.GetComponent<Image>().color = Color.red;
                SetSlotIcons(slot, slots);
            }

            //Only one player has that slot
            else if (totalOwners == 1)
            {
                int player = slot.PlayerOwner;
                slot.SetCenterIcon(player);
            }
        }
    }
}
