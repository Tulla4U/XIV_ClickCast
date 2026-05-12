using Dalamud.Game.Config;

namespace ClickCast.Util;

public static class PartyListRoleSorting
{
    //  gather/craft 0  tank 1; melee 2;  ranged 3; heal 4;
    private static uint[][] OrderOptions =
    {
        [4, 0, 2, 2, 1],
        [4, 0, 1, 1, 2],
        [4, 1, 2, 2, 0],
        [4, 2, 1, 1, 0],
        [4, 1, 0, 0, 2],
        [4, 2, 0, 0, 1]
    };
    
    public static uint GetPriorityForRole(byte playerRole, byte otherRole)
    {
        var orderingOptionToLoad = playerRole switch
        {
            0 => UiConfigOption.PartyListSortTypeOther,
            1 => UiConfigOption.PartyListSortTypeTank,
            2 => UiConfigOption.PartyListSortTypeDps,
            3 => UiConfigOption.PartyListSortTypeDps,
            _ => UiConfigOption.PartyListSortTypeHealer
        };

        if (Plugin.GameConfig.TryGet(orderingOptionToLoad, out uint orderingOption))
        {
            return OrderOptions[orderingOption][otherRole];
        }

        return 0;
    }
}
