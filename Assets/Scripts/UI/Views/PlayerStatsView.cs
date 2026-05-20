using UnityEngine;
using TMPro;
using MVVM;

public class PlayerStatsView : MonoBehaviour
{
    [Data("Score")] public TMP_Text ScoreText;
    [Data("LaserAmmo")] public TMP_Text LaserAmmo;
    [Data("LaserCooldown")] public TMP_Text LaserCooldown;
    [Data("XAxis")] public TMP_Text XAxis;
    [Data("YAxis")] public TMP_Text YAxis;
    [Data("ZRotation")] public TMP_Text ZRotation;
    [Data("Speed")] public TMP_Text SpeedText;
    [Data("Health")] public TMP_Text HealthText;
}