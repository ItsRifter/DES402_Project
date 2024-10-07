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
        POST,
        CONCLUDE
    }

    RoundStatus curRoundStatus = RoundStatus.IDLE;

    PlayerManager playerManager;
    bool isGamePlaying = false;

    #endregion

    #region Miscellanous

    //Should a bot fill in player slots
    bool CanBotsPlay() => playerList.Where(c => c.IsPlaying).Count() != 4;

    //Checks all players if they locked in their choice
    bool AllPlayersLocked() => playerList.Where(c => c.IsPlaying).Where(l => !l.LockIn).Count() == 0;

    #endregion

    #region Numbers Getters

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

    float GetStatusTimer()
    {
        switch(curRoundStatus)
        {
            case RoundStatus.IDLE: return -1;

            case RoundStatus.ACTIVE: return RoundTimer;
            case RoundStatus.POST: return IntermissionTimer;

            default: return -1;
        }
    }

    #endregion

    #region Lists Getters

    List<int> GetOwningPlayers(ChoiceSlot curSlot, List<ChoiceSlot> slots)
    {
        //Gets every player owning that slot as an integer into a list
        return slots.Where(s => s.RowIndex == curSlot.RowIndex).Select(p => p.PlayerOwner).ToList();
    }

    int GetTotalPlayersActive() => playerList.Where(a => a.IsPlaying).Count();

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

    /// <summary>
    /// Returns all players score
    /// </summary>
    /// <returns>Each players score of ranking in descending order</returns>
    List<int> GetPlayerRanking()
    {
        var scores = playerList.Select(p => p.Score).ToList();
        
        scores.Sort();
        scores.Reverse();

        return scores;
    }

    #endregion

    #region GameObjects Getters

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

    #region Awake, Start and Update Functions

    private void Awake()
    {
        MinigameLoaded.AddListener(InitialiseGame);
    }

    private void FixedUpdate()
    {
        //Using FixedUpdate as per the SimpleExampleGame script
    }
    
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

    #endregion

    #region Gameplay

    //Hacky fix to prevent loading more than once
    bool hasLoaded = false;

    //Startup game
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

    //Finishes the game, this does NOT stop the game
    public void FinishGame()
    {
        Debug.Log("GAME END");
        curRoundStatus = RoundStatus.CONCLUDE;

        var scoring = GetPlayerRanking();

        for (int i = 0; i < playerList.Length; i++)
        {
            Debug.Log($"RANK {i+1} - PLAYER {i} with {scoring[i]}");
        }

        EndGame();
    }

    //Stops the game from playing
    public void StopGame()
    {
        isGamePlaying = false;
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

                playerList[player].Score++;

                //Set other players grid of matching slot row to who owns it
                List<ChoiceSlot> otherSlots = GetSlotsFromGrid(false, slot.RowIndex);

                foreach (var other in otherSlots)
                    other.SetCenterIcon(player);
            }
        }
    }

    #region Rounds

    //Begins a new round
    public void StartRound()
    {
        Debug.Log("Round Start");

        if (!isGamePlaying)
            isGamePlaying = true;

        ResetPlayersChoice();

        gameTimer = new GameTimer(RoundTimer);
        curRoundStatus = RoundStatus.ACTIVE;
    }

    //Shows the round results
    void ShowRoundResults()
    {
        List<ChoiceSlot> slots = GetSelectedFromGrids();

        DetermineScore(slots);
    }

    //Ends that round
    public void EndRound()
    {
        Debug.Log("Round end");

        curRoundStatus = RoundStatus.POST;
        ForcePlayersChoice();

        Debug.Log($"Bots enabled: {CanBotsPlay()}");

        //If theres less than 4 players, fill in the missing slots
        if (CanBotsPlay())
        {
            for (int i = GetTotalPlayersActive(); i < 4; i++)
            {
                //Random Range goes from 0 to 4 to prevent bots avoiding specific slots
                //despite row index goes from 0 to 3
                UpdateSlot(i, Random.Range(0, 4));
                AssignSlot(i, true);
            }
        }

        ShowRoundResults();

        //Last round has concluded, end the game
        if (curRound == 0)
            FinishGame();

        //Otherwise continue with the game
        else
        {
            gameTimer = new GameTimer(IntermissionTimer);

            curRound--;
        }
    }

    #endregion

    #endregion

    public override GameScoreData GetScoreData()
    {
        GameScoreData gsd = new GameScoreData();

        for (int i = 0; i < 4; i++)
        {
            if (PlayerUtilities.GetPlayerState(i) == Player.PlayerState.ACTIVE)
            {
                gsd.PlayerScores[i] = playerList[i].Score;
                
                //gsd.PlayerTimes[i] = Mathf.Min(m_Scores[i] / 2, 1);
                //teamTime += gsd.PlayerTimes[i];
            }
        }

        gsd.ScoreSuffix = " points";

        return gsd;
    }

    #region Input Functions

    public override void OnPrimaryFire(int playerIndex)
    {
        ///Function for confirming the selected panel

        AssignSlot(playerIndex);
    }

    public override void OnSecondaryFire(int playerIndex)
    {
        ///Function for confirming the selected panel

        AssignSlot(playerIndex);
    }

    public override void OnDirectionalInput(int playerIndex, Vector2 direction)
    {
        //Player has locked in their choice
        if (playerList[playerIndex].LockIn) return;

        //Convert vector to choice on DPad
        int choice = VectorToInt(direction);

        //If a choice was made and game is active
        if (choice != -1 && isGamePlaying)
            UpdateSlot(playerIndex, choice);
    }

    #endregion

    #region Player Choice Functions

    //Locks that specific player from choosing
    void LockPlayerChoice(int playerIndex)
    {
        playerList[playerIndex].LockIn = true;
    }

    //Resets all players choice and locking
    void ResetPlayersChoice()
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            playerList[i].Choice = null;
            playerList[i].LockIn = false;
        }
    }

    //Resets all players for a new game
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

    //Forces player to lock in choice if possible
    void ForcePlayersChoice()
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            //That player did not make a choice
            if (playerList[i].Choice == null)
            {
                //Lock their selections anyway
                LockPlayerChoice(i);
                continue;
            }

            AssignSlot(i);
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
        ResetGrid();
    }

    void ResetGrid()
    {
        //Grid
        for (int g = 0; g < 4; g++)
        {
            var grid = GetGrid(g);

            //Column
            for (int c = 0; c < grid.GetLength(0); c++)
            {
                //Row
                for (int r = 0; r < grid.GetLength(1); r++)
                    grid[c, r].GetComponent<ChoiceSlot>().Reset();
            }
        }
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
        if (!isGamePlaying) return;
        
        CheckPlayers();

        gameTimer.Tick(Time.deltaTime);
        float timeLeft = gameTimer.TotalTime;

        if(timeLeft > GetStatusTimer())
        {
            switch(curRoundStatus)
            {
                case RoundStatus.IDLE: break;

                case RoundStatus.ACTIVE: EndRound(); break;
                case RoundStatus.POST: StartRound(); break;
                case RoundStatus.CONCLUDE: StopGame(); break;
            }
        }
    }

    #endregion

    #region Slot & Grid Functions

    void AssignSlot(int playerIndex, bool isBot = false)
    {
        //Game isn't properly playing
        if (!isGamePlaying) return;

        //If the player is not a bot, run checks
        if(!isBot)
        {
            //If the player made no choice
            if (playerList[playerIndex].Choice == null) return;

            //Player is already locked in
            if (playerList[playerIndex].LockIn) return;
        }

        ChoiceSlot slot = playerList[playerIndex].Choice.GetComponent<ChoiceSlot>();
        slot.Assign(playerIndex);

        LockPlayerChoice(playerIndex);
        
        //All players have locked in and round is active
        if (AllPlayersLocked() && curRoundStatus == RoundStatus.ACTIVE)
            EndRound();
    }

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

    void UpdateSlot(int player, int chosen)
    {
        GameObject[,] grid = GetGrid(player);
        
        GameObject slot = grid[curRound, chosen];

        GameObject lastSlot = playerList[player].Choice;

        if (lastSlot != null)
            lastSlot.GetComponent<Image>().color = Color.grey;

        slot.GetComponent<ChoiceSlot>().OnSelected(player);

        playerList[player].Choice = slot;    
    }

    #endregion
}
