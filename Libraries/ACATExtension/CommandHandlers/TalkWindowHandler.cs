﻿////////////////////////////////////////////////////////////////////////////
// <copyright file="TalkWindowHandler.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.CodeAnalysis;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Manages the Talk window - closes it, clears it or
    /// toggles its visibility
    /// </summary>
    public class TalkWindowHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public TalkWindowHandler(String cmd)
            : base(cmd)
        {
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="handled">set to true if the command was handled</param>
        /// <returns>true on success</returns>
        public override bool Execute(ref bool handled)
        {
            handled = true;

            switch (Command)
            {
                case "CmdTalkWindowToggle":
                    {
                        // first check if a functional agent is currently active
                        // if it is, instruct it to close and then execute the command
                        // after it has exited
                        IApplicationAgent agent = AgentManager.Instance.ActiveAgent;
                        if (agent is IFunctionalAgent)
                        {
                            IFunctionalAgent funcAgent = (IFunctionalAgent)agent;
                            if (funcAgent.ExitCommand == null)
                            {
                                funcAgent.ExitCommand = new PostExitCommand { Command = this, ContextSwitch = true };
                                funcAgent.OnRequestClose();
                                break;
                            }
                        }

                        Context.AppTalkWindowManager.ToggleTalkWindow();
                    }

                    break;

                case "CmdTalkWindowClear":
                    if (Context.AppTalkWindowManager.IsTalkWindowActive)
                    {
                        Context.AppAgentMgr.RunCommand("ClearTalkWindowText", ref handled);
                    }

                    break;

                case "CmdTalkWindowClose":
                    Context.AppTalkWindowManager.CloseTalkWindow();
                    break;

                default:
                    handled = false;
                    break;
            }

            return true;
        }
    }
}