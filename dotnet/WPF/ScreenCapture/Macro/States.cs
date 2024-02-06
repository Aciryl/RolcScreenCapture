using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
    public class States
    {
        protected States()
        {

        }

        public static States Mission { get; } = new States();
        public static States Pending { get; } = new States();
        public static States Reward { get; } = new States();
        public static States Lobby { get; } = new States();
        public static States LobbyMove { get; } = new States();
        public static States Ready { get; } = new States();
        public static States Error { get; } = new States();
        public static States Restart { get; } = new States();
    }
}
