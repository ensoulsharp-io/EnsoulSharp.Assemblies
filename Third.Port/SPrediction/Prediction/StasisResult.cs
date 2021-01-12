namespace SPrediction
{
    using System;

    using EnsoulSharp.SDK;

    /// <summary>
    /// Stasis prediction result
    /// </summary>
    public class StasisResult : EventArgs
    {
        /// <summary>
        /// The spell.
        /// </summary>
        public Spell Spell;

        /// <summary>
        /// The prediction result.
        /// </summary>
        public PredictionResult Prediction;
    }
}
