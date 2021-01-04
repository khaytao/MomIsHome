using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Dictionary<GameObject, Item> goToItem;
    private Dictionary<GameObject, Task> goToTask;
    public GameManager()
    {
        goToItem = new Dictionary<GameObject, Item>();
        goToTask = new Dictionary<GameObject, Task>();
    }

    public void addItem(Item item)
    {
        goToItem.Add(item.gameObject, item);
    }

    public void addTask(Task task)
    {
        goToTask.Add(task.gameObject, task);
    }

    public Item getItem(GameObject go)
    {
        return goToItem[go];
    }

    public Task getTask(GameObject go)
    {
        return goToTask[go];
    }

    public bool isGameOver()
    {
        return (goToTask.Count <= 0);
    }
}
