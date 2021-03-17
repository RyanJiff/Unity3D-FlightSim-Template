//
// Copyright (c) Ryan Jiffri. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{

    [Range(0.0f,1.0f)]
    public float throttleInput = 0.0f;
    [Space]

    [Header("Engine stats")]
    [Space]
    public bool ignition = false;
    [Space]
    public float fuelUsePerSecondAtMaxPower = 0.012f;
    [Space]
    public bool requireFuel = true;
    [Space]
    public float maxThrust = 3000;
    public AnimationCurve thrustAirspeedCurve;
    public Transform engineTransform;
    [Range(0.0f, 1.0f)]
    public float iddleInput = 0.05f;
    public float rampSpeed = 0.5f;
    [Space]

    [Header("Visual")]
    [SerializeField]
    float RPM = 0;
    public AnimationCurve RPMCurve;
    public float RPMMult = 3000;
    public Transform engineAnimationMesh;

    [Header("Debug")]
    [SerializeField]
    float currentThrust;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    float currentPower = 0.0f;

    private Rigidbody rigid;
    private FuelTank[] fuelTanks = null;

    const float msToKnots = 1.94384f;

    void Awake()
    {
        rigid = GetComponentInParent<Rigidbody>();
        fuelTanks = GetComponents<FuelTank>();
    }

    void FixedUpdate()
    {
        if (!engineTransform)
        {
            Debug.LogWarning(name + ": No engine Transform location assigned!");
            return;
        }
        
        if (rigid != null)
        {
            rigid.AddForceAtPosition(engineTransform.forward * currentPower * maxThrust * thrustAirspeedCurve.Evaluate(rigid.velocity.magnitude * msToKnots), engineTransform.position, ForceMode.Force);
            currentThrust = currentPower * maxThrust * thrustAirspeedCurve.Evaluate(rigid.velocity.magnitude * msToKnots);
        }
        if (engineAnimationMesh)
        {
            engineAnimationMesh.Rotate(0, 0, RPM * 6f * Time.fixedDeltaTime);
        }
    }
    void Update()
    {
        if (ignition)
        {
            //power calculations
            currentPower = Mathf.MoveTowards(currentPower, Mathf.Max(iddleInput, throttleInput), rampSpeed * Time.deltaTime * Mathf.Abs(Mathf.Max(iddleInput, throttleInput) - currentPower));
        }
        else
        {
            currentPower = Mathf.MoveTowards(currentPower, 0, rampSpeed/2 * Time.deltaTime);
        }
        if (requireFuel)
        {
            //fuel calculations
            fuelUsePerSecondAtMaxPower = Mathf.Abs(fuelUsePerSecondAtMaxPower) * -1;
            bool allDry = true;
            foreach (FuelTank f in fuelTanks)
            {
                f.ChangeAmount(fuelUsePerSecondAtMaxPower * currentPower * Time.deltaTime);
                if (!f.IsDry())
                {
                    allDry = false;
                }
            }
            if (allDry)
            {
                ignition = false;
            }
        }
        RPM = RPMCurve.Evaluate(currentPower) * RPMMult;
    }
    public float GetRPM()
    {
        return RPM;
    }
    public float GetPower()
    {
        return currentPower;
    }
    public void ToggleIgnition()
    {
        ignition = !ignition;
    }
    public bool IsOn()
    {
        return ignition;
    }
    public float GetFuelAmount()
    {
        float fuelAmount = 0f;
        foreach(FuelTank f in fuelTanks)
        {
            fuelAmount += f.GetCurrentFuelAmount();
        }
        return fuelAmount;
    }
}
