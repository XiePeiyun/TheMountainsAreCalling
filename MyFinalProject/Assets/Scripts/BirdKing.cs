using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Collider))]
public class BirdKing : MonoBehaviour
{
    [Header("Camera")]
    public Camera activeCamera;  // BirdCamera 会在切换时赋值
    public BirdCamera birdCamera;



    [Header("References")]
    public StoryButton familyStory; // 对应 FamilyStory 按钮
    public StoryButton gameStory;   // 对应 GameStory 按钮
    public SkyboxController skyboxController;
    public GridManager gridManager;
    public Pray pray;
    public Hunted hunted;
    public Disaster disaster;
    public BirdsGroup birdsGroup;
    public Scout scout;

    public Scout scoutButton;
    public Pray prayButton;
    public Move moveButton;
    public NextTurn nextTurnButton;
    public Calling callingButton;
    public Money moneyButton;
    public DiceImage3 diceImage3;


    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stepMoveDelay = 0.15f;
    public Color colorCanMove = Color.green;
    public Color colorCannotMove = Color.red;
    public Color colorNormal = Color.white;
    private bool isMoving = false;

    [Header("Game State")]
    public GameState currentState = GameState.Preparation;
    public int currentTurn = 0;
    public event Action<int> OnTurnChanged;
    public event System.Action<BirdKing.GameState> OnStateChanged;

    [Header("Status")]
    private GridCell currentCell;
    private GridCell hoveredCell;



    [Header("Steps")]
    public int hoveredNeededSteps = 0; //MouseGuideStep
    public int diceSteps = 0; //PrayWeather Steps ThisTurnTotalStep
    public int costSteps = 0; //BirdKing alreadymoveStep
    public int restSteps = 0; //thisTurn rest step
    public string currentWeather = "";

    public enum GameState
    {
        Preparation,    // UI click ：Money, Calling, Scout, Pray
        Movement,       // Map move 
        Ended           
    }

    private void Start()
    {
        //初始化格子
        GridCell startCell = gridManager.GetCellFromWorldPos(transform.position);


        if (startCell != null)
        {
            currentCell = startCell;
            transform.position = new Vector3(
                currentCell.transform.position.x,
                transform.position.y,
                currentCell.transform.position.z
            );
        }
        // FirstTurn
        StartNewTurn();

    }

    private void Update()
    {
        if (isMoving) return;

        // Mouse for Movement
        if (currentState == GameState.Movement)
        {
            HandleMouseHover();
            HandleMouseClick();
        }
    }

    // BirdsGroup Information
    public void SyncStepsFromPray()
    {

        diceSteps = pray.stepsResult;
        currentWeather = pray.weatherResult;
        CalculateRestSteps();

        //Debug.Log($"stepsync | 天气: {currentWeather} | 总步数: {diceSteps} | 已用: {costSteps} | 剩余: {restSteps}");
    
    }



    // CalculateRestSteps
    private void CalculateRestSteps()
    {
        restSteps = Mathf.Max(diceSteps - costSteps, 0);
    }


    // check reststeps
    public bool HasRemainingSteps()
    {
        return restSteps > 0;
    }


    //check distance
    public bool CanMoveToCell(GridCell targetCell)
    {
        if (targetCell == null || !targetCell.isWalkable) return false;

        int neededSteps = gridManager.GetPathStepCount(currentCell, targetCell);

        if (neededSteps == int.MaxValue) return false;

        return neededSteps <= restSteps;
    }




    private void HandleMouseHover()
    {
        GridCell hitCell = GetCellUnderMouse();
        if (hitCell == hoveredCell) return;

        // last one
        if (hoveredCell != null)
        {
            hoveredCell.SetHighlight(null);
        }
        hoveredCell = hitCell;

        if (hoveredCell == null)
        {
            hoveredNeededSteps = 0; // 没有悬停格子就设为0
            return;
        }

        // distance to step
        float dist = gridManager.GetDistance(currentCell, hoveredCell);
        int neededSteps = Mathf.CeilToInt(dist); //（1 cell≈1step）
        hoveredNeededSteps = neededSteps; // save


        // check reach 
        bool canReach = neededSteps <= restSteps && hoveredCell.isWalkable;

        // color set
        hoveredCell.SetHighlight(canReach ? colorCanMove : colorCannotMove);

        if (canReach)
        {
            Debug.Log($"canmove {hoveredCell.name} | 需要 {neededSteps}步 | 剩余 {restSteps}步");
        }

    }


    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridCell clickCell = GetCellUnderMouse();
            if (clickCell == null || clickCell == currentCell) return;

            if (!HasRemainingSteps())
            {
                Debug.Log("No more Steps！");
                return;
            }

