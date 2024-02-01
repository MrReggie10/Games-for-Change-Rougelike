using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public enum entityState
    {
        freeMove, //normal movement
        stun,     //can't move but forces can be applied
    }
    public IEnumerator ActivateStun(float secondsActive);
    public void Damage(int damage, float knockbackTime, float knockbackForce);
}
