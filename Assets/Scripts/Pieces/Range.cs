using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : Piece
{
    protected override void OnRoundStart()
    {
        FindTarget();
    }
    public void Update()
    {
        if (GameManager.Inst.gameState == GameState.Battle)
        {
           // if (!HasEnemy)
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
