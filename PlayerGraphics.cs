using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RWCustom;
using TimeStopDependency;

namespace TimeStop
{
    public partial class TimeStop
    {
        private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, UnityEngine.Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self.player == null || self.player?.abstractCreature == null || self.player?.room == null || self.player?.room?.game == null) return;
            if ((!self.player.dead) && (self.player.room.game.IsTimeStopActive() && self.player.IsTimeStopImmune()))
            {
                if (self.GetCustomData().hasTimeStopSkin) return;
                TimeStopSkin(sLeaser, rCam);
                self.GetCustomData().hasTimeStopSkin = true;
            }
            else
            {
                if (!self.GetCustomData().hasTimeStopSkin) return;
                UndoTimeStopSkin(sLeaser, rCam);
                self.GetCustomData().hasTimeStopSkin = false;
            }
        }

        private void TimeStopSkin(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            for (int i = 0; i < 9; i++)
            {
                sLeaser.sprites[i].shader = Custom.rainWorld.Shaders["Hologram"];
            }
        }

        private void UndoTimeStopSkin(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            for (int i = 0; i < 9; i++)
            {
                sLeaser.sprites[i].shader = rCam.game.rainWorld.Shaders["Basic"];
            }
        }
    }

    internal static class PlayerGraphicsCWT
    {
        static ConditionalWeakTable<PlayerGraphics, Data> table = new ConditionalWeakTable<PlayerGraphics, Data>();

        public static Data GetCustomData(this PlayerGraphics self)
        {
            if (table.TryGetValue(self, out Data data))
                return data;

            data = new Data(self);
            table.Add(self, data);
            return data;
        }

        public class Data
        {
            public PlayerGraphics Target { get; }

            internal bool hasTimeStopSkin;
            public Data(PlayerGraphics target)
            {
                Target = target;
            }
        }
    }
}
