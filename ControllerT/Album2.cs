using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControllerT
{
    [GeneratedController("api/v1/album2")]
    public class Album2
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }
    }
}
