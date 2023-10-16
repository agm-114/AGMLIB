using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSlider : MonoBehaviour
{
    // Start is called before the first frame update
    RectTransform rectTransform;
    float pos = 0;
    void Start()
    {
        rectTransform = transform as RectTransform;
    }

    // Update is called once per frame
    void Update()
    {
        pos += 0.01f;
        //rectTransform.anchoredPosition = new Vector2(0, 0);

        //rectTransform.offsetMax = new Vector2(0, 0);
        //rectTransform.offsetMax = new Vector2(0, 0);
        //rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax.Set(pos, 1);
        //rectTransform.anchorMax = new Vector2(pos, 1);
        //rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }
}
