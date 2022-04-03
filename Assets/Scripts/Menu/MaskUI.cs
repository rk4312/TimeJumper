using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MaskUI : MonoBehaviour
{
    // Fields
    public bool isText;
    public bool isNormalText;
    public bool isFuture;

    // Start is called before the first frame update
    void Start()
    {
        // Sets the appropriate render queue to the past backgrounds
        if (isFuture == false)
        {
            SwapRenderQueue();
        }
    }

    // Swaps render queues
    public void SwapRenderQueue()
    {
        // Image material render queue
        if (isText == false)
        {
            Material newMat = Instantiate(GetComponent<Image>().material);

            // Changes to appropriate render queue
            if (newMat.renderQueue == 3002)
            {
                newMat.renderQueue = 3000;
            }
            else
            {
                newMat.renderQueue = 3002;
            }

            GetComponent<Image>().material = newMat;
        }
        // Text material render queue
        else
        {
            // TMP material render queue
            if (isNormalText == false)
            {
                Material newMat = Instantiate(GetComponent<TMP_Text>().fontMaterial);

                // Changes to appropriate render queue
                if (newMat.renderQueue == 3002)
                {
                    newMat.renderQueue = 3000;
                }
                else
                {
                    newMat.renderQueue = 3002;
                }

                GetComponent<TMP_Text>().fontMaterial = newMat;
            }
            else
            {
                Material newMat = Instantiate(GetComponent<Text>().material);

                // Changes to appropriate render queue
                if (newMat.renderQueue == 3002)
                {
                    newMat.renderQueue = 3000;
                }
                else
                {
                    newMat.renderQueue = 3002;
                }

                GetComponent<Text>().material = newMat;
            }
        }
    }
}
