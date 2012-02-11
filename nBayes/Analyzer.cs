namespace nBayes
{
    using System;

    public class Analyzer
    {
        private float I = 0;
        private float invI = 0;

        public Analyzer()
        {
            this.Tolerance = .05f;
        }

        public float Tolerance { get; set; }

        public CategorizationResult Categorize(Entry item, Index first, Index second)
        {
            float prediction = GetPrediction(item, first, second);

            if (prediction <= .5f - this.Tolerance)
                return CategorizationResult.Second;

            if (prediction >= .5 + this.Tolerance)
                return CategorizationResult.First;


            return CategorizationResult.Undetermined;
        }

        public float GetPrediction(Entry item, Index first, Index second)
        {
            foreach (string token in item)
            {
                int firstCount = first.GetTokenCount(token);
                int secondCount = second.GetTokenCount(token);

                float probability = CalcProbability(firstCount, first.EntryCount, secondCount, second.EntryCount);

                Console.WriteLine("{0}: [{1}] ({2}-{3}), ({4}-{5})",
                    token,
                    probability,
                    firstCount,
                    first.EntryCount,
                    secondCount,
                    second.EntryCount);
            }

            float prediction = CombineProbability();
            return prediction;
        }

        #region Private Methods

        /// <summary>
        /// Calculates a probablility value based on statistics from two categories
        /// </summary>
        private float CalcProbability(float cat1count, float cat1total, float cat2count, float cat2total)
        {
            float bw = cat1count / cat1total;
            float gw = cat2count / cat2total;
            float pw = ((bw) / ((bw) + (gw)));
            float
                s = 1f,
                x = .5f,
                n = cat1count + cat2count;
            float fw = ((s * x) + (n * pw)) / (s + n);

            LogProbability(fw);

            return fw;
        }

        private void LogProbability(float prob)
        {
            if (!float.IsNaN(prob))
            {
                I = I == 0 ? prob : I * prob;
                invI = invI == 0 ? (1 - prob) : invI * (1 - prob);
            }
        }

        private float CombineProbability()
        {
            return I / (I + invI);
        }

        #endregion
    }
}
