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
        if (Input.GetKeyDown(KeyCode.Space))
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
        if (isGameReady)
        {
            xRotateRate = Mathf.PingPong(Time.time, 2f);
            yRotateRate = Mathf.PingPong(Time.time, 2f);
            zRotateRate = Mathf.PingPong(Time.time, 3f);

            Vector3 rotateDir = new Vector3(xRotateRate, yRotateRate, zRotateRate) * Mathf.Sin(Time.time);
            //Vector3 rotateDir = new Vector3(xRotateRate, yRotateRate, zRotateRate)  * 10;
            center.transform.Rotate(rotateDir);
        }

    }

    //hash by side

    List<CubeController>[] tables; //mod result 0
    // x -x y -y z -z
    public List<GameObject> FindNeighbourCubes(int x, int y, int z)
    {
        int count = 0;
        List<GameObject> ret;
        ret = new List<GameObject>();

        int side = FindSide(x, y, z);

        foreach (CubeController cube in tables[side])
        {
            if (((cube.y == y - 1 || cube.y == y + 1) || (cube.z == z - 1 || cube.z == z + 1) || (cube.x == x - 1 || cube.x == x + 1)) )
            {
                ret.Add(cube.gameObject);
                count++;
                if (count == 3)
                {
                    return ret;
                }
            }
            
        }

        return ret;
    }

    public void CreateEnemy()
    {
        tables = new List<CubeController>[6];
        isInitialized = false;
        cubeCount = 0;
        doneCount = 0;
        int colorSize = Mathf.CeilToInt(size / 2) + 1;
        Debug.Log("color size" + colorSize);
        for (int z=0;z<size;z++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if ((x == 0 || x == size - 1) || (y == 0 || y == size - 1) || (z == 0 || z == size - 1))
                    {
                        cubeCount++;
                        GameObject cube = Instantiate(cubePrefab);
                        cube.transform.SetParent(center.transform);

                        

                        int indexColor = 6;

                        for (int i = 0; i < colorSize; i++)
                        {
                            bool isXrim = x == size - 1 - i || x == i;
                            bool isYrim = y == size - 1 - i || y == i;
                            bool isZrim = z == size - 1 - i || z == i;


                            
                            if ((z == 0 || z == size-1) && (isXrim || isYrim)) //front face
                            {
                                
                                indexColor = i;
                                break;
                            }
                            if ((x == 0 || x == size - 1) && (isZrim || isYrim)) //side face
                            {
                                
                                indexColor = i;
                                break;
                            }
                            if ((y == 0 || y == size - 1) && (isXrim || isZrim)) //top face
                            {
                                
                                indexColor = i;
                                break;
                            }


                        }


                        Debug.Log("index" + indexColor);
                        cube.GetComponent<MeshRenderer>().material.color = colors[indexColor];


                        Vector3 tar = (new Vector3(x, y, z) + center.transform.position) * cubeGap;
                        float half = size / 2;
                        tar -= new Vector3(half, half, half) * cubeGap;
                        CubeController cubeController = cube.GetComponent<CubeController>();
                        cubeController.x = x;
                        cubeController.y = y;
                        cubeController.z = z;


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

    private int FindSide(int x,int y,int z)
    {
        return x == 0 ? 0 : x == size - 1 ? 1 : y == 0 ? 2 : y == size - 1 ? 3 : z == 0 ? 4 : 5; ;
    }
}