            float dist = gridManager.GetDistance(currentCell, clickCell);
            int neededSteps = Mathf.CeilToInt(dist);
            bool canReach = neededSteps <= restSteps && clickCell.isWalkable;

            if (canReach)
            {
                MoveToCell(clickCell);
            }
            else
            {
                Debug.Log("Over steps");
            }
        }
    }

    // Raycast check mouse point cell
    private GridCell GetCellUnderMouse()
    {
        if (activeCamera == null)
        {
            Debug.LogError("BirdKing.activeCamera is NULL! BirdCamera did not assign it.");
            return null;
        }

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponent<GridCell>();
        }
        return null;

    }


    //move
    public void MoveToCell(GridCell targetCell)
    {
        if (isMoving) return;

        List<GridCell> path = gridManager.FindPath(currentCell, targetCell);
        if (path == null || path.Count == 0)
        {
            Debug.Log("无法到达目标位置");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(MoveAlongPath(path));
    }


    public IEnumerator MoveAlongPath(List<GridCell> path)
    {
        if (path == null || path.Count == 0)
            yield break;

        isMoving = true;

        // 遍历路径中的每个格子
        foreach (GridCell cell in path)
        {
            if (cell == null) continue;

            // 不能经过 Desert
            if (!cell.isWalkable && !cell.isCrossable)
            {
                Debug.Log($"无法经过 {cell.cellType} 格子 {cell.gridPos}");
                continue;
            }

            Vector3 targetPos = new Vector3(cell.transform.position.x, transform.position.y, cell.transform.position.z);
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentCell = cell;
            yield return new WaitForSeconds(0.05f); // 稍作停顿
        }


        // step（distance - 1，因为起点不算步）
        costSteps += Mathf.Max(path.Count - 1, 0);
        CalculateRestSteps();

        Debug.Log($"路径移动完成 | 消耗步数: {path.Count - 1} | 剩余步数: {restSteps}");

        isMoving = false;

        // 移动结束后
        if (birdsGroup != null && currentCell != null)
        {
            if (currentCell.isSafe)
            {
                // 安全格 → Forest
                birdsGroup.UpdateScoutAfterMoveForest();
            }
            else
            {
                // 不安全格 → City
                Hunted.HuntedOption huntedOption = hunted != null ? hunted.RollHuntedOption() : null;
                Disaster.DisasterOption disasterOption = disaster != null ? disaster.RollDisasterOption() : null;

                birdsGroup.UpdateScoutAfterMove(huntedOption, disasterOption);
            }
        }

        ////移动结束后触发 Dice
        //if (!currentCell.isSafe)
        //{
        //    if (birdsGroup != null)
        //    {
        //        Hunted.HuntedOption huntedOption = hunted != null ? hunted.RollHuntedOption() : null;
        //        Disaster.DisasterOption disasterOption = disaster != null ? disaster.RollDisasterOption() : null;
        //        birdsGroup.UpdateScoutAfterMove(huntedOption, disasterOption);

        //        //birdsGroup.UpdateScoutAfterMove(huntedOption, disasterOption, currentCell);
        //    }
        //}


    }


    /// newTurn reset

    public void StartNewTurn()
    {
        Debug.Log("====== NewTurn ======");

        //rest steps

        costSteps = 0;
        diceSteps = 0;
        restSteps = 0;
        currentWeather = "";

        currentState = GameState.Preparation;

        // reset button
        if (pray != null)
            pray.ResetPrayButton();

        if (moveButton != null)
            moveButton.ResetMoveButton();

        if (nextTurnButton != null)
            nextTurnButton.ResetNextTurnButton();

        if (scoutButton != null)
            scoutButton.ResetScoutButton();

        if (moneyButton != null)
        {
            moneyButton.ResetMoneyButton();
            moneyButton.gameObject.SetActive(true);
        }

        //reset BirdsGroup scout
        if (birdsGroup != null)
        {
            birdsGroup.scoutGroup = 0;
            birdsGroup.mainGroup = birdsGroup.totalBirds;
            birdsGroup.UpdateFormula();
        }

        Debug.Log($"Prepare | Status: {currentState}");
        Debug.Log("Already Reset");


        skyboxController?.SetNightSky();
        OnStateChanged?.Invoke(currentState);

        // Preparation阶段：显示camera2
        birdCamera?.OnPreparationPhase();
        activeCamera = birdCamera.camera2;


        // Preparation阶段显示按钮
        if (familyStory != null) familyStory.SetButtonActive(true);
        if (gameStory != null) gameStory.SetButtonActive(true);
    }

    //movement
    public void StartMovementPhase()
    {

        if (currentState != GameState.Preparation)
        {
            Debug.LogWarning("Only can from prepare to movement");
            return;
        }


        if (!HasValidDiceResult())
        {
            Debug.LogWarning("press pray first");
            return;
        }

        if (birdsGroup == null || birdsGroup.scoutGroup <= 0)
        {
            Debug.LogWarning("set scout first");
            return;
        }

        currentState = GameState.Movement;

        Debug.Log($"Movement | restStep: {restSteps} | Scouts: {birdsGroup.scoutGroup}");

        // 通知 diceImage3 开始播放
        if (diceImage3 != null)
        {
            diceImage3.StartBirdKingAnimation();
        }


        if (moneyButton != null)
            moneyButton.gameObject.SetActive(false);

        // 移动阶段隐藏
        if (familyStory != null) familyStory.SetButtonActive(false);
        if (gameStory != null) gameStory.SetButtonActive(false);

        OnStateChanged?.Invoke(currentState);

        // Preparation -> Movement: camera2平滑移动到camera1FollowPos，然后切换camera1
        birdCamera?.OnMovementPhase();
        activeCamera = birdCamera.camera1;



    }

    // check move button
    public void OnMoveButtonPressed(Move moveButtonScript)
    {
        
        if (currentState == GameState.Movement)
        {
            Debug.Log("already in movement");
            return;
        }

        if (currentState != GameState.Preparation)
        {
            Debug.LogWarning("can't in movement");
            return;
        }

        if (!HasValidDiceResult())
        {
            Debug.LogWarning("Pray first");
            return;
        }

        if (birdsGroup == null || birdsGroup.scoutGroup <= 0)
        {
            Debug.LogWarning("Scout first");
            return;
        }

        
        StartMovementPhase();
        Debug.Log("click cell move");


    }

    // UIbutton for movement
    public void OnMoveButtonClicked()
    {

        if (currentState == GameState.Movement)
        {
            Debug.Log("alredy movement");
            return;
        }

        // 如果当前状态不是准备阶段，不允许移动
        if (currentState != GameState.Preparation)
        {
            Debug.LogWarning("can't move");
            return;
        }

        // 检查是否有祈祷结果
        if (!HasValidDiceResult())
        {
            Debug.LogWarning("Pray first");
            return;
        }

        // 检查是否有侦察部队
        if (birdsGroup == null || birdsGroup.scoutGroup <= 0)
        {
            Debug.LogWarning("Scout first");
            return;
        }

        // 启动移动阶段
        StartMovementPhase();

        // 可选：给出UI提示
        Debug.Log("click cell move");
    }

    /// end
    public void EndMovementPhase()
    {
        if (currentState == GameState.Movement)
        {
            currentState = GameState.Ended;
            Debug.Log($"end movement | cost: {costSteps} | rest: {restSteps}");

        }


    }



    /// 结束当前回合
    public void EndTurn()
    {
        currentState = GameState.Ended;
        Debug.Log($"end | total: {diceSteps} | use: {costSteps} | rest: {restSteps}");

        currentTurn++;
        Debug.Log($"NextTurn: 当前回合 = {currentTurn}");

        OnTurnChanged?.Invoke(currentTurn);  // 通知 Story 或其他监听者

        if (birdsGroup != null)
        {
            birdsGroup.MergeGroups();
        }


        if (callingButton != null)
        {
            callingButton.OnTurnEnded();
        }

        // 通知 diceImage3 停止播放
        if (diceImage3 != null)
        {
            diceImage3.StopBirdKingAnimation();
        }

        if (moneyButton != null)
            moneyButton.AddTurnReward();

        if (birdsGroup != null)
            birdsGroup.ResetWeatherResult();
        // 新回合
        StartNewTurn();

    }

    /// 检查是否有有效的骰子结果
    public bool HasValidDiceResult()
    {
        return pray != null && pray.stepsResult > 0 && !string.IsNullOrEmpty(pray.weatherResult);
    }



    // 外部可以调用：手动设置已消耗步数（如果需要）
    public void SetCostSteps(int cost)
    {
        costSteps = Mathf.Max(cost, 0);
        CalculateRestSteps();
    }

    // 外部可以调用：获取当前步数状态
    public void GetStepStatus(out int totalSteps, out int consumedSteps, out int remainingSteps)
    {
        totalSteps = diceSteps;
        consumedSteps = costSteps;
        remainingSteps = restSteps;
    }

    //获取当前游戏状态
    public GameState GetCurrentState()
    {
        return currentState;
    }

    //检查是否在移动阶段
    public bool IsInMovementPhase()
    {
        return currentState == GameState.Movement;
    }

    /// 强制同步 BirdsGroup 的步数（当外部修改 BirdsGroup.diceSteps 时调用）
    public void ForceSyncSteps()
    {
        SyncStepsFromPray();
    }


    //监听祈祷完成事件
    public void OnPrayCompleted()
    {
        Debug.Log("finish pray, data back");
        SyncStepsFromPray();
    }

}