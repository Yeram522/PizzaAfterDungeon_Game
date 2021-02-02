using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//몬스터클래스의 메서드들을 사용한다.
public class MonsterController : MonoBehaviour
{
    Monster monster;
    public static List<Monster> monsters = new List<Monster>();
    public static GameObject Burgerking, Burgersoldier;
    public GameObject player;
    public Vector2 playerPos; //플레이어의 실시간 위치를 반영한다(update에서)
    //public delegate void MyDelegate(); //델리게이트 선언, 함수를 매개변수로 할 수 있는 클래스 생성

    //몬스터 스폰 위치를 결정한다.
    public static Vector2 spon_position()
    {
        Vector2 rand_pos;
        rand_pos.x = 68.13f;
        rand_pos.y = -63.46f;
        //rand함수로 아무데나 스폰
        return rand_pos;
    }
  
    //일정시간 간격으로 스킬을 사용하게 하는 함수(매개변수: 시간간격값,행동함수)
    /*public static void skill_cool_down(float interver, MyDelegate skill_func)
    {
        if (Mathf.Round(interver) <= 0)
        {
          

        }

        interver -= Time.deltaTime;
    }*/

     //clear조건을 판단하는 함수_이후의 이벤트를 trigger 할 수 있다.
    void check_clear(Monster monster, string name)
    {
        //for을 이용해 mosters리스트를 탐색한다
        //그중 hp가 0이된 객체는 destroy monster()실행하며 animation보여주고 리스트에서 삭제한다.
        
        switch (name){
            case "burger_paze1":
                int count = 0;
                foreach (var item in monsters)
                {
                    var burger_soldier = item as normal_burgersoldier;
                    //burgersoldier type이고 죽었으면
                    if (burger_soldier != null && burger_soldier.condition==Costants.DIE)
                        count++;

                }
                break;

        }

        
    }

    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        playerPos.x = player.transform.position.x;
        monsters.Add(new normal_burgersoldier()); //옵젝생성
    }
}
