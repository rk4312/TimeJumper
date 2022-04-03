using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyListBank : BaseListBank
{
    private string[] _contents = { 
        "Level 1", "Level 2", "Level 3"
    };

    public override string GetListContent(int index)
    {
        return _contents[index];
    }

    public override int GetListLength()
    {
        return _contents.Length;
    }
}
