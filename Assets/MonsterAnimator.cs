using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimator : MonoBehaviour
{
    private Monster monster;
    private bool Isdead = false;

    // Start is called before the first frame update
    void Start()
    {
        if (monster == null)
        {
            monster = FindObjectOfType<Monster>();
            if (monster == null)
            {
                Debug.LogError("Monster not found in the scene!");
            }
        }
       
    }
    
    
        void Update()
    {
        if (monster == null || Isdead) return;

        if (monster.health <= 0)
        {
            Isdead = true;
            Animator anim = GetComponent<Animator>();
            if (anim != null) anim.SetTrigger("Die");
        }
    }
}