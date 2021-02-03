﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Costants
{
    public const float ACCESS = 0.5f;
 
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
    MonsterController ms;
    public MONSTER_STATE CurrentState = MONSTER_STATE.IDLE;
    public GameObject player = GameObject.FindGameObjectWithTag("Player");
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
        return Mathf.Abs(distance);
    }


    private void Start()
    {
        Debug.Log("IDLE    "+Animator.StringToHash("Base Layer.IDLE"));
        Debug.Log("PAZE    " + Animator.StringToHash("Base Layer.PAZE"));
        Debug.Log("PATROL    " + Animator.StringToHash("Base Layer.PATROL"));
        Debug.Log("CHASE    " + Animator.StringToHash("Base Layer.CHASE"));
        Debug.Log("BOSSATTACK    " + Animator.StringToHash("Base Layer.BOSSATTACK"));
        Debug.Log("BOSSATTACKB    " + Animator.StringToHash("Base Layer.BOSSATTACKB"));
        Debug.Log("ATTACK    " + Animator.StringToHash("Base Layer.ATTACK"));
        Debug.Log("ATTACKB    " + Animator.StringToHash("Base Layer.ATTACKB"));
        Debug.Log("DEATH    " + Animator.StringToHash("Base Layer.DEATH"));
    }
}

public class boss_burgerking : Monster
{
    public new float ChaseTimeOut = 5.0f;//하이딩
    public new float AttackDelay = 0f;//하이딩
    public new float std_time = 3.0f; //하이딩
    private int count_spon = 4; //미니 버거 소환하는 수 ++2 증감식으로 늘림/보호
    public boss_burgerking()
    {
        //instance 초기화
        this.id = 13;
        this.me = GameObject.Find("BurgerKing"); //오브젝트 할당 개체가 한개이므로 직접적으로 넣는다.
        this.animator = me.GetComponent<Animator>();
        this.rgd = this.me.GetComponent<Rigidbody2D>(); //rigidbody
        this.sRenderer = this.me.GetComponent<SpriteRenderer>();//Flip
        this.hp = 100;
        this.speed = 1.5f;
        this.eyesight = 0.7f; //test하면서 바꾸기
    }

    //boss 한정 paze1 state
    //버거병사가 다 죽고 paze2로 넘어가는 것 아직 미구현.
    public override IEnumerator State_Idle()
    {
        //현재상태를 설정한다
        CurrentState = MONSTER_STATE.IDLE;

        //메카님에서 idle 상태를 활성화 시킨다.
        animator.SetBool("isAtk_1", false);
        animator.SetBool("isAtk_2", false);

        //접근할때까지 아무것도 실행되지 않는다.
        if (isAccess() != true)
        {
            yield return null;
        }
        //접근 확인이 되면, paze1 idle이 실행된다.
      
        //대사UI생성후
        //버튼이 눌려지고 창이 닫히면, 아래 while문이 실행된다.
        //공격주기를 정하는 타이머를 설정한다.
        float ElapesedTime = 0f;

        //idle  상태에 있는 동안 무한히 반복한다.
        while (CurrentState == MONSTER_STATE.IDLE)
        {
            //타이머를 업데이트 한다.
            ElapesedTime += Time.deltaTime;

            //Attackdelay가 초기값이0이라서 무조건 한번은 공격시작한다.
            //4,6,8 ㄷㅏ 생성하면 끝
            if(ElapesedTime >= AttackDelay && count_spon<10)
            {
                //타이머 재설정 및 AttackDelay 증가
                ElapesedTime = 0f;
                AttackDelay += 30f;

                //공격을 한다.
                for (int i = 0; i < count_spon; i++)
                {
                    animator.SetBool("isAtk_1", true);//접근된것이 확인되면, 마법봉으로 내려치고 병사를 소환한다.
                    MonsterController.monsters.Add(new normal_burgersoldier()); //옵젝생성
                    animator.SetBool("isAtk_1", false);//다시 false로 바꾸여 대기상태로 전환!!
                }
                count_spon += 2; //다음 시간대 오면 8,10으로 소환!
            }

            //else if여야지 시간도달이 안되서 생성마법이 발동안한 틈에 공격할 수 있다.
            else if(animator.GetBool("isAtk_1")==false)
            {              
                animator.SetBool("isAtk_2", true);
                //충격파 생성->충격파는 에셋이 따로 있나?
                animator.SetBool("isAtk_2", false);
            }

            //모든 버거병사를 죽였을 때
            //Paze2로 넘어간다.
            StartCoroutine(State_Patrol());

            yield return null;
            
        }
    }

