public static class Quest
{
    public static bool IsAvailable(QuestData data)
    {
        var current_date = GameTime.GetCurrentDateTime();
        var next_date = GameTime.ParseDateTime(data.DateTimeNext);
        return current_date > next_date;
    }
}
