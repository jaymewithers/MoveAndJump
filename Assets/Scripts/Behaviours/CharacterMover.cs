﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMover : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 movement;

    public float rotateSpeed = 120f, gravity = -9.81f, jumpForce = 30f, energyChange = 0.1f;
    private float yVar;

    public IntData playerJumpCount;
    private int jumpCount;

    public FloatData normalSpeed, fastSpeed, energy;
    private FloatData moveSpeed;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        moveSpeed = normalSpeed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && energy.value > 0)
        {
            moveSpeed = fastSpeed;
            StopCoroutine(EnergyReFill());
            StartCoroutine(EnergyDrain());
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = normalSpeed;
            StopCoroutine(EnergyDrain());
            StartCoroutine(EnergyReFill());
        }

        var vInput = Input.GetAxis("Vertical") * moveSpeed.value;
        movement.Set(vInput, yVar, 0);

        var hInput = Input.GetAxis("Horizontal") * Time.deltaTime * rotateSpeed;
        transform.Rotate(0, hInput, 0);

        yVar += gravity * Time.deltaTime;

        if (controller.isGrounded && movement.y < 0)
        {
            jumpCount = 0;
        }

        if (Input.GetButtonDown("Jump") && jumpCount < playerJumpCount.value)
        {
            yVar = jumpForce;
            jumpCount++;
        }

        movement = transform.TransformDirection(movement);
        controller.Move(movement * Time.deltaTime);
    }
    
    public WaitForFixedUpdate wffu = new WaitForFixedUpdate();
    public float delayTime = 0.5f;

    private IEnumerator EnergyDrain()
    {
        while (moveSpeed == fastSpeed && energy.value >= 0)
        {
            yield return wffu;
            energy.value -= energyChange;
            yield return new WaitForSeconds(delayTime);
        }
    }

    private IEnumerator EnergyReFill()
    {
        while (moveSpeed == normalSpeed && energy.value < 1)
        {
            yield return wffu;
            energy.value += energyChange;
            yield return new WaitForSeconds(delayTime);
        }
    }
}