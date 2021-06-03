using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IInteractable
{
    Interactable Interactable { get; }
    bool PassThrough { get; }
    void Interact(Creatures interactor);
    void Nearby(Creatures interactor);
}
interface ITriggerable
{
    void OnTrigger();
    bool Triggered { get; }
    bool CanTrigger { get; }
}
public class SystemController : MonoBehaviour
{
    [SerializeField] private Vector2 _spawnPoint;
    [SerializeField] private Vector2 _YDeadZone;
    [SerializeField] private float _timeLimit;
    private float _startTime;
    private bool _running;
    public float TimeLeft { get { return (Mathf.Max(_startTime + _timeLimit - Time.fixedTime, 0)); } }
    private static SystemController _controller;
    public bool Running { get { return (_running); } }
    public static SystemController Controller { get { return _controller; } }
    public List<Item> ItemToLoad = new List<Item>();
    public static bool Loaded = false;
    public static Dictionary<int, Item> ItemList = new Dictionary<int, Item>();
    // Start is called before the first frame update
    void Awake()
    {
        _controller = this;
        _running = false;
        foreach (var item in ItemToLoad)
        {
            if (!ItemList.ContainsKey(item.GetIID()))
            {
                ItemList.Add(item.GetIID(), item);
                Debug.Log(item.GetIID() + " Was loaded");
            }
        }

        Debug.Log("Loaded items");

        Loaded = true;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (_running)
        {
            if (_startTime + _timeLimit <= Time.fixedTime)
            {
                Debug.Log("Time is up!");
            }
        }
    }
    void OnDrawGizmos()
    {
        if (_spawnPoint != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;
            Vector3 globalWaypointPosition = _spawnPoint + (Vector2)transform.position;
            Gizmos.DrawLine(globalWaypointPosition - Vector3.up * size, globalWaypointPosition + Vector3.up * size);
            Gizmos.DrawLine(globalWaypointPosition - Vector3.left * size, globalWaypointPosition + Vector3.left * size);

        }
        if (_YDeadZone != null)
        {
            Gizmos.color = Color.blue;
            float size = .3f;
            Vector3 globalWaypointPosition = _YDeadZone + (Vector2)transform.position;
            Gizmos.DrawLine(globalWaypointPosition - Vector3.up * size, globalWaypointPosition + Vector3.up * size);
            Gizmos.DrawLine(globalWaypointPosition - Vector3.left * size, globalWaypointPosition + Vector3.left * size);
        }
    }

    public static Vector2 GetSpawnPoint()
    {
        return _controller._spawnPoint;
    }
    public static Vector2 GetYDeadZone()
    {
        return _controller._YDeadZone;
    }

    public void StartMap()
    {
        _running = true;
        _startTime = Time.fixedTime;
    }

    public static void LoadScene(string scene)
    {
        if (scene != null)
        {
            SceneManager.LoadScene(scene);
            Debug.Log("Scene is not null!");

        }
    }
}
