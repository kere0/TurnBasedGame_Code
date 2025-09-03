using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatCreature_Slot : MonoBehaviour
{
    public GameObject skillSlot;
    public Image creatureImage;
    public Image skillImage;
    public Image coolTimeImage;
    private float elapsedTime;
    void Awake()
    {
        transform.GetChild(1).TryGetComponent(out creatureImage);
    }
    public void StartCoolTime_Coroutine(float coolTime, BaseCreature creature)
    {
        StartCoroutine(CoolTime(coolTime, creature));
    }
    public IEnumerator CoolTime(float coolTime, BaseCreature creature)
    {
        coolTimeImage.fillAmount = 1f;
        elapsedTime = 0f;
        while (elapsedTime < coolTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / coolTime);
            coolTimeImage.fillAmount = 1f - t;
            yield return null;
        }
        creature.skillCoolTimeCheck = false;
    }
}
