using UnityEngine;

public class GameManager : MonoBehaviour

{
    public enum GameState
    {
        Pickup,
        Delivery,
        Talking,
        Upgrade
    }   

    public GameState currentState = GameState.Pickup;
    public int loopNumber = 1;
    public GameObject goalPrefab;
    private GameObject currentGoal;

    void Start()
    {
        currentState = GameState.Pickup;
        Debug.Log("Loop: " + loopNumber);
        Debug.Log("Current State: " + currentState);

        SpawnGoal();

    }

    public void NextState()
    {
        if (currentGoal != null)
        {
            Destroy(currentGoal);
        }

        if (currentState == GameState.Pickup)
        {
            currentState = GameState.Delivery;
            SpawnGoal();
        }
        else if (currentState == GameState.Delivery)
        {
            currentState = GameState.Talking;
            Debug.Log("conversation over!");
            NextState();
        }
        else if (currentState == GameState.Talking)
        {
            currentState = GameState.Upgrade;
            Debug.Log("update over!");
            loopNumber++;
            NextState();
        }
        else if (currentState == GameState.Upgrade)
        {
            currentState = GameState.Pickup;
            Debug.Log("Loop: " + loopNumber);
            SpawnGoal();
        }
    }

    void SpawnGoal()
    {
        Vector3 position;

        if (currentState == GameState.Pickup)
        {
            position = new Vector3(5, 0, 0);
        }
        else
        {
            position = new Vector3(-5, 0, 0);
        }

        currentGoal = Instantiate(goalPrefab, position, Quaternion.identity);
}



}

