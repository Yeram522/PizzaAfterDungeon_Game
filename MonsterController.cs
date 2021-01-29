using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

static class Costants
{
    public const float ACCESS = 0.5f;

}

public class Monster
{
    MonsterController obj;
    public GameObject me;
    public Rigidbody2D rgd;
    public SpriteRenderer sRenderer;
    public float hp;  //체력
    public float speed; //속도
    public bool trace_trigger;

    //player와 moster 사이의 거리를 float형식으로 반환한다.
    public float cal_distance(GameObject player)
    {
        float playerpos = player.transform.position.x; //참조
        float monsterpose = me.transform.position.x;
        float distance;
        distance = playerpos - monsterpose;
        return Mathf.Abs(distance);
    }

    public bool Dialog()
    {
        return true;
    }

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

    public void ATTACK()
    {
        //공격에 따라서, 데미지주는 공격력등이 차이가 이씀
    }


    //public void rand_std()   계속 이동하면 정신 사나우니까 중간에 이동하다가 쉬느 ㄴ타이밍 랜덤으로 만들어주기
}

public class boss_burgerking : Monster
{
    public boss_burgerking()
    {
        this.me = GameObject.Find("BurgerKing"); //오브젝트 할당
        //this.pos = this.me.GetComponent<Transform>(); //transform
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
    }
    //충돌시 데미지 플레이어와 일정거리 가까어졌을 때 내리치는 모션
}

public class MonsterController : MonoBehaviour
{
    Animator animator;
    public GameObject Burgerking, Burgersoldier;
    public GameObject player;
    public Vector2 playerPos; //플레이어의 실시간 위치를 반영한다(update에서)
    private GameObject[,]  monster_array = new GameObject[2,10];
    private int[] count_monster = new int[2]; //현재 몬스터 얼마있는지 관리
    //몬스터 개수에 따라 크기는 달라질 수 있음

    void Start()
    {
        animator = Burgerking.GetComponent<Animator>();
    }

    bool isDeath(bool[] array)
    {
        return true;
    }
    void Create_burgerking(GameObject player)
    {
        boss_burgerking st1_boss = new boss_burgerking(); //버거킹 객체 생성
       
        st1_boss.isAccess(player);
        st1_boss.TRACE(player);
    }

    void Create_burgersoldier(int count)
    {
        normal_burgersoldier new_burgersoldier = new normal_burgersoldier();
         
        monster_array[1, count] = Instantiate(new_burgersoldier.me, new_burgersoldier.me.transform.position, new_burgersoldier.me.transform.rotation);
        //GameObject정보 저장!
        
        

    }

    // Update is called once per frame
    void Update()
    {
        playerPos.x = player.transform.position.x;
        

    }
}
