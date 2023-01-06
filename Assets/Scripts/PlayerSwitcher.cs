using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerSwitcher : MonoBehaviour
{
    [SerializeField] Player[] Players;
    int currentActivePlayer = 0;
    //on switch tells cam switch to blend and slow down time
    //and tells ui to clear
    public UnityEvent OnSwitch;
    public static PlayerSwitcher Instance;

    //if player references applied manually then initialise on awake instead of timeline setting it
    private void Awake()
    {
        Instance = this;
        if(Players.Length > 0)
        {
            foreach (Player player in Players)
            {
                player.cam.enabled = false;
            }
            InitialisePlayers();
        }
    }


    //will be called by timeline signal when timeline is over
    //make sure players start as disabled, 
    public void InitialisePlayers()
    {
        foreach (Player player in Players)
        {
            if (player == Players[currentActivePlayer])
            {
                print(player.transform.root + " is the current active player");
                player.enabled = true;
                Player.Instance = player;
                continue;
            }
            else
            {
                print(player.transform.root + " is inactive");
                player.enabled = false;
                player.cam.enabled = false;
            }

        }
    }

    private void Update()
    {
        ////test code, will be moved to player input class
        //if (Keyboard.current.digit1Key.wasPressedThisFrame)
        //{
        //    RequestPlayerSwitch(0);
        //}
        //if (Keyboard.current.digit2Key.wasPressedThisFrame)
        //{
        //    RequestPlayerSwitch(1);
        //}
        //if (Keyboard.current.digit3Key.wasPressedThisFrame)
        //{
        //    RequestPlayerSwitch(2);
        //}
    }

    [ContextMenu("test scroll")]
    void TestScroll()
    {
        ScrollPlayerSwitch(1);
    }

    //changes player to next one based on int sign
    public void ScrollPlayerSwitch(int Direction)
    {
        int TargetPlayer = currentActivePlayer;
        if(Direction > 0)
        {
            TargetPlayer++;
            print("scrolling player switch up to " + currentActivePlayer);
            if (TargetPlayer > Players.Length -1)
            {
                print("went over player length from " + Players.Length + " to " + currentActivePlayer);
                TargetPlayer = 0;
            }
            RequestPlayerSwitch(TargetPlayer);
        }
        if(Direction < 0)
        {
            TargetPlayer--;
            if (TargetPlayer < 0)
            {
                TargetPlayer = Players.Length -1;
            }
            RequestPlayerSwitch(TargetPlayer);
        }
    }

    public void AddPlayer(Player x)
    {
        print("adding new player : " + x);
        int n = Players.Length;
        // create a new array of size n+1
        Player[] newarr = new Player[n + 1];
        print("creating new array with size of " + (n + 1));
        int i;
        // insert the elements from the 
        // old array into the new array
        // insert all elements till pos
        // then insert x at pos
        // then insert rest of the elements
        for (i = 0; i < n + 1; i++)
        {

            if (i == 0)
            {
                x.name = "player" + i;
                print("player in index " + i + " is " + x.name);
                newarr[i] = x;
            }
            else
            {
                x.name = "player" + i;
                print("player in index " + i + " is " + x.name);
                newarr[i] = Players[i - 1];
            }
        }
        Players = newarr;

    }

    public void UpdateCurrentPlayer(Player player)
    {
        print("new player for index " + currentActivePlayer + " is " + player.transform.root);
        Players[currentActivePlayer] = player;
        Player.Instance = player;
    }

    public void RequestPlayerSwitch(int number)
    {
        print("requesting player switch for " + number);
        if(number >= Players.Length || number < 0)
        {
            print("cannot switch to plyaer: " + number);
            return;
        }

        if(number == currentActivePlayer)
        {
            print(number + " player is already active");
            return;
        }

        SwitchPlayer(number);
    }


    //leaving player class disable code here for now
    //will need to migrate it to player class if it gets complicated
    //need to account for death cam
    //enabling player will also run player's on enable code-
    //causing wrong action map to be enabled when player is dead
    //switching will also need to update the ui about the players state
    //if player is dead show the ui
    //ui will hide whenver a switch occurs through onswitch event
    void SwitchPlayer(int number)
    {
        OnSwitch?.Invoke();
        GetComponent<PlayerCamSwitcher>().StartCamSwitch();
        print(" disablig player" + currentActivePlayer);
        //disable current player
        Players[currentActivePlayer].enabled = false;
        //Players[currentActivePlayer].cam.Priority = 10;

        //enable target player
        print("enabling player " + number);
        Players[number].enabled = true;
        Player.Instance = Players[number];
        //Players[number].cam.Priority = 13;
        currentActivePlayer = number;



    }
}
