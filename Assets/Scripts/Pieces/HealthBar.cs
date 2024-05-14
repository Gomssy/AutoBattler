using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform bar;
    public Vector3 offset;
    private float max_health;
    private Transform target;

    public void Init(Transform _target, float health)
    {
        max_health = health;
        UpdateBar(max_health);
        target = _target;
    }

    public void UpdateBar(float val)
    {
        float scale = val / max_health;
        Vector3 barScale = bar.transform.localScale;
        barScale.x = scale;
        bar.transform.localScale = barScale;
    }

    private void Update()
    {
        if(target != null) 
        {
            transform.position = target.position + offset;
        }
    }

}
