using DimmedLight.GamePlay.Isplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DimmedLight.GamePlay.UI
{
    public class ScoreManager
    {
        private double _score;
        private double _increaseRateMs = 16.0;
        //private int _light;
        private int _soulGauge;
        private readonly List<FloatingText> _floatingTexts = new List<FloatingText>();
        //public int Light => _light;
        public int SoulGauge { get; private set; }
        public int Score => (int)Math.Floor(_score);
        public static readonly int MaxGauge = 600;

        //private float _lightDisplayTime;
        //private const float LightDisplayDuration = 1.5f;

        public int HighScore { get; private set; }
        private readonly string _highScoreFile = "highscore.txt";

        private Texture2D _soulGaugeBar;
        private Texture2D _soulGaugeFrame1;
        private Texture2D _soulGaugeFrame2;
        private readonly Vector2 _gaugePosition = new Vector2(680, 20);
        private readonly Vector2 _barOffset = new Vector2(66, 94);
        private const int BarWidth = 1211;
        private const int BarHeight = 67;

        private int _comboCount = 0;
        private int _eventComboCount = 0;
        private Vector2 _comboPosition = new Vector2(300, 150);
        private Vector2 _eventComboPosition = new Vector2(300, 350);
        private SpriteFont _comboFont;

        private float _comboScale = 1.0f;
        private float _targetComboScale = 1.0f;
        private const float _comboBaseScale = 1.0f;
        private const float _comboScalePerHit = 0.02f;
        private const float _comboMaxScale = 2.0f;
        private const float _comboScaleLerpSpeed = 10f;

        private Color _comboColor = Color.OrangeRed;
        private Color _targetComboColor = Color.OrangeRed;
        private const float _comboColorLerpSpeed = 8f;

        // --- ระดับคอมโบและสี ---
        private readonly (int threshold, Color color)[] _comboTiers = new[] {
            (10, Color.Yellow),
            (25, Color.HotPink),
            (50, Color.Cyan),
            (100, Color.LimeGreen) // เพิ่มได้อีกตามต้องการ
        };
        public ScoreManager()
        {
            _score = 0;
            //_light = 0;
            SoulGauge = 0;
            LoadHighScore();
        }

        public void LoadContent(ContentManager content)
        {
            _soulGaugeBar = content.Load<Texture2D>("MenuAsset/ultimateGauge");
            _soulGaugeFrame1 = content.Load<Texture2D>("MenuAsset/ultimatebar");
            _soulGaugeFrame2 = content.Load<Texture2D>("MenuAsset/ultimatebarMax");

            _comboFont = content.Load<SpriteFont>("Fonts/StepalangeShortFont");
        }

        public void SaveHighScore()
        {
            if (Score > HighScore)
            {
                HighScore = Score;
                File.WriteAllText(_highScoreFile, HighScore.ToString());
            }
        }

        private void LoadHighScore()
        {
            if (File.Exists(_highScoreFile))
            {
                string text = File.ReadAllText(_highScoreFile);
                if (int.TryParse(text, out int value))
                    HighScore = value;
            }
            else
            {
                HighScore = 0;
            }
        }

        public void Update(GameTime gameTime, Player player)
        {
            if (!player.canWalk) return;

            double elapsedMs = gameTime.ElapsedGameTime.TotalMilliseconds;
            _score += elapsedMs / _increaseRateMs;
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = _floatingTexts.Count - 1; i >= 0; i--)
            {
                _floatingTexts[i].Update(gameTime);
                if (_floatingTexts[i].IsDead)
                    _floatingTexts.RemoveAt(i);
            }
            /*if (_lightDisplayTime > 0)
            {
                _lightDisplayTime -= deltaSeconds;
            }*/
            _comboScale = MathHelper.Lerp(_comboScale, _targetComboScale, deltaSeconds * _comboScaleLerpSpeed);
            _comboColor = Color.Lerp(_comboColor, _targetComboColor, deltaSeconds * _comboColorLerpSpeed);
        }
        private void UpdateTargetComboEffects(int currentCombo)
        {
            _targetComboScale = MathHelper.Clamp(_comboBaseScale + (currentCombo * _comboScalePerHit), _comboBaseScale, _comboMaxScale);
            _targetComboColor = Color.OrangeRed;
            for(int i = _comboTiers.Length - 1; i >= 0; i--)
            {
                if (currentCombo >= _comboTiers[i].threshold)
                {
                    _targetComboColor = _comboTiers[i].color;
                    break;
                }
            }
        }
        public void AddPoints(string enemyType, string method)
        {
            int scorePoints = 0;
            //int lightPoints = 0;
            int soulPoints = 0;

            switch (enemyType)
            {
                case "Guilt":
                    scorePoints = (method == "Attack") ? 150 : 250;
                    //lightPoints = (method == "Attack") ? 50 : 150;
                    soulPoints = (method == "Attack") ? 30 : 60;
                    break;
                case "Trauma":
                case "FloorTrauma":
                    scorePoints = (method == "Attack") ? 75 : 250;
                    //lightPoints = (method == "Attack") ? 25 : 150;
                    soulPoints = (method == "Attack") ? 10 : 60;
                    break;
                case "Judgement":
                    if (method == "Attack")
                    {
                        scorePoints = 50;
                        //lightPoints = 10; 
                        soulPoints = 3;
                    }
                    break;
            }
            bool enemyDefeated = scorePoints > 0 || soulPoints > 0;
            if (enemyDefeated)
            {
                IncreaseCombo();
            }

            _score += scorePoints;
            if (scorePoints > 0) _floatingTexts.Add(new FloatingText($"+{scorePoints}", new Vector2(1800, 50), Color.Yellow, 1.5f));

            SoulGauge = Math.Min(SoulGauge + soulPoints, MaxGauge);

            /*_light += lightPoints;
            if (lightPoints > 0)
            {
                _floatingTexts.Add(new FloatingText($"+{lightPoints}", new Vector2(1800, 150), Color.Cyan, 1.5f));
                _lightDisplayTime = LightDisplayDuration;
            }*/
        }
        public void Draw(SpriteBatch sb, SpriteFont textFont, SpriteFont numberFont, bool isInEvent)
        {
            sb.DrawString(numberFont, $"{Score}", new Vector2(1750, 50), Color.White);

            string highText = "HIGH: ";
            Vector2 highTextSize = textFont.MeasureString(highText);
            Vector2 highTextPos = new Vector2(790, 50);
            sb.DrawString(textFont, highText, highTextPos, Color.Gold);
            sb.DrawString(numberFont, $"{HighScore}", highTextPos + new Vector2(highTextSize.X, 0), Color.Gold);


            /*if (_lightDisplayTime > 0)
            {
                float alpha = MathHelper.Clamp(_lightDisplayTime / LightDisplayDuration, 0f, 1f);
                Color color = Color.White * alpha;
                sb.DrawString(numberFont, $"{_light}", new Vector2(1750, 150), color);
            }*/

            int currentCombo = isInEvent ? _eventComboCount : _comboCount;
            Vector2 currentComboPos = isInEvent ? _eventComboPosition : _comboPosition;
            SpriteFont fontToUse = _comboFont ?? numberFont;
            if (currentCombo > 0)
            {
                string comboText = (currentCombo == 1) ? "1 HIT" : $"{currentCombo} HITS";
                Vector2 origin = fontToUse.MeasureString(comboText) / 2f;
                sb.DrawString(
                    fontToUse,
                    comboText,
                    currentComboPos + origin * _comboScale,
                    _comboColor,
                    0f,
                    origin,
                    _comboScale,
                    SpriteEffects.None,
                    0f);
            }

            DrawSoulGauge(sb);

            foreach (var ft in _floatingTexts)
            {
                ft.Draw(sb, numberFont);
            }
        }

        private void DrawSoulGauge(SpriteBatch sb)
        {
            float fillRatio = MathHelper.Clamp((float)SoulGauge / MaxGauge, 0f, 1f);
            int fillWidth = (int)(BarWidth * fillRatio);
            const float scale = 0.4f;

            if (_soulGaugeBar != null)
            {
                Rectangle srcRect = new Rectangle(0, 0, fillWidth, BarHeight);
                Vector2 barPos = _gaugePosition + _barOffset;
                sb.Draw(_soulGaugeBar, barPos, srcRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            Texture2D currentFrame = (SoulGauge >= MaxGauge) ? _soulGaugeFrame2 : _soulGaugeFrame1;
            if (currentFrame != null)
                sb.Draw(currentFrame, _gaugePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        public void SetComboPosition(Vector2 normalPos, Vector2 eventPos)
        {
            _comboPosition = normalPos;
            _eventComboPosition = eventPos;
        }

        public void EventBonus()
        {
            int bonusScore = 5000;
            int bonusLight = 500;

            _score += bonusScore;
            //_light += bonusLight;

            _floatingTexts.Add(new FloatingText($"+{bonusScore}", new Vector2(1800, 50), Color.Yellow, 2f));
            _floatingTexts.Add(new FloatingText($"+{bonusLight}", new Vector2(1800, 150), Color.Cyan, 2f));

            //_lightDisplayTime = LightDisplayDuration;
        }

        public void Reset()
        {
            SaveHighScore();
            _score = 0;
            //_light = 0;
            SoulGauge = 0;
            _floatingTexts.Clear();
            ResetCombo();
            ResetEventCombo();
        }

        public void removeSoul(int amount) => SoulGauge = Math.Max(0, SoulGauge - amount);
        public void clear() => _floatingTexts.Clear();
        public void IncreaseCombo()
        {
            _comboCount++;
            UpdateTargetComboEffects(_comboCount);
        }
        public void IncreaseEventCombo()
        {
            _eventComboCount++;
            UpdateTargetComboEffects(_eventComboCount);
        }
        public void ResetCombo()
        {
            if (_comboCount > 0)
            {
                _targetComboScale = _comboBaseScale;
                _targetComboColor = Color.OrangeRed;
                _comboScale = _comboBaseScale;
                _comboColor = Color.OrangeRed;
            }
            _comboCount = 0;
        }
        public void ResetEventCombo()
        {
            if (_eventComboCount > 0)
            {
                _targetComboScale = _comboBaseScale;
                _targetComboColor = Color.OrangeRed;
                _comboScale = _comboBaseScale;
                _comboColor = Color.OrangeRed;
            }
            _eventComboCount = 0;
        }
        public void DrainSoulGauge(float amountToDrain)
        {
            int drainAmountInt = (int)Math.Ceiling(amountToDrain);
            SoulGauge = Math.Max(0, SoulGauge - drainAmountInt);
        }
    }
}