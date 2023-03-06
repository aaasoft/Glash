using Glash.Model;
using Quick.Protocol;

namespace Glash.Agent.Protocol
{
    public class Instruction
    {
        public static QpInstruction Instance = new QpInstruction()
        {
            Id = typeof(Instruction).Namespace,
            Name = "Glash Agent Protocol",
            NoticeInfos = new[]
            {
                QpNoticeInfo.Create<G.D>(),
                QpNoticeInfo.Create<TunnelClosed>()
            },
            CommandInfos = new[]
            {
                QpCommandInfo.Create(new QpCommands.Register.Request()),
                QpCommandInfo.Create(new QpCommands.CreateTunnel.Request()),
                QpCommandInfo.Create(new QpCommands.StartTunnel.Request())
            }
        };
    }
}