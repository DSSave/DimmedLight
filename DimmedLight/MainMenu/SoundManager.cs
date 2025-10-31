using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.MainMenu
{
    public static class SoundManager
    {
        public static float BgmVolume { get; set; } = 1.0f;
        public static float SfxVolume { get; set; } = 1.0f;

        public static SoundEffect UIHover { get; private set; }
        public static SoundEffect UIClick { get; private set; }

        public static void LoadUISound(ContentManager content)
        {
            UIHover = content.Load<SoundEffect>("Audio/LOOP_UI_UiMoving");
            UIClick = content.Load<SoundEffect>("Audio/LOOP_UI_Interact");
        }
        public static void PlayUIHover()
        {
            UIHover?.Play(SfxVolume * 0.3f, 4f, 0f);
        }
        public static void PlayUIClick()
        {
            UIClick?.Play(SfxVolume * 0.3f, 4f, 0f);
        }
    }
}
