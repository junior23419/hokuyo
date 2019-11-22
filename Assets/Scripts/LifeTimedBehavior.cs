using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTimedBehavior : MonoBehaviour
{
    public float life;
    float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > life)
            Destroy(gameObject);
    }
}
