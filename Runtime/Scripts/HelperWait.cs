using System;
using System.Collections;
using UnityEngine;

public class HelperWait
{
    public static IEnumerator ActionAfterWait(float wait, Action action)
    {
        yield return new WaitForSeconds(wait);
        action.Invoke();
    }
}
