using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;

    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { // if left button pressed...
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // the object identified by hit.transform was clicked
                // do whatever you want
                if(hit.transform.tag == "Cube" && enemyController.isGameReady)
                {

                    hit.transform.gameObject.GetComponent<CubeController>().ExplicitDestroy();
                }
            }
        }
    }

    public void CastRay(Vector2 pos)
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("pos:" + pos);
        }
        
    }
}
