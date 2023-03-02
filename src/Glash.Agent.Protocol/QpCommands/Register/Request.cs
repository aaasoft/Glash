using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Agent.Protocol.QpCommands.Register
{
    [DisplayName("Register")]
    public class Request : IQpCommandRequest<Response>
    {
        public string Name { get; set; }
    }
}
