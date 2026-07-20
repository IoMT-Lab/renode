using System;
using System.Collections.Generic;
using System.Linq;

using Antmicro.Renode.Core;
using Antmicro.Renode.Hooks;
using Antmicro.Renode.Peripherals;
using Antmicro.Renode.Peripherals.Bus;
using Antmicro.Renode.Peripherals.CPU;
using Antmicro.Renode.Utilities;

using Newtonsoft.Json;

namespace Antmicro.Renode.WebSockets.Providers
{
    public class CommandProvider : IWebSocketAPIProvider
    {
        public CommandProvider()
        { }

        public bool Start(WebSocketAPISharedData sharedData)
        {
            this.SharedData = sharedData;

            return true;
        }

        [WebSocketAPIAction("get-command-set-program-counter", "1.5.0")]
        private WebSocketAPIResponse SetProgramCounter(uint offset)
        {
            var emulationManager = EmulationManager.Instance;
            var emulation = emulationManager.CurrentEmulation;

            var machine = emulation.Machines.FirstOrDefault();
            if(machine == null)
            {
                return WebSocketAPIUtils.CreateEmptyActionResponse("No machine found in the current emulation");
            }
            else
            {
                var cpus = machine.SystemBus.GetCPUs();
                if(cpus.Count() == 0)
                {
                    return WebSocketAPIUtils.CreateEmptyActionResponse("No CPU found in the current machine");
                }
                else if(cpus.Count() > 1)
                {
                    return WebSocketAPIUtils.CreateEmptyActionResponse("Multiple CPUs found in the current machine. This provider supports only single CPU machines.");
                }
                else
                {
                    var cpu = cpus.First();
                    if(machine.TryGetAnyName(cpu, out var name))
                    {
                        var command = "{0} PC {1}".FormatWith(name, offset);
                        return WebSocketAPIUtils.CreateActionResponse(new[] { command });
                    }
                    else
                    {
                        return WebSocketAPIUtils.CreateEmptyActionResponse("Could not find a name for the CPU in the current machine");
                    }
                }
            }
        }

        private WebSocketAPISharedData SharedData;
    }
}