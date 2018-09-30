// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// See https://github.com/microsoft/botbuilder-samples for a more comprehensive list of samples.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BasicBot;
using BasicBot.Dialog;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Main entry point and orchestration for bot.
    /// </summary>
    public class BasicBot : IBot
    {
        // Supported LUIS Intents
        public const string GreetingIntent = "Greeting";
        public const string CancelIntent = "Cancel";
        public const string HelpIntent = "Help";
        public const string NoneIntent = "None";

        /// <summary>
        /// Key in the bot config (.bot file) for the LUIS instance.
        /// In the .bot file, multiple instances of LUIS can be configured.
        /// </summary>
        public static readonly string LuisConfiguration = "BasicBotLuisApplication";

        private readonly BotServices _services;

        private readonly ILogger<BasicBot> _logger;

        private readonly BotAccessors _accessors;

        private readonly SavingsDialogs _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicBot"/> class.
        /// </summary>
        /// <param name="botServices">Bot services.</param>
        public BasicBot(BotAccessors accessors, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BasicBot>();
            _logger.LogTrace("EchoBot turn start.");
            _accessors = accessors;
            _dialogs = new SavingsDialogs(_accessors.DialogStateAccessor);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                await dc.ContinueDialogAsync(cancellationToken);
                if (!turnContext.Responded)
                {
                    await dc.BeginDialogAsync(SavingsDialogs.MainMenu, null, cancellationToken);
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                var activity = turnContext.Activity.AsConversationUpdateActivity();
                if (activity.MembersAdded.Any(member => member.Id != activity.Recipient.Id))
                {
                    await dc.BeginDialogAsync(SavingsDialogs.MainMenu, null, cancellationToken);
                }
            }

            await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}
