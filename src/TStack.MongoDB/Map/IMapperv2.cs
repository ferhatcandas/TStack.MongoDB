using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Entity;

namespace TStack.MongoDB.Map
{
    internal interface IMapper
    {
        string PrimaryKey { get; set; }
        List<MapCollection> Relations { get; set; }
    }
}
