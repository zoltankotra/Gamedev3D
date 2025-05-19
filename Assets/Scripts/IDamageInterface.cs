using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageInterface
{
    void TakeDamage(int damage);
    void SetHealthAndAttack(int health, int attack);
}
