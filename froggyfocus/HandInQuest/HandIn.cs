using Godot;
using System.Linq;

public static class HandIn
{
    public static HandInData GetOrCreateData(string id)
    {
        var data = Data.Game.HandIns.FirstOrDefault(x => x.Id == id);
        if (data == null)
        {
            data = new HandInData
            {
                Id = id,
            };

            Data.Game.HandIns.Add(data);
        }

        return data;
    }

    public static bool IsAvailable(string id)
    {
        var data = GetOrCreateData(id);
        return Quest.IsAvailable(data);
    }

    public static void InitializeData(HandInInfo info)
    {
        var data = GetOrCreateData(info.Id);

        if (data.Initialized) return;
        data.Initialized = true;

        ResetData(info);
        data.DateTimeNext = GameTime.GetCurrentDateTimeString();

        Data.Game.Save();
    }

    public static void ResetData(HandInInfo info)
    {
        var data = GetOrCreateData(info.Id);
        data.Claimed = false;

        ResetData_Requests(info, data);
        ResetData_Date(info, data);
    }

    private static void ResetData_Requests(HandInInfo info, HandInData data)
    {
        var rng = new RandomNumberGenerator();
        var count = rng.RandiRange(info.CountRange.X, info.CountRange.Y);

        data.RequestInfos = info.PossibleRequests
            .TakeRandom(count, allow_duplicates: !info.UniqueRequests)
            .Select(x => x.ResourcePath)
            .ToList();

        var reward_base = Constants.BUG_BASE_VALUE * count;
        var reward_mul = rng.RandfRange(4f, 6f);
        var reward = (int)(reward_base * reward_mul);
        data.MoneyReward = reward;
    }

    private static void ResetData_Date(HandInInfo info, HandInData data)
    {
        var date_now = GameTime.GetCurrentDateTime();
        var date_next = date_now.AddSeconds(info.CooldownInSeconds);
        data.DateTimeNext = GameTime.GetDateTimeString(date_next);
    }
}
