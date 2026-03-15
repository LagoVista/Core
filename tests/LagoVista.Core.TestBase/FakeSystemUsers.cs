using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.TestBase
{
    public class FakeSystemUsers : ISystemUsers
    {
        public EntityHeader SystemOrg => EntityHeader.Create("1338237877F4441DA200F4DCF9BBB218", "System");

        public EntityHeader HostUser => EntityHeader.Create("24B5D5B5D5F4441DA200F4DCF9BBB218", "Frank-Host");

        public EntityHeader InstanceUser => EntityHeader.Create("34B5D5B5D5F4441DA200F4DCF9BBB218", "Tracey-Instance");

        public EntityHeader DeviceManagerUser => EntityHeader.Create("44B5D5B5D5F4441DA200F4DCF9BBB218", "Bill-Device");

        public EntityHeader JobServiceUser => EntityHeader.Create("54B5D5B5D5F4441DA200F4DCF9BBB218", "Sally-Job");

        private static readonly FakeSystemUsers _instance = new FakeSystemUsers();
        public static FakeSystemUsers Instance => _instance;
    }
}
