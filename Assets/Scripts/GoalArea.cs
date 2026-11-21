using UnityEngine;

public class GoalArea : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<HorseMovement>() != null)
        {
            FindAnyObjectByType<GameManager>().NextState();
        }
    }
}
