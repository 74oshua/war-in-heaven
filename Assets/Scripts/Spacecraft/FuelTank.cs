using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FuelType
{
    HYDROGEN,
    STEAM,
    HYDRAZINE
}

public class FuelTank : SpacecraftPart
{
    // type of fuel
    public FuelType fuel_type = FuelType.HYDROGEN;

    // max amount of fuel in tank in kg
    public float max_fuel = 0.0f;

    // high priority tanks will be drawn from before lower priority tanks
    public int priority = 0;

    // mass of empty tank in kg, assigned from mass during init()
    private float _dry_mass;

    // current amount of fuel in tank in kg
    private float _fuel;

    public override void Init(Spacecraft _sc)
    {
        base.Init(_sc);

        _dry_mass = mass;
        _fuel = max_fuel;
        updateMass();
        _sc.addFuelTank(this);
    }
    
    // reduces amount of fuel in tank, affecting the tank's mass
    // returns amount of fuel used, will be less than amount if _fuel < amount
    public float useFuel(float amount)
    {
        // return 0 if empty
        // prevents returning tiny values due to rounding error
        if (_fuel == 0)
        {
            return 0;
        }

        float used = amount;
        _fuel -= amount;

        if (_fuel < 0)
        {
            used += _fuel;
            _fuel = 0;
        }

        updateMass();

        return used;
    }

    // creates fuel in tank
    // returns amount of fuel added, will be less than amount if (_fuel + amount) > max_fuel
    public float addFuel(float amount)
    {
        float added = amount;
        _fuel += amount;

        if (_fuel > max_fuel)
        {
            added -= (_fuel - max_fuel);
            _fuel = max_fuel;
        }

        updateMass();

        return added;
    }

    // transfers fuel from tank a to tank b
    // returns amount of fuel transfered, will be less than amount if tank being
    public static float transferFuel(FuelTank source, FuelTank destination, float amount)
    {
        // if tanks contain different fuel types, return 0
        if (source.fuel_type != destination.fuel_type)
        {
            return 0;
        }

        return destination.addFuel(source.useFuel(amount));
    }

    private void updateMass()
    {
        mass = _dry_mass + _fuel;
    }
}
