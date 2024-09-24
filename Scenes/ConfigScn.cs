using HlyssMG.Graphics.Viewports;
using HlyssMG.Scenes;

namespace SnowballSpin.Scenes
{
    class ConfigScn : Scene
    {
        public override void OnInit()
        {
            base.OnInit();

            if (true)
            {
                Game.WindowWidth = 1920;
                Game.WindowHeight = 1080;
                Game.IsFullscreen = true;
            }
            else
            {
                Game.WindowWidth = 800;
                Game.WindowHeight = 600;
            }

            Game.Renderer.ViewportType = new FillUniformViewport();

            Game.SceneManager.SetScene(new MenuScn());
        }
    }
}
