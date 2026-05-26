using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float speed = 50;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        if (player != null)
            FixCameraPos();
    }

    void FixCameraPos()
    {
        if (player == null) return;
        float pPosX = player.transform.position.x;
        float cPosX = transform.position.x;
        float pPosY = player.transform.position.y;
        float cPosY = transform.position.y;

        float threshold = 1;
        float newPosX = transform.position.x;
        float newPosY = transform.position.y;

        if (pPosX - cPosX > threshold)
        {
            newPosX = cPosX + speed * Time.deltaTime;
        }
        else if (pPosX - cPosX < -threshold)
        {
            newPosX = cPosX - speed * Time.deltaTime;
        }
        else if (pPosY - cPosY > threshold)
        {
            newPosY = cPosY + speed * Time.deltaTime;
        }
        else if (pPosY - cPosY < -threshold)
        {
            newPosY = cPosY - speed * Time.deltaTime;
        }

        transform.position = new Vector3(newPosX, newPosY, transform.position.z);
    }
}
