using Quick.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Core.Server
{
    public class GlashClientContext
    {
        public QpChannel Channel { get; private set; }
        public GlashClientContext(QpChannel channel)
        {
            this.Channel = channel;
        }
    }
}
