using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DimmedLight.GamePlay.Animated
{
    public class AnimatedTexture
    {
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public float Depth { get; set; }
        public Vector2 Origin { get; set; }
        public bool IsPaused { get; private set; }
        public bool IsEnded { get; private set; }
        public bool Loop { get; set; } = true;

        private Texture2D _texture;
        private int _frameCount;
        private int _frameRowCount;
        private int _currentFrame;
        private int _currentRow;
        private float _timePerFrame;
        private float _totalElapsed;
        private bool _flip;
        private int _pauseFrame = -1;
        private int _pauseRow = -1;

        public AnimatedTexture(Vector2 origin, float rotation, float scale, float depth)
        {
            Origin = origin;
            Rotation = rotation;
            Scale = scale;
            Depth = depth;
        }

        public void Load(ContentManager content, string asset, int frameCount, int frameRow, int framesPerSec)
        {
            _texture = content.Load<Texture2D>(asset);
            SetupAnimation(frameCount, frameRow, framesPerSec);
        }

        public void LoadFromTexture(Texture2D texture, int frameCount, int frameRow, int framesPerSec)
        {
            _texture = texture;
            SetupAnimation(frameCount, frameRow, framesPerSec);
        }

        private void SetupAnimation(int frameCount, int frameRow, int framesPerSec)
        {
            _frameCount = frameCount;
            _frameRowCount = frameRow;
            _timePerFrame = 1f / framesPerSec;
            Reset();
        }

        public AnimatedTexture Clone()
        {
            var clone = new AnimatedTexture(Origin, Rotation, Scale, Depth);
            clone.LoadFromTexture(_texture, _frameCount, _frameRowCount, (int)(1 / _timePerFrame));
            return clone;
        }

        public void UpdateFrame(float elapsed)
        {
            if (_pauseFrame > -1 && _pauseRow > -1)
            {
                _currentRow = _pauseRow;
                _currentFrame = _pauseFrame;
                IsPaused = true;
                _pauseFrame = -1;
                _pauseRow = -1;
            }

            if (IsPaused || (!Loop && IsEnded)) return;

            _totalElapsed += elapsed;

            while (_totalElapsed > _timePerFrame && _timePerFrame > 0)
            {
                _totalElapsed -= _timePerFrame;
                _currentFrame++;

                if (_currentFrame >= _frameCount)
                {
                    _currentFrame = 0;
                    _currentRow++;

                    if (_currentRow >= _frameRowCount)
                    {
                        if (!Loop)
                        {
                            IsEnded = true;
                            _currentRow = _frameRowCount - 1;
                            _currentFrame = _frameCount - 1;
                            return;
                        }
                        else
                        {
                            _currentRow = 0;
                        }
                    }
                }
            }
        }

        public void DrawFrame(SpriteBatch batch, Vector2 screenPos, bool flip = false, Color? color = null)
        {
            if (_texture == null) return;

            _flip = flip;
            int frameWidth = _texture.Width / _frameCount;
            int frameHeight = _texture.Height / _frameRowCount;
            Rectangle sourceRect = new Rectangle(frameWidth * _currentFrame, frameHeight * _currentRow, frameWidth, frameHeight);

            SpriteEffects effects = _flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            batch.Draw(_texture, screenPos, sourceRect, color ?? Color.White, Rotation, Origin, Scale, effects, Depth);
        }

        public void Reset()
        {
            _currentFrame = 0;
            _currentRow = 0;
            _totalElapsed = 0f;
            IsPaused = false;
            IsEnded = false;
        }

        public void Stop()
        {
            Pause();
            Reset();
        }

        public void Play() => IsPaused = false;
        public void Pause() => IsPaused = true;

        public void Pause(int frame, int row)
        {
            _pauseFrame = frame;
            _pauseRow = row;
        }
    }
}