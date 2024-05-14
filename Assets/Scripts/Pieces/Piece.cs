using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Piece : MonoBehaviour
{
    public GameObject healthBarPrefab;
    protected HealthBar healthBar;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public int dmg = 1;
    public int health = 5;
    public int range = 1;
    public float attackSpeed = 1f;
    public float moveSpeed = 1f;

    public Team myTeam;
    protected Piece curTarget = null;
    protected Node curNode;
    public Node CurNode => curNode;
    protected Node dest;


    protected bool HasEnemy => curTarget != null;
    protected virtual bool InRange => curTarget != null && Vector3.Distance(transform.position, curTarget.transform.position) <= range;
    protected bool isMoving;
    protected bool isDead = false;
    protected bool canAttack = true;
    protected float attackDelay;

    public void Init(Team team, Node curNode)
    {
        myTeam = team;
        if(myTeam == Team.Enemy)
        {
            spriteRenderer.flipX = true;
        }

        this.curNode = curNode;
        transform.position = curNode.worldPos;
        curNode.SetOccupied(true);

        GameObject healthBarGO = Instantiate(healthBarPrefab);
        healthBar = healthBarGO.GetComponent<HealthBar>();
        healthBar.transform.SetParent(transform);
        healthBar.Init(transform, health);
    }

    protected void Start()
    {
        GameManager.Inst.OnRoundStart += OnRoundStart;
        GameManager.Inst.OnRoundEnd += OnRoundEnd;
        GameManager.Inst.OnPieceDead += OnUnitDied;
    }

    protected virtual void OnRoundStart() { }

    protected virtual void OnRoundEnd() { }
    protected virtual void OnUnitDied(Piece diedUnit) { }

    protected virtual void FindTarget()
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
        if (curTarget == null)
            return;

        if (!isMoving)
        {
            dest = null;
            List<Node> candidates = BoardManager.Inst.GetNodesNear(curTarget.curNode);
            candidates = candidates.OrderBy(x => Vector3.Distance(x.worldPos, transform.position)).ToList();

            for (int i = 0; i < candidates.Count; i++)
            {
                if (!candidates[i].IsOccupied)
                {
                    dest = candidates[i];
                    break;
                }
            }

            if (dest == null)
                return;

            var path = BoardManager.Inst.GetPath(curNode, curTarget.curNode);
            if (path == null || path.Count < 2)
                return;

            dest = path[1];
            if (dest.IsOccupied)
                return;

            dest.SetOccupied(true);
        }

        isMoving = !Move(dest);
        if (!isMoving)
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
        healthBar.UpdateBar(health);
        if(health <= 0 && !isDead)
        {
            isDead = true;
            curNode.SetOccupied(false);
            GameManager.Inst.PieceDead(this);
        }
    }

    protected virtual void AttackEnd()
    {
        if(curTarget != null)
            curTarget.TakeDamage(dmg);
    }
    protected virtual void Attack()
    {
        if (!canAttack)
            return;

        canAttack = false;
        animator.SetTrigger("attack");

        attackDelay = 1 / attackSpeed;
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(attackDelay);
        animator.ResetTrigger("attack");
        canAttack = true;
    }
}
    
