using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwordRush;

public class PlayerEffectScreen
{
    public readonly Animation<Color> _animation = new(0.05f);
    private Point _windowSize;
    private Texture2D border;
    public bool firstTime;
    public PlayerEffectScreen(Point windowSize)
    {
        firstTime = true;
        _animation.AddFrame(new Color(40, 0, 0, 100));
        _animation.AddFrame(new Color(40, 0, 0, 100));
        _animation.AddFrame(new Color(20, 0, 0, 100));
        _animation.AddFrame(new Color(15, 0, 0, 60));
        _animation.AddFrame(new Color(0, 0, 0, 0));

        _windowSize = windowSize;

        border = GameManager.Get.ContentManager.Load<Texture2D>("BloodEffect");
    }

    public void Start()
    {
        if (firstTime)
        {
            firstTime = false;
        }

        _animation.Reset();
    }

    public void Update(GameTime gt)
    {
        _animation.Update(gt);
    }

    public void Draw(SpriteBatch sb)
    {
        sb.Draw(border, new Rectangle(0, 0, _windowSize.X, _windowSize.Y), _animation.Frame);
        //System.Diagnostics.Debug.WriteLine(_animation.Frame);
    }
}