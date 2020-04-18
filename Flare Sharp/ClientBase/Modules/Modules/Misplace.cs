﻿using Flare_Sharp.ClientBase.Categories;
using Flare_Sharp.Memory.FlameSDK;
namespace Flare_Sharp.ClientBase.Modules.Modules
{
    public class Misplace : Module
    {
        public Misplace() : base("Misplace", CategoryHandler.registry.categories[0], (char)0x07, false)
        {
            RegisterSliderSetting("Range", 0, 120, 640);
        }

        bool doubleMp = false;
        public override void onTick()
        {
            base.onTick();
            foreach (Mob e in Minecraft.clientInstance.localPlayer.level.getMovingEntities)
            {
                if (e.distanceTo(Minecraft.clientInstance.localPlayer) < 2)
                {
                    continue;
                }

                float lpX = Minecraft.clientInstance.localPlayer.X1;
                float lpY = Minecraft.clientInstance.localPlayer.Y1;
                float lpZ = Minecraft.clientInstance.localPlayer.Z1;

                float eX = e.currentX3;
                float eY = e.currentY3;
                float eZ = e.currentZ3;

                float mpX = (lpX + eX) / 2;
                float mpY = ((lpY + eY) / 2) - 1;
                float mpZ = (lpZ + eZ) / 2;

                if (doubleMp)
                {
                    mpX = (mpX + lpX) / 2;
                    mpY = ((mpY + lpY) / 2);
                    mpZ = (mpZ + lpZ) / 2;
                }
                if (e.distanceTo(Minecraft.clientInstance.localPlayer) <= sliderSettings[0].value/10) {
                    e.teleportE(mpX, mpY, mpZ);
                }
            }
        }
    }
}
