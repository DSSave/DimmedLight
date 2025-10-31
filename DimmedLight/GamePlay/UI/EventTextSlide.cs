using DimmedLight.GamePlay.ETC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.GamePlay.UI
{
    public class EventTextSlide
    {
        private enum SlideState
        {
            Hidden,
            SlidingIn,
            Holding,
            SlidingOut
        }
        private string _text = "";
        private SpriteFont _font;
        private Vector2 _currentPos;
        private Vector2 _startPos;
        private Vector2 _centerPos;
        private Vector2 _endPos;
        private Color _textColor;

        private SlideState _state = SlideState.Hidden;
        private float _timer = 0f;

        private const float SlideInDuration = 0.5f;
        private const float HoldDuration = 1.5f;
        private const float SlideOutDuration = 0.5f;

        private GraphicsDevice _graphicsDevice;
        private Camera camera;

        public bool IsActive => _state != SlideState.Hidden;

        public EventTextSlide(GraphicsDevice graphicsDevice, SpriteFont font, Camera camera)
        {
            _graphicsDevice = graphicsDevice;
            _font = font;
            this.camera = camera;
        }
        public void StartAnimation(string text, Color color)
        {
            _text = text;
            _textColor = color;

            float screenWidth = _graphicsDevice.Viewport.Width;
            float screenHeight = _graphicsDevice.Viewport.Height;
            Vector2 textSize = _font.MeasureString(_text);

            _centerPos = new Vector2(screenWidth / 2f, screenHeight / 2f);
            _startPos = new Vector2(screenWidth + textSize.X / 2f, screenHeight / 2f);
            _endPos = new Vector2(-textSize.X / 2f, screenHeight / 2f);

            _currentPos = _startPos;
            _state = SlideState.SlidingIn;
            _timer = 0f;
        }
        public void Update(float deltaTime)
        {
            if (!IsActive) return;
            _timer += deltaTime;
            switch (_state)
            {
                case SlideState.SlidingIn:
                    float slideInRatio = MathHelper.Clamp(_timer / SlideInDuration, 0f, 1f);
                    _currentPos = Vector2.Lerp(_startPos, _centerPos, slideInRatio);
                    if (_timer >= SlideInDuration)
                    {
                        _currentPos = _centerPos;
                        _state = SlideState.Holding;
                        _timer = 0f;
                    }
                    break;
                case SlideState.Holding:
                    if (_timer >= HoldDuration)
                    {
                        _state = SlideState.SlidingOut;
                        _timer = 0f;
                    }
                    break;
                case SlideState.SlidingOut:
                    float slideOutRatio = MathHelper.Clamp(_timer / SlideOutDuration, 0f, 1f);
                    if (_timer >= SlideOutDuration)
                    {
                        _state = SlideState.Hidden;
                        _timer = 0f;
                    }
                    break;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 cameraOffset = camera != null ? camera.CurrentPosition + camera.ShakeOffset : Vector2.Zero;

            if (!IsActive || string.IsNullOrEmpty(_text)) return;
            Vector2 origin = _font.MeasureString(_text) / 2f;
            spriteBatch.DrawString(_font, _text, _currentPos + new Vector2(2, 2) + cameraOffset, Color.Black * 0.5f, 0f, origin, 1.5f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, _text, _currentPos + cameraOffset, _textColor, 0f, origin, 1.5f, SpriteEffects.None, 0f);
        }
    }
}
