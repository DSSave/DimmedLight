using DimmedLight.Enemies;
using DimmedLight.Isplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static DimmedLight.Game1;
using static System.Formats.Asn1.AsnWriter;
using static DimmedLight.Gameplay;

namespace DimmedLight.UI
{
    public class ScoreManager
    {
        private double score;
        private double increaseRateMs = 16.0; // 1 ต่อ 16ms
        private int light;
        private int soulGauge;
        private List<FloatingText> floatingTexts = new List<FloatingText>();
        public int Light => light;
        public int SoulGauge => soulGauge;
        public int Score => (int)Math.Floor(score);
        private const int MaxGauge = 600;

        private float lightDisplayTime = 0f;
        private const float lightDisplayDuration = 1.5f;

        public int HighScore { get; private set; }
        private string highScoreFile = "highscore.txt";

        public ScoreManager()
        {
            score = 0;
            light = 0;
            soulGauge = 0;
            LoadHighScore();
        }
        public void SaveHighScore()
        {
            if (Score > HighScore)
            {
                HighScore = Score;
                File.WriteAllText(highScoreFile, HighScore.ToString());
            }
        }
        private void LoadHighScore()
        {
            if (File.Exists(highScoreFile))
            {
                string text = File.ReadAllText(highScoreFile);
                int value;
                if (int.TryParse(text, out value))
                    HighScore = value;
            }
            else
            {
                HighScore = 0;
            }
        }

        public void Update(GameTime gameTime , Player player)
        {
            if (!player.canWalk)
            {
                return;
            }
            double elapsedMs = gameTime.ElapsedGameTime.TotalMilliseconds;
            score += elapsedMs / increaseRateMs;
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = floatingTexts.Count - 1; i >= 0; i--)
            {
                floatingTexts[i].Update(gameTime);
                if (floatingTexts[i].IsDead)
                    floatingTexts.RemoveAt(i);
            }
            if(lightDisplayTime > 0)
            {
                lightDisplayTime -= deltaSeconds;
            }
        }
        public void AddPoints(string enemyType, string method)
        {
            int scorePoints = 0;
            int lightPoints = 0;
            int soulPoints = 0;

            switch (enemyType)
            {
                case "Guilt":
                    if(method == "Attack")
                    {
                        scorePoints = 150;
                        lightPoints = 50;
                        soulPoints = 30;
                    }
                    else if (method == "Parry")
                    {
                        scorePoints = 250;
                        lightPoints = 150;
                        soulPoints = 60;
                    }
                    break;
                case "Trauma":
                    if (method == "Attack")
                    {
                        scorePoints = 75;
                        lightPoints = 25;
                        soulPoints = 10;
                    }
                    else if (method == "Parry")
                    {
                        scorePoints = 250;
                        lightPoints = 150;
                        soulPoints = 60;
                    }
                    break;
                case "Judgement":
                    if (method == "Attack")
                    {
                        scorePoints = 50;
                        lightPoints = 10;
                        soulPoints = 3;
                    }
                    else if (method == "Parry")
                    {
                        scorePoints = 0;
                        lightPoints = 0;
                        soulPoints = 0;
                    }
                    break;
                case "FloorTrauma":
                    if (method == "Attack")
                    {
                        scorePoints = 75;
                        lightPoints = 25;
                        soulPoints = 10;
                    }
                    else if (method == "Parry")
                    {
                        scorePoints = 250;
                        lightPoints = 150;
                        soulPoints = 60;
                    }
                    break;
            }
            score += scorePoints;
            if(scorePoints > 0)
            {
                floatingTexts.Add(new FloatingText($"+{scorePoints}", new Vector2(1800, 50), Color.Yellow, 1.5f));
            }
            soulGauge += soulPoints;
            if(soulGauge > MaxGauge)
            {
                soulGauge = MaxGauge;
            }
            light += lightPoints;
            if(lightPoints > 0)
            {
                floatingTexts.Add(new FloatingText($"+{lightPoints}", new Vector2(1800, 150), Color.Cyan, 1.5f));
                lightDisplayTime = lightDisplayDuration;
            }
        }
        public void Draw(SpriteBatch sb, SpriteFont font)
        {
            sb.DrawString(font, $"{Score}", new Vector2(1750, 50), Color.White);
            sb.DrawString(font, $"HIGH: {HighScore}", new Vector2(710, 50), Color.Gold);
            if (lightDisplayTime > 0)
            {
                float alpha = MathHelper.Clamp(lightDisplayTime / lightDisplayDuration, 0f, 1f);
                Color color = Color.White * alpha;
                sb.DrawString(font, $"{light}", new Vector2(1750, 150), color);
            }

            int barWidth = 500;
            int barHeight = 30;
            Vector2 barPos = new Vector2(710, 100);

            // Background (เทา)
            sb.Draw(Texture2DHelper.Pixel, new Rectangle((int)barPos.X, (int)barPos.Y, barWidth, barHeight), Color.Gray * 0.5f);

            // แถบสีเหลือง
            float fillRatio = MathHelper.Clamp((float)soulGauge / MaxGauge, 0f, 1f);
            int fillWidth = (int)(barWidth * fillRatio);
            sb.Draw(Texture2DHelper.Pixel, new Rectangle((int)barPos.X, (int)barPos.Y, fillWidth, barHeight), Color.Yellow);

            // กรอบขาว
            sb.Draw(Texture2DHelper.Pixel, new Rectangle((int)barPos.X, (int)barPos.Y, barWidth, 1), Color.White); // top
            sb.Draw(Texture2DHelper.Pixel, new Rectangle((int)barPos.X, (int)barPos.Y + barHeight - 1, barWidth, 1), Color.White); // bottom
            sb.Draw(Texture2DHelper.Pixel, new Rectangle((int)barPos.X, (int)barPos.Y, 1, barHeight), Color.White); // left
            sb.Draw(Texture2DHelper.Pixel, new Rectangle((int)barPos.X + barWidth - 1, (int)barPos.Y, 1, barHeight), Color.White); // right

            foreach (var ft in floatingTexts)
            {
                ft.Draw(sb, font);
            }
        }
        public void EventBonus()
        {
            int bonusScore = 5000;
            int bonusLight = 500;

            score += bonusScore;
            light += bonusLight;

            floatingTexts.Add(new FloatingText($"+{bonusScore}", new Vector2(1800, 50), Color.Yellow, 2f));
            floatingTexts.Add(new FloatingText($"+{bonusLight}", new Vector2(1800, 150), Color.Cyan, 2f));

            lightDisplayTime = lightDisplayDuration;
        }

        public void Reset()
        {
            SaveHighScore();
            score = 0;
            light = 0;
            soulGauge = 0;
            floatingTexts.Clear();
        }

        public int GetScore()
        {
            return (int)score;
        }

        public void removeSoul(int amount)
        {
            soulGauge -= amount;
            if (soulGauge < 0)
                soulGauge = 0;
        }
        public void clear()
        {
            floatingTexts.Clear();
        }
    }
}
