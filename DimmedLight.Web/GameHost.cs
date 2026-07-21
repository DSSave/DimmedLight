using DimmedLight;

namespace DimmedLight.Web;

public sealed class GameHost : IDisposable
{
    private Game1? _game;

    public void Start()
    {
        if (_game != null)
            return;

        try
        {
            _game = new Game1();
            _game.Run();
        }
        catch
        {
            _game?.Dispose();
            _game = null;
            throw;
        }
    }

    public void Dispose()
    {
        _game?.Dispose();
        _game = null;
    }
}
