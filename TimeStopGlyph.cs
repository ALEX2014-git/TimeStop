using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TimeStop;
using TimeStopDependency;

namespace TimeStop
{
    internal class TimeStopGlyph : CosmeticSprite, IAmImmuneToTimeStop
    {
        public Vector2? setPos;
        public float? setScale;
        public int[] glyphs;
        public int visibleGlyphs;
        public float scale = 1f;
        public float lastScale = 1f;
        public Color color;
        public bool holo;
        public float minAlpha;
        public float alpha;
        public int lifeTime;
        public int timer = 0;
        public int spawnTimer;
        public int spawnTimerValue;
        public int mainPhaseTimer = 0;
        public int mainPhaseTimerValue;
        public float maxScale;

        public TimeStopGlyph(Vector2 pos, int[] glyphs, Color color, int lifeTime = 300, int spawnTime = 20, int mainLifeTime = 60)
        {
            this.pos = pos;
            this.lastPos = pos;
            this.glyphs = glyphs;
            this.visibleGlyphs = glyphs.Length;
            this.color = color;
            this.holo = false;
            this.alpha = 0f;
            this.scale = 0f;
            this.spawnTimerValue = spawnTime;
            this.spawnTimer = this.spawnTimerValue;
            this.mainPhaseTimerValue = mainLifeTime;
            this.lifeTime = lifeTime;
            this.minAlpha = UnityEngine.Random.Range(0.1f, 0.9f);
            this.maxScale = UnityEngine.Random.Range(0.5f, 1f);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (this.setPos != null)
            {
                this.pos = this.setPos.Value;
                this.setPos = null;
            }
            this.lastScale = this.scale;
            if (this.setScale != null)
            {
                this.scale = this.setScale.Value;
                this.setScale = null;
            }

            this.spawnTimer--;
            if (spawnTimer < 0)
            {
                mainPhaseTimer++;
                if (mainPhaseTimer > mainPhaseTimerValue)
                {
                    if (this.timer > this.lifeTime)
                    {
                        this.Destroy();
                        return;
                    }
                    this.pos.y -= 1f;
                    this.timer++;
                }
            }
            this.pos.x += UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1f : -1f;
            this.scale = spawnTimer > 0
            ? Mathf.Lerp(0f, maxScale, Mathf.Clamp01((float)spawnTimer / (float)spawnTimerValue))
            : Mathf.Lerp(maxScale, 0f, Mathf.Clamp01((float)timer / (float)lifeTime));
            this.alpha = spawnTimer > 0
            ? Mathf.Lerp(1f, minAlpha, Mathf.Clamp01((float)spawnTimer / (float)spawnTimerValue))
            : Mathf.Lerp(minAlpha, 1f, Mathf.Clamp01((float)timer / (float)lifeTime)); }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[this.glyphs.Length];
            for (int i = 0; i < this.glyphs.Length; i++)
            {
                sLeaser.sprites[i] = new FSprite("glyphs", true);
                sLeaser.sprites[i].color = this.color;
                sLeaser.sprites[i].anchorX = 0f;
                sLeaser.sprites[i].anchorY = 0f;
                if (this.holo)
                {
                    sLeaser.sprites[i].shader = rCam.game.rainWorld.Shaders["GateHologram"];
                }
                else
                {
                    sLeaser.sprites[i].shader = rCam.game.rainWorld.Shaders["SingleGlyph"];
                }
            }
            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = Vector2.Lerp(this.lastPos, this.pos, timeStacker);
            float num = Mathf.Lerp(this.lastScale, this.scale, timeStacker);
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].x = vector.x + (float)i * 15f * num - camPos.x;
                sLeaser.sprites[i].y = vector.y - camPos.y;
                sLeaser.sprites[i].isVisible = (i < this.visibleGlyphs && this.glyphs[i] >= 0);
                sLeaser.sprites[i].alpha = this.alpha;
                sLeaser.sprites[i].color = this.color;
                sLeaser.sprites[i].scaleX = 15f / Futile.atlasManager.GetElementWithName("glyphs").sourcePixelSize.x * num;
                sLeaser.sprites[i].scaleY = num;
            }
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (newContatiner == null)
            {
                newContatiner = rCam.ReturnFContainer("BackgroundShortcuts");
            }
            base.AddToContainer(sLeaser, rCam, newContatiner);
        }

        public static int[] RandomString(int minLength, int maxLength, int seed, bool cyrillic)
        {
            UnityEngine.Random.State state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);
            int[] array = new int[UnityEngine.Random.Range(minLength, maxLength)];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (cyrillic ? UnityEngine.Random.Range(26, 47) : UnityEngine.Random.Range(0, 14));
            }
            UnityEngine.Random.state = state;
            return array;
        }

        public static int[] RandomString(int length, int seed, bool cyrillic)
        {
            UnityEngine.Random.State state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(seed);
            int[] array = new int[length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (cyrillic ? UnityEngine.Random.Range(26, 47) : UnityEngine.Random.Range(0, 14));
            }
            UnityEngine.Random.state = state;
            return array;
        }
    }

}
