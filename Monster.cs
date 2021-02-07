using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Costants
{
    public const float ACCESS = 0.2f;
 
}

//moster의 가능한 상태들을 정의한다 (각 상태는 대문자 문자열에 의해서 해시코드에 대응한다)
public enum MONSTER_STATE
{
    IDLE = -1009909005,
    PAZE = -192123430,
    PATROL = -131052876,
    CHASE = -454707541,
    BOSSATTACK = -2136634337,
    BOSSATTACKB = -950507704,
    ATTACK = -1274329858,
    ATTACKB = 274799470,
    DEATH = -1788913636 };



public class Monster : MonoBehaviour
{
    public MONSTER_STATE CurrentState = MONSTER_STATE.IDLE;
    public GameObject player;
    public int id;
    public Animator animator;
    public GameObject me;
    public Rigidbody2D rgd;
    public SpriteRenderer sRenderer;
    public float hp;  //체력
    public float speed; //속도
    public bool trace_trigger;
    public float eyesight;
    public float std_time = 0;
    public float ChaseTimeOut = 0f;
    public float AttackDelay = 0f;
    public bool onDeath = false;

    public static Vector2 spon_position()
    {
        Vector2 rand_pos;
        rand_pos.x = 68.13f;
        rand_pos.y = -61.85f;
        //rand함수로 아무데나 스폰
        return rand_pos;
    }

    public virtual IEnumerator State_Idle()
    {
        yield return null;
    }
    //공격전 대기상태_다른몬스터에도 적용
    public virtual IEnumerator State_Chase()
    {
        float playerpos = player.transform.position.x;
        float monsterpose = me.transform.position.x;
        int flip_con = 0;

        //현재상태를 설정한다
        CurrentState = MONSTER_STATE.CHASE;

        //메카님에서 chase 상태를 활성화 시킨다.
        animator.SetBool("isFound", true);

        //chase  상태에 있는 동안 무한히 반복한다.
        while (CurrentState == MONSTER_STATE.CHASE)
        {
            //플레이어의 방향에 따라서 보는 방향이 달라진다
            if (monsterpose < playerpos)
            {
                flip_con = 1;
            }

            else if (monsterpose > playerpos)
            {
                flip_con = -1;
            }
            else
                flip_con = 0;

            //플레이어를 향해서 달려간다.
            rgd.velocity = Vector2.zero;
            rgd.velocity = new Vector2(flip_con * speed, rgd.velocity.y);

            yield return null;
        }

        // 플레이어에게 도달했다면 공격한다.
        if(cal_distance() <= Costants.ACCESS)
        {
            StartCoroutine(State_Attack());
            yield break;
        }
    }

    public virtual IEnumerator State_Patrol()
    {
        yield return null;
    }
    public virtual IEnumerator State_Attack()
    {

        yield return null;
    }

    public virtual IEnumerator State_Death()
    {

        yield return null;
    }

    //update함수에서 계속 체력을 받아야한다.
    public void ChangeHealth(float Amount)
    {
        //체력을 감소시킨다
        hp += Amount;

        // 죽게되는가?
        if(hp<=0)
        {
            //죽는 애니네이션 실행
            animator.SetTrigger("isDeath");
            StopAllCoroutines();
            Destroy(me, 0.3f); //몇초 간격을 두고 Destroy
            return;
        }
    }

    //player와 moster 사이의 거리를 float형식으로 반환한다.
    public float cal_distance()
    {
        float playerpos = player.transform.position.x; //참조
        float monsterpose = me.transform.position.x;

        float distance = playerpos - monsterpose;
        //Debug.Log("cal_distance:" + Mathf.Abs(distance));

        return Mathf.Abs(distance);
        
    }


    private void Start()
    {
        
    }
}



