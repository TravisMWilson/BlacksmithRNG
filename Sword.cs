using System.Collections.Generic;

public class PartOffset {
    #region Properties

    public string PartName { get; set; }
    public int StartPos { get; set; }
    public int EndPos { get; set; }

    #endregion
    #region Constructor

    public PartOffset(string partName, int startPos, int endPos = 0) {
        PartName = partName;
        StartPos = startPos;
        EndPos = endPos;
    }

    #endregion
}

public class PartOffsetList {
    #region PublicMethods

    public static void SetPartOffset(string partName, out int startPos, out int endPos) {
        startPos = 0;
        endPos = 0;

        foreach (PartOffset offset in PartOffsetList.GetOffsets()) {
            if (offset.PartName == partName) {
                startPos = offset.StartPos;
                endPos = offset.EndPos;
            }
        }
    }

    public static List<PartOffset> GetOffsets() {
        return new List<PartOffset> {
            // Pommels
            new PartOffset("Pommel_Elven Steel", 4),
            new PartOffset("Pommel_Carbon Steel", 4),
            new PartOffset("Pommel_Sandstone", 4),
            new PartOffset("Pommel_Bronze Alloy", 3),
            new PartOffset("Pommel_Limestone", 1),
            new PartOffset("Pommel_Stormshard", 7),
            new PartOffset("Pommel_Fireheart", 4),
            new PartOffset("Pommel_Elderglow", 5),
            new PartOffset("Pommel_Soulstone", 3),
            new PartOffset("Pommel_Quartz", 2),
            new PartOffset("Pommel_Mithril", 3),
            new PartOffset("Pommel_Forest Steel", 7),
            new PartOffset("Pommel_Silver", 6),
            new PartOffset("Pommel_Iron", 2),
            new PartOffset("Pommel_Clay", 5),
            new PartOffset("Pommel_Marble", 3),
            new PartOffset("Pommel_Shadowglass", 1),
            new PartOffset("Pommel_Brass Alloy", 7),
            new PartOffset("Pommel_Nightsilver", 5),
            new PartOffset("Pommel_Spectral", 4),
            new PartOffset("Pommel_Electrum", 2),
            new PartOffset("Pommel_Mysticite", 5),
            new PartOffset("Pommel_Aluminum", 3),
            new PartOffset("Pommel_Void Iron", 5),
            new PartOffset("Pommel_Runestone", 9),
            new PartOffset("Pommel_Manasteel", 5),
            new PartOffset("Pommel_Basalt", 5),
            new PartOffset("Pommel_Adamantine", 2),
            new PartOffset("Pommel_Steel", 3),
            new PartOffset("Pommel_Zinc", 4),
            new PartOffset("Pommel_Slate", 6),
            new PartOffset("Pommel_Moonstone", 7),
            new PartOffset("Pommel_Celestium", 5),
            new PartOffset("Pommel_Tungsten", 2),
            new PartOffset("Pommel_Necrotite", 3),
            new PartOffset("Pommel_Darksteel", 5),
            new PartOffset("Pommel_Lead", 3),
            new PartOffset("Pommel_Flint", 3),
            new PartOffset("Pommel_Mossy Iron", 4),
            new PartOffset("Pommel_Aetherium", 4),
            new PartOffset("Pommel_Tin", 2),
            new PartOffset("Pommel_Thunderite", 9),
            new PartOffset("Pommel_Dragonbone", 5),
            new PartOffset("Pommel_Ironwood", 4),
            new PartOffset("Pommel_Bloodstone", 5),
            new PartOffset("Pommel_Copper", 4),
            new PartOffset("Pommel_Star Metal", 4),
            new PartOffset("Pommel_Obsidian", 4),
            new PartOffset("Pommel_Cobalt", 3),
            new PartOffset("Pommel_Granite", 3),
            new PartOffset("Pommel_Lifestone", 3),
            new PartOffset("Pommel_Pewter", 3),
            new PartOffset("Pommel_Nickel", 2),
            new PartOffset("Pommel_Gold", 10),
            new PartOffset("Pommel_Moonsteel", 6),
            new PartOffset("Pommel_Orichalcum", 6),
            new PartOffset("Pommel_Phoenix Feather", 14),
            new PartOffset("Pommel_Froststeel", 4),
            new PartOffset("Pommel_Iridium", 3),
            new PartOffset("Pommel_Chalk", 4),
            new PartOffset("Pommel_Sunsilver", 8),
            new PartOffset("Pommel_Eternal Ice", 5),
            new PartOffset("Pommel_Netherite", 4),
            new PartOffset("Pommel_Platinum", 3),
            new PartOffset("Pommel_Twilight", 6),
            new PartOffset("Pommel_Titanium", 3),
            new PartOffset("Pommel_Voidstone", 5),
            new PartOffset("Pommel_Arcane Crystal", 7),

            // Handles
            new PartOffset("Handle_Elven Steel", 24, 23),
            new PartOffset("Handle_Carbon Steel", 17, 17),
            new PartOffset("Handle_Sandstone", 11, 10),
            new PartOffset("Handle_Bronze Alloy", 19, 19),
            new PartOffset("Handle_Limestone", 14, 13),
            new PartOffset("Handle_Stormshard", 23, 23),
            new PartOffset("Handle_Fireheart", 18, 18),
            new PartOffset("Handle_Elderglow", 11, 10),
            new PartOffset("Handle_Soulstone", 24, 24),
            new PartOffset("Handle_Quartz", 13, 13),
            new PartOffset("Handle_Mithril", 22, 22),
            new PartOffset("Handle_Forest Steel", 17, 16),
            new PartOffset("Handle_Silver", 11, 10),
            new PartOffset("Handle_Iron", 15, 14),
            new PartOffset("Handle_Clay", 18, 17),
            new PartOffset("Handle_Marble", 15, 15),
            new PartOffset("Handle_Shadowglass", 14, 14),
            new PartOffset("Handle_Brass Alloy", 16, 15),
            new PartOffset("Handle_Nightsilver", 14, 14),
            new PartOffset("Handle_Spectral", 16, 15),
            new PartOffset("Handle_Electrum", 13, 12),
            new PartOffset("Handle_Mysticite", 16, 15),
            new PartOffset("Handle_Aluminum", 16, 15),
            new PartOffset("Handle_Void Iron", 15, 15),
            new PartOffset("Handle_Runestone", 9, 9),
            new PartOffset("Handle_Manasteel", 14, 14),
            new PartOffset("Handle_Basalt", 15, 14),
            new PartOffset("Handle_Adamantine", 13, 12),
            new PartOffset("Handle_Steel", 12, 12),
            new PartOffset("Handle_Zinc", 15, 14),
            new PartOffset("Handle_Slate", 13, 13),
            new PartOffset("Handle_Moonstone", 30, 29),
            new PartOffset("Handle_Celestium", 15, 14),
            new PartOffset("Handle_Tungsten", 9, 8),
            new PartOffset("Handle_Necrotite", 15, 15),
            new PartOffset("Handle_Darksteel", 22, 21),
            new PartOffset("Handle_Lead", 8, 7),
            new PartOffset("Handle_Flint", 10, 9),
            new PartOffset("Handle_Mossy Iron", 10, 9),
            new PartOffset("Handle_Aetherium", 24, 23),
            new PartOffset("Handle_Tin", 8, 8),
            new PartOffset("Handle_Thunderite", 17, 16),
            new PartOffset("Handle_Dragonbone", 24, 24),
            new PartOffset("Handle_Ironwood", 12, 12),
            new PartOffset("Handle_Bloodstone", 13, 13),
            new PartOffset("Handle_Copper", 14, 14),
            new PartOffset("Handle_Star Metal", 18, 17),
            new PartOffset("Handle_Obsidian", 17, 17),
            new PartOffset("Handle_Cobalt", 11, 11),
            new PartOffset("Handle_Granite", 9, 9),
            new PartOffset("Handle_Lifestone", 11, 11),
            new PartOffset("Handle_Pewter", 10, 9),
            new PartOffset("Handle_Nickel", 11, 10),
            new PartOffset("Handle_Gold", 21, 20),
            new PartOffset("Handle_Moonsteel", 21, 21),
            new PartOffset("Handle_Orichalcum", 13, 13),
            new PartOffset("Handle_Phoenix Feather", 6, 6),
            new PartOffset("Handle_Froststeel", 21, 20),
            new PartOffset("Handle_Iridium", 12, 11),
            new PartOffset("Handle_Chalk", 16, 16),
            new PartOffset("Handle_Sunsilver", 21, 20),
            new PartOffset("Handle_Eternal Ice", 8, 8),
            new PartOffset("Handle_Netherite", 16, 16),
            new PartOffset("Handle_Platinum", 19, 18),
            new PartOffset("Handle_Twilight", 21, 20),
            new PartOffset("Handle_Titanium", 12, 12),
            new PartOffset("Handle_Voidstone", 12, 12),
            new PartOffset("Handle_Arcane Crystal", 12, 12),

            // Guards
            new PartOffset("Guard_Elven Steel", 2, 2),
            new PartOffset("Guard_Carbon Steel", 6, -5),
            new PartOffset("Guard_Sandstone", 4, 0),
            new PartOffset("Guard_Bronze Alloy", 4, 1),
            new PartOffset("Guard_Limestone", 2, 0),
            new PartOffset("Guard_Stormshard", 3, 3),
            new PartOffset("Guard_Fireheart", 3, 0),
            new PartOffset("Guard_Elderglow", 4, 0),
            new PartOffset("Guard_Soulstone", 7, 3),
            new PartOffset("Guard_Quartz", 4, -1),
            new PartOffset("Guard_Mithril", 1, 2),
            new PartOffset("Guard_Forest Steel", 3, 0),
            new PartOffset("Guard_Silver", 4, 0),
            new PartOffset("Guard_Iron", 3, 0),
            new PartOffset("Guard_Clay", 7, -4),
            new PartOffset("Guard_Marble", 2, 0),
            new PartOffset("Guard_Shadowglass", 1, 2),
            new PartOffset("Guard_Brass Alloy", 16, 13),
            new PartOffset("Guard_Nightsilver", 32, -17),
            new PartOffset("Guard_Spectral", 9, 5),
            new PartOffset("Guard_Electrum", 3, -1),
            new PartOffset("Guard_Mysticite", 14, -8),
            new PartOffset("Guard_Aluminum", 3, 0),
            new PartOffset("Guard_Void Iron", 2, 1),
            new PartOffset("Guard_Runestone", 2, 0),
            new PartOffset("Guard_Manasteel", 13, 0),
            new PartOffset("Guard_Basalt", 14, -10),
            new PartOffset("Guard_Adamantine", 2, 0),
            new PartOffset("Guard_Steel", 2, 0),
            new PartOffset("Guard_Zinc", 0, 1),
            new PartOffset("Guard_Slate", 4, -1),
            new PartOffset("Guard_Moonstone", 7, -2),
            new PartOffset("Guard_Celestium", 28, -13),
            new PartOffset("Guard_Tungsten", 2, 0),
            new PartOffset("Guard_Necrotite", 27, -16),
            new PartOffset("Guard_Darksteel", 18, 8),
            new PartOffset("Guard_Lead", 2, 1),
            new PartOffset("Guard_Flint", 2, 1),
            new PartOffset("Guard_Mossy Iron", 4, -1),
            new PartOffset("Guard_Aetherium", 12, 5),
            new PartOffset("Guard_Tin", 2, 1),
            new PartOffset("Guard_Thunderite", 5, 1),
            new PartOffset("Guard_Dragonbone", 17, 4),
            new PartOffset("Guard_Ironwood", 2, 2),
            new PartOffset("Guard_Bloodstone", 0, 4),
            new PartOffset("Guard_Copper", 2, 0),
            new PartOffset("Guard_Star Metal", 10, -7),
            new PartOffset("Guard_Obsidian", 7, 3),
            new PartOffset("Guard_Cobalt", 7, 6),
            new PartOffset("Guard_Granite", 3, -1),
            new PartOffset("Guard_Lifestone", 0, 3),
            new PartOffset("Guard_Pewter", 2, 2),
            new PartOffset("Guard_Nickel", 1, 1),
            new PartOffset("Guard_Gold", 8, 7),
            new PartOffset("Guard_Moonsteel", 2, 1),
            new PartOffset("Guard_Orichalcum", 11, 3),
            new PartOffset("Guard_Phoenix Feather", -1, 4),
            new PartOffset("Guard_Froststeel", 3, 7),
            new PartOffset("Guard_Iridium", 4, 0),
            new PartOffset("Guard_Chalk", 3, -1),
            new PartOffset("Guard_Sunsilver", 14, -3),
            new PartOffset("Guard_Eternal Ice", 9, 8),
            new PartOffset("Guard_Netherite", 16, -11),
            new PartOffset("Guard_Platinum", 3, -1),
            new PartOffset("Guard_Twilight", 16, -9),
            new PartOffset("Guard_Titanium", 2, 2),
            new PartOffset("Guard_Voidstone", 2, 1),
            new PartOffset("Guard_Arcane Crystal", 12, 5),

            // Blades
            new PartOffset("Blade_Elven Steel", 26),
            new PartOffset("Blade_Carbon Steel", 30),
            new PartOffset("Blade_Sandstone", 32),
            new PartOffset("Blade_Bronze Alloy", 30),
            new PartOffset("Blade_Limestone", 27),
            new PartOffset("Blade_Stormshard", 25),
            new PartOffset("Blade_Fireheart", 32),
            new PartOffset("Blade_Elderglow", 32),
            new PartOffset("Blade_Soulstone", 17),
            new PartOffset("Blade_Quartz", 32),
            new PartOffset("Blade_Mithril", 28),
            new PartOffset("Blade_Forest Steel", 30),
            new PartOffset("Blade_Silver", 32),
            new PartOffset("Blade_Iron", 32),
            new PartOffset("Blade_Clay", 32),
            new PartOffset("Blade_Marble", 32),
            new PartOffset("Blade_Shadowglass", 32),
            new PartOffset("Blade_Brass Alloy", 26),
            new PartOffset("Blade_Nightsilver", 32),
            new PartOffset("Blade_Spectral", 32),
            new PartOffset("Blade_Electrum", 32),
            new PartOffset("Blade_Mysticite", 32),
            new PartOffset("Blade_Aluminum", 28),
            new PartOffset("Blade_Void Iron", 32),
            new PartOffset("Blade_Runestone", 32),
            new PartOffset("Blade_Manasteel", 32),
            new PartOffset("Blade_Basalt", 32),
            new PartOffset("Blade_Adamantine", 32),
            new PartOffset("Blade_Steel", 32),
            new PartOffset("Blade_Zinc", 20),
            new PartOffset("Blade_Slate", 32),
            new PartOffset("Blade_Moonstone", 17),
            new PartOffset("Blade_Celestium", 32),
            new PartOffset("Blade_Tungsten", 32),
            new PartOffset("Blade_Necrotite", 32),
            new PartOffset("Blade_Darksteel", 20),
            new PartOffset("Blade_Lead", 32),
            new PartOffset("Blade_Flint", 32),
            new PartOffset("Blade_Mossy Iron", 31),
            new PartOffset("Blade_Aetherium", 22),
            new PartOffset("Blade_Tin", 23),
            new PartOffset("Blade_Thunderite", 29),
            new PartOffset("Blade_Dragonbone", 20),
            new PartOffset("Blade_Ironwood", 28),
            new PartOffset("Blade_Bloodstone", 32),
            new PartOffset("Blade_Copper", 32),
            new PartOffset("Blade_Star Metal", 32),
            new PartOffset("Blade_Obsidian", 30),
            new PartOffset("Blade_Cobalt", 29),
            new PartOffset("Blade_Granite", 32),
            new PartOffset("Blade_Lifestone", 32),
            new PartOffset("Blade_Pewter", 31),
            new PartOffset("Blade_Nickel", 29),
            new PartOffset("Blade_Gold", 24),
            new PartOffset("Blade_Moonsteel", 32),
            new PartOffset("Blade_Orichalcum", 29),
            new PartOffset("Blade_Phoenix Feather", 32),
            new PartOffset("Blade_Froststeel", 28),
            new PartOffset("Blade_Iridium", 32),
            new PartOffset("Blade_Chalk", 32),
            new PartOffset("Blade_Sunsilver", 23),
            new PartOffset("Blade_Eternal Ice", 26),
            new PartOffset("Blade_Netherite", 32),
            new PartOffset("Blade_Platinum", 32),
            new PartOffset("Blade_Twilight", 22),
            new PartOffset("Blade_Titanium", 26),
            new PartOffset("Blade_Voidstone", 30),
            new PartOffset("Blade_Arcane Crystal", 31)
        };
    }

    #endregion
}

public class Sword {
    #region Properties
    
    public Item Pommel { get; set; }
    public Item Handle { get; set; }
    public Item Guard { get; set; }
    public Item Blade { get; set; }

    #endregion
    #region Constructor

    public Sword(Item pommel, Item handle, Item guard, Item blade) {
        Pommel = pommel;
        Handle = handle;
        Guard = guard;
        Blade = blade;
    }

    #endregion
    #region PublicMethods

    public ItemQuality Quality() {
        ItemQuality averageQuality;
        averageQuality = (ItemQuality)(((int)Pommel.Quality + (int)Handle.Quality + (int)Guard.Quality + (int)Blade.Quality) / 4);
        return averageQuality;
    }

    public double Power() {
        double Rarity = 0;

        Rarity += Pommel.Rarity;
        Rarity += Handle.Rarity;
        Rarity += Guard.Rarity;
        Rarity += Blade.Rarity;

        return Rarity;
    }

    #endregion
}
