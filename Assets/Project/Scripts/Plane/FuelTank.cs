using UnityEngine;

public class FuelTank : MonoBehaviour
{
    /*
     *Fuel tank to make engines run if they require.
     */

    [SerializeField]
    float maxCapacityLitres = 200f;

    [SerializeField]
    float currentFuelAmountLitres = 100f;


    /// <summary>
    /// returns true if fuel is empty
    /// </summary>
    public bool IsDry()
    {
        if(currentFuelAmountLitres <= 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// change the current amount of fuel by amount. Used by engines and to refuel.
    /// </summary>
    public void ChangeAmount(float amount)
    {
        currentFuelAmountLitres += amount;
        if(currentFuelAmountLitres <= 0)
        {
            currentFuelAmountLitres = 0;
        }
        else if(currentFuelAmountLitres > maxCapacityLitres)
        {
            currentFuelAmountLitres = maxCapacityLitres;
        }
    }
    /// <summary>
    /// Get current fuel amount
    /// </summary>
    public float GetCurrentFuelAmount()
    {
        return currentFuelAmountLitres;
    }
}
