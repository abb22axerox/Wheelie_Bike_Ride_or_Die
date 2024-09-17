using UnityEngine;

public class Truck : MonoBehaviour
{
    public float speed = 10.0f;          // Speed at which the truck moves toward the player
    public float despawnDistance = 20.0f; // Distance behind the player at which the truck will be destroyed

    private Transform playerTransform;

    void Start()
    {
        // Find the player in the scene (assuming it has the "Player" tag)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found!");
        }
    }

    void Update()
    {
        // Move the truck towards the player along the negative z-axis
        transform.Translate(0, 0, -speed * Time.deltaTime);

        // Destroy the truck when it is behind the player by despawnDistance
        if (playerTransform != null && transform.position.z < playerTransform.position.z - despawnDistance)
        {
            Destroy(gameObject);
        }
    }
}
