using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BepInEx;

namespace TimeStop
{
    public partial class TimeStop
    {
    }

    internal static class UpdatableAndDeletableCWT
    {
        static ConditionalWeakTable<UpdatableAndDeletable, Data> table = new ConditionalWeakTable<UpdatableAndDeletable, Data>();

        public static Data GetCustomData(this UpdatableAndDeletable self)
        {
            if (table.TryGetValue(self, out Data data))
                return data;

            data = new Data(self);
            table.Add(self, data);
            return data;
        }

        public class Data
        {
            public UpdatableAndDeletable Target { get; }

            public bool isEligableForTimeStopVFX
            {
                get
                {
                    if (Target is not Player) return false;
                    Player player = Target as Player;
                    if (player == null || player.abstractCreature == null || player.room == null || player.dead) return false;
                    return true;
                }
            }
            public Data(UpdatableAndDeletable target)
            {
                Target = target;
            }
        }
    }
}
