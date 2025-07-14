using Godot;
using System.Linq;

public static class Fetch
{
    public static FetchData GetOrCreateData(string id)
    {
        var data = Data.Game.Fetchs.FirstOrDefault(x => x.Id == id);
        if (data == null)
        {
            data = new FetchData
            {
                Id = id,
            };

            Data.Game.Fetchs.Add(data);
        }

        return data;
    }

    public static bool IsAvailable(string id)
    {
        var data = GetOrCreateData(id);
        return Quest.IsAvailable(data);
    }

    public static void InitializeData(FetchInfo info)
    {
        var data = GetOrCreateData(info.Id);

        if (data.Initialized) return;
        data.Initialized = true;

        ResetData(info);
        data.DateTimeNext = GameTime.GetCurrentDateTimeString();

        Data.Game.Save();
    }

    public static void ResetData(FetchInfo info)
    {
        var data = GetOrCreateData(info.Id);
        data.Claimed = false;
        data.Started = false;

        var rng = new RandomNumberGenerator();
        data.Count = rng.RandiRange(info.CountRange.X, info.CountRange.Y);

        var mul_reward = rng.RandfRange(1.0f, 1.2f);
        data.MoneyReward = (int)(info.MoneyRewardBase * mul_reward);

        var date_now = GameTime.GetCurrentDateTime();
        var date_next = date_now.AddSeconds(info.CooldownInSeconds);
        data.DateTimeNext = GameTime.GetDateTimeString(date_next);
    }
}
