using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Enemy : MonoBehaviour
{
    public Transform target;
    float attackDelay;
    Animator enemyAnimator;
    Enemystate enemy;
    public bool attacking;
    private Vector3 respawnpoint;
   // Animator enemyAnimator;
 
    int i = 0;
    public bool seeleft;
    attackon Attackon;

    public GameObject rub;
    int c = 0;
    void Start()
    {

        Attackon = GetComponentInChildren<attackon>();
        enemy = GetComponent<Enemystate>();
        attackDelay = enemy.atkSpeed;
        respawnpoint = transform.position;
        enemyAnimator = enemy.enemyAnimator;

        if (enemy.snailon)
        {
            attackDelay = 7f;
        }
    }

    void Update()
    {

        if (enemy.snailon)
        {



        }
        attackDelay -= Time.deltaTime;
        if (attackDelay < 0) attackDelay = 0;

        float distance = Vector2.Distance(transform.position, target.position);
        if(attackDelay< 3 && attackDelay > 2.7)
        {
            if (enemy.snailon)
            {
                c = 1;
                enemyAnimator.SetTrigger("attack2");
  
             
            }
        }


        if (attackDelay == 0 && distance <= enemy.fieldOfVision)
        {
            //enemyAnimator.SetBool("Moving", false);
            FaceTarget();
          
            if (distance <= enemy.atkRange)
            {
                if (enemy.snailon)
                {
                    Debug.Log("attacksnail");
                    c = 0;
                }
                
                AttackTarget();
            }
            else
            {

                if (enemy.snailon)
                {
                    Debug.Log(distance);
                }
                MoveToTarget();

            }
        }
        //StartCoroutine(Whenplayerdead());
    }

    void MoveToTarget()
    {

        //enemyAnimator.SetBool("Moving",true);
        float dir = target.position.x - transform.position.x;
        dir = (dir < 0) ? -1 : 1;
        transform.Translate(new Vector2(dir, 0) * enemy.moveSpeed * Time.deltaTime);

    }

    void FaceTarget()
    {
 
        if (target.position.x - transform.position.x < 0) // 타겟이 왼쪽에 있을 때
        {
            seeleft = true;
            if (enemy.snailon)
            {
                transform.localScale = new Vector3(2, 2, 1);
            }
            else
                transform.localScale = new Vector3(2F, 2F, 1);
        }
        else // 타겟이 오른쪽에 있을 때
        {
            seeleft = false;
            if (enemy.snailon)
            {
                transform.localScale = new Vector3(-2, 2, 1);
            }
            else
                transform.localScale = new Vector3(-2F, 2F, 1);
        }
    }

    void AttackTarget()
    {
        if (i == 0)
        {
            enemyAnimator.SetTrigger("attack");
 
            i = i + 1;
        }

        //enemyAnimator.SetBool("Moving", false);


    } 
    void ResetTarget()
    {
       
        attackDelay = enemy.atkSpeed;
        i = 0;
       
    }
    void attackonfalse()
    {
        Attackon.attacktrue = false;
    }
    void attackontrue()
    {
        Attackon.attacktrue = true;
    }
    IEnumerator Whenplayerdead()
    {
        if (target.GetComponent<PlayerMovement_2>().health.MyCurrentValue <= 0 || target.transform.position.y < -20)
        {

            Debug.Log("playerdead");
            yield return new WaitForSeconds(1);

            transform.position = respawnpoint;
         

        }
        else
        {
            yield break;
        }




    }

    void attack2()
    {
        if (c == 1)
        {

            Instantiate(rub, new Vector3(target.position.x + 3f, target.position.y - 18f), Quaternion.identity);
            c = c + 1;
        }

    }

    void attacktrue()
    {
        attacking = true;
    }

    void attackfalse()
    {
        attacking = false;
    }




}