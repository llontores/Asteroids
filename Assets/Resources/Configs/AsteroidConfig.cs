using System;

[Serializable]
public class AsteroidConfig
{
    public float Thrust;
    public float Drag;
    public float MaxSpeed;
    public float SpinningMinSpeed;
    public float SpinningMaxSpeed;
    public int MinFragmentAmount;
    public int MaxFragmentAmount;
    public float BounceForce;
    public int Reward;
}