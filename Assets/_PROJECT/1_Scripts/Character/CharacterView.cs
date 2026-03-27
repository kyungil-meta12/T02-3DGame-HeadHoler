
using System;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    private Character character;
    private Collider col;

    private void Awake()
    {
        character = GetComponentInParent<Character>();
        col = GetComponent<Collider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Evidence")) 
        {
            if (character.curSecondState == SecondState.Hurt)
            {
                return;
            }
            Vector3 dirToTarget = (other.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < character.viewAngle / 2)
            {
                float dst = Vector3.Distance(transform.position, other.transform.position);
                if (Physics.Raycast(transform.position, dirToTarget, dst,
                        1<<LayerMask.NameToLayer("Entity")))
                {
                    Debug.DrawRay(transform.position, dirToTarget, Color.red);
                    character.curSecondState = SecondState.Scream;
                    character.target = other.transform;
                    character.React();
                }
            }
        }
        
        //todo 다친 사람 발견
        if (other.CompareTag(this.tag) && character.curSecondState == SecondState.Hurt)
        {
            character.target = other.transform;
            
        }
    }
}
