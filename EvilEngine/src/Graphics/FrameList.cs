using System.Collections.Generic;
using EvilEngine.Core;

namespace EvilEngine.Graphics
{
    public class FrameList
    {
        public string Id { get; private set; }
        public List<Sprite> Frames;
        public readonly float DeltaTimeFrame;
        public int CurrentFrameIndex;
        private float _currentDeltaTime;
        public bool Changed { get; private set; }
        public bool Static;

        public Sprite CurrentFrame => Frames[CurrentFrameIndex];

        public FrameList(string id, float deltaTimeFrame, bool staticAnimation = false)
        {
            Id = id;
            DeltaTimeFrame = deltaTimeFrame;
            _currentDeltaTime = 0;
            CurrentFrameIndex = 0;
            Changed = true;
            Static = staticAnimation;
        }

        public void Update()
        {
            if (Static)
                return;
            
            _currentDeltaTime += GameCore.DeltaTime;
            if (_currentDeltaTime >= DeltaTimeFrame)
            {
                NextFrame();
                _currentDeltaTime = 0;
                Changed = true;
            }
            else
            {
                Changed = false;
            }
        }

        public void NextFrame()
        {
            if (CurrentFrameIndex + 1 >= Frames.Count)
            {
                CurrentFrameIndex = 0;
            }
            else
            {
                CurrentFrameIndex++;
            }
        }

    }
}