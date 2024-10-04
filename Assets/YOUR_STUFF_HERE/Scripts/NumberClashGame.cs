using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public struct PlayerStats
{
    public GameObject Choice;
    public bool LockIn;
    public int Score;

    public int ID;

    public bool IsPlaying;
}

public class NumberClashGame : MinigameBase
{
    #region Variables

    [Header("Slots")]
    
    [SerializeField] GameObject slotPrefab;
    GameObject[,] slotArray;

    [Space(16)]

    [Header("Grids")]
    [SerializeField] GameObject[] uiGrids;
    [SerializeField] GameObject[] uiGridsModulo;

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

    [Header("Gameplay")]
    public int MaxRounds = 7;

    [Tooltip("How long the round will play before reveal")]
    [SerializeField] float RoundTimer = 10.0f;

    [Tooltip("How long to wait after the reveal for a new round")]
    [SerializeField] float IntermissionTimer = 6.0f;

    GameTimer gameTimer;

    int curRound;

    PlayerStats[] playerList;

    enum RoundStatus
    {
        IDLE,
        ACTIVE,
        POST
    }

    RoundStatus curRoundStatus = RoundStatus.IDLE;

    PlayerManager playerManager;

    bool IsSinglePlayer() => playerList.Where(c => c.IsPlaying).Count() == 1;

    //Checks all players if they locked in their choice
    bool AllPlayersLocked() => playerList.Where(c => c.IsPlaying).Where(l => !l.LockIn).Count() == 0;

    #region Integers

    int choice = -1;

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

    #endregion

    #endregion

    #region Awake, Start and Update Functions

    private void Awake()
    {
        MinigameLoaded.AddListener(InitialiseGame);
    }

    private void FixedUpdate()
    {
        //Using FixedUpdate as per the SimpleExampleGame script
    }

    #endregion

    //Hacky fix to prevent loading more than once
    bool hasLoaded = false;

    //Creates the players
    void CreatePlayers()
    {
        playerList = new PlayerStats[4];

        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i].ID = i;
            playerList[i].IsPlaying = false;

            playerList[i].Score = 0;
        }
    }

    public void InitialiseGame()
    {
        if (hasLoaded) return;

        playerManager = FindFirstObjectByType<PlayerManager>();

        //Treat curRound as the index for the array
        curRound = MaxRounds-1;

        CreatePlayers();

        //Create the grid for each player
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

        hasLoaded = true;
        Debug.Log("NUMBER CLASH: Loaded");
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

        gameTimer = new GameTimer(RoundTimer);
        curRoundStatus = RoundStatus.ACTIVE;
    }

    void ShowRoundResults()
    {
        List<ChoiceSlot> slots = GetSelectedFromGrids();

        DetermineScore(slots);
    }

    public void EndRound()
    {
        Debug.Log("Round end");
        gameTimer = new GameTimer(IntermissionTimer);

        if(IsSinglePlayer())
        {
            UpdateSlot(1, Random.Range(0, 3));
            AssignSlot(1);
        }

        ShowRoundResults();

        curRoundStatus = RoundStatus.POST;

        curRound--;
    }

    #endregion

    #region Input Functions

    public override void OnPrimaryFire(int playerIndex)
    {
        ///Function for confirming the selected panel

        if (playerList[playerIndex].Choice == null) return;

        AssignSlot(playerIndex);
    }

    public override void OnSecondaryFire(int playerIndex)
    {
        ///Function for confirming the selected panel

        if (playerList[playerIndex].Choice == null) return;

        AssignSlot(playerIndex);
    }

    public override void OnDirectionalInput(int playerIndex, Vector2 direction)
    {
        //Player has locked in their choice
        if (playerList[playerIndex].LockIn) return;

        //Convert vector to choice on DPad
        int choice = VectorToInt(direction);

        //If no choice was made
        if (choice != -1)
        {
            UpdateSlot(playerIndex, choice);
        }

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
            playerList[i].Choice = null;
            playerList[i].LockIn = false;
        }
    }

    void ResetPlayers()
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i].Choice = null;
            playerList[i].LockIn = false;
            playerList[i].Score = 0;
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
    }

    void CheckPlayers()
    {
        int players = playerManager.players.Count();

        for (int i = 0; i < players; i++)
        {
            if (playerManager.players[i].playerState == Player.PlayerState.IDLE)
                playerList[i].IsPlaying = false;
            else
                playerList[i].IsPlaying = true;
        }
    }

    protected override void OnUpdate()
    {
        ///Game complete function here
        if (curRoundStatus == RoundStatus.IDLE) return;

        CheckPlayers();

        float timeLeft = gameTimer.Tick(Time.deltaTime);

        if(timeLeft <= 0.0f)
        {
            switch(curRoundStatus)
            {
                case RoundStatus.IDLE: break;

                case RoundStatus.ACTIVE: EndRound(); break;
                case RoundStatus.POST: StartRound(); break;
            }
        }
    }

    #endregion

    #region Slot Functions

    void AssignSlot(int playerIndex)
    {        
        ChoiceSlot slot = playerList[playerIndex].Choice.GetComponent<ChoiceSlot>();
        slot.Assign(grid);

        LockPlayerChoice(playerIndex);
        Debug.Log(AllPlayersLocked());
        //All players have locked in
        if (AllPlayersLocked())
            EndRound();
    }

    void SetSlotIcons(ChoiceSlot curSlot, List<ChoiceSlot> slots)
    {
        List<int> players = GetOwningPlayers(curSlot, slots);

        for (int i = 0; i < players.Count; i++)
            curSlot.SetCornerIcon(i, players[i]);
    }

    void UpdateSlot(int player, int chosen)
    {
        Debug.Log($"Player {player}, Round {curRound}, chosen {chosen}");
        GameObject[,] grid = GetGrid(player);
        
        GameObject slot = grid[curRound, chosen];

        Debug.Log(slot);
        GameObject lastSlot = playerList[player].Choice;

        if (lastSlot != null)
            lastSlot.GetComponent<Image>().color = Color.grey;

        slot.GetComponent<ChoiceSlot>().OnSelected(player);

        playerList[player].Choice = slot;    
    }

    #endregion

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

    //Determine score from that round for each player
    void DetermineScore(List<ChoiceSlot> slots)
    {
        foreach (ChoiceSlot slot in slots)
        {
            int totalOwners = CountMatchingSlotOwners(slot, slots);

            //Multiple players has that slot
            if (totalOwners > 1)
            {
                slot.SetCenterIcon(-1, true);
                SetSlotIcons(slot, slots);
            }

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
