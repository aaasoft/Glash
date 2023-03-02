using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client.Protocol.QpCommands.GetServerInfo
{
    [DisplayName("Get Server Info")]
    public class Request : IQpCommandRequest<Response> { }
}
