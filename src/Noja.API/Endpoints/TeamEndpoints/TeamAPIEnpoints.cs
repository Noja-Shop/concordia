using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Noja.API.Endpoints.TeamEndpoints
{
    public class TeamAPIEnpoints
    {
        private const string ApiBase = "api";

        public static class TeamManagement
        {
            private const string Base = $"{ApiBase}/teams";
            public const string CreateTeam = $"{Base}/create";
            public const string GetTeamByProduct = $"{Base}/product/";
            public const string GetTeamDetails = $"{Base}/{{id}}";
            public const string GetTeamDetailsWithMember = $"{Base}/{{id}}/member";
            public const string GetTeamsByCustomer = $"{Base}/myteams";
            public const string JoinTeam = $"{Base}/join";
        }
    }
}