using UnityEngine;
using TMPro;
using MVVM;

public class PlayerStatsView : MonoBehaviour
{
    [Data("Score")] public TMP_Text _scoreText;
    [Data("LaserAmmo")] public TMP_Text _laserAmmo;
    [Data("LaserCooldown")] public TMP_Text _laserCooldown;
    [Data("XAxis")] public TMP_Text _xAxis;
    [Data("YAxis")] public TMP_Text _yAxis;
    [Data("ZRotation")] public TMP_Text _zRotation;
    [Data("Speed")] public TMP_Text _speedText;
}