using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstiwTest.DB.Model
{
    public interface ITrackItem
    {
        bool IsChanged { get; }
        bool IsValid { get;  }
    }
}
