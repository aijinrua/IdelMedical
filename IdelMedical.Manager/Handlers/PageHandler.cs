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
        public int TotalPage { get; private set; }

        public int TotalItemCount { get; private set; }
        public int TakeItemCount { get; private set; }

        public PageHandler(int currentPage, int totalItemCount, int takeItemCount)
        {
            var totalPage = (int)Math.Ceiling((double)totalItemCount / takeItemCount);
            totalPage = totalPage < 1 ? 1 : totalPage;
            currentPage = currentPage < 1 ? 1 : currentPage;
            currentPage = currentPage > totalPage ? totalPage : currentPage;

            CurrentPage = currentPage;
            TotalPage = totalPage;
            PrevPage = currentPage <= 1 ? 1 : currentPage - 1;
            NextPage = currentPage >= totalPage ? totalPage : currentPage + 1;

            TotalItemCount = totalItemCount;
            TakeItemCount = takeItemCount;
        }
    }
}
