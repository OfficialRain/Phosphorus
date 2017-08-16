using System;
using static Phosphorus.Core;

namespace Phosphorus
{
    public static class Triggers
    {
        public static void InitializeTriggers()
        {
            Trigger CuntTrigger = new Trigger()
            {
                Key = "cunt",
                Response = "r00d {user}",
                SearchType = TriggerSearchType.SearchFor
            };
            Trigger BrandonsPhoneNumberTrigger = new Trigger()
            {
                Key = "what's brandon's phone number?",
                Response = "It's `+44 7469 627541`! {user}",
                SearchType = TriggerSearchType.FullMessage
            };
            Trigger TwentyOneTrigger = new Trigger()
            {
                Key = "wuz nine plus tin?",
                Response = "twendy one",
                SearchType = TriggerSearchType.FullMessage
            };
            Trigger FiveTrigger = new Trigger()
            {
                Key = "5",
                Response = "five",
                SearchType = TriggerSearchType.FullMessage
            };
            Trigger ShiftOSTrigger = new Trigger()
            {
                Key = "call of duty",
                Response = "did you mean \"cancer\"? {user}",
                SearchType = TriggerSearchType.SearchFor
            };

        }
    }
}
