using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using USPinTable;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
[ExecuteInEditMode]
public class UIC_Manager : MonoBehaviour
{
    // v2.0 - entity list not static to localize UIC to children
    // list of entities in the scene, used to improve performance of detections 

    [HideInInspector] public int animationDuration = 0;
    [HideInInspector] public int GenerateAnimationDuration = 0;

    [HideInInspector] public float currentValue = 0f;
    [HideInInspector] public float GenerateCurrentValue = 0f;
    List<UIC_Entity> _entityList;
    public List<UIC_Entity> EntityList { get => _entityList; }

    // v2.0 - made not static to localize UIC to children
    // list of connections in the scene, used to improve performance of detections
    List<UIC_Connection> _connectionsList;
    public List<UIC_Connection> ConnectionsList { get => _connectionsList; }

    public VideoPinTableManager videoPinTableManager;
    // v2.0 - made not static to localize UIC to children
    public UIC_LineRenderer uiLineRenderer;
    public List<UIC_Line> UILinesList { get => uiLineRenderer.UILines; }
    public UIC_Entity startEntity;
    // v2.0 - made not static to localize UIC to children
    public Canvas canvas;
    public RectTransform canvasRectTransform;
    public VideoPinTableConnector videoPinTableConnector;
    // v2.0 - made not static to localize UIC to children
    // v1.3 - reference to rendermode
    public RenderMode CanvasRenderMode
    {
        get
        {
            if (!canvas)
                canvas = GetComponent<Canvas>();

            return canvas.renderMode;
        }
    }
    public Camera mainCamera;

    // list of selected uic objects, used for single or multi selection
    public List<I_UIC_Selectable> selectedUIObjectsList = new List<I_UIC_Selectable>();
    public I_UIC_Object clickedUIObject;

    public T GetClickedObjectOfType<T>()
    {
        if (clickedUIObject is T)
            return (T)clickedUIObject;
        else
            return default(T);
    }

    // v2.0 - made public and not static
    public UIC_Pointer pointer;

    [Header("Logic")]
    public bool replaceConnectionByReverse;
    // v2.0 - using detect nodes from raycast instead of distance to pointer
    //float _maxPointerDetectionDistance = 30;
    //public float MaxPointerDetectionDistance
    //{
    //    get
    //    {
    //        return _maxPointerDetectionDistance * canvasRectTransform.localScale.x;
    //    }
    //    set
    //    {
    //        _maxPointerDetectionDistance = value;
    //    }
    //}

    [Header("Connection Settings (for new liens)")]
    public bool disableConnectionClick = false;
    public int globalLineWidth = 2;
    public Color globalLineDefaultColor = Color.white;
    public UIC_Connection.LineTypeEnum globalLineType;
    [Header("- line caps")]
    public UIC_Line.CapTypeEnum globalCapStartType;
    public float globalCapStartSize;
    public Color globalCapStartColor;
    public float globalCapStartAngleOffset;
    public UIC_Line.CapTypeEnum globalCapEndType;
    public float globalCapEndSize;
    public Color globalCapEndColor;
    public float globalCapEndAngleOffset;

    [Header("- line animation")]
    public UIC_LineAnimation globalLineAnimation = new UIC_LineAnimation();

    private void OnEnable()
    {
        InitUILineRenderer();
    }

