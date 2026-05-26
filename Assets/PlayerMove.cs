using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float wizardMovespeed = 5f;
    public Rigidbody2D wizardRB;

    void Start()
    {
        wizardRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (wizardRB == null) return;
        float horizontalNum = Input.GetAxis("Horizontal");
        float verticalNum = Input.GetAxis("Vertical");
        wizardRB.velocity = new Vector2(wizardMovespeed * horizontalNum, wizardMovespeed * verticalNum);
    }
}
