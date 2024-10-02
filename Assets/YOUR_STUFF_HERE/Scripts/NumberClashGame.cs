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
    #region Variables

    [Header("Slots")]
    [SerializeField] GameObject slotPrefab;
    GameObject[,] slotArray;

    [Space(10)]

    [Header("Grids")]
    [SerializeField] GameObject[] uiGrids;
    [SerializeField] GameObject[] uiGridsModulo;

    GameObject[,] curGrid;
    GameObject[,] gridOne;
    GameObject[,] gridTwo;
    GameObject[,] gridThree;
    GameObject[,] gridFour;

    int grid = 0;

    [Space(5)]

    [Tooltip("The horizontal positioning for the diagonal offset style every 2 rows")]
    public float GridModuloOffset = 68;
    
    [Space(5)]

    public float GridPosOffsetX = 48;
    public float GridPosOffsetY = 64;

    [Space(10)]

    [Header("Rounds")]
    public int MaxRounds = 7;
    int curRound;

    PlayerChoice[] playerList;

    //Checks all players if they locked in their choice
    bool IsPlayersLocked() => playerList.Where(c => !c.LockIn).Any();

    #region Integers

    int choice = -1;
    int playersLeft = 4;

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

    int CountMatchingSlotOwners(ChoiceSlot curSlot, List<ChoiceSlot> slots)
    {
        //Counts how many players have selected that slot from row index
        //EXAMPLE: if 2 players selected row 3, it'll return 2 otherwise 1
        int owners = slots.Where(s => s.RowIndex == curSlot.RowIndex).Count(c => c.IsSelected);

        return owners;
    }

    #endregion

    #region Lists

    List<int> GetOwningPlayers(ChoiceSlot curSlot, List<ChoiceSlot> slots)
    {
        //Gets every player owning that slot as an integer into a list
        return slots.Where(s => s.RowIndex == curSlot.RowIndex).Select(p => p.PlayerOwner).ToList();
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

    #endregion

    #region GameObjects

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

    #endregion

    #endregion

    #region Awake, Start and Update Functions

    private void Awake()
    {
        MinigameLoaded.AddListener(InitialiseGame);
    }

    private void Start()
    {
        curRound = MaxRounds - 1;

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

        curGrid = gridOne;
    }

    private void FixedUpdate()
    {
        //Using FixedUpdate as per the SimpleExampleGame script

        if (curRound > MaxRounds) return;

        ///Copied over
        //DetermineInput();

        UpdateSlot();

        ///Copied over
        //if (Input.GetKeyDown(KeyCode.Space) && choice != -1)
        //    AssignSlot();
    }

    #endregion

    public void InitialiseGame()
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

    public override GameScoreData GetScoreData()
    {
        throw new System.NotImplementedException();

        //Copied from the SimpleExampleGame script

        //int teamTime = 0;
        //GameScoreData gsd = new GameScoreData();
        //for (int i = 0; i < 4; i++)
        //{
        //    if (PlayerUtilities.GetPlayerState(i) == Player.PlayerState.ACTIVE)
        //    {
        //        gsd.PlayerScores[i] = m_Scores[i];
        //        gsd.PlayerTimes[i] = Mathf.Min(m_Scores[i] / 2, 1);
        //        teamTime += gsd.PlayerTimes[i];
        //    }
        //}
        //gsd.ScoreSuffix = " cleaned";
        //gsd.TeamTime = teamTime;
        //return gsd;
    }

    #region Rounds

    public void StartRound()
    {
        Debug.Log("Round Start");
    }

    public void EndRound()
    {
        Debug.Log("Round end");
        curRound++;
    }

    #endregion

    #region Input Functions

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

    #endregion

    #region Player Choice Functions

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

    #endregion

    #region Inherited Minigame Class Functions
    public override void TimeUp()
    {
        ///End the selection phase

    }

    protected override void OnResetGame()
    {
        Debug.Log("RESET");
        ResetPlayersChoice();

        //From the SimpleExampleGame script
        //InitialiseGame();
    }

    protected override void OnUpdate()
    {
        ///Game complete function here

    }

    #endregion

    #region Slot Functions

    void AssignSlot()
    {
        ChoiceSlot slot = curGrid[curRound, choice].GetComponent<ChoiceSlot>();
        slot.Assign(grid);

        //lastSelected = null;

        choice = -1;
        playersLeft--;

        if (playersLeft <= 0)
            EndRoundResults();
        else
            IncrementGridSelect();
    }

    void SetSlotIcons(ChoiceSlot curSlot, List<ChoiceSlot> slots)
    {
        List<int> players = GetOwningPlayers(curSlot, slots);

        for (int i = 0; i < players.Count; i++)
            curSlot.SetCornerIcon(i, players[i]);
    }

    void UpdateSlot()
    {
        if (choice == -1) return;

        ///Copied over
        //ChoiceSlot slot = curGrid[curRound, choice].GetComponent<ChoiceSlot>();

        //if (lastSelected != slot)
        //{
        //    if (lastSelected != null)
        //        lastSelected.GetComponent<Image>().color = Color.grey;

        //    slot.OnSelected(grid);

        //    lastSelected = slot;
        //}
    }

    #endregion

    void IncrementGridSelect()
    {
        grid++;

        if (grid > 3)
            grid = 0;

        //if (lastSelected != null)
        //    lastSelected.GetComponent<Image>().color = Color.grey;

        //lastSelected = null;

        switch (grid)
        {
            case 0: curGrid = gridOne; break;
            case 1: curGrid = gridTwo; break;
            case 2: curGrid = gridThree; break;
            case 3: curGrid = gridFour; break;
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

    void EndRoundResults()
    {
        ///Copied over

        IncrementGridSelect();

        List<ChoiceSlot> slots = GetSelectedFromGrids();

        DetermineScore(slots);

        playersLeft = 4;
        curRound--;
    }
}
