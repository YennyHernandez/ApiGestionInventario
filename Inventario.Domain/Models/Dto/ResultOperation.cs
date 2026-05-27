using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Domain.Models.Dto
{
    public class ResultOperation
    {
        public bool stateOperation { get; set; }
        public string MessageResult { get; set; }
        public string MessageExceptionUser { get; set; }
        public string MessageExceptionTechnical { get; set; }
        public bool RollBack { get; set; }
    }
    public class ResultOperation<T> : ResultOperation
    {
        public T Result { get; set; }
        public List<T> Results { get; set; }
    }
}
