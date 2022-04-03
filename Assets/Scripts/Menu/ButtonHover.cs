using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Fields
    // First sprite is normal, second is hover
    public Sprite[] buttonSprites;
    public Color[] buttonColors;
    public int spriteIndex;

    // Start is called before the first frame update
    void Start()
    {
        spriteIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Hover
        if (spriteIndex == 0)
        {
            spriteIndex = 1;

            // Hover sprite
            if (spriteIndex < buttonSprites.Length)
            {
                GetComponent<Image>().sprite = buttonSprites[spriteIndex];
            }

            // Hover text
            if (spriteIndex < buttonColors.Length)
            {
                GetComponentInChildren<Text>().color = buttonColors[spriteIndex];
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Normal
        if (spriteIndex == 1)
        {
            spriteIndex = 0;
            // Hover sprite
            if (spriteIndex < buttonSprites.Length)
            {
                GetComponent<Image>().sprite = buttonSprites[spriteIndex];
            }

            // Hover text
            if (spriteIndex < buttonColors.Length)
            {
                GetComponentInChildren<Text>().color = buttonColors[spriteIndex];
            }
        }
    }
}
