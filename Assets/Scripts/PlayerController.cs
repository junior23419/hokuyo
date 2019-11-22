using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerManager playerManager;
    RectTransform rt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        if(playerManager == null)
        {
            playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        }
        if(rt == null)
        {
            rt = GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerManager.CastRay(rt.position);
    }
}
