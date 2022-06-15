using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggerTool : MonoBehaviour
{
    [SerializeField] Light Sunlight;
    // Start is called before the first frame update
    void Start()
    {
        //ToggleFog();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            ToggleFog();
        if (Input.GetKeyDown(KeyCode.K))
            ToggleSunlight();
    }

    private void ToggleFog()
    {
        RenderSettings.fog = !RenderSettings.fog;
    }

    private void ToggleSunlight()
    {
        if(Sunlight != null)
        {
            Sunlight.enabled = !Sunlight.enabled;
        }
    }
}
