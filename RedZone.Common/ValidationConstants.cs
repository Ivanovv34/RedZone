namespace RedZone.Common
{
    public static class ValidationConstants
    {
        public static class Match
        {
            public const int TeamNameMinLength = 2;
            public const int TeamNameMaxLength = 100;
            public const int CompetitionIdMin = 1;
            public const int CompetitionIdMax = int.MaxValue;
            public const string TeamNameLengthError = "Team name must be between 2 and 100 characters.";
            public const string TeamNameRequiredError = "Team name is required.";
            public const string DateRequiredError = "Match date is required.";
            public const string CompetitionRequiredError = "Please select a competition.";
            public const string CompetitionInvalidError = "Please select a valid competition.";
        }

        public static class Competition
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 100;
            public const string NameRequiredError = "Competition name is required.";
            public const string NameLengthError = "Name must be between 2 and 100 characters.";
        }

        public static class Prediction
        {
            public const int GoalsMin = 0;
            public const int GoalsMax = 20;
            public const string HomeGoalsRequiredError = "Home goals prediction is required.";
            public const string AwayGoalsRequiredError = "Away goals prediction is required.";
            public const string GoalsRangeError = "Goals must be between 0 and 20.";
        }

        public static class MatchResult
        {
            public const int GoalsMin = 0;
            public const int GoalsMax = 50;
            public const string HomeGoalsRequiredError = "Home goals are required.";
            public const string AwayGoalsRequiredError = "Away goals are required.";
            public const string GoalsRangeError = "Goals must be between 0 and 50.";
        }
    }
}