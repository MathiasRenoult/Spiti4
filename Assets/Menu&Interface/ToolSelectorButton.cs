using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToolSelectorButton : MonoBehaviour, IPointerDownHandler
{
    public Tool associatedTool;
    public GameObject specs;
    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        foreach(Tool t in ToolManager.singleton.tools)
        {
            t.button.transform.localScale = Vector3.one;
        }
        StartCoroutine(RotateAnimation(0.25f));
    }

    public IEnumerator RotateAnimation(float totalTime)
    {
        float time = 0f;

        while(time < totalTime)
        {
            if(time < totalTime/2) transform.localScale+=Vector3.one/60f;
            else transform.localScale-=Vector3.one/60f;
            yield return new WaitForEndOfFrame();
            time+= Time.deltaTime;
        }

        transform.localScale = Vector3.one;
    }
}
