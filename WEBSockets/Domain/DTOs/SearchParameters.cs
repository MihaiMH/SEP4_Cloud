using System;

namespace WEBSockets.Domain.DTOs
{
    public class SearchParameters
    {
        public DateTime? DateTime { get; set; }
        public SearchParameters() { }

        public SearchParameters(DateTime? dateTime) { DateTime = dateTime; }


    }

}
