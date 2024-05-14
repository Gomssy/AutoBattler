using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Piece
{
    protected override bool InRange => curTarget != null && Vector3.Distance(transform.position, curTarget.transform.position) <= Mathf.Sqrt(Mathf.Pow(range,2) + Mathf.Pow(range,2));
    protected override void OnRoundStart()
    {
        FindTarget();
    }
    public void Update()
    {
        if(GameManager.Inst.gameState == GameState.Battle)
        {
            //if (!HasEnemy)
                FindTarget();



            if (InRange && !isMoving)
            {
                if (canAttack)
                {
                    Attack();
                }
            }
            else
                GetInRange();
        }

    }
}
