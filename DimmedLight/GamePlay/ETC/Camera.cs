using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DimmedLight.GamePlay.ETC
{
    public class Camera
    {
        private Vector2 defaultPosition = Vector2.Zero;
        private Vector2 targetPosition;
        private Vector2 currentPosition;
        private float moveSpeed = 2f;

        private Vector2 shakeOffset = Vector2.Zero;
        private float shakeMagnitude = 0f;
        private float shakeDuration = 0f;
        public Camera()
        {
            currentPosition = defaultPosition;
            targetPosition = defaultPosition;
        }
        public void MoveTo(Vector2 target)
        {
            targetPosition = target;
        }

        public void MoveCamTocenter(float delta)
        {
            float t = MathHelper.Clamp(delta * moveSpeed, 0f, 1f);
            currentPosition = Vector2.Lerp(currentPosition, targetPosition, t);

            if (shakeDuration > 0)
            {
                shakeDuration -= delta;
                float currentMagnitude = shakeMagnitude * (shakeDuration / 2f);
                Random rng = new Random();
                float offsetX = ((float)rng.NextDouble() * 2f - 1f) * currentMagnitude;
                float offsetY = ((float)rng.NextDouble() * 2f - 1f) * currentMagnitude;
                shakeOffset = new Vector2(offsetX, offsetY);
            }
            else
            {
                shakeOffset = Vector2.Zero;
            }
        }

        public void ResetPosition()
        {
            targetPosition = defaultPosition;
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-currentPosition - shakeOffset, 0f));
        }
        public void StartShake(float duration, float magnitude)
        {
            shakeDuration = duration;
            shakeMagnitude = magnitude;
        }
    }
}
