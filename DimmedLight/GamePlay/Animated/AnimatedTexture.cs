#region File Description
//-----------------------------------------------------------------------------
// AnimatedTexture.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DimmedLight.GamePlay.Animated
{
    public class AnimatedTexture
    {
        private int framecount;
        private Texture2D myTexture;
        private float TimePerFrame;
        private int Frame;
        private int framerow = 1; // frame row
        private int frame_r; // count frame row 
        private int startframe;
        private int endframe;
        private float TotalElapsed;
        private bool Paused;
        private bool Ended;
        private int Overload;
        private int startrow;
        private bool flip = false;
        private int pauseFrame = -1;
        private int pauseRow = -1;

        private int framesPerSecond;

        public float Rotation, Scale, Depth;
        public Vector2 Origin;
        private bool loop = true;
        public AnimatedTexture(Vector2 origin, float rotation, float scale, float depth)
        {
            Origin = origin;
            Rotation = rotation;
            Scale = scale;
            Depth = depth;
        }
        public void Load(ContentManager content, string asset, int frameCount, int frameRow, int framesPerSec)
        {
            framecount = frameCount;
            framerow = frameRow;
            startframe = 0;
            endframe = frameCount * framerow - 1;
            myTexture = content.Load<Texture2D>(asset);
            framesPerSecond = framesPerSec;
            TimePerFrame = (float)1 / framesPerSec;
            Frame = 0;
            frame_r = 0;
            TotalElapsed = 0;
            Paused = false;
            Ended = false;
            Overload = 1;
            startrow = 1;
        }
        public void LoadFromTexture(Texture2D texture, int frameCount, int frameRow, int framesPerSec, int startRow = 1, int overload = 1)
        {
            myTexture = texture;
            framecount = frameCount;
            framerow = frameRow;
            framesPerSecond = framesPerSec;
            TimePerFrame = (float)1 / framesPerSec;
            Frame = 0;
            frame_r = 0;
            TotalElapsed = 0;
            Paused = false;
            Ended = false;
            Overload = overload;
            startrow = Math.Max(1, startRow);
            startframe = 0;
            endframe = framecount * framerow - 1;
        }
        public AnimatedTexture Clone()
        {
            var clone = new AnimatedTexture(Origin, Rotation, Scale, Depth);
            clone.LoadFromTexture(myTexture, framecount, framerow, framesPerSecond, startrow, Overload);
            return clone;
        }
        // class AnimatedTexture
        public bool Loop
        {
            get => loop;
            set
            {
                loop = value;
                if (loop) Ended = false;
            }
        }
        public void UpdateFrame(float elapsed)
        {
            if (pauseFrame > -1 && pauseRow > -1)
            {

                frame_r = pauseRow;
                Frame = pauseFrame;
                Paused = true;
                pauseFrame = -1;
                pauseRow = -1;

            }
            if (Paused)
                return;
            if (!loop && Ended) return;
            TotalElapsed += elapsed;

            while (TotalElapsed > TimePerFrame)
            {
                // คำนวนเฟรมถัดไป
                int nextFrame = Frame + 1;
                int nextRow = frame_r;

                if (nextFrame >= framecount)
                {
                    nextFrame = 0;
                    nextRow = frame_r + 1;
                }

                // ถ้าไม่วนและเฟรมถัดไปออกนอกแถวสุดท้าย -> ถือว่า animation จบ
                if (!loop && nextRow >= framerow)
                {
                    // ตั้งค่าให้เป็นเฟรมสุดท้ายของ animation แล้วทำ Ended = true
                    Frame = Math.Max(0, framecount - 1);
                    frame_r = Math.Max(0, framerow - 1);
                    Ended = true;
                    TotalElapsed = 0f;
                    return;
                }

                // ปกติอัพเดตไปเฟรมถัดไป
                Frame = nextFrame;
                frame_r = nextRow % framerow;

                // ป้องกันการ overflow
                Frame = Frame % framecount;

                TotalElapsed -= TimePerFrame;
            }

            /*if (TotalElapsed > TimePerFrame)
            {
                Frame++;
                if (Frame == framecount)
                {
                    Frame = 0;
                    frame_r++;
                    if (Overload == 2)
                    {
                        Ended = true;
                    }
                }
                if (frame_r == framerow)
                {
                    frame_r = 0;
                    if (Overload == 1)
                    {
                        Ended = true;
                    }
                }

                // Keep the Frame between 0 and the total frames, minus one.
                Frame = Frame % framecount;
                // check start check end

                TotalElapsed -= TimePerFrame;
            }*/
        }

        // class AnimatedTexture
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos, bool flip)
        {
            this.flip = flip;
            DrawFrame(batch, Frame, screenPos);
        }
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos)
        {
            DrawFrame(batch, Frame, screenPos);
        }
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos, int row)
        {
            DrawFrame(batch, Frame, screenPos, row);
        }
        public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos)
        {
            int FrameWidth = myTexture.Width / framecount;
            int FrameHeight = myTexture.Height / framerow;
            Rectangle sourcerect;
            if (Overload == 1)
            {
                sourcerect = new Rectangle(FrameWidth * frame, FrameHeight * frame_r,
                    FrameWidth, FrameHeight);
            }
            else
            {
                sourcerect = new Rectangle(FrameWidth * frame, FrameHeight * (startrow - 1),
                    FrameWidth, FrameHeight);
            }
            if (!flip)
            {
                batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                    Rotation, Origin, Scale, SpriteEffects.None, Depth);
            }
            else
            {
                batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                    Rotation, Origin, Scale, SpriteEffects.FlipHorizontally, Depth);
            }
        }
        public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos, int row)
        {
            int FrameWidth = myTexture.Width / framecount;
            int FrameHeight = myTexture.Height / framerow;
            startrow = row;
            Rectangle sourcerect = new Rectangle(FrameWidth * frame, FrameHeight * (startrow - 1),
                    FrameWidth, FrameHeight);
            if (!flip)
            {
                batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                    Rotation, Origin, Scale, SpriteEffects.None, Depth);
            }
            else
            {
                batch.Draw(myTexture, screenPos, sourcerect, Color.White,
                    Rotation, Origin, Scale, SpriteEffects.FlipHorizontally, Depth);
            }
        }
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos, Color color, bool flip = false)
        {
            int FrameWidth = myTexture.Width / framecount;
            int FrameHeight = myTexture.Height / framerow;
            Rectangle sourcerect;

            if (Overload == 1)
            {
                sourcerect = new Rectangle(FrameWidth * Frame, FrameHeight * frame_r,
                    FrameWidth, FrameHeight);
            }
            else
            {
                sourcerect = new Rectangle(FrameWidth * Frame, FrameHeight * (startrow - 1),
                    FrameWidth, FrameHeight);
            }

            if (!flip)
            {
                batch.Draw(myTexture, screenPos, sourcerect, color,
                    Rotation, Origin, Scale, SpriteEffects.None, Depth);
            }
            else
            {
                batch.Draw(myTexture, screenPos, sourcerect, color,
                    Rotation, Origin, Scale, SpriteEffects.FlipHorizontally, Depth);
            }
        }
        public bool IsPaused => Paused;
        public bool IsEnd => Ended;
        public void Reset()
        {
            Frame = 0;
            TotalElapsed = 0f;
            Paused = false;
            Ended = false;
        }
        public void Stop()
        {
            Pause();
            Reset();
        }
        public void Play()
        {
            Paused = false;
        }
        public void Pause()
        {
            Paused = true;
        }
        public void Pause(int frame, int row)
        {
            pauseFrame = frame;
            pauseRow = row;
        }
    }
}
