using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SwordRush;

public class Animation<T>
{
    private readonly List<T> _frames = new();
    private readonly double _frameDuration;
    private double _frameTime;
    private int _frameCurIndex;
    public bool Done { get; set; }
    
    public T Frame => _frames[_frameCurIndex];

    public Animation(double frameDuration)
    {
        _frameDuration = frameDuration;
    }

    public void AddFrame(T frame)
    {
        _frames.Add(frame);
    }

    public void Reset()
    {
        _frameTime = 0;
        _frameCurIndex = 0;
        Done = false;
    }

    public void Update(GameTime gt)
    {
        if (Done) return;

        if (_frameTime >= _frameDuration)
        {
            _frameTime = 0;
            _frameCurIndex++;
            if (_frameCurIndex >= _frames.Count - 1)
            {
                Done = true;
            }
        }
        else
        {
            _frameTime += gt.ElapsedGameTime.TotalSeconds;
        }
    }
}