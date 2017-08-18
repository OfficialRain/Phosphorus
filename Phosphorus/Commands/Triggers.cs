using static Phosphorus.Core;

namespace Phosphorus
{
    public static class Triggers
    {
		/// <summary>
		/// Initializes Triggers. The constructor for <see cref="Trigger"/> adds itself to a list of triggers.
		/// </summary>
        public static void InitializeTriggers()
        {
            Trigger CuntTrigger = new Trigger()
            {
                Key = "cunt",
                Response = "r00d {user}",
                SearchType = TriggerSearchType.Contains
            };
            Trigger BrandonsPhoneNumberTrigger = new Trigger()
            {
                Key = "what's brandon's phone number?",
				//Response = "It's `+44 7469 627541`! {user}",
				Response = "It's something I don't have anymore, that's for sure.",
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
            Trigger CODTrigger = new Trigger()
            {
                Key = "call of duty",
                Response = "did you mean \"cancer\"? {user}",
                SearchType = TriggerSearchType.Contains
            };
			Trigger HotStringTrigger = new Trigger()
			{
				Key = "hot string",
				Response = "```hot```",
				SearchType = TriggerSearchType.FullMessage
			};

		}
	}
}
