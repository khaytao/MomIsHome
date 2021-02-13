using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseFurnitureSetup : MonoBehaviour
{
    public List<Task> toBreak;

    private HashSet<Task> _toBreak;
    // Start is called before the first frame update
    void Start()
    {
        _toBreak = new HashSet<Task>(toBreak);
        Invoke(nameof(Break), 0.5f);
        MyGameManager.Instance.addLevelToBreakData(toBreak);
    }

    void Break()
    {
        foreach (Task Furniture in _toBreak)
        {
            if (Furniture.isFurniture)
            {
                Furniture.breakFurniture();
            }
        }
    }
}
