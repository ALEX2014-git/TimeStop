using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MonoMod.Cil;
using UnityEngine;
using Mono.Cecil.Cil;
using Watcher;
using RWCustom;
using MoreSlugcats;
using TimeStopDependency;

namespace TimeStop
{
    
    public partial class TimeStop
    {
        private bool timeStopKeyDown;

        private void RainWorldGame_RawUpdate(On.RainWorldGame.orig_RawUpdate orig, RainWorldGame self, float dt)
        {
            orig(self, dt);
            if (Input.GetKey(TimeStop.Instance.options.TimeSwitchKeyCode.Value) && !this.timeStopKeyDown)
            {
                self.SwitchTimeState();
            }
            this.timeStopKeyDown = Input.GetKey(TimeStop.Instance.options.TimeSwitchKeyCode.Value);
        }
    }
}
