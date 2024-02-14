using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Api.Towers;

namespace IndustrialFarmer;

public class IndustrialFarmer : ModHero
{
    public override string BaseTower => TowerType.BananaFarmer;

    public override int Cost => 1400;

    public override string DisplayName => "诺曼";
    public override string Title => "专业农夫";
    public override string Level1Description => "收集附近的香蕉，提供一个免费的香蕉农场";

    public override string Description =>
        "诺曼可以收集香蕉，帮助发展你的农业";


    public override string NameStyle => TowerType.Gwendolin; // Yellow colored
    public override string BackgroundStyle => TowerType.Etienne; // Yellow colored
    public override string GlowStyle => TowerType.StrikerJones; // Yellow colored


    public override int MaxLevel => 20;
    public override float XpRatio => 1.0f;

    /// <param name="towerModel"></param>
    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        var quincy = Game.instance.model.GetTowerWithName(TowerType.Quincy);
        towerModel.AddBehavior(quincy.GetBehavior<CreateSoundOnUpgradeModel>().Duplicate());
        towerModel.AddBehavior(quincy.GetBehavior<CreateEffectOnUpgradeModel>().Duplicate());

        towerModel.radius = quincy.radius;
        towerModel.footprint = quincy.footprint.Duplicate();
    }

    public const string IndustrialFarmerDiscount = "IndustrialFarmerDiscount";
}