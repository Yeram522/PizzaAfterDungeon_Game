using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Costants
{
    public const float ACCESS = 0.5f;
    public const int STD = 0;
    public const int TRACE = 1;
    public const int ATTACK = 2;
    public const int DIE = 3;
    public const int ATTACK_STD = 4;
}

public class Monster
{
    public static List<Monster> monsters = new List<Monster>();
    public GameObject player = GameObject.FindGameObjectWithTag("Player");
    public int id;
    public Animator animator;
    public GameObject me;
    public Rigidbody2D rgd;
    public SpriteRenderer sRenderer;
    public float hp;  //체력
    public float speed; //속도
    public int condition; //0=std 1=trace 2=attack 3=die 4=attack_std
    public bool trace_trigger;
    public float eyesight;
    public float std_time = 0;
   

    //player와 moster 사이의 거리를 float형식으로 반환한다.
    public float cal_distance()
    {
        float playerpos = player.transform.position.x; //참조
        float monsterpose = me.transform.position.x;

        float distance = playerpos - monsterpose;
        return Mathf.Abs(distance);
    }

    //information update
    public void update_infor()
    {

    }
    public virtual void check_condition()
    {
        //몬스터마다 컨디션에 따른 행동 기준이 다르기 떄문에 재정의한다.
    }

    //플레이어의 방향에 따라 몬스터가 감지하고 쫓아간다.
    public void Trace()
    {
        float playerpos = player.transform.position.x;
        float monsterpose = me.transform.position.x;
        int flip_con=0;
      
        if (trace_trigger == false)
        {
            //Debug.Log("outtrace");
            return;
        }
        //player가 특정범위 안에 있으면 대기상태이고 없으면 추적한다.


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

        rgd.velocity = Vector2.zero;
        rgd.velocity = new Vector2(flip_con * speed, rgd.velocity.y);
    }

    //자식class에서 재정의 가능하다.
    public virtual void Attack()
    {
        if (condition != Costants.ATTACK)
            return;
        //공격에 따라서, 데미지주는 공격력등이 차이가 이씀
    }

    //대기상태 
    public virtual void Std()
    {
    }

    //몬스터의 개체를 삭제한다.
    public void Death()
    {
        animator.SetBool("isDeath", true);//죽음 애니메이션 실행

        MonsterController.Destroy(me, 0.3f); //몇초 간격을 두고 Destroy
    }
}

public class boss_burgerking : Monster
{
    public new float std_time = 5; //하이딩
    private int count_spon =4; //미니 버거 소환하는 수 ++2 증감식으로 늘림/보호
    public boss_burgerking()
    {
        //instance 초기화
        this.id = 13;
        this.me = GameObject.Find("BurgerKing"); //오브젝트 할당
        this.animator = me.GetComponent<Animator>(); 
        this.rgd = this.me.GetComponent<Rigidbody2D>(); //rigidbody
        this.sRenderer = this.me.GetComponent<SpriteRenderer>();//Flip
        this.hp = 100;
        this.speed = 1.5f;
        this.condition = Costants.STD; //초기는 std
        this.eyesight = 0.7f; //test하면서 바꾸기
    }

    //계속 update함수에 있어야함.
    public override void check_condition()
    {
        //base.check_condition();
        switch (condition)
        {
            case Costants.STD:
                Std(); //condition이 바뀌지 않는한 계속 이 함수가 실행될것임.
                break;
            case Costants.TRACE:
                //paze2에서 필요
                Trace();
                break;
            case Costants.ATTACK:
                Attack(); //세부 어택 미구현
                break;
            case Costants.DIE:
                Death();
                //배열에서도 삭제되야함!! 아직 미구현
                break;
            case Costants.ATTACK_STD:
                //필용없음 곧 지울거임
                
                break;
        }

        
    }

