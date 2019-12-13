using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingController : MonoBehaviour
{
    [SerializeField] GameObject Controls;
    [SerializeField] Camera[] cameras;
    bool status = false;
    // Start is called before the first frame update
    void Start()
    {
        cameras[0].enabled = true;
        cameras[1].enabled = false; ;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            status = !status;
            Controls.SetActive(status);
            cameras[0].enabled = !status;
            cameras[1].enabled = status;
        }
    }
}
