using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace EvilEngine.Physics
{
    public class Transform
    {
        public static Transform Zero = new Transform(0, 0, 0, 0);
        public float Height;

        public float Width;

        public float X;
        public float Y;

        public Transform()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }

        public Transform(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Transform(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public Transform(float x, float y, Vector2 size)
        {
            X = x;
            Y = y;
            Size = size;
        }

        public Transform(Vector2 position, float width, float height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        public Transform(Rectangle rectangle)
        {
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        public Transform(Transform transform)
        {
            X = transform.X;
            Y = transform.Y;
            Width = transform.Width;
            Height = transform.Height;
        }

        public float Top
        {
            get => Y;
            set => Y = value;
        }

        public float Bottom
        {
            get => Y + Height;
            set => Y = value - Height;
        }

        public float Left
        {
            get => X;
            set => X = value;
        }

        public float Right
        {
            get => X + Width;
            set => X = value - Width;
        }

        public float MiddleX
        {
            get => X + Width / 2;
            set => X = value - Width / 2;
        }

        public float MiddleY
        {
            get => Y;
            set => Y = value;
        }

        public float CenterX
        {
            get => X + Width / 2;
            set => X = value - Width / 2;
        }

        public float CenterY
        {
            get => Y + Height / 2;
            set => Y = value - Height / 2;
        }


        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public Vector2 TopRight
        {
            get => new Vector2(X + Width, Y);
            set
            {
                X = value.X - Width;
                Y = value.Y;
            }
        }

        public Vector2 BottomLeft
        {
            get => new Vector2(X, Y + Height);
            set
            {
                X = value.X;
                Y = value.Y - Height;
            }
        }

        public Vector2 BottomRight
        {
            get => new Vector2(X + Width, Y + Height);
            set
            {
                X = value.X - Width;
                Y = value.Y - Height;
            }
        }

        public Vector2 MiddleTop
        {
            get => new Vector2(Width + Width / 2, Y);
            set
            {
                X = value.X - Width / 2;
                Y = value.Y;
            }
        }

        public Vector2 MiddleLeft
        {
            get => new Vector2(X, Y + Height / 2);
            set
            {
                X = value.X;
                Y = value.Y - Height / 2;
            }
        }

        public Vector2 MiddleRight
        {
            get => new Vector2(X + Width, Y + Height / 2);
            set
            {
                X = value.X - Width;
                Y = value.Y - Height / 2;
            }
        }

        public Vector2 MiddleBottom
        {
            get => new Vector2(X + Width / 2, Y);
            set
            {
                X = value.X - Width / 2;
                Y = value.Y - Height;
            }
        }

        public Vector2 Center
        {
            get => new Vector2(X + Width / 2, Y + Height / 2);
            set
            {
                X = value.X + Width / 2;
                Y = value.Y + Height / 2;
            }
        }

        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public Rectangle Bounds
        {
            get => new Rectangle(Convert.ToInt32(X), Convert.ToInt32(Y), Convert.ToInt32(Width),
                Convert.ToInt32(Height));
            set
            {
                X = value.X;
                Y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }

        public static Transform operator +(Transform a, Transform b)
        {
            return new Transform(a.Position + b.Position, a.Size + b.Size);
        }

        public static Transform operator -(Transform a, Transform b)
        {
            return new Transform(a.Position - b.Position, a.Size - b.Size);
        }

        public static Transform operator *(Transform a, Transform b)
        {
            return new Transform(a.Position * b.Position, a.Size * b.Size);
        }

        public static Transform operator /(Transform a, Transform b)
        {
            return new Transform(a.Position / b.Position, a.Size / b.Size);
        }

        public static bool operator ==(Transform a, Transform b)
        {
            return a?.Position == b?.Position && a?.Size == b?.Size;
        }

        public static bool operator !=(Transform a, Transform b)
        {
            return a?.Position != b?.Position || a?.Size != b?.Size;
        }

        public bool IntersectBox(float x, float y, float width, float height)
        {
            return X < x + width && Right > x && Bottom > y && Y < y + height;
        }

        public bool IntersectBox(Transform other)
        {
            return X < other.Right && Right > other.X && Bottom > other.Y && Y < other.Bottom;
        }

        public override bool Equals(object obj)
        {
            return this == obj as Transform;
        }

        public Transform Clone()
        {
            return new Transform(X, Y, Width, Height);
        }

        public void Copy(Transform transformCopy)
        {
            X = transformCopy.X;
            Y = transformCopy.Y;
            Width = transformCopy.Width;
            Height = transformCopy.Height;
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return X.GetHashCode() * Y.GetHashCode() * Width.GetHashCode() * Height.GetHashCode();
        }

        public override string ToString()
        {
            return $"[ X:{X} Y:{Y} Width:{Width} Height:{Height} ]";
        }
    }
}