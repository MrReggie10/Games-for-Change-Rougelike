using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public void Damage(int damage, float knockbackTime, float knockbackForce);
}
