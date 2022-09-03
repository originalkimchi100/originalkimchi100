using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemystate : MonoBehaviour
{
    public string enemyName;
    public int maxHp;
    public int nowHp;
    public int atkDmg;
    public float atkSpeed;
    public float moveSpeed;
    public float atkRange;
    public float fieldOfVision;

    public Animator enemyAnimator;
    public HealthBarBehaviour healthbar;
    public bool snailon;
    private void SetEnemyStatus(string _enemyName, int _maxHp, int _atkDmg, float _atkSpeed, float _moveSpeed, float _atkRange, float _fieldOfVision)
    {
        enemyName = _enemyName;
        maxHp = _maxHp;
        nowHp = _maxHp;
        atkDmg = _atkDmg;
        atkSpeed = _atkSpeed;
        moveSpeed = _moveSpeed;
        atkRange = _atkRange;
        fieldOfVision = _fieldOfVision;
    }



    
    public float height = 1.7f;

    public PlayerCombat player;
  

    void Start()
    {
        if (name.Equals("skeleton"))
        {
            SetEnemyStatus("skeleton", 100, 10, 1.5f, 2, 1.5f, 7f);
        }
 
        if (name.Equals("Warlock"))
        {
            SetEnemyStatus("Warlock", 100, 10, 3f, 3, 3f, 20f);
        }

        if(name.Equals("Mushroom"))
        {
            SetEnemyStatus("Mushroom", 100, 10, 1.5f, 0, 1f, 60f);
        }

        if (name.Equals("HedgeHog"))
        {
            SetEnemyStatus("HedgeHog", 100, 50, 4f, 8, 10f, 10f);
        }
        if (name.Equals("kiwi"))
        {
            SetEnemyStatus("kiwi", 100, 10, 4f, 0, 100f, 120f);
        }
        if (name.Equals("middleboss_snail"))
        {
            SetEnemyStatus("middleboss_snail", 500, 60, 7f, 12, 15f, 30f);
            snailon = true;
       
        }
        if (name.Equals("crow"))
        {
            SetEnemyStatus("crow", 100, 10, 4f, 0, 100f, 120f);
        }
        if (name.Equals("orcarcher"))
        {
            SetEnemyStatus("orcarcher", 70, 10, 4f, 3f, 40f, 40f);
        }
        if (name.Equals("orcsword"))
        {
            SetEnemyStatus("orcsword", 120, 10, 2f, 10f, 11f, 40f);
        }
    }


    public void TakeDamage(int damage, Vector2 pos)
    {
        transform.GetChild(0).gameObject.SetActive(true);


        enemyAnimator.SetTrigger("hurt");
        nowHp -= damage;
        healthbar.SetHealth(nowHp, maxHp);
        if (nowHp <= 0) // 적 사망
        {
            //enemyAnimator.SetTrigger("Death");
            Die();
                 
        }
 
    }

    void Die()
    {
       
        //GetComponent<Enemy>().enabled = false;    // 추적 비활성화
        GetComponent<Collider2D>().enabled = false; // 충돌체 비활성화
        Destroy(GetComponent<Rigidbody2D>());       // 중력 비활성화
        Destroy(gameObject, 3);
    }

    void Update()
    {

    }
}
