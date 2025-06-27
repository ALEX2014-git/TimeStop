using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Logging;
using Mono.Cecil;
using MonoMod.RuntimeDetour;
using TimeStopDependency;
using UnityEngine;

#pragma warning disable CS0618
[module: UnverifiableCode]
[assembly: SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace TimeStop;

[BepInDependency(TimeStopDependency.TimeStopDependency.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public partial class TimeStop : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "ALEX2014.TimeStop";
    public const string PLUGIN_NAME = "Time Stop";
    public const string PLUGIN_VERSION = "1.0.0";
    internal PluginOptions options;

    public Player player;

    public static ManualLogSource Logger { get; private set; }

    private static TimeStop _instance;
    public static TimeStop Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new Exception("Instance of PebblesBlastsCaramelldansen is not created yet!");
            }
            return _instance;
        }
    }

    public TimeStop()
    {
        try
        {
            _instance = this;
            options = new PluginOptions(this, Logger);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }

    private void OnEnable()
    {
        
        TimeStop.Logger = base.Logger;
        On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
    }

    BindingFlags propFlags = BindingFlags.Instance | BindingFlags.NonPublic;
    BindingFlags myMethodFlags = BindingFlags.Static | BindingFlags.NonPublic;
    private bool IsInit;
    private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return;
            On.RainWorldGame.ShutDownProcess += RainWorldGame_ShutDownProcess;
            On.GameSession.ctor += GameSession_ctor;
            On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
            On.RainWorldGame.RawUpdate += RainWorldGame_RawUpdate;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.RoomCamera.Update += RoomCamera_Update;
            new Hook(typeof(TimeStopDependency.RainWorldGameExtensions).GetMethod(nameof(TimeStopDependency.RainWorldGameExtensions.FreezeTime)), TimeStopDependency_RainWorldGameExtensions_FreezeTime);
            new Hook(typeof(TimeStopDependency.RainWorldGameExtensions).GetMethod(nameof(TimeStopDependency.RainWorldGameExtensions.UnfreezeTime)), TimeStopDependency_RainWorldGameExtensions_UnfeezeTime);

            MachineConnector.SetRegisteredOI("ALEX2014.TimeStop", options);
            IsInit = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }

    private void TimeStopDependency_RainWorldGameExtensions_FreezeTime(Action<RainWorldGame> orig, RainWorldGame self)
    {
        orig(self);
        TimeStateSwitchVFX(self);
        self.AllowEffectsUpdateDuringTimeStop();
    }

    private void TimeStopDependency_RainWorldGameExtensions_UnfeezeTime(Action<RainWorldGame> orig, RainWorldGame self)
    {
        orig(self);
        TimeStateSwitchVFX(self);
        self.DisallowEffectsUpdateDuringTimeStop();
        self.cameras[0].GetCustomData().needGhostModeUpdate = true;
    }

    private void TimeStateSwitchVFX(RainWorldGame rwg)
    {
        rwg.cameras[0].virtualMicrophone.PlaySound(SoundID.SB_A14, 0f, 1f, 1f);
        foreach (AbstractCreature abstractCreature in rwg.AlivePlayers)
        {
            if (abstractCreature == null || abstractCreature.realizedCreature == null || abstractCreature.realizedCreature?.room == null) continue;
            Player player = (abstractCreature.realizedCreature as Player);
            if (player.GetCustomData().isEligableForTimeStopVFX)
            {
                Color vfxColor = PlayerGraphics.SlugcatColor((player.graphicsModule as PlayerGraphics).CharacterForColor);
                for (int i = 0; i < 50; i++)
                {
                    float maxRadius = 150f; // Максимальное расстояние от центра
                    Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * maxRadius;
                    Vector2 spawnPos = player.mainBodyChunk.pos + randomOffset;

                    var timeStopVFX = new TimeStopGlyph(
                    spawnPos,
                    TimeStopGlyph.RandomString(1, 1, UnityEngine.Random.Range(0, int.MaxValue) + i * 2, false),
                    vfxColor,
                    150 + (int)(i * 0.5),
                    20 + i,
                    60 + (int)(i * 1.2)
                    );
                    player.room.AddObject(timeStopVFX);
                }
            }
        }
    }

    private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);

    }

    private void GameSession_ctor(On.GameSession.orig_ctor orig, GameSession self, global::RainWorldGame game)
    {
        orig(self, game);
        ClearMemory();
    }

    private void RainWorldGame_ShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, global::RainWorldGame self)
    {
        orig(self);
        ClearMemory();
    }

    #region Helper Methods

    private void ClearMemory()
    {
        //If you have any collections (lists, dictionaries, etc.)
        //Clear them here to prevent a memory leak
        //YourList.Clear();
    }

    #endregion
}