    public override void Std()
    {
        base.Std();
        //paze2. 이걸 구별하는 방법은. ent_pa_2가 true가 되면 바뀐다.
        if (animator.GetBool("ent_pa_2") == true)
        {
            animator.SetBool("isFound", false);
            condition = 4;
            animator.SetBool("isFound", true); //달리는 모션 활성화

            return; //밑에 실행 안하고 빠져나가기, 스위치 문으로 바꿀예정.
        }

        //paze1 std 랑 paze2 std가 다르다.
        //가만히 있다가 일정시간 움직인다.
        //paze1에서는 일정시간마다 마법봉으로 버거 소환함
        if(isAccess()!=true)
        {
            
            return;
        }

        //countdown으로 설정된 time값이 0이 될때마다 실행한다. 실행된후 타임을 다시 셋팅
        if (Mathf.Round(std_time) <= 0)
        {
            
            for(int i =0; i< count_spon; i++)
            {
                animator.SetBool("isAtk_2", true);//접근된것이 확인되면, 마법봉으로 내려치고 병사를 소환한다.
                new normal_burgersoldier();
                animator.SetBool("isAtk_2", false);//다시 false로 바꾸여 대기상태로 전환!!
            }
            std_time += 10; //총3번소환하는데 갈수롥 버거가 많아지므로 시간간격 늘이기?
            count_spon += 2; //다음 시간대 오면 8,10으로 소환!

        }

        std_time -= Time.deltaTime;
    }

    public override void Attack()
    {
        base.Attack();
    }
    //player가 일정 거리에 접근하면 true를 반환한다.
    public bool isAccess() 
    {
        if (Costants.ACCESS < cal_distance()) //값은 테스트하면서 변경하기
            return false;
        Debug.Log("player_monster_ACCESS");
        animator.SetBool("isAtk_1", true); //처음 시작하는 모션 burgerking_atk_21
        //paze1이기 때문에 컨디션은 그대로.
        return true;
    }

}

public class normal_burgersoldier: Monster
{
    //new normal_burgersoldier을 하면 생성자에서 자동으로 prefab clone을 복제한다.
    public normal_burgersoldier()
    {
        this.me = 
            MonsterController.Instantiate(GameObject.Find("Burgersoldier"), MonsterController.spon_position(), me.transform.rotation);
        this.rgd = this.me.GetComponent<Rigidbody2D>(); //rigidbody
        this.sRenderer = this.me.GetComponent<SpriteRenderer>();//Flip
        this.hp = 100;
        this.speed = 1.5f;
        this.eyesight = 0.4f; //test하면서 바꾸기
        Monster.monsters.Add(this); //자기 자신을 배열에 넣기
    }
    //충돌시 데미지 플레이어와 일정거리 가까어졌을 때 내리치는 모션
    public override void Attack()
    {
        //가시거리 안에 들면 내리치는 모션을 한다.
        if (cal_distance() < eyesight)
            return; // 내리치는 모션, 충돌감지되면 player는 데미지입음
    }
   
}

//몬스터클래스의 메서드들을 사용한다.
public class MonsterController : MonoBehaviour
{  
    public GameObject Burgerking, Burgersoldier;
    public GameObject player;
    public Vector2 playerPos; //플레이어의 실시간 위치를 반영한다(update에서)

    //몬스터 스폰 위치를 결정한다.
    public static Vector2 spon_position()
    {
        Vector2 rand_pos;
        rand_pos.x = 68.13f;
        rand_pos.y = -63.46f;
        //rand함수로 아무데나 스폰
        return rand_pos;
    }
  

    //몬스터의 체력정보와 condition을 업데이트한다.
    void updating_data()
    {
        //for을 이용해 mosters리스트를 탐색한다
        //그중 hp가 0이된 객체는 destroy monster()실행하며 animation보여주고 리스트에서 삭제한다.
        foreach(var item in Monster.monsters)
        {
            
        }
    }

    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        playerPos.x = player.transform.position.x;

    }
}
