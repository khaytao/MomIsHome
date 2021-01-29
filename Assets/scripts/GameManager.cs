using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public const int NumOfLevels = 2;
    private float timeStarted;
    public bool gameOver;
    
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
    private List<Bounds> wallBounds;
    private List<(GameObject, Bounds)> fireBounds;
    private List<(Task, Bounds)> furnitureTaskBounds;
    private PlayerScript playerScript;
    private int curLevel = 1;

    
    public GameManager()
    {
        goToItem = new Dictionary<GameObject, Item>();
        goToTask = new Dictionary<GameObject, Task>();
        timeStarted = -1;
        gameOver = false;
        wallBounds = new List<Bounds>();
        fireBounds = new List<(GameObject, Bounds)>();
        furnitureTaskBounds = new List<(Task, Bounds)>();
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

    public void addFire(GameObject fireGO, Bounds fireBound)
    {
        fireBounds.Add((fireGO, fireBound));
        Task furniture = inFurniture(fireBound);
        if(furniture)
            furniture.burnFurniture();
    }

    public void removeFire(GameObject fireGO)
    {
        int i = 0;
        for (; i < fireBounds.Count; i++)
        {
            if (fireGO == fireBounds[i].Item1)
            {
                Destroy(fireGO);
                break;
            }
        }
        fireBounds.RemoveAt(i);
    }

    public bool isInWall(Bounds other)
    {
        foreach (Bounds bounds in wallBounds)
        {
            if (other.Intersects(bounds))
                return true;
        }

        return false;
    }

    public bool isInFire(Bounds other)
    {
        foreach (var (go, bounds) in fireBounds)
        {
            if (other.Intersects(bounds))
                return true;
        }

        return false;
    }

    public void addToTaskCount(int num)
    {
        taskCount += num;
    }

    public void addItem(Item item)
    {
        if (item.forTaskType == TaskType.Trash)
            taskCount++;
        goToItem.Add(item.gameObject, item);
    }

    // returns the furniture bounds is on
    public Task inFurniture(Bounds bounds)
    {
        foreach (var (furn, furnBounds) in furnitureTaskBounds)
        {
            if (furnBounds.Intersects(bounds))
                return furn;
        }

        return null;
    }

    public void addTask(Task task)
    {
        if(task.type != TaskType.Trash && task.type != TaskType.Furniture)
            taskCount++;

        if (task.type == TaskType.Furniture)
        {
            Bounds furnitureBounds = task.GetComponent<SpriteRenderer>().bounds;
            furnitureTaskBounds.Add((task, furnitureBounds));
            if(isInFire(furnitureBounds))
                task.burnFurniture();
        }

        goToTask.Add(task.gameObject, task);
    }

    public void removeTask(Task task)
    {
        playerScript.removeTask(task);
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
        Debug.Log("Game over!  You lost" + Time.timeScale);
        endGame(false);
    }
    
    private void endGame(bool GameWon)
    {
        Time.timeScale = 0;
        gameOver = true;
        if (GameWon)
        {
            FindObjectOfType<CanvasManager>().WonScreenFade();
        }
        else
        {
            FindObjectOfType<CanvasManager>().LostScreen();
        }
        
    }

    public void reloadLevel()
    {
        gameOver = false;
        LoadLevelPrefabs(curLevel);
    }

    public void areTasksOver()
    {
        Debug.Log(taskCount);
        
        if (taskCount <= 1)
        {
            Debug.Log("Game over! You Won");
            taskCount--; //this removes the trash can autamatically
            endGame(true);
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

    public void LoadLevelPrefabs(int levelNum)
    {
        Time.timeScale = 1;
        if (levelNum > NumOfLevels)
        {
            MainMenu();
        }
        
        foreach (var task in goToTask.Keys)
        {
            Destroy(task);
        }
        
        foreach (var item in goToItem.Keys)
        {
            Destroy(item);
        }

        string levelName = "levels/Demolevel" + levelNum.ToString();
        
        GameObject cur = GameObject.FindWithTag("level");
        Destroy(cur);
        GameObject house = GameObject.FindWithTag("House");
        Destroy(house);
        
        GameObject level = Instantiate(Resources.Load(levelName, typeof(GameObject)) as GameObject);
        //Instantiate(Resources.Load("House no walls", typeof(GameObject)) as GameObject);
        CanvasManager.instance.GameScreen();
    }

    IEnumerator LoadScene()
    {
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        LoadLevelPrefabs(1);
        SoundManager.PlaySound(AudioFileGetter.i.BackGroundLevel);
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void NextLevel()
    {
        gameOver = false;
        curLevel++;
        LoadLevelPrefabs(curLevel);
    }

    public void FirstLevel()
    {
        ResetVals();
        StartCoroutine(LoadScene());
    }

    private void ResetVals()
    {
        goToItem = new Dictionary<GameObject, Item>();
        goToTask = new Dictionary<GameObject, Task>();
        timeStarted = -1;
        gameOver = false;
        wallBounds = new List<Bounds>();
        fireBounds = new List<(GameObject, Bounds)>();
        furnitureTaskBounds = new List<(Task, Bounds)>();
        taskCount = 0;
        curLevel = 1;
    }
}
