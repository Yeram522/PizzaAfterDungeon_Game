using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_BurgerKing : Monster
{
    public GameObject Burgersoldier;
    private new float ChaseTimeOut = 5.0f;//하이딩
    private new float AttackDelay = 0f;//하이딩
    private new float std_time = 3.0f; //하이딩
    private int count_spon = 4; //미니 버거 소환하는 수 ++2 증감식으로 늘림/보호

    private new void Awake()
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
        StartCoroutine(State_Idle());
        Debug.Log("초기화 완료:burgerking");
    }

    public bool isAccess()
    {
        if (Costants.ACCESS < cal_distance()) //값은 테스트하면서 변경하기
            return false;


        Debug.Log("player_monster_ACCESS");
        
        //paze1이기 때문에 컨디션은 그대로.
        return true;
    }

  

    //boss 한정 paze1 state
    //버거병사가 다 죽고 paze2로 넘어가는 것 아직 미구현.
    public override IEnumerator State_Idle()
    {
        //현재상태를 설정한다
        CurrentState = MONSTER_STATE.IDLE;
        Debug.Log("Corutin: IDLE enter");

        //메카님에서 idle 상태를 활성화 시킨다.
        //animator.SetBool("isAtk_1", false);
        //animator.SetBool("isAtk_2", false);

        //idle  상태에 있는 동안 무한히 반복한다.
        while (CurrentState == MONSTER_STATE.IDLE)
        {
           // Debug.Log("Corutin: IDLE enter");
            //접근할때까지 아무것도 실행되지 않는다.
            if (isAccess() == true)
            {
                StartCoroutine(State_Attack0());
                yield return null;
            }
            //접근 확인이 되면, paze1 idle이 실행된다.
            
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
        Debug.Log("Corutin: IDLE chase");

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
        Debug.Log("Corutin: IDLE Patrol");
        animator.SetBool("isAtk_3", false);
        animator.SetBool("isAtk_4", false);
        animator.SetBool("isFound", false);
        //patrol 상태는 쉬어가는거다..음..대기 상태로 만들기!!

        while (CurrentState == MONSTER_STATE.PATROL)
        {
            // 시간 초과 계산 지점
            float ElapsedTime_2 = 0f;
            while (true)
            {
                //시간을 증가시킨다
                ElapsedTime_2 += Time.deltaTime;

                //다음프레임까지 기다린다.
                yield return null;

                if (ElapsedTime_2 >= std_time)
                {
                    StartCoroutine(State_Chase());
                    yield break;
                }
            }

        }
        yield return null;
    }

    public IEnumerator State_Attack0()
    {
        CurrentState = MONSTER_STATE.ATTACK;

        Debug.Log("Corutin: Attack0 enter");
        //대사UI생성후
        //버튼이 눌려지고 창이 닫히면, 아래 while문이 실행된다.
        //공격주기를 정하는 타이머를 설정한다.
        float ElapesedTime_1 = 0f;

        //ATTACK  상태에 있는 동안 무한히 반복한다.
        while (CurrentState == MONSTER_STATE.ATTACK)
        {
            //타이머를 업데이트 한다.
            ElapesedTime_1 += Time.deltaTime;

            //Attackdelay가 초기값이0이라서 무조건 한번은 공격시작한다.
            //4,6,8 ㄷㅏ 생성하면 끝
            if (ElapesedTime_1 >= AttackDelay && count_spon < 10)
            {
                //타이머 재설정 및 AttackDelay 증가
                ElapesedTime_1 = 0f;
                AttackDelay += 30f;

                //공격을 한다.
                for (int i = 0; i < count_spon; i++)
                {
                    animator.SetBool("isAtk_1", true);//접근된것이 확인되면, 마법봉으로 내려치고 병사를 소환한다.
                    yield return null;
                    var norbur = Instantiate(Burgersoldier, new Vector2(68.13f, -61.85f), me.transform.rotation);
                    norbur.AddComponent<normal_burgersoldier>();//오브젝트에 스크립트 추가
                   
                }
                
                count_spon += 2; //다음 시간대 오면 8,10으로 소환!
            }

            else if(animator.GetBool("isAtk_1") == true)
            {
                animator.SetBool("isAtk_1", false);
            }

            //else if여야지 시간도달이 안되서 생성마법이 발동안한 틈에 공격할 수 있다.
            //else if (animator.GetBool("isAtk_1") == false)
            //{
            //    animator.SetBool("isAtk_2", true);
            //    //충격파 생성->충격파는 에셋이 따로 있나?
            //    animator.SetBool("isAtk_2", false);
            //}

            //모든 버거병사를 죽였을 때
            //Paze2로 넘어간다.
            //StartCoroutine(State_Patrol());

            yield return null;

        }
    }

    public override IEnumerator State_Attack()
    {
        CurrentState = MONSTER_STATE.ATTACKB;
        int ran = Random.Range(0, 101);

        if (ran < 75)
        {
            //총2번쏘는 스킬
            for (int i = 0; i < 2; i++)
            {
                //CurrentState = MONSTER_STATE.ATTACK;
                animator.SetBool("isAtk_3", true);
                animator.SetBool("isAtk_3", false);
            }
        }
        else
        {
            //25%확률로 난사.
            //CurrentState = MONSTER_STATE.ATTACKB;
            animator.SetBool("isAtk_4", true);
            animator.SetBool("isAtk_4", false);
        }


        //스킬사용 후 버거병사 3마리 소환!!new burger~
        for (int i = 0; i < 3; i++)
        {
            var norbur = Instantiate(Burgersoldier, new Vector2(68.13f, -61.85f), me.transform.rotation);
            norbur.AddComponent<normal_burgersoldier>();//오브젝트에 스크립트 추가
        }

        //공격을 마친후 대기상태로 돌아간다.
        StartCoroutine(State_Patrol());


        yield return null;
    }



    //update objects' state

    private void Update()
    {
        //damage==null이면 실행ㄴㄴ null이 아니면 changeHealth(damage);


    }
}
