using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool bIsFirstUpdate = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (bIsFirstUpdate)
        {
            bIsFirstUpdate = false;
            Loader.LoaderCallback();
        }
    }
}