    void OnValidate()
    {
        Awake();
    }

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        pointer = GetComponentInChildren<UIC_Pointer>();
    }

    void Start()
    {
        _entityList = new List<UIC_Entity>();
        UpdateEntityList();
        _connectionsList = new List<UIC_Connection>();

        InitUILineRenderer();
    }

    void InitUILineRenderer()
    {
        uiLineRenderer = GetComponentInChildren<UIC_LineRenderer>();
        if (!uiLineRenderer)
        {
            uiLineRenderer = GetComponentInChildren<UIC_LineRenderer>();

            if (!uiLineRenderer)
            {
                uiLineRenderer = Instantiate(Resources.Load("UIC_LineRenderer") as UIC_LineRenderer, transform.position, Quaternion.identity, transform);
                uiLineRenderer.name = "UIC_LineRenderer";
            }
        }
    }

    public void AddLine(UIC_Line line)
    {
        if (!uiLineRenderer.UILines.Contains(line))
            uiLineRenderer.UILines.Add(line);
    }

    public void RemoveLine(UIC_Line line)
    {
        if (uiLineRenderer.UILines.Contains(line))
            uiLineRenderer.UILines.Remove(line);
    }

    // v2.0 - made not static
    // v1.3 - new method instantiate Entity at position
    public void InstantiateEntityAtPosition(UIC_Entity entityToInstantiate, Vector3 position)
    {
        GameObject go = Instantiate(entityToInstantiate.gameObject, new Vector3(200, 100), Quaternion.identity, canvas.transform);
        AddEntity(go.GetComponent<UIC_Entity>());

        // v1.5 - instantiate entity at a convenient world space position 
        if (CanvasRenderMode == RenderMode.ScreenSpaceOverlay)
        {
            go.transform.position = position + new Vector3(15, 15, 0);
        }
        else if (CanvasRenderMode == RenderMode.ScreenSpaceCamera)
        {
            position.z = 0;
            go.transform.localPosition = position + new Vector3(1, 1, 0);
        }
        else if (CanvasRenderMode == RenderMode.WorldSpace)
        {
            position.z = 0;
            go.transform.localPosition = position + new Vector3(1, 1, 0);
            go.transform.localRotation = Quaternion.identity;
        }
    }

    // v2.0 - made not static
    public void AddEntity(UIC_Entity entityToAdd)
    {
        if (!EntityList.Contains(entityToAdd))
        {
            EntityList.Add(entityToAdd);
        }
    }

    // v2.0 - made not static and get entities in children
    public void UpdateEntityList()
    {
        _entityList = new List<UIC_Entity>();
        _entityList.AddRange(GetComponentsInChildren<UIC_Entity>());
    }

    // v1.3 - UIC_Manager.AddConnection return UIC_Connection
    public UIC_Connection AddConnection(UIC_Node node0, UIC_Node node1, UIC_Connection.LineTypeEnum lineType = UIC_Connection.LineTypeEnum.Spline)
    {
        UIC_Connection previousConnectionWithSameNode = NodesAreConnected(node0, node1);
        if (previousConnectionWithSameNode != null)
        {
            if (replaceConnectionByReverse)
            {
                previousConnectionWithSameNode.Remove();
                return previousConnectionWithSameNode;
            }
            else
            {
                return previousConnectionWithSameNode;
            }
        }

        UIC_Connection _connection = CreateConnection(node0, node1, lineType);
        ConnectionsList.Add(_connection);

        node0.connectionsList.Add(_connection);
        node1.connectionsList.Add(_connection);

        AddLine(_connection.line);

        _connection.line.width = globalLineWidth;
        _connection.line.defaultColor = globalLineDefaultColor;
        _connection.line.color = globalLineDefaultColor;

        _connection.line.SetCap(UIC_Line.CapIDEnum.Start, globalCapStartType, globalCapStartSize, globalCapStartColor, globalCapStartAngleOffset);
        _connection.line.SetCap(UIC_Line.CapIDEnum.End, globalCapEndType, globalCapEndSize, globalCapEndColor, globalCapEndAngleOffset);

        CopyAnimation(globalLineAnimation, _connection.line.animation);

        _connection.UpdateLine();

        return _connection;
    }
    void Update()
    {
        // Check if the right mouse button is pressed down
        if (Input.GetMouseButtonDown(1))
        {
            disableConnectionClick = true;
        }
        // Check if the right mouse button is released
        else if (Input.GetMouseButtonUp(1))
        {
            disableConnectionClick = false;
        }
    }

    // void Update()
    // {
    //     //     if (currentValue < 1f) // Check if the bar is not yet full
    //     //     {
    //     //         currentValue += Time.deltaTime / animationDuration; // Increment the value over time
    //     //         Debug.Log("currentValue: " + currentValue);
    //     //         loadingSlider.value = currentValue; // Apply the new value to the slider
    //     //     }


    // }
    public static void CopyAnimation(UIC_LineAnimation from, UIC_LineAnimation to)
    {
        to.Type = from.Type;
        to.isActive = from.isActive;
        to.pointCount = from.pointCount;
        to.size = from.size;
        to.color = from.color;
        to.DrawType = from.DrawType;
        to.speed = from.speed;
        to.time = from.time;
    }

    // v1.4 - NodesAreConnected verification method made public 
    public UIC_Connection NodesAreConnected(UIC_Node node0, UIC_Node node1)
    {
        foreach (UIC_Connection connection in ConnectionsList)
        {
            if ((node0 == connection.node0 && node1 == connection.node1) ||
                    (node0 == connection.node1 && node1 == connection.node0))
                return connection;
        }
        return null;
    }

    UIC_Connection CreateConnection(UIC_Node node0, UIC_Node node1, UIC_Connection.LineTypeEnum lineType)
    {
        UIC_Connection _connection = new UIC_Connection(node0, node1, lineType);
        _connection.line = new UIC_Line();
        _connection.line.width = 2;

        return _connection;
    }

    public void RemoveUIObject()
    {
        for (int i = selectedUIObjectsList.Count - 1; i >= 0; i--)
        {
            selectedUIObjectsList[i].Remove();
        }
    }
    public void Generate()
    {
        if (EntityList == null || EntityList.Count == 0)
        {
            Debug.Log("No entities found.");
            return;
        }

        // Find the starting entity, assuming it has one node only connected to another node

        if (startEntity == null)
        {
            Debug.Log("No starting entity found with exactly one node and one connection.");
            return;
        }

        List<UIC_Entity> traversalPath = new List<UIC_Entity>();
        Debug.Log("Start Traversal Path:");
        TraverseEntity(startEntity, traversalPath);

        // Log the traversal path
        Debug.Log("Traversal Path:");
        if (traversalPath.Last().name != "EndAnimationBlock")
        {
            return;

        }
        StartCoroutine(ProcessTraversalPath(traversalPath));

    }
    private IEnumerator ProcessTraversalPath(List<UIC_Entity> traversalPath)
    {
        Debug.Log("Traversal Path:");
        currentValue = 0;
        animationDuration = 0;

        foreach (var entity in traversalPath)
        {
            if (entity.name == "TimeAnimationBlock(Clone)")
            {
                string numberString = entity.tMP_Text.text;
                numberString = Regex.Replace(numberString, @"[^\d]", "");
                int pinDuration = 0;

                // Try to parse the numberString to an integer
                bool parsed = int.TryParse(numberString, out int result);
                if (parsed)
                {
                    pinDuration = result;
                }
                else
                {
                    // Log a warning or handle the case where parsing fails
                    Debug.LogWarning($"Failed to parse '{numberString}' as an integer.");
                }

                // Update the animation duration
                animationDuration +=
                 10 + (pinDuration == 0 ? 1 : pinDuration);


            }
        }
        Debug.Log("animationDuration: " + animationDuration);
        float waitTime = 0;

        foreach (var entity in traversalPath)
        {
            String entityName = entity.name;
            Debug.Log(entityName);
            if (entityName == "StartAnimationBlock")
            {
                Debug.Log("start");
            }
            else if (entityName == "EndAnimationBlock")
            {
                Debug.Log("end");
            }
            else if (entityName == "LibraryAnimationBlock(Clone)")
            {
                yield return StartCoroutine(videoPinTableManager.LoadDataFromFile(entity.libraryFileWithoutPlayButton.path, waitTime));

                // Wait until the height transition for all pins is finished
                yield return new WaitUntil(() => videoPinTableManager.AllHeightTransitionsFinished());
            }
            else if (entityName == "TimeAnimationBlock(Clone)")
            {
                string numberString = entity.tMP_Text.text;
                numberString = Regex.Replace(numberString, @"[^\d]", "");
                float pinDuration = 0;

                // Try to parse the numberString to an integer
                bool parsed = float.TryParse(numberString, out float result);
                if (parsed)
                {
                    pinDuration = result;
                }
                else
                {
                    // Log a warning or handle the case where parsing fails
                    Debug.LogWarning($"Failed to parse '{numberString}' as an integer.");
                }

                // Update the animation duration
                waitTime = pinDuration;
            }

            // Wait for a frame to ensure coroutine behavior
            yield return null;
        }
    }

    private void TraverseEntity(UIC_Entity currentEntity, List<UIC_Entity> path, UIC_Node previousNode = null)
    {
        if (currentEntity == null || path.Contains(currentEntity))
            return;

        path.Add(currentEntity);
        UIC_Node currentNode;
        // Get the single node of the current entity
        if (previousNode != null)
        {
            // Debug.Log("Previous node: " + previousNode.entity.name);
            currentNode = currentEntity.nodeList.Find(node => node != previousNode);
            if (currentNode == null)
                return;

            // Debug.Log("Current node: " + currentNode.entity.name);
        }
        else
        {
            currentNode = currentEntity.nodeList[0];

        }


        // Find the connection of this node
        UIC_Connection connection = currentNode.connectionsList.Count > 0 ? currentNode.connectionsList[0] : null;
        // Debug.Log("Current node: " + currentNode.entity.name + ", connection: ");
        if (connection == null)
            return;

        // Get the next node
        UIC_Node nextNode = (connection.node0 == currentNode) ? connection.node1 : connection.node0;
        previousNode = nextNode;
        // Find the entity that contains this next node
        UIC_Entity nextEntity = nextNode.entity;
        // Debug.Log("Next entity: " + nextEntity.name);
        if (nextEntity != null)
        {
            TraverseEntity(nextEntity, path, previousNode);
        }
    }
    public void GenerateToPinTable()
    {
        if (EntityList == null || EntityList.Count == 0)
        {
            Debug.Log("No entities found.");
            return;
        }

        // Find the starting entity, assuming it has one node only connected to another node

        if (startEntity == null)
        {
            Debug.Log("No starting entity found with exactly one node and one connection.");
            return;
        }

        List<UIC_Entity> traversalPath = new List<UIC_Entity>();
        Debug.Log("Start Traversal Path:");
        TraverseEntity(startEntity, traversalPath);

        // Log the traversal path
        Debug.Log("Traversal Path:");
        if (traversalPath.Last().name != "EndAnimationBlock")
        {
            return;

        }
        StartCoroutine(VideoProcessTraversalPath(traversalPath));

    }

    private IEnumerator VideoProcessTraversalPath(List<UIC_Entity> traversalPath)
    {
        Debug.Log("Traversal Path:");
        GenerateCurrentValue = 0;
        GenerateAnimationDuration = 0;

        foreach (var entity in traversalPath)
        {
            if (entity.name == "TimeAnimationBlock(Clone)")
            {
                string numberString = entity.tMP_Text.text;
                numberString = Regex.Replace(numberString, @"[^\d]", "");
                int pinDuration = 0;

                // Try to parse the numberString to an integer
                bool parsed = int.TryParse(numberString, out int result);
                if (parsed)
                {
                    pinDuration = result;
                }
                else
                {
                    // Log a warning or handle the case where parsing fails
                    Debug.LogWarning($"Failed to parse '{numberString}' as an integer.");
                }

                // Update the animation duration
                GenerateAnimationDuration +=
                 1 + (pinDuration < 5 ? 5 : pinDuration);


            }
        }
        Debug.Log("animationDuration: " + GenerateAnimationDuration);
        float waitTime = 0;

        foreach (var entity in traversalPath)
        {
            String entityName = entity.name;
            Debug.Log(entityName);
            if (entityName == "StartAnimationBlock")
            {
                Debug.Log("start");
            }
            else if (entityName == "EndAnimationBlock")
            {
                Debug.Log("end");
            }
            else if (entityName == "LibraryAnimationBlock(Clone)")
            {
                // yield return StartCoroutine(videoPinTableManager.LoadDataFromFile(entity.libraryFileWithoutPlayButton.path, waitTime));

                // // Wait until the height transition for all pins is finished
                // yield return new WaitUntil(() => videoPinTableManager.AllHeightTransitionsFinished());
                if (File.Exists(entity.libraryFileWithoutPlayButton.path))
                {
                    // Read the data from the file
                    string serializedData = File.ReadAllText(entity.libraryFileWithoutPlayButton.path);

                    // Deserialize the data to re-initialize the object
                    yield return StartCoroutine(videoPinTableConnector.SendPackages(serializedData));
                    yield return new WaitForSeconds(waitTime < 5 ? 5 : waitTime);
                }
                else
                {
                    Debug.LogError("File not found");
                    yield break;
                }

            }
            else if (entityName == "TimeAnimationBlock(Clone)")
            {
                string numberString = entity.tMP_Text.text;
                numberString = Regex.Replace(numberString, @"[^\d]", "");
                float pinDuration = 0;

                // Try to parse the numberString to an integer
                bool parsed = float.TryParse(numberString, out float result);
                if (parsed)
                {
                    pinDuration = result;
                }
                else
                {
                    // Log a warning or handle the case where parsing fails
                    Debug.LogWarning($"Failed to parse '{numberString}' as an integer.");
                }

                // Update the animation duration
                waitTime = pinDuration;
            }

            // Wait for a frame to ensure coroutine behavior
            yield return null;
        }
    }

    public UIC_Connection FindClosestConnectionToPosition(Vector3 position, float maxDistance)
    {
        float minDist = Mathf.Infinity;
        UIC_Connection closestConnection = null;
        foreach (UIC_Connection connection in ConnectionsList)
        {
            int connectionPointsCount = connection.line.points.Count;
            if (connectionPointsCount > 0)
            {
                // v1.2 - changed from DistanceToSpline(obsolete) to DistanceToConnection, a general and more precise way to find distance to connections independent of the lineType
                for (int i = 1; i < connectionPointsCount; i++)
                {
                    float distance = UIC_Utility.DistanceToConnectino(connection, position, maxDistance);
                    if (distance < minDist)
                    {
                        closestConnection = connection;
                        minDist = distance;
                    }
                }
            }
        }

        return closestConnection;
    }

}

