using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField] GameObject[] effectPrefabs;
    bool isMoving = false;
    [SerializeField] float speedFactor;
    float speed;
    // Start is called before the first frame update
    Vector3 target;
    Vector3 direction;
    // Update is called once per frame

    EnemyController enemyController;

    private void Start()
    {
        enemyController = GameObject.Find("EnemyController").GetComponent<EnemyController>();
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            transform.position += direction * Time.deltaTime * speed;
            if(Vector3.Distance(transform.position,target)<1f)
            {
                gameObject.transform.position = target;
                isMoving = false;
                enemyController.NotifyDone();
            }
        }
    }

    private void OnDestroy()
    {
        GameObject fx = Instantiate(effectPrefabs[Random.Range(0, effectPrefabs.Length)]);
        fx.transform.position = transform.position;
    }

    public void SetTarget(Vector3 val)
    {
        target = val;
        direction = val - transform.position;
        speed = Random.Range(0.5f, 1f) * speedFactor;
    }

    public void StartMove()
    {
        StartCoroutine(WaitAndMove());

        IEnumerator WaitAndMove()
        {
            yield return new WaitForSeconds(0.1f);
            isMoving = true;
        }
    }
}
