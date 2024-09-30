using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberClashGame : MinigameBase
{
    [SerializeField] int MaxRounds = 7;

    [SerializeField] GameObject slotPrefab;
    [SerializeField] GameObject layoutParent;

    GameObject[,] slotArray;

    enum RoundStatus
    {
        IDLE,
        ACTIVE,
        END,
    }

    RoundStatus roundStats;


    public void Initialise()
    {
        Debug.Log("Number Clash game loaded");
        roundStats = RoundStatus.IDLE;

        slotArray = new GameObject[MaxRounds, 4];

        //Create rows for each round
        for (int m = 0; m < slotArray.GetLength(0); m++)
        {

            //Create the panels on that row
            for (int p = 0; p < slotArray.GetLength(1); p++)
            {
                //var panel = Instantiate(slotPrefab, layoutParent);

                //slotArray[m, p] = panel;

                //Debug.Log($"R{m}, P{p}");
            }
        }
    }

    public void StartRound()
    {
        Debug.Log("Round Start");
        roundStats = RoundStatus.ACTIVE;
    }

    public void EndRound()
    {
        Debug.Log("Round end");
        roundStats = RoundStatus.END;
    }

    public override GameScoreData GetScoreData()
    {
        throw new System.NotImplementedException();
    }

    public override void OnDirectionalInput(int playerIndex, Vector2 direction)
    {
        ///Translate each player's input into a number
        ///Example:
        /// -1 on the x-axis = left d-pad
        /// +1 on the x-axis = right d-pad
        /// -1 on the y-axis = down d-pad
        /// +1 on the y-axis = up d-pad

        ///Example:
        ///if (playerIndex == 0)
        ///{
        ///    if (direction.x == -1)
        ///    {
        ///
        ///    }
        ///}

    }

    public override void OnPrimaryFire(int playerIndex)
    {
        ///Function for confirming the selected panel


    }

    public override void OnSecondaryFire(int playerIndex)
    {
        ///Function for confirming the selected panel


    }

    public override void TimeUp()
    {
        ///End the selection phase

    }

    protected override void OnResetGame()
    {

    }

    protected override void OnUpdate()
    {
        ///Timer function here

    }
}
