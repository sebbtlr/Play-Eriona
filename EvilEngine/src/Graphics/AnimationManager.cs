using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace EvilEngine.Graphics
{
   public class AnimationManager : Sprite
    {
        private Dictionary<string, FrameList> _animations;

        public FrameList CurrentAnimation { get; private set; }

        public string CurrentAnimationId => CurrentAnimation == null ? string.Empty : CurrentAnimation.Id;

        private string _nextAnimation;

        public AnimationManager() : base(default(Texture2D))
        {
            _animations = new Dictionary<string, FrameList>();
        }

        public void Update()
        {
            if (_nextAnimation != CurrentAnimationId)
            {
                CurrentAnimation = _animations[_nextAnimation];

                Texture = CurrentAnimation.CurrentFrame.Texture;
                TextureOffset = CurrentAnimation.CurrentFrame.TextureOffset;
                TextureClip = CurrentAnimation.CurrentFrame.TextureClip;
                Hitbox = CurrentAnimation.CurrentFrame.Hitbox;
            }

            if (CurrentAnimation == null) return;
            
            CurrentAnimation.Update();

            if (!CurrentAnimation.Changed) return;
            
            Texture = CurrentAnimation.CurrentFrame.Texture;
            TextureOffset = CurrentAnimation.CurrentFrame.TextureOffset;
            TextureClip = CurrentAnimation.CurrentFrame.TextureClip;
            Hitbox = CurrentAnimation.CurrentFrame.Hitbox;
        }

        public void AddAnimation(string id, FrameList frames)
        {
            if (!_animations.ContainsKey(id))
            {
                _animations.Add(id, frames);
            }
        }

        public void AddAnimation(string id, float delta, params Sprite[] frames)
        {
            if (!_animations.ContainsKey(id))
            {
                _animations.Add(id, new FrameList(id, delta) { Frames = new List<Sprite>(frames) });
            }
        }
        
        public void AddAnimation(string id, bool staticAnimation, params Sprite[] frames)
        {
            if (!_animations.ContainsKey(id))
            {
                _animations.Add(id, new FrameList(id, 0.0f, staticAnimation) { Frames = new List<Sprite>(frames) });
            }
        }

        public void RemoveAnimation(string id)
        {
            if (_animations.ContainsKey(id) && CurrentAnimation.Id != id)
            {
                _animations.Remove(id);
            }
        }

        public void ChangeAnimation(string id)
        {
            if (_animations.ContainsKey(id))
            {
                _nextAnimation = id;
            }
        }
    }
}