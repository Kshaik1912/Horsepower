using UnityEngine;

public class GameManager : MonoBehaviour

{
    public enum GameState
    {
        Pickup,
        Delivery,
        Talking,
        Upgrade,
        GameOver
    }  

    public GameState currentState = GameState.Pickup;
    public int loopNumber = 1;
    public GameObject goalPrefab;
    private GameObject currentGoal;
    public float timeRemaining;
    private float previousDeliveryTime;
    private float previousDeliveryDistance;
    private Vector3 previousDeliveryPosition;
    private Vector3 pickupPosition;
    private Vector3 deliveryPosition;
    private bool timerActive = false;   
    private int failureCount = 0;
    private bool skipPickupTimer = false; 


    public Transform[] cityPizzas;
    public Transform[] suburbPizzas;
    public Transform[] countryPizzas;

    public Transform[] cityHouses;
    public Transform[] suburbHouses;
    public Transform[] countryHouses;

    private int currentArea = 0;
    private int sameAreaCount = 0;
    private bool differentPickupArea = false;
    private int deliveryArea = 0;



    void Start()
    {
        currentState = GameState.Pickup;
        Debug.Log("Loop: " + loopNumber);
        Debug.Log("Current State: " + currentState);

        SpawnGoal();

    }

    void Update()
    {
        if (timerActive)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerActive = false;
                Debug.Log("Time ran out!");
                HandleFailure();
            }
        }
    }


    void StartPickupTimer()
    {
        
        if (loopNumber == 1 || skipPickupTimer)
        {
            timerActive = false;
            timeRemaining = 0f;
            skipPickupTimer = false;
            return;
        }


        float distance = Vector3.Distance(previousDeliveryPosition, pickupPosition);

        timeRemaining = distance + 30f;

        if (differentPickupArea)
        {
            timeRemaining += 40f;
        }

        timerActive = true;

        Debug.Log("Pickup Timer: " + timeRemaining);

    }


    void StartDeliveryTimer()
    {
        float distance = Vector3.Distance(pickupPosition, deliveryPosition);

        if (loopNumber == 1)
        {
            timeRemaining = distance + 30f;
        }
        else
        {
            timeRemaining = previousDeliveryTime;

            if (distance < previousDeliveryDistance)
            {
                timeRemaining -= 3f;
            }
        }

        previousDeliveryDistance = distance;
        previousDeliveryTime = timeRemaining;
        timerActive = true;

        Debug.Log("Delivery Timer: " + timeRemaining);

    }


    void HandleFailure()
    {
        failureCount++;

        if (failureCount == 1)
        {
            Debug.Log("Warning! Delivery failed.");

            if (currentGoal != null)
            {
                Destroy(currentGoal);
            }

            skipPickupTimer = true;
            loopNumber++;
            currentState = GameState.Pickup;
            SpawnGoal();
        }
        else
        {
            Debug.Log("Game Over!");
            currentState = GameState.GameOver;

            if (currentGoal != null)
            {
                Destroy(currentGoal);
            }


        }


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
            timerActive = false;
            previousDeliveryPosition = deliveryPosition;

            if (currentArea != deliveryArea)
            {
                sameAreaCount = 0;
            }

            currentArea = deliveryArea;
            currentState = GameState.Talking;
        }

        else if (currentState == GameState.Talking)
        {
            Debug.Log("conversation over!");
            currentState = GameState.Upgrade;
            NextState();
        }

        else if (currentState == GameState.Upgrade)
        {
            Debug.Log("update over!");
            loopNumber++;
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
            ChoosePickupArea();
            position = GetRandomPickupPosition();
        }
        else
        {
            position = GetRandomDeliveryPosition();
        }

        currentGoal = Instantiate(goalPrefab, position, Quaternion.identity);

        if (currentState == GameState.Pickup)
        {
            pickupPosition = position;
            StartPickupTimer();
        }

        
        else if (currentState == GameState.Delivery)
        {
            deliveryPosition = position;
            SetDeliveryArea(position);
            StartDeliveryTimer();
        }




    }



    Vector3 GetRandomPickupPosition()
    {
        Transform[] locations;

        if (currentArea == 0)
        {
            locations = cityPizzas;
        }
        else if (currentArea == 1)
        {
            locations = suburbPizzas;
        }
        else
        {
            locations = countryPizzas;
        }

        int randomIndex = Random.Range(0, locations.Length);
        return locations[randomIndex].position;
    }

    
    Vector3 GetRandomDeliveryPosition()
    {
        if (loopNumber == 1)
        {
            int index = Random.Range(0, cityHouses.Length);
            return cityHouses[index].position;
        }

        Transform[] allHouses = new Transform[
            cityHouses.Length + suburbHouses.Length + countryHouses.Length
        ];

        cityHouses.CopyTo(allHouses, 0);
        suburbHouses.CopyTo(allHouses, cityHouses.Length);
        countryHouses.CopyTo(allHouses, cityHouses.Length + suburbHouses.Length);

        
        int validCount = 0;

        for (int i = 0; i < allHouses.Length; i++)
        {
            float distance = Vector3.Distance(
                pickupPosition, allHouses[i].position
            );

            if (Mathf.Abs(distance - previousDeliveryDistance) <= 10f)
            {
                validCount++;
            }
        }

        if (validCount > 0)
        {
            int selected = Random.Range(0, validCount);

            for (int i = 0; i < allHouses.Length; i++)
            {
                float distance = Vector3.Distance(
                    pickupPosition, allHouses[i].position
                );

                if (Mathf.Abs(distance - previousDeliveryDistance) <= 10f)
                {
                    if (selected == 0)
                    {
                        return allHouses[i].position;
                    }

                    selected--;
                }
            }
        }

        
        int closest = 0;
        float smallestDifference = Mathf.Infinity;

        for (int i = 0; i < allHouses.Length; i++)
        {
            float distance = Vector3.Distance(
                pickupPosition, allHouses[i].position
            );

            float difference = Mathf.Abs(distance - previousDeliveryDistance);

            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                closest = i;
            }
        }

        Debug.LogWarning("No delivery location meets the distance rule.");
        return allHouses[closest].position;

    }


    void SetDeliveryArea(Vector3 position)
    {
        deliveryArea = 0;

        foreach (Transform house in suburbHouses)
        {
            if (house.position == position)
            {
                deliveryArea = 1;
                return;
            }
        }

        foreach (Transform house in countryHouses)
        {
            if (house.position == position)
            {
                deliveryArea = 2;
                return;
            }
        }
    }


    void ChoosePickupArea()
    {
        int oldArea = currentArea;

        if (loopNumber == 1)
        {
            currentArea = 0;
            sameAreaCount = 0;
            differentPickupArea = false;
            return;
        }

        int chance = Mathf.Min(sameAreaCount * 10, 100);
        int random = Random.Range(0, 100);

        if (random < chance)
        {
            int newArea = Random.Range(0, 3);

            while (newArea == currentArea)
            {
                newArea = Random.Range(0, 3);
            }

            currentArea = newArea;
            sameAreaCount = 0;
        }
        else
        {
            sameAreaCount++;
        }

        differentPickupArea = (currentArea != oldArea);

    }







}

