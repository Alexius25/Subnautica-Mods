using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace SubnauticaToolConfig.Settings;

[Menu("Tool Config")]
public class ModConfig : ConfigFile
{
    internal static ModConfig Instance { get { return Plugin.ModConfig; } }

    //Air Bladder Config
    [Slider("Air Bladder: Oxygen Capacity", 1, 1000, DefaultValue = 15, Tooltip = "Changes the Air Bladders Oxygen Capacity.")]
    public float AirBladderOxygenCapacity = 15;

    [Slider("Air Bladder: Buoyancy Force", 1, 1000, DefaultValue = 76, Tooltip = "Changes the Air Bladders Buoyancy Force.")]
    public float AirBladderBuoyancyForce = 76;
    // Air Bladder Conig End

    // Fire Extinguisher Config
    [Slider("Fire Extinguisher: Max Fuel [EXPERIMENTAL]", 1, 1000, DefaultValue = 100, Tooltip = "Changes the Fire Extinguishers Max Fuel. [EXPERIMENTAL]")]
    public float FireExtinguisherMaxFuel = 100;

    [Toggle("Fire Extinguisher: Unlimited Fuel [EXPERIMENTAL]", Tooltip = "Enables Unlimited Fire Extinguisher Fuel. [EXPERIMENTAL]")]
    public bool FireExtinguisherUnlimitedFuel = false;
    // Fire Extinguisher Config End

    // Flare Config
    [Slider("Flare: Trow Force", 1, 1000, DefaultValue = 100, Tooltip = "Changes the Flares Throw Force.")]
    public float FlareThrowForce = 100;
    // Flare Config End

    // Knife Config
    [Slider("Knife: Damage", 1, 1000, DefaultValue = 20, Tooltip = "Changes the Knifes Damage.")]
    public float KnifeDamge = 20;

    [Slider("Knife: Range", 1f, 1000f, DefaultValue = 1.2f, Tooltip = "Changes the Knifes Range.")]
    public float KnifeAttackDist = 1.2f;

    [Toggle("Knife: One Hit Mode", Tooltip = "Enables Knife One Hit Mode.")]
    public bool KnifeOneHit = false;
    // Knife Config End

    // Heat Knife Config
    [Slider("Heat Blade: Damage", 1, 1000, DefaultValue = 20, Tooltip = "Changes the Heat Blades Damage.")]
    public float HeatBladeDamge = 20;

    [Slider("Heat Blade: Range", 1f, 1000f, DefaultValue = 1.2f, Tooltip = "Changes the Heat Blades Range.")]
    public float HeatBladeAttackDist = 1.2f;

    [Toggle("Heat Blade: One Hit Mode", Tooltip = "Enables Heat Blade One Hit Mode.")]
    public bool HeatBladeOneHit = false;
    // Heat Knife Config End

    // Laser Cutter Config
    [Slider("Laser Cutter: Energy Cost", 0.1f, 5f, DefaultValue = 1f, Tooltip = "Changes the Laser Cutters Energy Cost.")]
    public float LaserCutterEnergyCost = 1f;

    [Toggle("Laser Cutter: Unlimited Energy", Tooltip = "Enables Unlimited Laser Cutter Energy.")]
    public bool LaserCutterUnlimitedEnergy = false;
    // Laser Cutter Config End

    // Seaglide Config
    [Slider("Seaglide: Speed Multiplier", 1f, 25f, DefaultValue = 1f, Tooltip = "Changes the Seaglides Speed Multiplier. Warning: High Speed Could Result in Glitches. Restart Required")]
    public float SeaglideSpeedMultiplier = 1f;
    // Seaglide Config End

    // Stasis Rifle Config
    [Slider("Stasis Rifle: Energy Cost", 0.1f, 10f, DefaultValue = 5f, Tooltip = "Changes the Stasis Rifles Energy Cost.")]
     public float StasisRifleEnergyCost = 5f;

    [Toggle("Stasis Rifle: Unlimited Energy", Tooltip = "Enables Unlimited Stasis Rifle Energy.")]
    public bool StasisRifleUnlimitedEnergy = false;
    // Stasis Rifle Config End

    // Stasis Sphere Config
    [Slider("Stasis Sphere: Max Radius", 1f, 100f, DefaultValue = 10f, Tooltip = "Changes the Stasis Spheres Max Radius.")]
    public float StasisSphereMaxRadius = 10f;

    [Slider("Stasis Sphere: Min Radius", 0.1f, 5f, DefaultValue = 1f, Tooltip = "Changes the Stasis Spheres Min Radius.")]
    public float StasisSphereMinRadius = 1f;

    [Slider("Stasis Sphere: Max Time", 1f, 100f, DefaultValue = 20f, Tooltip = "Changes the Stasis Spheres Max Time.")]
    public float StasisSphereMaxTime = 20f;

    [Slider("Stasis Sphere: Min Time", 0.1f, 5f, DefaultValue = 4f, Tooltip = "Changes the Stasis Spheres Min Time.")]
    public float StasisSphereMinTime = 4f;
    // Stasis Sphere Config End

    // Welder Config
    [Slider("Welder: Energy Cost", 0.1f, 5f, DefaultValue = 1f, Tooltip = "Changes the Welders Energy Cost.")]
    public float WelderEnergyCost = 1f;

    [Toggle("Welder: Unlimited Energy", Tooltip = "Enables Unlimited Welder Energy.")]
    public bool WelderUnlimitedEnergy = false;

    [Slider("Welder: Health per Weld", 1, 100, DefaultValue = 10, Tooltip = "Changes the Welders Health per Weld.")]
    public float WelderHealthPerWeld = 10;
    
    [Toggle("Welder: Unlimited Health per Weld", Tooltip = "Enables Welder Unlimited Health per Weld Mode.")]
    public bool WelderUnlimitedHealthPerWeld = false;
    // Welder Config End
}