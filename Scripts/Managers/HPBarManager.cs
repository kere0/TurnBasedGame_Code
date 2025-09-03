using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarManager
{
    public Dictionary<Transform, HPBar> HPBarDic = new Dictionary<Transform, HPBar>();
    public void Register(Transform target)
    {
        GameObject prefab = Resources.Load<GameObject>("HPBar");
        GameObject go = Object.Instantiate(prefab);
        go.name = prefab.name;
        HPBar hpBar = go.GetComponent<HPBar>();
        go.transform.SetParent(target);
        HPBarDic.Add(target, hpBar);
    }
    public void Unregister(Transform target)
    {
        if(HPBarDic.TryGetValue(target, out HPBar hpBar))
        {
            Object.Destroy(hpBar.gameObject);
            HPBarDic.Remove(target);
        }
    }
    public HPBar GetHpBar(Transform target)
    {
        if (HPBarDic.TryGetValue(target, out HPBar hpBar))
        {
            return hpBar;
        }
        return null;
    }
}
