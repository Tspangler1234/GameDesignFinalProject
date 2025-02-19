using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyScript : EntityScript
{
    //variables
    [SerializeField] private int DetectRange;
    [SerializeField] private GameObject target;
    [SerializeField] private List<GameObject> createOnDefeat;
    [SerializeField] private Slider hpBar;
    private protected Vector3 targetPos;
    private protected Vector3 targetDir;
    private NavMeshAgent agent;
    private RoomScript room;
    private bool defeated = false;
    private Collider2D targetHitboxCollider;
    private Vector3 targetHitboxPos;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = 0;

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        if (currentWeapon != null)
        {
            weaponScript = currentWeapon.GetComponent<WeaponScript>();
        }
        
        hpBar.maxValue = hp;
        hpBar.value = hp;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        agent.speed = moveSpeed;

        if (target != null && isAlive)
        {
            GameObject targetHitboxObj = target.transform.FindObjectsWithTag("Hitbox").FirstOrDefault();

            targetPos = target.transform.position;

            if (targetHitboxObj != null )
            {
                targetHitboxCollider = targetHitboxObj.GetComponent<Collider2D>();
                targetHitboxPos = targetHitboxCollider.transform.position;

                targetPos = target.GetComponent<Collider2D>().transform.position;

                //addjust aim to the hitbox + offset
                targetHitboxPos = new Vector3(targetHitboxPos.x + targetHitboxCollider.offset.x, targetHitboxPos.y + targetHitboxCollider.offset.y, targetHitboxPos.z);

                weaponPos = weaponHolder.transform.position;

                AimWeapon(weaponPos, targetHitboxPos);

            }

            MoveToTarget();
            TryAttack();
        }
    }

    public void MoveToTarget()
    {
        float moveRange = 0;

        if (weaponScript != null)
        {
            moveRange = weaponScript.GetTotalStatPower("atkRange");
        }

        agent.SetDestination(new Vector3(targetPos.x, targetPos.y, transform.position.z));
        agent.stoppingDistance = moveRange * 0.5f;
    }

    public void TryAttack()
    {
        if (currentWeapon != null)
        {
            //checks if player is inrange of weapon
            weaponScript = currentWeapon.GetComponent<WeaponScript>();
            targetDir = new Vector2(targetHitboxPos.x - transform.position.x, targetHitboxPos.y - transform.position.y).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDir, weaponScript.GetTotalStatPower("atkRange"), 1 << target.layer);
            if (hit.collider != null)
            {
                if (hit.transform.tag == "Player")
                {
                    UseWeapon();
                }
            }
        }
    }

    private protected override void OnDefeated()
    {
        base.OnDefeated();
        if (!defeated)
        {
            foreach (GameObject obj in createOnDefeat)
            {
                Instantiate(obj, transform.position, Quaternion.identity);
            }
            defeated = true;
         

            //remove itself from room list
            if (room != null)
            {
                room.RemoveEnemy(gameObject);
            }
            //Destroy(gameObject);
            hpBar.transform.parent.gameObject.SetActive(false);
            hpBar.gameObject.SetActive(false);

            GetComponent<NavMeshAgent>().enabled = false;
        }
    }
    public void SetRoom(RoomScript _room)
    {
        room = _room;
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        targetPos = target.transform.position;
    }

    private void OnDrawGizmos()
    {
        float range = 1f;

        if (weaponScript != null)
        {
            range = weaponScript.GetTotalStatPower("atkRange");
        }

        if (target != null)
        {
            Gizmos.DrawLine(transform.position, transform.position + targetDir * range);
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        hpBar.value = hp;
    }
}
