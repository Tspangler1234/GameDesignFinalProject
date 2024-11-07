using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : EntityScript
{
    //variables
    [SerializeField] private int currency;
    private Vector2 lastInput;
    private Vector3 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        weaponHolder = transform.Find("WeaponHolder");
        currentWeapon = weaponHolder.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = new Vector2(lastInput.x, lastInput.y) * moveSpeed;

        mousePos = Input.mousePosition;
        weaponPos = Camera.main.WorldToScreenPoint(weaponHolder.position);
        mousePos.z = 10;

        AimWeapon(weaponPos, mousePos);
    }

    public void OnMove(InputAction.CallbackContext mv)
    {
        if (mv.started || mv.performed)
        {
            //movement started or changed
            lastInput = mv.ReadValue<Vector2>();
        }
        else if (mv.canceled)
        {
            //no movement
            lastInput = Vector2.zero;
        }
    }

    public void OnFire(InputAction.CallbackContext fb)
    {
        canAttack = true;

        if (fb.started)
        {
            UseWeapon();
        }
    }

    public void SwitchWeapon()
    {

    }
}