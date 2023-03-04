using Glash.Client.ConsoleApp;
using YiQiDong.Agent;

#if DEBUG
Thread.Sleep(2000);
#endif
Quick.Protocol.QpAllClients.RegisterUriSchema();
AgentContext.Instance.InitContainerInfo(args);
AgentContext.Instance.InitAgentInstance(new Agent());
AgentContext.Instance.Run();
AgentContext.Instance.Dispose();