    public override IEnumerator State_Chase()
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

            //moster시야 밖에 플레이어가 있다면 향해서 달려간다,
            if (cal_distance() > eyesight)
            {
                // 시간 초과 계산 지점
                float ElapsedTime = 0f;

                //계속 추적한다.
                while (true)
                {
                    //시간을 증가시킨다
                    ElapsedTime += Time.deltaTime;

                    //계속 player을 향해 달린다.
                    rgd.velocity = Vector2.zero;
                    rgd.velocity = new Vector2(flip_con * speed, rgd.velocity.y);

                    yield return null;

                    if (ElapsedTime >= ChaseTimeOut)
                    {
                        //여전히 간격차가 좁혀지지 않으면 patrol상태로 전환한다.공격대기상태
                        if (cal_distance() > eyesight)
                        {
                            StartCoroutine(State_Patrol());
                            yield break;
                        }

                        else break; //다시 플레이어가 보이므로 추격

                    }
                }
                //monster시야에 player가 들어오면 달리는 모션을 비활성하고 공격한다.
                //StartCoroutine(State_ATTACK());
                if (cal_distance() < 2.0f)
                {
                    StartCoroutine(State_Attack());
                    yield break;
                }
            }
            yield return null;
        }
    }

    public override IEnumerator State_Patrol()
    {
        if (animator.GetBool("ent_Pa_2") != true)
            yield return null;

        CurrentState = MONSTER_STATE.PATROL;
        animator.SetBool("isAtk_3", false);
        animator.SetBool("isAtk_4", false);
        animator.SetBool("isFound", false);
        //patrol 상태는 쉬어가는거다..음..대기 상태로 만들기!!
  
        while (CurrentState == MONSTER_STATE.PATROL)
        {
            // 시간 초과 계산 지점
            float ElapsedTime = 0f;
            while (true)
            {
                //시간을 증가시킨다
                ElapsedTime += Time.deltaTime;

                //다음프레임까지 기다린다.
                yield return null;

                if (ElapsedTime >= std_time)
                {
                    StartCoroutine(State_Chase());
                    yield break;
                }
            }

        }
        yield return null;
    }

    public override IEnumerator State_Attack()
    {
        int ran = Random.Range(0, 101);

        if(ran<75)
        {
            //총2번쏘는 스킬
            for(int i =0;i<2;i++)
            {
                CurrentState = MONSTER_STATE.ATTACK;
                animator.SetBool("isAtk_3", true);
                animator.SetBool("isAtk_3", false);
            }   
        }
        else
        {
            //25%확률로 난사.
            CurrentState = MONSTER_STATE.ATTACKB;
            animator.SetBool("isAtk_4", true);
            animator.SetBool("isAtk_4", false);
        }


        //스킬사용 후 버거병사 3마리 소환!!new burger~
        for (int i = 0; i < 3; i++)
        {
            normal_burgersoldier.burger_solider.Add(new normal_burgersoldier());
        }

        //공격을 마친후 대기상태로 돌아간다.
        StartCoroutine(State_Patrol());


        yield return null;
    }

   
    public bool isAccess()
    {
        if (Costants.ACCESS < cal_distance()) //값은 테스트하면서 변경하기
            return false;
        Debug.Log("player_monster_ACCESS");
        animator.SetBool("isAtk_1", true); //처음 시작하는 모션 burgerking_atk_21
        //paze1이기 때문에 컨디션은 그대로.
        return true;
    }

    //update objects' state
    private void Update()
    {
        //damage==null이면 실행ㄴㄴ null이 아니면 changeHealth(damage);
    }

}


public class normal_burgersoldier : Monster
{
    public static List<Monster> burger_solider = new List<Monster>();
    //new normal_burgersoldier을 하면 생성자에서 자동으로 prefab clone을 복제한다.
    public normal_burgersoldier()
    {
        this.me =
            MonsterController.Instantiate(MonsterController.Burgersoldier, MonsterController.spon_position(), me.transform.rotation);
        this.rgd = this.me.GetComponent<Rigidbody2D>(); //rigidbody
        this.sRenderer = this.me.GetComponent<SpriteRenderer>();//Flip
        this.hp = 100;
        this.speed = 1.5f;
        this.eyesight = 0.4f; //test하면서 바꾸기

    }
    //충돌시 데미지 플레이어와 일정거리 가까어졌을 때 내리치는 모션
    /*public override void Attack()
    {
        //가시거리 안에 들면 내리치는 모션을 한다.
        if (cal_distance() < eyesight)
            return; // 내리치는 모션, 충돌감지되면 player는 데미지입음
    }*/

}
