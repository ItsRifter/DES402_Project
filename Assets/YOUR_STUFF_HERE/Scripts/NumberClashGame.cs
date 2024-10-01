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
    [SerializeField] int MaxRounds = 7;

    [SerializeField] GameObject slotPrefab;
    [SerializeField] GameObject layoutParent;

    GameObject[,] slotArray;

    int curRound;

    PlayerChoice[] playerList;

    public void Initialise()
    {
        Debug.Log("Number Clash game loaded");

        //Treat curRound as the index for the array
        curRound = MaxRounds-1;

        playerList = new PlayerChoice[3];
        ResetPlayersChoice();

        slotArray = new GameObject[MaxRounds, 4];

        //Create rows for each round
        for (int m = 0; m < slotArray.GetLength(0); m++)
        {
            //Create the panels on that row
            for (int p = 0; p < slotArray.GetLength(1); p++)
            {
                var panel = Instantiate(slotPrefab, layoutParent.transform);
                slotArray[m, p] = panel;
            }
        }
    }

    public void StartRound()
    {
        Debug.Log("Round Start");
    }

    public void EndRound()
    {
        Debug.Log("Round end");
        curRound--;
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
}
