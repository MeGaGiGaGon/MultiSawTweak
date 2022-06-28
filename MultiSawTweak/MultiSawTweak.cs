using BepInEx;
using BepInEx.Configuration;
using EntityStates.Toolbot;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MultiSawTweak
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class MultiSawTweak : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "GiGaGon";
        public const string PluginName = "MultiSawTweak";
        public const string PluginVersion = "1.0.0";

        internal class ModConfig
        {
            public static ConfigEntry<float> sawSize;
            public static ConfigEntry<float> barrierPercentagePerHit;

            public static void InitConfig(ConfigFile config)
            {
                sawSize = config.Bind("General", "Saw Size", 1f, "What number to multiply the sawblade's hitbox size by.");
                barrierPercentagePerHit = config.Bind("General", "Barrier percentage per hit", 0.128f, "Percentage of max health gained as barrier per sawblade hit.");
            }
        }

        public void Awake()
        {
            ModConfig.InitConfig(Config);

            Vector3 biggerSawSize = new Vector3(24, 24, 12);
            biggerSawSize *= ModConfig.sawSize.Value;

            On.EntityStates.Toolbot.FireBuzzsaw.OnEnter += (orig, self) =>
            {
                orig(self);
                self.attack.hitBoxGroup.hitBoxes[0].transform.localScale = biggerSawSize;
            };

            On.EntityStates.Toolbot.FireBuzzsaw.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (self.hitOverlapLastTick) self.healthComponent.AddBarrierAuthority(.01f * ModConfig.barrierPercentagePerHit.Value * self.healthComponent.fullBarrier);
            };
        }
    }
}
