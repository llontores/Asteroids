

public class ConfigProvider
{
    public PlayerConfig Player { get; private set; }
    public BulletConfig Bullet { get; private set; }
    public AsteroidConfig Asteroid { get; private set; }
    public FragmentConfig Fragment { get; private set; }
    public UFOConfig UFO { get; private set; }
    public HazardSpawnerConfig HazardSpawner { get; private set; }
    public BulletsShooterConfig BulletsShooter { get; private set; }
    public ExplosionParticlesPoolConfig ExplosionParticlesPool { get; private set; }

    public void LoadAll()
    {
        Player = JsonConfigLoader.LoadFromResources<PlayerConfig>("Configs/player_config");
        Bullet = JsonConfigLoader.LoadFromResources<BulletConfig>("Configs/bullet_config");
        Asteroid = JsonConfigLoader.LoadFromResources<AsteroidConfig>("Configs/asteroid_config");
        Fragment = JsonConfigLoader.LoadFromResources<FragmentConfig>("Configs/fragment_config");
        UFO = JsonConfigLoader.LoadFromResources<UFOConfig>("Configs/ufo_config");
        HazardSpawner = JsonConfigLoader.LoadFromResources<HazardSpawnerConfig>("Configs/hazardSpawner_config");
        BulletsShooter = JsonConfigLoader.LoadFromResources<BulletsShooterConfig>("Configs/bulletsShooter_config");
        ExplosionParticlesPool = JsonConfigLoader.LoadFromResources<ExplosionParticlesPoolConfig>("Configs/explosionParticlesPool_config");
    }
}