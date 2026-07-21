using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DimmedLight.MainMenu
{
    public static class SoundManager
    {
        public static float _bgmVolume = 1.0f;
        public static float BgmVolume
        {
            get { return _bgmVolume; }
            set 
                {
                    _bgmVolume = Math.Clamp(value, 0.0f, 1.0f);
#if !WEBGL
                    MediaPlayer.Volume = _bgmVolume;
#endif
                }
        }
        public static float SfxVolume { get; set; } = 1.0f;

        public static SoundEffect UIHover { get; private set; }
        public static SoundEffect UIClick { get; private set; }

        private static Song _mainMenuMusic;
        private static Song _bmg;
        private static Song _eventSound;
        private static Song _gameOverSound;

        public static void LoadUISound(ContentManager content)
        {
            TryAudio(() =>
            {
                UIHover = content.Load<SoundEffect>("Audio/LOOP_UI_UiMoving");
                UIClick = content.Load<SoundEffect>("Audio/LOOP_UI_Interact");
            });
        }
        public static void LoadMusic(ContentManager content)
        {
#if WEBGL
            // Browser autoplay policies can reject music setup before user input.
            // Keep the web build playable even if background music is unavailable.
            return;
#else
            _mainMenuMusic = content.Load<Song>("Audio/MainMenu");
            _bmg = content.Load<Song>("Audio/MainTheme");
            _eventSound = content.Load<Song>("Audio/Event");
            _gameOverSound = content.Load<Song>("Audio/EasyGameOver");

            MediaPlayer.Volume = BgmVolume;
#endif
        }
        public static void PlayMainMenuMusic()
        {
#if WEBGL
            return;
#else
            if(MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != _mainMenuMusic)
            {
                MediaPlayer.Play(_mainMenuMusic);
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = BgmVolume * 0.1f;
            }
#endif
        }
        public static void PlayBGM()
        {
#if WEBGL
            return;
#else
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != _bmg)
            {
                MediaPlayer.Play(_bmg);
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = BgmVolume * 0.2f;
            }
#endif
        }
        public static void PlayEventSound()
        {
#if WEBGL
            return;
#else
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != _eventSound)
            {
                MediaPlayer.Play(_eventSound);
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Volume = BgmVolume * 0.25f;
            }
#endif
        }
        public static void PlayGameOverSound()
        {
#if WEBGL
            return;
#else
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != _gameOverSound)
            {
                MediaPlayer.Play(_gameOverSound);
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Volume = BgmVolume;
            }
#endif
        }
        public static void StopMusic()
        {
#if !WEBGL
            MediaPlayer.Stop();
#endif
        }
        public static void PauseMusic()
        {
#if !WEBGL
            if(MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
#endif
        }
        public static void ResumeMusic()
        {
#if !WEBGL
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
#endif
        }
        public static void PlayUIHover()
        {
            TryAudio(() => UIHover?.Play(SfxVolume * 0.3f, 4f, 0f));
        }
        public static void PlayUIClick()
        {
            TryAudio(() => UIClick?.Play(SfxVolume * 0.3f, 4f, 0f));
        }

        private static void TryAudio(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                // Audio can be blocked or unavailable in browser embeds.
                // Gameplay should continue without sound.
            }
        }
    }
}
