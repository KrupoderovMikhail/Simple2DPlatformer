using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Unit
{
    /* Разрушение при косании, стандартный метод */
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Bullet bullet = collider.GetComponent<Bullet>();

        if (bullet)
        {
            ReceivedDamage();
        }
    }
}
