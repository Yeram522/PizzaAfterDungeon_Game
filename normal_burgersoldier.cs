using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//IEnumerator에서 파생한 클래스
//씬의 모든 몬스터를 경꼐를 넘어가지 않고 안전하게 순회하도록 처리한다.
public class burgerEnumerator : IEnumerator
{
    //열거자가 가리키는 현재  몬스터 오브젝트의 참조
    private normal_burgersoldier CurrentObj = null;

    //--------------------------------------------------
    //MoveNExt 이벤트를 재정의 한다. - 다음 몬스터로 반복기를 증가시킨다.
    public bool MoveNext()
    {
        //열거자가 가리키는 현재 몬스터 오브젝트의 참조
        CurrentObj = (CurrentObj == null) ?
            normal_burgersoldier.FirstCreated : CurrentObj.NextMonster;

        //다음 마법사를 반환한다.
        return (CurrentObj != null);
    }
    //--------------------------------------------------------
    //첫번째 몬스터로 반복기를 재설정해 되돌린다.
    public void Reset()
    {
        CurrentObj = null;
    }
    //------------------------------------------------------------
    //현재 몬스터를 얻기 위한 C# 프로퍼티
    public object Current
    {
        get { return CurrentObj; }
    }
}

//IEnumerator에서 파생되어 foreach로 순회하는 것이 가능하다.
[System.Serializable]
public class normal_burgersoldier : Monster, IEnumerable
{
   // public static GameObject Burgersoldier;

    public normal_burgersoldier()
    {
        
    }


    //마지막으로 생성된 몬스터에 대한 참조
    public static normal_burgersoldier LastCreated = null;

    //처음으로 생성된 몬스터에 대한 참조
    public static normal_burgersoldier FirstCreated = null;

    //리스트 다음 마법사에 대한 참조
    public normal_burgersoldier NextMonster = null;

    //리스트 이전 몬스터에 대한 참조
    public normal_burgersoldier PrevMonster = null;

    //이 몬스터의 이름
    public string MonsterName = "";
    //-------------------------------------------------------------------
    //생성자
    new void Awake()
    {
        this.me = gameObject; //스크립트가 붙어있는 게임오브젝트 지칭
        this.rgd = this.me.GetComponent<Rigidbody2D>(); //rigidbody
        this.sRenderer = this.me.GetComponent<SpriteRenderer>();//Flip
        this.animator = this.me.GetComponent<Animator>();
        this.hp = 100;
        this.speed = 1.5f;
        this.eyesight = 0.3f; //test하면서 바꾸기
        
        me.transform.position = new Vector2(Random.Range(60.15f,70.91f), -63.5f);
        CurrentState = MONSTER_STATE.CHASE;
        StartCoroutine("State_Chase");
        Debug.Log("생성자 진입:burgersoldier");
        //처음 생성된 몬스터를 업데이트해야하는가
        if (FirstCreated == null)
            FirstCreated = this;

        //마지막으로 생성된 몬스터를 업데이트해야 하는가
        if (normal_burgersoldier.LastCreated != null)
        {
            normal_burgersoldier.LastCreated.NextMonster = this;
            PrevMonster = normal_burgersoldier.LastCreated;
        }

        normal_burgersoldier.LastCreated = this;
    }
    //-------------------------------------------------------------------
    //오브젝트 파괴시 호출된다.
    void OnDestroy()
    {
        //연결된 오브젝트가 파괴된 경우 연결고리를 고친다
        if (PrevMonster != null)
            PrevMonster.NextMonster = NextMonster;

        if (NextMonster != null)
            NextMonster.PrevMonster = PrevMonster;
    }
    //------------------------------------------------------------------------
    //이 클래스를 열거자로 얻는다
    public IEnumerator GetEnumerator()
    {
        return new burgerEnumerator();
    }
    //public static List<Monster> burger_solider = new List<Monster>();
    //new normal_burgersoldier을 하면 생성자에서 자동으로 prefab clone을 복제한다.


    //버거병사는 Idle상태가 따로 없어서 곧바로 CHASE상태가 된다.

    public override IEnumerator State_Chase()
    {
        //float playerpos = player.transform.position.x;
        //float monsterpose = me.transform.position.x;
        int flip_con = 0;
        Debug.Log("버거소져 chase 진입");
        //현재상태를 설정한다
        CurrentState = MONSTER_STATE.CHASE;

        //chase  상태에 있는 동안 무한히 반복한다.
        while (CurrentState == MONSTER_STATE.CHASE)
        {
            float playerpos = player.transform.position.x;
            float monsterpose = me.transform.position.x;

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

            if (cal_distance() <= Costants.ACCESS)
                StartCoroutine(State_Attack());


            yield return null;
        }

        // 플레이어에게 도달했다면 공격한다.
       
    }

    //충돌시 데미지 플레이어와 일정거리 가까어졌을 때 내리치는 모션
    public override IEnumerator State_Attack()
    {
        Debug.Log("burgerSoldier_ Corutin: Attack enter");
        rgd.velocity = Vector2.zero; // 한번초기화
        CurrentState = MONSTER_STATE.ATTACK;

        while (CurrentState == MONSTER_STATE.ATTACK)
        {
            //attack루프에 들어왔으므로 SetBool을 true로
            if (animator.GetBool("isFound")==false)
            {
                animator.SetBool("isFound", true);
                yield return null;               
            }
            else
            {
                animator.SetBool("isFound", false);
                StartCoroutine(State_Chase());//바꿔주고 chase로 ㄱㄱ
            }
        }
            //가시거리 안에 들면 내리치는 모션을 한다.

        yield return null;

        // 내리치는 모션, 충돌감지되면 player는 데미지입음

    }

    private void Update()
    {

    }
}
