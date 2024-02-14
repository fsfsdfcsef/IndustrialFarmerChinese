using System.Runtime.CompilerServices;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Utils;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace IndustrialFarmer;

public class Levels
{
    public class Level2 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "收集的香蕉价值增加10%";

        public override int Level => 2;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetBehavior<CollectCashZoneModel>().name = "CollectCashZoneModel_" + Id;
        }
    }

    public class Level3 : ModHeroLevel<IndustrialFarmer>
    {
        public override string AbilityName => "气球杀灭剂";

        public override string AbilityDescription =>
            "向气球喷洒大范围的农药，每秒造成数次伤害";

        public override string Description => $"{AbilityName}: {AbilityDescription}";

        public override int Level => 3;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var quincy = Game.instance.model.GetTowerWithName(TowerType.Quincy + " 10");
            var glue = Game.instance.model.GetTowerWithName(TowerType.GlueGunner + "-300");
            var glueStorm = Game.instance.model.GetTowerWithName(TowerType.GlueGunner + "-040");

            var dot = glue.GetDescendant<AddBehaviorToBloonModel>().Duplicate();
            dot.mutationId = "Pesticide";
            // dot.overlayLayer = 2; TODO
            // dot.overlays = glue.GetDescendant<SlowModel>().overlays;
            dot.glueLevel = 0;
            dot.collideThisFrame = true;

            var abilityModel = quincy.GetAbility(1).Duplicate();
            abilityModel.name = "AbilityModel_BloonPesticide";
            abilityModel.displayName = AbilityName;
            abilityModel.addedViaUpgrade = Id;
            abilityModel.icon = GetSpriteReference("BloonPesticide");
            abilityModel.RemoveBehavior<CreateSoundOnAbilityModel>();
            abilityModel.AddBehavior(glueStorm.GetDescendant<CreateSoundOnAbilityModel>());
            abilityModel.Cooldown = 30;
            towerModel.AddBehavior(abilityModel);

            var attack = abilityModel.GetBehavior<ActivateAttackModel>().attacks[0];
            attack.targetProvider = attack.GetBehavior<TargetStrongModel>();
            attack.RemoveBehavior<TargetFirstModel>();
            attack.RemoveBehavior<TargetCloseModel>();
            attack.RemoveBehavior<TargetLastModel>();

            var splat = Game.instance.model.GetTower(TowerType.GlueGunner, 3, 2)
                .GetDescendant<CreateEffectOnContactModel>().effectModel.assetId;
            var effectModel = attack.GetDescendant<CreateEffectOnExhaustFractionModel>().effectModel;
            effectModel.assetId = splat;
            effectModel.scale = 3f;

            var projectile = attack.GetDescendant<CreateProjectileOnExhaustFractionModel>().projectile;
            projectile.RemoveBehavior<DamageModel>();
            projectile.RemoveBehavior<DamageModifierForTagModel>();
            projectile.GetBehavior<AgeModel>().Lifespan = .1f;
            projectile.RemoveBehavior<ClearHitBloonsModel>();
            projectile.GetBehavior<ProjectileFilterModel>().filters = projectile
                .GetBehavior<ProjectileFilterModel>().filters
                .RemoveItemOfType<FilterModel, FilterWithChanceModel>();
            projectile.collisionPasses = new[] { -1, 0 };
            projectile.filters = projectile.GetBehavior<ProjectileFilterModel>().filters;
            projectile.radius = 50;
            projectile.AddBehavior(dot);
        }
    }

    public class Level4 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "范围内的所有香蕉农场获得免费1阶升级";

        public override int Level => 4;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.AddBehavior(new FreeUpgradeSupportModel("", 1,
                "IndustrialFarmer:FreeUpgrades",new []
                {
                    new FilterInBaseTowerIdModel("", new[] { TowerType.BananaFarm })
                }));
        }
    }


    public class Level5 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "香蕉农场能够放置在附近的水域";

        public override int Level => 5;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.AddBehavior(new FreezeNearbyWaterModel("", towerModel.range, 1,
                new PrefabReference()));
        }
    }

    public class Level6 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "范围内的所有香蕉农场获得免费2阶升级";

        public override int Level => 6;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetBehavior<FreeUpgradeSupportModel>().upgrade = 2;
        }
    }

    public class Level7 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "范围增加";

        public override int Level => 7;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.range += 15;
            towerModel.GetBehavior<CollectCashZoneModel>().attractRange += 15;
        }
    }

    public class Level8 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "范围内的香蕉农场升级价格减少10%";

        public override int Level => 8;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.AddBehavior(new DiscountZoneModel("", .1f, 1,
                IndustrialFarmer.IndustrialFarmerDiscount,
                "IndustrialFarmer", false, 0, "IndustrialFarmer", GetTextureGUID("IndustrialFarmer-Icon")));
        }
    }

    public class Level9 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "收集的香蕉价值增加15%";

        public override int Level => 9;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetBehavior<CollectCashZoneModel>().name = "CollectCashZoneModel_" + Id;
        }
    }

    public class Level10 : ModHeroLevel<IndustrialFarmer>
    {
        public override string AbilityName => "绿色ge命";

        public override string AbilityDescription =>
            "将一些最大的气球变成绿气球，最高锁定BFB";

        public override string Description => $"{AbilityName}: {AbilityDescription}";

        public override int Level => 10;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var bma = Game.instance.model.GetTower(TowerType.Alchemist, 0, 0, 5);
            var techTerror = Game.instance.model.GetTower(TowerType.SuperMonkey, 0, 4);

            var shrink = bma.GetAttackModel(2).Duplicate();

            var filterModels = shrink.GetBehavior<AttackFilterModel>().filters.ToList();
            filterModels.Add(new FilterOutTagModel("1", BloonTag.Red, new Il2CppStringArray(0)));
            filterModels.Add(new FilterOutTagModel("2", BloonTag.Blue, new Il2CppStringArray(0)));
            filterModels.Add(new FilterOutTagModel("3", BloonTag.Green, new Il2CppStringArray(0)));
            filterModels.Add(new FilterOutTagModel("4", BloonTag.Ddt, new Il2CppStringArray(0)));
            filterModels.Add(new FilterOutTagModel("5", BloonTag.Zomg, new Il2CppStringArray(0)));

            var projectile = shrink.GetDescendant<CreateProjectileOnExhaustFractionModel>().projectile;
            projectile.radius = 50;
            projectile.GetBehavior<MorphBloonModel>().bloonId = BloonType.Green;
            projectile.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
            projectile.pierce += 100;

            var abilityModel = towerModel.GetAbility().Duplicate();
            abilityModel.name = "AbilityModel_GreenRevolution";
            abilityModel.displayName = AbilityName;
            abilityModel.addedViaUpgrade = Id;
            abilityModel.GetDescendant<AttackFilterModel>().filters = filterModels.ToIl2CppReferenceArray();
            abilityModel.GetDescendant<CreateProjectileOnExhaustFractionModel>().projectile = projectile;
            abilityModel.GetDescendant<CreateEffectOnExhaustFractionModel>().effectModel =
                techTerror.GetDescendant<CreateEffectOnAbilityModel>().effectModel;
            abilityModel.GetDescendant<CreateProjectileOnExhaustFractionModel>().AddChildDependant(projectile);
            abilityModel.RemoveBehavior<CreateSoundOnAbilityModel>();
            abilityModel.Cooldown = 60;
            abilityModel.icon = GetSpriteReference("GreenRevolution");
            towerModel.AddBehavior(abilityModel);
        }
    }

    public class Level11 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "气球杀灭剂伤害大幅增加";

        public override int Level => 11;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var dot = towerModel.GetAbility(0).GetDescendant<DamageOverTimeModel>();

            dot.Interval = .1f;
        }
    }

    public class Level12 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "范围增加";

        public override int Level => 12;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.range += 15;
            towerModel.GetBehavior<CollectCashZoneModel>().attractRange += 15;
        }
    }

    public class Level13 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "收集的成箱香蕉价值增加20%";

        public override int Level => 13;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetBehavior<CollectCashZoneModel>().name = "CollectCashZoneModel_" + Id;
        }
    }

    public class Level14 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "被气球杀灭剂喷到的气球将受到更多伤害";

        public override int Level => 14;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var superBrittle = Game.instance.model.GetTower(TowerType.IceMonkey, 5);

            var brittler = superBrittle.GetDescendant<AddBonusDamagePerHitToBloonModel>().Duplicate();
            var projectile = towerModel.GetAbility(0).GetDescendant<CreateProjectileOnExhaustFractionModel>()
                .projectile;
            var addBehavior = projectile.GetBehavior<AddBehaviorToBloonModel>();

            brittler.mutationId = "PesticideDamageBonus";
            brittler.layers = addBehavior.layers;
            brittler.lifespan = addBehavior.lifespan;
            brittler.lifespanFrames = addBehavior.lifespanFrames;

            projectile.AddBehavior(brittler);
        }
    }


    public class Level15 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "范围内的香蕉农场升级价格减少20%";

        public override int Level => 15;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetBehavior<DiscountZoneModel>().discountMultiplier = .2f;
        }
    }


    public class Level16 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "绿色ge命可转换更多气球，包括ZOMG和DDT";

        public override int Level => 16;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var projectile = towerModel.GetAbility(1).GetDescendant<CreateProjectileOnExhaustFractionModel>()
                .projectile;
            projectile.pierce += 100;

            var attackFilterModel = towerModel.GetAbility(1).GetDescendant<AttackFilterModel>();
            var filterModels = attackFilterModel.filters.ToList();
            filterModels.RemoveAll(model => model.IsType<FilterOutTagModel>(out var tag) && tag.tag == BloonTag.Ddt);
            filterModels.RemoveAll(model => model.IsType<FilterOutTagModel>(out var tag) && tag.tag == BloonTag.Zomg);
            attackFilterModel.filters = filterModels.ToIl2CppReferenceArray();
        }
    }

    public class Level17 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "气球杀灭剂的伤害巨幅增加";

        public override int Level => 17;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var dot = towerModel.GetAbility(0).GetDescendant<DamageOverTimeModel>();

            dot.damage = 5f;
        }
    }


    public class Level18 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "范围增加";

        public override int Level => 18;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.range += 15;
            towerModel.GetBehavior<CollectCashZoneModel>().attractRange += 15;
        }
    }


    public class Level19 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "收集的成箱香蕉价值增加25%";

        public override int Level => 19;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetBehavior<CollectCashZoneModel>().name = "CollectCashZoneModel_" + Id;
        }
    }

    public class Level20 : ModHeroLevel<IndustrialFarmer>
    {
        public override string Description => "绿色ge命可转换更多气球，减少绿色ge命技能冷却";

        public override int Level => 20;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var abilityModel = towerModel.GetAbility(1);
            abilityModel.Cooldown = 45;

            var projectile = abilityModel.GetDescendant<CreateProjectileOnExhaustFractionModel>().projectile;
            projectile.pierce += 700;
        }
    }
}