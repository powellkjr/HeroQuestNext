using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingProgressBar : MonoBehaviour
{
    private Image iLoadingForeGround;

    private void Awake()
    {
        iLoadingForeGround = transform.GetComponent<Image>();

    }

    // Update is called once per frame
    private void Update()
    {
        iLoadingForeGround.fillAmount = Loader.GetLoadingProgress();
    }
}
