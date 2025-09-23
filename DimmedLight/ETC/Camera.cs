using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DimmedLight.ETC
{
    public class Camera
    {
        private Vector2 defaultPosition = Vector2.Zero;
        private Vector2 targetPosition;
        private Vector2 currentPosition;
        private float moveSpeed = 2f;
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
            currentPosition = Vector2.Lerp(currentPosition, targetPosition, delta * moveSpeed);
        }

        public void ResetPosition()
        {
            targetPosition = defaultPosition;
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-currentPosition, 0f));
        }
    }
}
