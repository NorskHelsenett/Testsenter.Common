namespace Shared.Common.Resources
{
    public class ShortStatus
    {
        public int NumberOfOk { get; set; }
        public int NumberOfFail { get; set; }
        public int NumberOfDone { get; set; }
        public int TotalNumber { get; set; }

        public void Add(ShortStatus item)
        {
            NumberOfOk += item.NumberOfOk;
            NumberOfFail += item.NumberOfFail;
            NumberOfDone += item.NumberOfDone;
            TotalNumber += item.TotalNumber;
        }

        public int GetDonePercentage()
        {
            return GetPercentage(NumberOfOk, TotalNumber);
        }

        public int GetFailPercentage()
        {
            return GetPercentage(NumberOfFail, TotalNumber);
        }

        private int GetPercentage(int d1, int d2)
        {
            if (d2 == 0)
                return 100;

            double total = d2;
            double oks = d1;

            var percent = (oks / total) * 100;
            return (int)percent;
        }
    }
}
