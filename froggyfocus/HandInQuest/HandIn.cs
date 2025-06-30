using Godot;
using System.Linq;

public static class HandIn
{
    public static bool IsHandInAvailable(string id)
    {
        var current_date = GameTime.GetCurrentDateTime();
        var data = GetOrCreateData(id);
        var next_date = GameTime.ParseDateTime(data.DateTimeNext);
        return current_date > next_date;
    }

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

    public static void InitializeData(HandInInfo info)
    {
        var data = GetOrCreateData(info.Id);

        if (data.Initialized) return;
        data.Initialized = true;

        ResetData(info);
        data.DateTimeNext = GameTime.GetCurrentDateTimeString();
    }

    public static void ResetData(HandInInfo info)
    {
        var data = GetOrCreateData(info.Id);
        var rng = new RandomNumberGenerator();

        var count = rng.RandiRange(info.CountRange.X, info.CountRange.Y);
        var request_infos = info.PossibleRequests.TakeRandom(count, allow_duplicates: true);

        data.Requests = request_infos.Select(x => new InventoryCharacterData
        {
            InfoPath = x.ResourcePath
        }).ToList();

        var reward_base = request_infos.Sum(x => x.CurrencyReward);
        var reward_mul = rng.RandfRange(info.RewardMultiplierRange.X, info.RewardMultiplierRange.Y);
        var reward = (int)(reward_base * reward_mul);
        data.MoneyReward = reward;

        var date_now = GameTime.GetCurrentDateTime();
        var date_next = date_now.AddSeconds(info.CooldownInSeconds);
        data.DateTimeNext = GameTime.GetDateTimeString(date_next);
    }
}
