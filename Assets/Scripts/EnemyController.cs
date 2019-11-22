using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [SerializeField] float cubeGap;
    [SerializeField] GameObject cubePrefab;
    [SerializeField] GameObject center;
    [SerializeField] long size;
    public bool isGameReady = false;

    float xRotateRate = 0;
    float yRotateRate = 0;
    float zRotateRate = 0;
    // Start is called before the first frame update
    void Start()
    {
        CreateEnemy();
    }

    // Update is called once per frame

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Transform child in center.transform)
            {
                Destroy(child.gameObject);
            }
            CreateEnemy();
        }
    }

    private void FixedUpdate()
    {
        if(isGameReady)
        {
            xRotateRate = Mathf.PingPong(Time.time, 2f);
            yRotateRate = Mathf.PingPong(Time.time, 2f);
            zRotateRate = Mathf.PingPong(Time.time, 3f);

            Vector3 rotateDir = new Vector3(xRotateRate, yRotateRate, zRotateRate) * Mathf.Sin(Time.time);
            //Vector3 rotateDir = new Vector3(xRotateRate, yRotateRate, zRotateRate)  * 10;
            center.transform.Rotate(rotateDir);
        }
        
    }

    public void CreateEnemy()
    {
        isInitialized = false;
        cubeCount = 0;
        doneCount = 0;
        for (int z=0;z<size;z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if ((x == 0 || x == size - 1) || (y == 0||y==size-1)|| (z==0||z==size-1))
                    {
                        cubeCount++;
                        GameObject cube = Instantiate(cubePrefab);
                        cube.transform.SetParent(center.transform);
                        
                        cube.GetComponent<MeshRenderer>().material.color = colors[Random.Range(0, colors.Length)];
                        Vector3 tar = new Vector3(x, y, z) * cubeGap;
                        float half = size / 2;
                        tar -= new Vector3(half, half, half) * cubeGap;
                        CubeController cubeController = cube.GetComponent<CubeController>();
                        float fluctuateX = Random.Range(-5f, 5f);
                        cube.transform.localPosition = Camera.main.transform.position + new Vector3(fluctuateX,0,-1);
                        cubeController.SetTarget(tar);
                        cubeController.StartMove();
                        
                    }
                    
            }
            }
        }

        isInitialized = true;
    }
    bool isInitialized = false;
    int cubeCount = 0;
    int doneCount = 0;

    public void NotifyDone()
    {
        doneCount++;
        if(doneCount >= cubeCount && isInitialized)
        {
            isGameReady = true;
        }
    }
}
