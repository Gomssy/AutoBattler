using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWide : Piece
{
    protected List<Piece> targets;
    protected override bool InRange => curTarget != null && Vector3.Distance(transform.position, curTarget.transform.position) <= Mathf.Sqrt(Mathf.Pow(range, 2) + Mathf.Pow(range, 2));

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

    protected override void AttackEnd()
    {
        if(targets != null)
        {
            foreach (Piece target in targets)
            {
                target.TakeDamage(dmg);
            }
        }

    }

    protected override void Attack()
    {
        if (!canAttack)
            return;

        canAttack = false;
        animator.SetTrigger("attack");

        attackDelay = 1 / attackSpeed;
        StartCoroutine(AttackNearBy());
    }

    IEnumerator AttackNearBy()
    {
        targets = new List<Piece>();
        List<Node> surroundings = BoardManager.Inst.GetSurroundingNodes(curNode, 1);

        foreach (Node node in surroundings)
        {
            Piece piece = BoardManager.Inst.GetPieceOnNode(node);
            if (piece != null)
            {
                if (piece.myTeam != myTeam)
                    targets.Add(piece);
            }
        }
        yield return new WaitForSeconds(attackDelay);

        animator.ResetTrigger("attack");
        canAttack = true;
    }

}
