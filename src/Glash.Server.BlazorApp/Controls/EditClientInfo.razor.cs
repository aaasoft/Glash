using Microsoft.AspNetCore.Components;
using Quick.EntityFrameworkCore.Plus;

namespace Glash.Server.BlazorApp.Controls
{
    public partial class EditClientInfo
    {
        [Parameter]
        public Model.ClientInfo Model { get; set; }

        [Parameter]
        public Action<Model.ClientInfo, string[]> OkAction { get; set; }

        public class SelectAgentInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public bool Checked { get; set; }
        }

        private SelectAgentInfo[] selectAgents;

        private void Ok()
        {
            var agents = selectAgents
                .Where(t => t.Checked)
                .Select(t => t.Id)
                .ToArray();
            OkAction?.Invoke(Model, agents);
        }

        public static Dictionary<string, object> PrepareParameter(Model.ClientInfo model, Action<Model.ClientInfo, string[]> okAction)
        {
            return new Dictionary<string, object>()
            {
                [nameof(Model)] = model,
                [nameof(OkAction)] = okAction,
            };
        }

        private void checkAgent(SelectAgentInfo tag)
        {
            tag.Checked = !tag.Checked;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            var checkedAgentHashSet = ConfigDbContext.CacheContext
                .Query<Model.ClientAgentRelation>(t => t.ClientId == Model.Id)
                .Select(t => t.AgentId)
                .ToHashSet();

            selectAgents = ConfigDbContext.CacheContext.Query<BlazorApp.Model.AgentInfo>()
                .Select(t => new SelectAgentInfo()
                {
                    Id = t.Id,
                    Name = t.Name,
                    Checked = checkedAgentHashSet.Contains(t.Id)
                }).ToArray();
        }
    }
}
