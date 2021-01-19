using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    private float timeStarted;
    private bool gameOver;
    
    private int taskCount;
    // how much time has elapsed since the start of game (in case there is an opening scene its not equal to Time.time)
    public float elapsedTime { get
        {
            if (timeStarted == -1)
                timeStarted = Time.time;
            return Time.time - timeStarted;
        }
    }
    private Dictionary<GameObject, Item> goToItem;
    private Dictionary<GameObject, Task> goToTask;
    private ArrayList wallBounds;
    private ArrayList fireBounds;
    private PlayerScript playerScript;

    
    public GameManager()
    {
        goToItem = new Dictionary<GameObject, Item>();
        goToTask = new Dictionary<GameObject, Task>();
        timeStarted = -1;
        gameOver = false;
        wallBounds = new ArrayList();
        fireBounds = new ArrayList();
        taskCount = 0;
    }
    
    public PlayerScript getPlayerScript()
    {
        return playerScript;
    }

    public void setPlayerScript(PlayerScript player)
    {
        playerScript = player;
    }

    public void addWallBounds(Bounds wallBound)
    {
        wallBounds.Add(wallBound);
    }

    public void addFire(Bounds fireBound)
    {
        fireBounds.Add(fireBound);
    }

    public bool isInFireOrWall(Bounds other)
    {
        foreach (Bounds bounds in wallBounds)
        {
            if (other.Intersects(bounds))
                return true;
        }
        foreach (Bounds bounds in fireBounds)
        {
            if (other.Intersects(bounds))
                return true;
        }

        return false;
    }

    public void addItem(Item item)
    {
        if (item.forTaskType == TaskType.Trash)
            taskCount++;
        goToItem.Add(item.gameObject, item);
    }

    public void addTask(Task task)
    {
        if(task.type != TaskType.Trash)
            taskCount++;
        goToTask.Add(task.gameObject, task);
    }

    public void removeTask(Task task)
    {
        goToTask.Remove(task.gameObject);
    }

    public Item getItem(GameObject go)
    {
        if (!goToItem.ContainsKey(go))
            return null;
        return goToItem[go];
    }
    
    public void removeItem(Item item)
    {
        goToItem.Remove(item.gameObject);
    }

    public Task getTask(GameObject go)
    {
        if (!goToTask.ContainsKey(go))
            return null;
        return goToTask[go];
    }

    public void timeOver()
    {
        Debug.Log("Game over!  You lost");
        gameOver = true;
        endGame();
    }
    
    public void endGame()
    {
        gameOver = false;
        timeStarted = -1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void areTasksOver()
    {
        if (taskCount <= 0)
        {
            Debug.Log("Game over! You Won");
            endGame();
        }
    }

    public void FinishTask(Task curTask)
    {
        //progressBar.enabled = false;
        
        //Destroy(holdingItem.gameObject);
        //holdingItem.transform.parent = null;
        //holdingItem = null;
        taskCount--;
        areTasksOver();
    }
}
