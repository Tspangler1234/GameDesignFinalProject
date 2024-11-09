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
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        rb2d.velocity = new Vector2(lastInput.x, lastInput.y) * moveSpeed;

        mousePos = Input.mousePosition;
        weaponPos = Camera.main.WorldToScreenPoint(weaponHolder.position);
        mousePos.z = 10;

        if (mousePos != null && weaponPos != null )
        {
            AimWeapon(weaponPos, mousePos);
        }

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

    public void ChangeCurrency(int amount)
    {
        currency += amount;
    }
}
