using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public int dmg = 1;
    public int health = 5;
    public int range = 1;
    public float attackSpeed = 1f;
    public float moveSpeed = 1f;

    protected Team myTeam;
    protected Piece curTarget = null;
    protected Node curNode;
    public Node CurNode => curNode;
    protected Node dest;


    protected bool HasEnemy => curTarget != null;
    protected bool InRange => curTarget != null && Vector3.Distance(transform.position, curTarget.transform.position) <= range;
    protected bool isMoving;
    protected bool isDead = false;
    protected bool canAttack = true;
    protected float attackDelay;

    public void Setup(Team team, Node curNode)
    {
        myTeam = team;
        if(myTeam == Team.Enemy)
        {
            spriteRenderer.flipX = true;
        }

        this.curNode = curNode;
        transform.position = curNode.worldPos;
        curNode.SetOccupied(true);
    }

    protected void Start()
    {
    }

    protected void FindTarget()
    {
        var enemies = GameManager.Inst.GetEnemyPieces(myTeam);
        float minDist = Mathf.Infinity;
        Piece piece = null;

        foreach(var enemy in enemies)
        {
            if ((Vector3.Distance(enemy.transform.position, transform.position) <= minDist))
            {
                minDist = Vector3.Distance(enemy.transform.position, transform.position);
                piece = enemy;
            }
        }
        curTarget = piece;
    }

    protected bool Move(Node nextNode)
    {
        Vector3 dir = (nextNode.worldPos - transform.position);
        if(dir.sqrMagnitude <= 0.005f)
        {
            transform.position = nextNode.worldPos;
            animator.SetBool("isMoving", false);
            return true;
        }
        animator.SetBool("isMoving", true);

        transform.position += dir.normalized * moveSpeed * Time.deltaTime;
        return false;
    }

    protected void GetInRange()
    {
        if (curTarget != null)
            return;

        if(!isMoving)
        {
            dest = null;
            List<Node> candidates = BoardManager.Inst.GetNodesNear(curTarget.curNode);
            candidates = candidates.OrderBy(x => Vector3.Distance(x.worldPos, transform.position)).ToList();

            for(int i = 0; i < candidates.Count; i++)
            {
                if (!candidates[i].IsOccupied)
                {
                    dest = candidates[i];
                    break;
                }
            }
            if (dest == null)
                return;

            var path = BoardManager.Inst.GetPath(curNode, dest);
            if (path == null && path.Count >= 1)
                return;
            if (path[1].IsOccupied)
                return;

            path[1].SetOccupied(true);
            dest = path[1];
        }

        isMoving = !Move(dest);
        if(!isMoving)
        {
            curNode.SetOccupied(false);
            SetCurrentNode(dest);
        }
    }

    public void SetCurrentNode(Node node)
    {
        curNode = node;
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if(health <= 0 && !isDead)
        {
            isDead = true;
            curNode.SetOccupied(false);
            GameManager.Inst.PieceDead(this);
        }
    }

    protected virtual void Attack()
    {
        if (!canAttack)
            return;

        animator.SetTrigger("attack");
        attackDelay = 1 / attackSpeed;
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        canAttack = false;
        yield return null;
        animator.ResetTrigger("attack");
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }
}
    
