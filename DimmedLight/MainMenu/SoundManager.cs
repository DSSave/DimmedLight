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
                    MediaPlayer.Volume = _bgmVolume;
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
            UIHover = content.Load<SoundEffect>("Audio/LOOP_UI_UiMoving");
            UIClick = content.Load<SoundEffect>("Audio/LOOP_UI_Interact");
        }
        public static void LoadMusic(ContentManager content)
        {
            _mainMenuMusic = content.Load<Song>("Audio/MainMenu");
            _bmg = content.Load<Song>("Audio/MainTheme");
            _eventSound = content.Load<Song>("Audio/Event");
            _gameOverSound = content.Load<Song>("Audio/EasyGameOver");

            MediaPlayer.Volume = BgmVolume;
        }
        public static void PlayMainMenuMusic()
        {
            if(MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != _mainMenuMusic)
            {
                MediaPlayer.Play(_mainMenuMusic);
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = BgmVolume * 0.1f;
            }
        }
        public static void PlayBGM()
        {
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != _bmg)
            {
                MediaPlayer.Play(_bmg);
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = BgmVolume * 0.2f;
            }
        }
        public static void PlayEventSound()
        {
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != _eventSound)
            {
                MediaPlayer.Play(_eventSound);
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Volume = BgmVolume * 0.2f;
            }
        }
        public static void PlayGameOverSound()
        {
            if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != _gameOverSound)
            {
                MediaPlayer.Play(_gameOverSound);
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Volume = BgmVolume;
            }
        }
        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }
        public static void PauseMusic()
        {
            if(MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
        }
        public static void ResumeMusic()
        {
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
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
