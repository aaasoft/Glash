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
            },
            CommandInfos = new[]
            {
                QpCommandInfo.Create(new QpCommands.Register.Request())
            }
        };
    }
}