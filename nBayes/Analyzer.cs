using System.Collections.Generic;
using System.Linq;

namespace nBayes
{
    using System;

    public class Analyzer
    {
        private float I = 0;
        private float invI = 0;
        private float tokenCount = 0;
        private float totalEntriesInAllCategories = 0;

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

        public CategorizationResult Categorize(Entry item, List<Index> indexList)
        {
            float prediction = GetPrediction(item, indexList);

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

        public float GetPrediction(Entry item, List<Index> indexList)
        {
            totalEntriesInAllCategories = GetTotalEntryCount(indexList);
            foreach (string token in item)
            {
                tokenCount = GetTotalTokenCount(indexList, token);

                List<Index>.Enumerator e = indexList.GetEnumerator();
                e.MoveNext();
                if(e.Current == null)
                    Console.WriteLine("am I here at least?");
                while (e.Current != null)
                {
                    
                    Index current = e.Current;
                    int firstCount = current.GetTokenCount(token);
                    int firstEntryCount = current.EntryCount;
                    e.MoveNext();

                    float probability = CalcProbability(firstCount, firstEntryCount, tokenCount, totalEntriesInAllCategories);

                    Console.WriteLine("{0}: [{1}] ({2}-{3}), ({4}-{5})",
                        token,
                        probability,
                        firstCount,
                        firstEntryCount,
                        tokenCount,
                        totalEntriesInAllCategories);
                }
            }

            float prediction = CombineProbability();
            return prediction;
        }

        private int GetTotalTokenCount(List<Index> indexList, String token)
        {
            int count = 0;
            foreach(Index index in indexList)
            {
                count += index.GetTokenCount(token);
            }
            return count;
        }

        private int GetTotalEntryCount(List<Index> indexList)
        {
            int count = 0;
            foreach (Index index in indexList)
            {
                count += index.EntryCount;
            }
            return count;
        }

        /// <summary>
        /// Calculates a probablility value based on statistics from two categories
        /// </summary>
        private float CalcProbability(float cat1Count, float cat1Total, float cat2Count, float cat2total)
        {
            float categor1Proportion = cat1Count / cat1Total;
            float category2Proportion = cat2Count / cat2total;
            float pw = ((categor1Proportion) / ((category2Proportion) + (categor1Proportion)));
            float
                //fake data assuming is equivalent of n
                s = 1f,
                //x is like pw, used as an assumption
                x = .5f,
                n = cat1Count + cat2Count;
            //(#of sucesses assumed + #successes observed)/ (#of observations assumed + number of observations observed)
            float fw = ((s * x) + (n * pw)) / (s + n);

            LogProbability(fw);

            return fw;
        }

        //look up multinomial distribution
        private float CalcProbability(float cat1Count, float cat1Total)
        {
            float bw = cat1Count / cat1Total;
            //float gw = cat2count / cat2total;
            //float pw = ((bw) / ((bw) + (gw)));
            float
                s = 1f,
                x = .5f,
                n = cat1Count;
            float fw = ((s * x) + (n)) / (s + n);

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
