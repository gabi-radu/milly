using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BasicBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace BasicBot.Dialog
{
    /// <summary>Contains the set of dialogs and prompts for the hotel bot.</summary>
    public class SavingsDialogs : DialogSet
    {
        /// <summary>The ID of the top-level dialog.</summary>
        public const string MainMenu = "mainMenu";

        public SavingsDialogs(IStatePropertyAccessor<DialogState> dialogStateAccessor)
            : base(dialogStateAccessor)
        {
            Add(new ChoicePrompt(Inputs.Choice));
            Add(new NumberPrompt<int>(Inputs.Number));

            // Define the steps for and add the main welcome dialog.
            WaterfallStep[] welcomeDialogSteps = new WaterfallStep[]
            {
                MainDialogSteps.PresentMenuAsync,
                MainDialogSteps.ProcessInputAsync,
                MainDialogSteps.RepeatMenuAsync,
            };

            Add(new WaterfallDialog(MainMenu, welcomeDialogSteps));

            // Define the steps for and add the reserve-table dialog.
            WaterfallStep[] remindLaterSteps = new WaterfallStep[]
            {
                RemindLaterSteps.StubAsync,
            };

            Add(new WaterfallDialog(Dialogs.RemindLater, remindLaterSteps));

            WaterfallStep[] bestDealSteps = new WaterfallStep[]
            {
                BestDealSteps.ShowBestDealAsync,
                BestDealSteps.ProcessInputAsync,
                BestDealSteps.RepeatMenuAsync,
            };

            Add(new WaterfallDialog(Dialogs.Renew, bestDealSteps));
        }

        /// <summary>Contains the IDs for the other dialogs in the set.</summary>
        private static class Dialogs
        {
            public const string Renew = "renew";
            public const string Explore = "explore";
            public const string RemindLater = "later";
            public const string ApplyOnline = "applyOnline";
            public const string CallBack = "callBack";
        }

        /// <summary>Contains the IDs for the prompts used by the dialogs.</summary>
        private static class Inputs
        {
            public const string Choice = "choicePrompt";
            public const string Number = "numberPrompt";
        }

        /// <summary>Contains the keys used to manage dialog state.</summary>
        private static class Outputs
        {
            public const string GivenName = "givenName";
            public const string UserName = "username";
            public const string CurrentMortgage = "CurrentMortgage";
            public const string AvailableDeals = "AvailableDeals";
        }

        /// <summary>Describes an option for the top-level dialog.</summary>
        private class WelcomeChoice
        {
            /// <summary>Gets or sets the text to show the guest for this option.</summary>
            public string Description { get; set; }

            /// <summary>Gets or sets the ID of the associated dialog for this option.</summary>
            public string DialogName { get; set; }
        }

        /// <summary>Contains the lists used to present options to the guest.</summary>
        private static class Lists
        {
            /// <summary>Gets the options for the top-level dialog.</summary>
            public static List<WelcomeChoice> WelcomeOptions { get; } = new List<WelcomeChoice>
            {
                new WelcomeChoice { Description = "Apply now", DialogName = Dialogs.ApplyOnline },
                new WelcomeChoice { Description = "View details", DialogName = Dialogs.Renew },
                new WelcomeChoice { Description = "Remind me later", DialogName = Dialogs.RemindLater },
            };

            private static readonly List<string> _welcomeList = WelcomeOptions.Select(x => x.Description).ToList();

            /// <summary>Gets the choices to present in the choice prompt for the top-level dialog.</summary>
            public static IList<Choice> WelcomeChoices { get; } = ChoiceFactory.ToChoices(_welcomeList);

            /// <summary>Gets the reprompt action for the top-level dialog.</summary>
            public static Activity WelcomeReprompt
            {
                get
                {
                    var reprompt = MessageFactory.SuggestedActions(_welcomeList, "Please choose an option");
                    reprompt.AttachmentLayout = AttachmentLayoutTypes.List;
                    return reprompt as Activity;
                }
            }

            public static Activity WelcomePrompt
            {
                get
                {
                    var reprompt = MessageFactory.SuggestedActions(_welcomeList, "How would you like to continue?");
                    reprompt.AttachmentLayout = AttachmentLayoutTypes.List;
                    return reprompt as Activity;
                }
            }

            /// <summary>Gets the options for the food-selection dialog.</summary>
            public static List<WelcomeChoice> BestDealOptions { get; } = new List<WelcomeChoice>
            {
                new WelcomeChoice { Description = "Apply online", DialogName = Dialogs.ApplyOnline },
                new WelcomeChoice { Description = "Call me", DialogName = Dialogs.CallBack },
            };

            private static readonly List<string> _bestDealList = BestDealOptions.Select(x => x.Description).ToList();

            /// <summary>Gets the choices to present in the choice prompt for the food-selection dialog.</summary>
            public static IList<Choice> BestDealChoices { get; } = ChoiceFactory.ToChoices(_bestDealList);

            /// <summary>Gets the reprompt action for the food-selection dialog.</summary>
            public static Activity BestDealReprompt
            {
                get
                {
                    var reprompt = MessageFactory.SuggestedActions(_bestDealList, "Please choose an option");
                    reprompt.AttachmentLayout = AttachmentLayoutTypes.List;
                    return reprompt as Activity;
                }
            }

            public static Activity BestDealPrompt
            {
                get
                {
                    var reprompt = MessageFactory.SuggestedActions(_bestDealList, "How would you like to continue?");
                    reprompt.AttachmentLayout = AttachmentLayoutTypes.List;
                    return reprompt as Activity;
                }
            }
        }

        /// <summary>
        /// Contains the waterfall dialog steps for the order-dinner dialog.
        /// </summary>
        private static class MainDialogSteps
        {
            public static async Task<DialogTurnResult> PresentMenuAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                var username = "mike4mail@gmail.com";
                var givenname = "Stephen";

                var deals = CustomerData.Find(username);

                var activity = new Activity();
                activity.Type = ActivityTypes.Typing;
                activity.Text = "";

                await stepContext.Context.SendActivityAsync("Hi " + givenname + "!");
                await stepContext.Context.SendActivityAsync(activity);
                Thread.Sleep(2000);

                await stepContext.Context.SendActivityAsync("We are analysing your current mortgage deal and spending patterns.");
                await stepContext.Context.SendActivityAsync(activity);
                Thread.Sleep(3000);

                await stepContext.Context.SendActivityAsync("Great news! Your house value went **up by 17%**.");
                await stepContext.Context.SendActivityAsync(activity);
                Thread.Sleep(1000);

                stepContext.Values[Outputs.GivenName] = givenname;
                stepContext.Values[Outputs.UserName] = username;
                stepContext.Values[Outputs.CurrentMortgage] = deals.Item1;
                stepContext.Values[Outputs.AvailableDeals] = deals.Item2;

                // Greet the guest and ask them to choose an option.
                await stepContext.Context.SendActivityAsync(
                    ProcessDeals(username),
                    cancellationToken: cancellationToken);

                return await stepContext.PromptAsync(
                    Inputs.Choice,
                    new PromptOptions
                    {
                        Prompt = Lists.WelcomePrompt,
                        RetryPrompt = Lists.WelcomeReprompt,
                        Choices = Lists.WelcomeChoices,
                    },
                    cancellationToken);
            }

            public static async Task<DialogTurnResult> ProcessInputAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                // Begin a child dialog associated with the chosen option.
                var choice = (FoundChoice)stepContext.Result;
                var dialogId = Lists.WelcomeOptions[choice.Index].DialogName;

                if (dialogId == Dialogs.ApplyOnline)
                {
                    await stepContext.Context.SendActivityAsync("Thank you. [Click here](https://personal.rbs.co.uk/personal/mortgages/secure/mortgage-agreement-in-principle.html) to complete your paperless application today.", cancellationToken: cancellationToken);
                    return await stepContext.CancelAllDialogsAsync(cancellationToken);
                }

                return await stepContext.BeginDialogAsync(dialogId, stepContext.Values, cancellationToken);
            }

            public static async Task<DialogTurnResult> RepeatMenuAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                // Start this dialog over again.
                return await stepContext.ReplaceDialogAsync(MainMenu, null, cancellationToken);
            }

            private static string ProcessDeals(string username)
            {
                var customerData = CustomerData.Find(username);

                var maxSavingDeal = customerData.Item2.OrderBy(m => m.TotalRepayment).First();

                return string.Format("For **just £{0:0}** extra per month, you can **save up to £{1:0}k** and reduce your term by up to {2} years.", maxSavingDeal.MonthlyRepayment - customerData.Item1.MonthlyRepayment, (customerData.Item1.TotalRepayment - maxSavingDeal.TotalRepayment) / 1000, customerData.Item1.Term - maxSavingDeal.Term);
            }
        }

        /// <summary>
        /// Contains the waterfall dialog steps for the reserve-table dialog.
        /// </summary>
        private static class RemindLaterSteps
        {
            public static async Task<DialogTurnResult> StubAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                await stepContext.Context.SendActivityAsync(
                    "Come back any time, " + ((IDictionary<string, object>)stepContext.Options)[Outputs.GivenName] + "!",
                    cancellationToken: cancellationToken);

                return await stepContext.CancelAllDialogsAsync(cancellationToken);
            }
        }

        private static class BestDealSteps
        {
            public static async Task<DialogTurnResult> ShowBestDealAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                var activity = new Activity();
                activity.Type = ActivityTypes.Typing;
                activity.Text = "";

                await stepContext.Context.SendActivityAsync("Let me bring up your details, " + ((IDictionary<string, object>)stepContext.Options)[Outputs.GivenName], cancellationToken: cancellationToken);
                await stepContext.Context.SendActivityAsync(activity);
                Thread.Sleep(2000);

                var currentDeal = (Mortgage)((IDictionary<string, object>)stepContext.Options)[Outputs.CurrentMortgage];

                //await stepContext.Context.SendActivityAsync(
                //    string.Format("Your current mortgage balance is £{0:0.##}k and your current deal is {1}. You pay £{2:0.##} per month and it will take you {3} years to clear your balance, for a total cost of £{4:0.##}k.", currentDeal.Balance / 1000, currentDeal.Description, currentDeal.MonthlyRepayment, currentDeal.Term, currentDeal.TotalRepayment / 1000), cancellationToken: cancellationToken);

                //await stepContext.Context.SendActivityAsync(activity);
                //Thread.Sleep(2000);

                var bestDeal = (((IDictionary<string, object>)stepContext.Options)[Outputs.AvailableDeals] as IEnumerable<Mortgage>).OrderBy(m => m.TotalRepayment).First();

                //await stepContext.Context.SendActivityAsync(
                //    string.Format("By increasing your monthly payments to £{0:0.##} and reducing your term to {1} years, your total repayment cost could be as low as £{2:0.##}k, saving you up to £{3:0.##}k. This is a {4} deal.", bestDeal.MonthlyRepayment, bestDeal.Term, bestDeal.TotalRepayment / 1000, (currentDeal.TotalRepayment - bestDeal.TotalRepayment) / 1000, bestDeal.Description), cancellationToken: cancellationToken);

                await stepContext.Context.SendActivityAsync(string.Format(@"|       | Current  &nbsp; &nbsp; | Our best offer |
| ----- | ----- | ----- |
| Deal | {0} | {1} |
| Monthly Rate | £{2:0} | £{3:0} |
| Term | {4} | {5} |
| Total Repayment  &nbsp; &nbsp; | £{6:0.##}k | £{7:0.##}k |", currentDeal.Description,bestDeal.Description,currentDeal.MonthlyRepayment,bestDeal.MonthlyRepayment,currentDeal.Term,bestDeal.Term,currentDeal.TotalRepayment/1000,bestDeal.TotalRepayment/1000), cancellationToken: cancellationToken);

                return await stepContext.PromptAsync(
                    Inputs.Choice,
                    new PromptOptions
                    {
                        Prompt = Lists.BestDealPrompt,
                        RetryPrompt = Lists.BestDealReprompt,
                        Choices = Lists.BestDealChoices,
                    },
                    cancellationToken);
            }

            public static async Task<DialogTurnResult> ProcessInputAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                // Begin a child dialog associated with the chosen option.
                var choice = (FoundChoice)stepContext.Result;
                var dialogId = Lists.BestDealOptions[choice.Index].DialogName;

                switch (dialogId)
                {
                    case Dialogs.CallBack:
                        await stepContext.Context.SendActivityAsync("One of our advisers will be with you shortly, on your mobile banking registered phone number.", cancellationToken: cancellationToken);
                        break;
                    case Dialogs.ApplyOnline:
                        await stepContext.Context.SendActivityAsync("Thank you. [Click here](https://personal.rbs.co.uk/personal/mortgages/secure/mortgage-agreement-in-principle.html) to complete your paperless application today.", cancellationToken: cancellationToken);
                        break;
                }

                return await stepContext.CancelAllDialogsAsync(cancellationToken);
            }

            public static async Task<DialogTurnResult> RepeatMenuAsync(
                WaterfallStepContext stepContext,
                CancellationToken cancellationToken)
            {
                // Start this dialog over again.
                return await stepContext.ReplaceDialogAsync(MainMenu, null, cancellationToken);
            }
        }
    }
}
