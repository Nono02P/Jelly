﻿namespace Jelly
{
    public class Spring
    {
        #region Propriétés
        /// <summary>
        /// Valeur du ressort au repos.
        /// </summary>
        public float TargetValue { get; set; }

        /// <summary>
        /// Valeur actuelle du ressort.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Accélération du ressort.
        /// </summary>
        public float Velocity { get; set; }

        /// <summary>
        /// Rigidité du ressort.
        /// Faible valeur => taille de l'oscillation importante.
        /// 0.025f pour de l'eau.
        /// </summary>
        public float Tension { get; set; }

        /// <summary>
        /// Coefficient d'amortissement.
        /// Si valeur faible, durée d'oscillation élevée. 
        /// 0.025f pour de l'eau.
        /// </summary>
        public float Dampening { get; set; }
        #endregion

        #region Update
        public void Update()
        {
            Velocity += - Tension * (Value - TargetValue) - Velocity * Dampening;
            Value += Velocity;
        }
        #endregion
    }
}