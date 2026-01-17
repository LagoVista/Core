using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IAIPlaybook
    {
        string GetAiPromptInstructions();
        string GetDefaultMode();
    }
}
