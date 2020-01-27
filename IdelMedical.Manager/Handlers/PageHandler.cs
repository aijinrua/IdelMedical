using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdelMedical.Manager.Handlers
{
    public class PageHandler
    {
        public int PrevPage { get; private set; }
        public int NextPage { get; private set; }
        public int CurrentPage { get; private set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int TotalPageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int TakeItemCount { get; private set; }

        public PageHandler(int currentPage, int totalItemCount, int takeItemCount)
        {
            var totalPageCount = Math.Max((int)Math.Ceiling(totalItemCount / (double)takeItemCount), 1);
            currentPage = Math.Min(currentPage, totalPageCount);
            var startPage = (((int)Math.Ceiling(currentPage / 5d) - 1) * 5) + 1;
            var endPage = Math.Min(startPage + 4, totalPageCount);
            var prevPage = startPage <= 1 ? -1 : startPage - 5;
            var nextPage = endPage >= totalPageCount ? -1 : endPage + 1;

            this.PrevPage = prevPage;
            this.NextPage = nextPage;
            this.CurrentPage = currentPage;
            this.StartPage = startPage;
            this.EndPage = endPage;
            this.TotalPageCount = totalPageCount;
            this.TotalItemCount = totalItemCount;
            this.TakeItemCount = takeItemCount;
        }
    }
}
