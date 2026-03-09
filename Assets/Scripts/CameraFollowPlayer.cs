using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform playerPosition;

    public void Initiate()
    {
        playerPosition = GameObject.Find("Player").GetComponent<Transform>();
        transform.position = new Vector3(playerPosition.position.x, playerPosition.position.y, transform.position.z);
    }

    void Update()
    {
        var newPosition = new Vector3(playerPosition.position.x, playerPosition.position.y, transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, newPosition, Vector3.Distance(transform.position, newPosition) * Time.deltaTime * 3);
    }
}
