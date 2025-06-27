using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TimeStopDependency;

namespace TimeStop
{
    public partial class TimeStop
    {
        private void RoomCamera_Update(On.RoomCamera.orig_Update orig, RoomCamera self)
        {
            orig(self);
            var ghostMode = self.ghostMode;
            if (self.game.IsTimeStopActive())
            {
                self.ghostMode = 1f;
            }
            if (self.GetCustomData().needGhostModeUpdate)
            {
                self.UpdateGhostMode(self.room, self.currentCameraPosition);
                self.GetCustomData().needGhostModeUpdate = false;
            }
        }
    }

    internal static class RoomCameraCWT
    {
        static ConditionalWeakTable<RoomCamera, Data> table = new ConditionalWeakTable<RoomCamera, Data>();

        public static Data GetCustomData(this RoomCamera self)
        {
            if (table.TryGetValue(self, out Data data))
                return data;

            data = new Data(self);
            table.Add(self, data);
            return data;
        }

        public class Data
        {
            public RoomCamera Target { get; }

            internal bool needGhostModeUpdate;
            public Data(RoomCamera target)
            {
                Target = target;
            }
        }
    }
}
