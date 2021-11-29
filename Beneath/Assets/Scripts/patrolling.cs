using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patrolling : MonoBehaviour
{
    public Transform[] positions;
    private Rigidbody2D body;
    int x=0;
    // Start is called before the first frame update
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, positions[x].position, Time.deltaTime);
        if (transform.position.x == positions[x].position.x && transform.position.y == positions[x].position.y)
        {
            x = (x + 1) % positions.Length;
        }
    }
}
