using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TankGame
{
    class Tile
    {
#region MVars

        int tileWidth;
        int tileHight;

        Random rng;

        Vector2 tileOrigin;
        Vector3 tilePos;

        TileAssets a;
        Texture2D tileTexB, tileTexA;
        Color tileColB, tileColA;

        Matrix tileM;

        Color neutral = new Color(0, 0, 0, 0);

        public Color BaseCol { get { return tileColB; } }
        public Color AccentCol { get { return tileColA; } }


#endregion

#region Make & Change

        public Tile(TileAssets assets, int accentTex, Color colorB, Color colorA, int width, int hight, Vector2 pos, Random ran, bool jiggle, bool darken)
        {
            tileWidth = width;
            tileHight = hight;
            tileOrigin = new Vector2(tileWidth / 2, tileHight / 2);
            tilePos = new Vector3(pos.X * tileWidth, pos.Y * tileHight, 0);
            a = assets;
            tileTexB = a.TileBase;
            tileTexA = a.TileAccent(accentTex);
            tileColB = colorB;
            tileColA = colorA;
            rng = ran;
            if (jiggle)
            {
                tileColB = JiggleColour(tileColB, 25);
                tileColA = JiggleColour(tileColA, 25);
            }
            if (darken)
            {
                tileColB = DarkenColour(tileColB);
                tileColA = DarkenColour(tileColA);
            }
        }

        public void ChangeColour(Color? baseCol, Color? accentCol)
        {
            tileColB = baseCol ?? tileColB;
            tileColA = baseCol ?? tileColA;
        }

        public Color JiggleColour(Color input, int jiggleThreshold)
        {
            Color output = new Color(
                (byte)MathHelper.Clamp((input.R + rng.Next(-jiggleThreshold, jiggleThreshold)), 0, 255),
                (byte)MathHelper.Clamp((input.G + rng.Next(-jiggleThreshold, jiggleThreshold)), 0, 255),
                (byte)MathHelper.Clamp((input.B + rng.Next(-jiggleThreshold, jiggleThreshold)), 0, 255),
                255);
            return output;
        }

        public Color DarkenColour(Color input, int darkenAmount)
        {
            Color output = new Color(
                (byte)MathHelper.Clamp((input.R - darkenAmount), 0, 255),
                (byte)MathHelper.Clamp((input.G - darkenAmount), 0, 255),
                (byte)MathHelper.Clamp((input.B - darkenAmount), 0, 255),
                255);
            return output;
        }

        public Color DarkenColour(Color input)
        {
            Color output = new Color(
                (byte)MathHelper.Clamp((input.R - (input.R * .75f)), 0, 255),
                (byte)MathHelper.Clamp((input.G - (input.G * .75f)), 0, 255),
                (byte)MathHelper.Clamp((input.B - (input.B * .75f)), 0, 255),
                255);
            return output;
        }

#endregion

#region U&D
        
        public void Update(GameTime gameTime, Matrix cameraMFinal, bool pause)
        {
            tileM = Matrix.CreateTranslation(tilePos);
            tileM *= cameraMFinal;
        }

        public void DrawMini(SpriteBatch sb, Matrix miniMatrix)
        {
            sb.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, tileM * miniMatrix);
            sb.Draw(tileTexA, Vector2.Zero, null, tileColA, 0, tileOrigin, 1, SpriteEffects.None, 0);
            sb.Draw(tileTexB, Vector2.Zero, null, tileColB, 0, tileOrigin, 1, SpriteEffects.None, 0);
            sb.End();
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, null, null, null, tileM);
            sb.Draw(tileTexA, Vector2.Zero, null, tileColA, 0, tileOrigin, 1, SpriteEffects.None, 1);
            sb.Draw(tileTexB, Vector2.Zero, null, tileColB, 0, tileOrigin, 1, SpriteEffects.None, 1);
            sb.End();
        }

#endregion
    }
}
