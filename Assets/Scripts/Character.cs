using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    [SerializeField]
    private int lives = 5;

    [SerializeField]
    private float speed = 3.0F;

    [SerializeField]
    private float jumpForce = 15.0F;

    private bool isGrounded = false;

    private Bullet bullet;

    private CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value);  }
    }

    new private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer sprite;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        bullet = Resources.Load<Bullet>("Bullet");
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        State = CharState.Idle;

        if (Input.GetButtonDown("Fire1")) Shoot();
        if (Input.GetButton("Horizontal")) Run();
        if (isGrounded && Input.GetButtonDown("Jump")) Jump(); // Если на земле и нажат пробел, персонаж прыгает
    }

    private void Run()
    {
        Vector3 direction = transform.right * Input.GetAxis("Horizontal");

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

        sprite.flipX = direction.x < 0.0F;

        State = CharState.Run;
    }


    /* Прыжок персонажа */
    private void Jump() 
    {
        rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

        State = CharState.Jump;
    }
    
    /* Стрельба персонажа */
    private void Shoot()
    {
        Vector3 position = transform.position;
        position.y += 0.8F;
        Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation) as Bullet;

        newBullet.Parent = gameObject;
        newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1.0F : 1.0F);
    }

    public override void ReceivedDamage()
    {
        lives--;

        rigidbody.velocity = Vector3.zero; // Обнуляем ускорение при соприкосновении 
        rigidbody.AddForce(transform.up * 8.0F, ForceMode2D.Impulse);

        Debug.Log(lives);
    }


    /* Находится ли персонаж на земле */
    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3F);  // Коллайдер под персонажем(сфера)

        isGrounded = colliders.Length > 1; // Если коллайдер больше 1, то мы на земле
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.gameObject.GetComponent<Unit>();
        if (unit) ReceivedDamage();
    }

    /* Состояния игрока */
    public enum CharState
    {
        Idle, // 0
        Run,  // 1
        Jump  // 2
    }
}

