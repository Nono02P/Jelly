using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jelly
{
    public class JellySpring : Spring
    {
        public bool Collide { get; set; }
    }
    
    public class Jelly
    {
        #region Constantes
        private const float GRAVITY = 0.8f;
        private const float GRAVITY_MAX = 9f;
        #endregion

        #region Variables privées
        private int _radius;
        private float _tension;
        private float _dampening;
        private bool _collide;
        #endregion

        #region Propriétés
        public Rectangle Floor { get; set; }
        public JellySpring[] Springs { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public int DeltaDegrees { get; set; }
        public int Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                for (int i = 0; i < Springs.Length; i++)
                {
                    ref JellySpring s = ref Springs[i];
                    s.TargetValue = value;
                    if (s.Value == 0)
                        s.Value = s.TargetValue;
                }
            }
        }
        public float Spread { get; set; } = 0.25f;
        public float Tension
        {
            get => _tension;
            set
            {
                _tension = value;
                for (int i = 0; i < Springs.Length; i++)
                {
                    ref JellySpring s = ref Springs[i];
                    s.Tension = value;
                }
            }
        }
        public float Dampening
        {
            get => _dampening;
            set
            {
                _dampening = value;
                for (int i = 0; i < Springs.Length; i++)
                {
                    ref JellySpring s = ref Springs[i];
                    s.Dampening = value;
                }
            }
        }
        #endregion

        #region Constructeur
        public Jelly(Vector2 pPosition, int pRadius, int pDeltaDegrees = 1)
        {
            DeltaDegrees = pDeltaDegrees;
            int numberOfSprings = (int)360 / pDeltaDegrees;
            Springs = new JellySpring[numberOfSprings];
            for (int i = 0; i < Springs.Length; i++)
            {
                Springs[i] = new JellySpring();
            }
            Position = pPosition;
            Radius = pRadius;
            Tension = 0.0001f;
            Dampening = 0.025f;
        }
        #endregion

        #region Splash
        public void Splash(Vector2 pPosition)
        {
            Vector2 d = pPosition - Position;
            float dist = (float)Math.Sqrt(Math.Pow(d.X, 2) + Math.Pow(d.Y, 2));
            if (dist < Radius)
            {
                float force = - dist / Radius;
                float angle = MathHelper.ToDegrees((float)Math.Atan2(d.Y, d.X));
                int index = ((int)Math.Round((angle / DeltaDegrees)) + Springs.Length) % Springs.Length;
                Springs[index].Velocity = force;
            }
        }

        private void Splash(JellySpring pSpring)
        {
            pSpring.Velocity = Velocity.Y;
        }
        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            #region Application de la gravité
            float vx = Velocity.X;
            float vy = Velocity.Y;

            if (_collide)
            {
                vy = 0;
            }
            vy += GRAVITY;
            vy = MathHelper.Clamp(vy, 0, GRAVITY_MAX);
            #endregion

            #region Update des ressorts
            for (int i = 0; i < Springs.Length; i++)
            {
                ref JellySpring s = ref Springs[i];
                s.Update();
            }
            #endregion

            #region Propagation des ressorts voisins
            float[] leftDeltas = new float[Springs.Length];
            float[] rightDeltas = new float[Springs.Length];
            for (int i = 0; i < Springs.Length; i++)
            {
                ref JellySpring s0 = ref Springs[(i - 1 + Springs.Length) % Springs.Length];
                ref JellySpring s1 = ref Springs[i];
                ref JellySpring s2 = ref Springs[(i + 1) % Springs.Length];
                leftDeltas[i] = Spread * (s1.Value - s0.Value);
                s0.Velocity += leftDeltas[i];
                rightDeltas[i] = Spread * (s1.Value - s2.Value);
                s2.Velocity += rightDeltas[i];
            }

            for (int i = 1; i < Springs.Length - 1; i++)
            {
                ref JellySpring s0 = ref Springs[i - 1];
                ref JellySpring s2 = ref Springs[i + 1];
                s0.Value += leftDeltas[i];
                s2.Value += rightDeltas[i];
            }


            ///////////////////////
            _collide = false;
            float vyMin = vy;
            for (int i = 0; i < Springs.Length; i++)
            {
                ref JellySpring s = ref Springs[i];
                float prevValue = s.Value;
                //s.Update();
                float angle = MathHelper.ToRadians(i * DeltaDegrees);
                Vector2 p = new Vector2(s.Value * (float)Math.Cos(angle), s.Value * (float)Math.Sin(angle));
                if (Floor.Contains(Position + p))
                {
                    if (!s.Collide)
                    {
                        Splash(s);
                        s.Collide = true;
                    }
                    else
                    {
                        float delta = Math.Abs(s.Value - prevValue);
                        if (vyMin < delta)
                            vyMin = delta * (float)Math.Sin(angle);
                    }
                    _collide = true;
                }
                else
                {
                    s.Collide = false;
                }
            }
            if (_collide)
                vy -= vyMin;
            #endregion

            Velocity = new Vector2(vx, vy);
            Position += Velocity;
        }
        #endregion

        #region Draw
        public void Draw(PrimitiveBatch primitiveBatch, GameTime gameTime)
        {
            Color midnightBlue = new Color(0, 15, 40) * 0.9f;
            Color lightBlue = new Color(0.2f, 0.5f, 1f) * 0.8f;

            for (int i = 0; i < Springs.Length; i++)
            {
                Spring s1 = Springs[i];
                Spring s2 = Springs[(i + 1) % Springs.Length];

                float angleS1 = MathHelper.ToRadians(i * DeltaDegrees);
                float angleS2 = MathHelper.ToRadians((i + 1) * DeltaDegrees);

                Vector2 p1 = new Vector2(s1.Value * (float)Math.Cos(angleS1), s1.Value * (float)Math.Sin(angleS1));
                Vector2 p2 = new Vector2(s2.Value * (float)Math.Cos(angleS2), s2.Value * (float)Math.Sin(angleS2));

                primitiveBatch.AddVertex(Position + p1, midnightBlue);
                primitiveBatch.AddVertex(Position + p2, midnightBlue);
                primitiveBatch.AddVertex(Position, lightBlue);
            }
        }
        #endregion
    }
}