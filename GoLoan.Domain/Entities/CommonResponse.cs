using System;
using System.Collections.Generic;
using System.Text;

namespace GoLoan.Domain.Entities
{
    public class CommonResponse
    {
        
            public bool success { get; set; }
            public bool error { get; set; }
            public object data { get; set; }
            public string message { get; set; }
        
    }
}
