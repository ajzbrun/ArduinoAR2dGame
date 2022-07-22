using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    GameManager gm;
    public float vel;

    void Start()
    {
        gm = GameObject.Find("Main Camera").GetComponent<GameManager>();
    }

    void Update()
    {
        if (transform.position.x < -11)
        {
            gm.IncrementScore();
            Destroy(gameObject);
        }
        transform.position = new Vector3(transform.position.x - (6f * Time.deltaTime * (gm.GetGameSpeed() * vel)), transform.position.y);
    }
}
