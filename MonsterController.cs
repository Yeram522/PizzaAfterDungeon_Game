using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Costants
{
    public const float ACCESS = 0.5f;

}

public class Monster
{
    public int id;
    public Animator animator;
    public GameObject me;
    public Rigidbody2D rgd;
    public SpriteRenderer sRenderer;
    public float hp;  //체력
    public float speed; //속도
    public bool trace_trigger;
    public float eyesight;

    //player와 moster 사이의 거리를 float형식으로 반환한다.
    public float cal_distance(GameObject player)
    {
        float playerpos = player.transform.position.x; //참조
        float monsterpose = me.transform.position.x;

        eyesight = playerpos - monsterpose;
        return Mathf.Abs(eyesight);
    }

    public virtual void check_condition()
    {
        //몬스터마다 컨디션에 따른 행동 기준이 다르기 떄문에 재정의한다.
    }
    //플레이어의 방향에 따라 몬스터가 감지하고 쫓아간다.
    public void TRACE(GameObject player)
    {
        float playerpos = player.transform.position.x;
        float monsterpose = me.transform.position.x;
        int flip_con=0;
      
        if (trace_trigger == false)
        {
            //Debug.Log("outtrace");
            return;
        }
        //animator.SetBool("isFound", true); //달리는 모션

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
    public virtual void ATTACK(GameObject player)
    {
        //공격에 따라서, 데미지주는 공격력등이 차이가 이씀
    }

    //대기상태
    public void STD()
    {
        //가만히 있다가 일정시간 움직인다.
    }

    //몬스터의 개체를 삭제한다.
    void Death()
    {
        animator.SetBool("isDeath", true);//죽음 애니메이션 실행

        MonsterController.Destroy(me, 0.3f); //몇초 간격을 두고 Destroy
    }
}

public class boss_burgerking : Monster
{
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
    }
    
    //player가 일정 거리에 접근하면 true를 반환한다.
    public bool isAccess(GameObject player) 
    {
        if (Costants.ACCESS < cal_distance(player)) //값은 테스트하면서 변경하기
            return false;
        Debug.Log("player_monster_ACCESS");
        //trace_trigger = true;
        return true;
    }
}

public class normal_burgersoldier: Monster
{
    public normal_burgersoldier()
    {
        this.me = GameObject.Find("Burger_soldier"); //오브젝트 할당
        this.rgd = this.me.GetComponent<Rigidbody2D>(); //rigidbody
        this.sRenderer = this.me.GetComponent<SpriteRenderer>();//Flip
        this.hp = 100;
        this.speed = 1.5f;
        this.eyesight = 1.2f;
    }
    //충돌시 데미지 플레이어와 일정거리 가까어졌을 때 내리치는 모션
    public override void ATTACK(GameObject player)
    {
        //가시거리 안에 들면 내리치는 모션을 한다.
        if (cal_distance(player) < eyesight)
            return; // 내리치는 모션, 충돌감지되면 player는 데미지입음

    }
}

public class MonsterController : MonoBehaviour
{ 
    public  List<Monster> monsters = new List<Monster>();
    public GameObject Burgerking, Burgersoldier;
    public GameObject player;
    public Vector2 playerPos; //플레이어의 실시간 위치를 반영한다(update에서)
                              //몬스터 개수에 따라 크기는 달라질 수 있음

    //몬스터를 생성한다.
    


    //몬스터 스폰 위치를 결정한다.
    public Vector2 spon_position()
    {
        Vector2 rand_pos;
        rand_pos.x = 1.2f;
        rand_pos.y = 1.3f;
        //rand함수로 아무데나 스폰
        return rand_pos;
    }

    //monobehavior을 이용하여 몬스터를 생성한다.
    void spon_monster(Monster monster)
    {
        Vector2 rand_pos = spon_position();
        //몬스터의 생성위치/방향을 정한다..(monster class에서 정의된 메서드 사용)

        //생성하고싶은 몬스터 타입을 받아와서 위치에 생성
        GameObject instance = Instantiate(monster.me, rand_pos, monster.me.transform.rotation) as GameObject;

        //생성여부를 확인한다.
        if (instance != null)
            Debug.Log(monster + "캐스팅 성공");
        else
            Debug.Log(monster + "캐스팅 실패");

        //monster data에 넣기 계속 업데이트 해줘야함
        //생성된 monster의 정보를 list에 업데이트 한다.
        monsters.Add(new Monster() { me = monster.me, hp = monster.hp });
    }

  

    //몬스터의 체력정보와 condition을 업데이트한다.
    void updating_data()
    {
        //for을 이용해 mosters리스트를 탐색한다
        //그중 hp가 0이된 객체는 destroy monster()실행하며 animation보여주고 리스트에서 삭제한다.
        foreach(var item in monsters)
        {
            //2021/01.31 패턴일치사용한 클래스 분석?
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
