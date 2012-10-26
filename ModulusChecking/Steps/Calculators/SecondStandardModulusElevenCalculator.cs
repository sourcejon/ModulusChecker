using System.Linq;
using ModulusChecking.Loaders;
using ModulusChecking.Models;

namespace ModulusChecking.Steps.Calculators
{
    class SecondStandardModulusElevenCalculator : BaseModulusCalculator
    {

        protected readonly int[] NoMatchWeights = new[] { 0, 0, 1, 2, 5, 3, 6, 4, 8, 7, 10, 9, 3, 1 };
        protected readonly int[] OneMatchWeights = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 8, 7, 10, 9, 3, 1 };

        public SecondStandardModulusElevenCalculator()
        {
            Modulus = 11;
        }

        public override bool Process(BankAccountDetails bankAccountDetails, IModulusWeightTable modulusWeightTable)
        {
            var firstRule = modulusWeightTable.GetRuleMappings(bankAccountDetails.SortCode).First();
            var secondRule = modulusWeightTable.GetRuleMappings(bankAccountDetails.SortCode).ElementAt(1);
            bool secondResult;

            if (firstRule.Exception == 2 && secondRule.Exception == 9)
            {
                SetupForExceptionTwoAndNine(bankAccountDetails, secondRule);
                secondResult = ProcessWeightingRule(bankAccountDetails, secondRule);
                //may be Lloyd's TSB euro account quoted with a sterling sort code
                if (!secondResult)
                {
                    bankAccountDetails.SortCode = new SortCode("309634");
                    //load new step after change of sort code
                    secondResult = ProcessWeightingRule(bankAccountDetails,
                                                        modulusWeightTable
                                                            .GetRuleMappings(bankAccountDetails.SortCode)
                                                            .First());
                }
            }
            else
            {
                secondResult = ProcessWeightingRule(bankAccountDetails,
                                                    secondRule);
            }
            return secondResult;
        }

        private void SetupForExceptionTwoAndNine(BankAccountDetails bankAccountDetails, IModulusWeightMapping secondRule)
        {
            if (bankAccountDetails.AccountNumber.IntegerAt(0) != 0)
            {
                secondRule.WeightValues = bankAccountDetails.AccountNumber.IntegerAt(6) == 9
                                              ? OneMatchWeights
                                              : NoMatchWeights;
            }
        }
    }
}