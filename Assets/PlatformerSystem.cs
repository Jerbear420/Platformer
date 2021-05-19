using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlatformerSystem : MonoBehaviour
{
    // Start is called before the first frame update 
    public delegate void RunAfterLoad();
    private float _childWait = 0f;
    public interface ISystem
    {

    }
    public IEnumerator WaitForSeconds(float seconds, RunAfterLoad callback)
    {
        Debug.Log("Wait for sec");
        yield return new WaitForSeconds(seconds);
        callback();
        Debug.Log("Waited for sec;");
    }
    public IEnumerator WaitForLoaded(bool value, RunAfterLoad callback)
    {
        Debug.Log("Wait for bool");
        yield return new WaitUntil(() => value == true);
        callback();
        Debug.Log("Waited for bool;");
    }
    public IEnumerator SetupChildWait()
    {
        Debug.Log("Perform a wait action");
        yield return new WaitForSeconds(.1f);
        _childWait += .1f;
        Debug.Log("Waited for child");
    }

    public GameObject WaitForChild(GameObject parent, int id)
    {
        GameObject child = parent.transform.GetChild(id).gameObject;
        if (child == null)
        {
            if (_childWait < 3f)
            {
                StartCoroutine(SetupChildWait());
                child = parent.transform.GetChild(id).gameObject;

            }
        }
        _childWait = 0f;
        return child;
    }
}
