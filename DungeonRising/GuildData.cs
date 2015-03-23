﻿using System;
using C5;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonRising
{
    public class GuildClass
    {
        public string Kind;
        public int BaseRank;
        public int TalentPoints;
        public int Refresh;
        public int Gold;
        public HashDictionary<string, int> Skills;
        public HashDictionary<string, int> Defenses;
        public HashBag<Item> Items;
        public GuildClass(string kind, int baseRank, int talentPoints, int refresh, int gold, HashDictionary<string, int> skills, HashDictionary<string, int> defenses, HashBag<Item> items)
        {
            Kind = kind;
            BaseRank = baseRank;
            TalentPoints = talentPoints;
            Refresh = refresh;
            Gold = gold;
            Skills = skills;
            Defenses = defenses;
            Items = items;
        }
    }
    public class Item
    {
        public string Kind;
        public int Rank;
        public Item(string kind, int rank)
        {
            Kind = kind;
            Rank = rank;
        }
    }
    public class GuildData
{



        public static HashDictionary<string, GuildClass> AllClasses = new HashDictionary<string, GuildClass>
{
{"Abjurer", new GuildClass("Abjurer", 2, 9, 2, 2,
new HashDictionary<string, int>{{"Ward Magic", 8},
{"Dispel", 7},
{"Persistent Arcana", 5},
{"Insight", 5},
{"Vigilance", 4}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 4}
},
new HashBag<Item>{new Item ("Grimoire", 1), new Item ("Light Armor", 1), new Item ("Cloak", 1)
})},
{"Acrobat", new GuildClass("Acrobat", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Acrobatics", 10},
{"Quick Draw", 6},
{"Knife Training", 6},
{"Scan", 5},
{"Vanish", 5}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 2}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Utility Harness", 2), new Item ("Thievery Tools", 1), new Item ("Daggers", 1)
})},
{"Adept", new GuildClass("Adept", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Ritual Magic", 10},
{"Hex Magic", 10},
{"Insight", 5},
{"Herbalism", 4}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 3}
},
new HashBag<Item>{new Item ("Amulet", 2), new Item ("Totem", 1)
})},
{"Agent", new GuildClass("Agent", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Unarmed Training", 8},
{"Backstab", 8},
{"Mentalism", 6},
{"Unarmed Techniques", 6},
{"Acrobatics", 5},
{"Scan", 5}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 3}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Stealth Garb", 1), new Item ("Utility Harness", 1), new Item ("Forged Marque", 1)
})},
{"Alchemist", new GuildClass("Alchemist", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Potion Crafting", 10},
{"Bomb Training", 7},
{"Prototype Crafting", 7},
{"Poison Lore", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Brewing Kit", 2), new Item ("Bomb Satchel", 1), new Item ("Acid Satchel", 1), new Item ("Potion Satchel", 1), new Item ("Elixir", 1)
})},
{"Anarchist", new GuildClass("Anarchist", 2, 4, 1, 3,
new HashDictionary<string, int>{{"Axe Training", 10},
{"Confusion", 6},
{"Breaker", 6},
{"Taunt", 5},
{"Backstab", 3}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 2}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Greataxe", 1), new Item ("Mask", 1), new Item ("Light Armor", 1), new Item ("Banner", 1), new Item ("Handaxes", 1)
})},
{"Arborist", new GuildClass("Arborist", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Plant Bane", 8},
{"Axe Training", 7},
{"Herbalism", 7},
{"Scan", 6},
{"Repair", 6}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Utility Harness", 1), new Item ("Battleaxe", 1), new Item ("Nuts", 1), new Item ("Encyclopedia", 1), new Item ("Woodworking Kit", 1)
})},
{"Archer", new GuildClass("Archer", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Bow Training", 10},
{"Bow Techniques", 10},
{"Vigilance", 6},
{"Scan", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 4}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Bow", 2)
})},
{"Architect", new GuildClass("Architect", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Repair", 8},
{"Fortify", 7},
{"Scan", 7},
{"Bludgeon Training", 6},
{"Teach", 6}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Smithing Kit", 2), new Item ("Warhammer", 1), new Item ("Trapping Tools", 1)
})},
{"Ardent", new GuildClass("Ardent", 2, 4, 1, 3,
new HashDictionary<string, int>{{"Mentalism", 8},
{"Saber Training", 7},
{"Leadership", 6},
{"Insight", 5},
{"Telekinesis", 3}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 2}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Saber", 2), new Item ("Heavy Armor", 1), new Item ("Shield", 1), new Item ("Crystal", 1)
})},
{"Armorer", new GuildClass("Armorer", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Armor Crafting", 10},
{"Bludgeon Training", 7},
{"Repair", 7},
{"Scan", 5}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Smithing Kit", 2), new Item ("Heavy Armor", 1), new Item ("Warhammer", 1), new Item ("Shield", 1)
})},
{"Arrow Sage", new GuildClass("Arrow Sage", 2, 6, 2, 1,
new HashDictionary<string, int>{{"Tornado Weapon", 9},
{"Bow Training", 9},
{"Teach", 7},
{"Scan", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Bow", 1), new Item ("Ring", 1)
})},
{"Artificer", new GuildClass("Artificer", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Infuse Armor", 7},
{"Earthshaking Weapon", 7},
{"Repair", 6},
{"Bludgeon Training", 6},
{"Artifact Crafting", 6},
{"Armor Crafting", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Heavy Armor", 2), new Item ("Maul", 1), new Item ("Amulet", 1), new Item ("Smithing Kit", 1)
})},
{"Ascetic", new GuildClass("Ascetic", 3, 4, 1, 1,
new HashDictionary<string, int>{{"Insight", 10},
{"Confusion", 7},
{"Cleanse", 6},
{"Herbalism", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 3}
},
new HashBag<Item>{new Item ("Pure Robes", 2)
})},
{"Assassin", new GuildClass("Assassin", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Backstab", 9},
{"Knife Training", 7},
{"Vanish", 7},
{"Poison Lore", 7}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Daggers", 1), new Item ("Stealth Garb", 1), new Item ("Utility Harness", 1)
})},
{"Astrologer", new GuildClass("Astrologer", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Cosmic Magic", 10},
{"Fortune Magic", 8},
{"Insight", 7},
{"Persistent Arcana", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Orb", 2), new Item ("Ring", 1)
})},
{"Atomic Mage", new GuildClass("Atomic Mage", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Atomic Magic", 10},
{"Defile", 10},
{"Dispel", 5},
{"Potent Arcana", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Wand", 2), new Item ("Cloak", 1), new Item ("Foul Robes", 1)
})},
{"Avenger", new GuildClass("Avenger", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Counterattack", 8},
{"Knife Training", 8},
{"Cleanse", 6},
{"Vanish", 6},
{"Vigilance", 5}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 3}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Daggers", 1), new Item ("Light Armor", 1), new Item ("Stealth Garb", 1)
})},
{"Baffler", new GuildClass("Baffler", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Whip Training", 10},
{"Whip Techniques", 9},
{"Confusion", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Cape", 2), new Item ("Stealth Garb", 1)
})},
{"Baker", new GuildClass("Baker", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Food Crafting", 10},
{"Ooze Bane", 7},
{"Bludgeon Training", 7},
{"Herbalism", 4}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Cooking Kit", 2), new Item ("Club", 1), new Item ("Breads", 1), new Item ("Cakes", 1)
})},
{"Balancer", new GuildClass("Balancer", 2, 4, 1, 3,
new HashDictionary<string, int>{{"Axe Training", 9},
{"Counterattack", 7},
{"Leadership", 7},
{"Insight", 4}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 2}, {"Spell", 4}
},
new HashBag<Item>{new Item ("Heavy Armor", 1), new Item ("Battleaxe", 1), new Item ("Shield", 1)
})},
{"Barbarian", new GuildClass("Barbarian", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Axe Training", 10},
{"Fury", 9},
{"Breaker", 9},
{"Charge", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Greataxe", 1)
})},
{"Bard", new GuildClass("Bard", 4, 5, 2, 2,
new HashDictionary<string, int>{{"Sound Magic", 8},
{"Charm Magic", 8},
{"Heal Magic", 6},
{"Taunt", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Talisman", 1), new Item ("Light Armor", 1), new Item ("Ring", 1)
})},
{"Bartender", new GuildClass("Bartender", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Scan", 8},
{"Potion Crafting", 8},
{"Persuasion", 7},
{"Vigilance", 6},
{"Unarmed Training", 5}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Beer", 1), new Item ("Brewing Kit", 1), new Item ("Potion Satchel", 1), new Item ("Hard Liquor", 1), new Item ("Wine", 1)
})},
{"Beguiler", new GuildClass("Beguiler", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Charm Magic", 10},
{"Persuasion", 9},
{"Linguistics", 6},
{"Confusion", 4}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Amulet", 2), new Item ("Ring", 1), new Item ("Cloak", 1), new Item ("Stealth Garb", 1)
})},
{"Berserker", new GuildClass("Berserker", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Wild Morph", 10},
{"Fury", 9},
{"Unarmed Training", 9},
{"Roar", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Meats", 1)
})},
{"Binder", new GuildClass("Binder", 1, 6, 3, 1,
new HashDictionary<string, int>{{"Vile Morph", 10},
{"Cosmic Magic", 10},
{"Defile", 6},
{"Celestial Bane", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Grimoire", 1), new Item ("Foul Robes", 1)
})},
{"Bladesmith", new GuildClass("Bladesmith", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Weapon Crafting", 10},
{"Blade Training", 9},
{"Repair", 8}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Smithing Kit", 2), new Item ("Katana", 2), new Item ("Longsword", 1), new Item ("Chakrams", 1)
})},
{"Blighter", new GuildClass("Blighter", 1, 6, 3, 1,
new HashDictionary<string, int>{{"Defile", 10},
{"Anathema Magic", 10},
{"Poison Lore", 6},
{"Potent Arcana", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Foul Robes", 1), new Item ("Talisman", 1), new Item ("Trapping Tools", 1)
})},
{"Blood Mage", new GuildClass("Blood Mage", 2, 6, 2, 1,
new HashDictionary<string, int>{{"Pain Surge", 10},
{"Regenerate", 8},
{"Potion Crafting", 6},
{"Anathema Magic", 4},
{"Hex Magic", 4}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Potion Satchel", 1), new Item ("Wand", 1), new Item ("Elixir", 1)
})},
{"Bodyguard", new GuildClass("Bodyguard", 2, 4, 1, 3,
new HashDictionary<string, int>{{"Vigilance", 10},
{"Unarmed Training", 7},
{"Scan", 6},
{"Counterattack", 4}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 2}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Suit", 2), new Item ("Elixir", 1)
})},
{"Bomber", new GuildClass("Bomber", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Bomb Training", 10},
{"Breaker", 8},
{"Prototype Crafting", 8},
{"Scan", 7}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Bomb Satchel", 1), new Item ("Brewing Kit", 1), new Item ("Trapping Tools", 1)
})},
{"Bone Mage", new GuildClass("Bone Mage", 3, 4, 1, 1,
new HashDictionary<string, int>{{"Pain Surge", 10},
{"Unarmed Training", 6},
{"Infuse Armor", 6},
{"Malevolence", 5},
{"Counterattack", 5}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Heavy Armor", 2), new Item ("Potion Satchel", 1), new Item ("Armor Spikes", 1)
})},
{"Bowyer", new GuildClass("Bowyer", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Weapon Crafting", 10},
{"Bow Training", 7},
{"Scan", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 3}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Woodworking Kit", 2), new Item ("Bow", 2), new Item ("Crossbow", 1)
})},
{"Brahmin", new GuildClass("Brahmin", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Teach", 8},
{"Insight", 8},
{"Persuasion", 7},
{"Herbalism", 6},
{"Linguistics", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Encyclopedia", 2), new Item ("Holy Water", 1)
})},
{"Brewer", new GuildClass("Brewer", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Potion Crafting", 10},
{"Cleanse", 7},
{"Herbalism", 7},
{"Poison Lore", 5}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Brewing Kit", 2), new Item ("Beer", 1), new Item ("Potion Satchel", 1), new Item ("Hard Liquor", 1)
})},
{"Brute", new GuildClass("Brute", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Unarmed Training", 10},
{"Breaker", 9},
{"Fury", 9},
{"Roar", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Beer", 1)
})},
{"Butcher", new GuildClass("Butcher", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Food Crafting", 10},
{"Knife Training", 9},
{"Animal Bane", 6}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Meats", 2), new Item ("Cooking Kit", 1), new Item ("Carving Knife", 1), new Item ("Oils", 1)
})},
{"Cackling Witch", new GuildClass("Cackling Witch", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Hex Magic", 9},
{"Sound Magic", 9},
{"Anathema Magic", 6},
{"Roar", 6},
{"Defile", 4}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Grimoire", 2), new Item ("Foul Robes", 1)
})},
{"Cannoneer", new GuildClass("Cannoneer", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Gun Training", 10},
{"Gun Techniques", 10},
{"Scan", 6},
{"Breaker", 5}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 4}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Cannon", 2), new Item ("Light Armor", 1), new Item ("Beer", 1)
})},
{"Cantor", new GuildClass("Cantor", 3, 6, 3, 1,
new HashDictionary<string, int>{{"Sound Magic", 8},
{"Heal Magic", 7},
{"Ward Magic", 6},
{"Cleanse", 5},
{"Insight", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Talisman", 1), new Item ("Pure Robes", 1), new Item ("Holy Water", 1)
})},
{"Cavalier", new GuildClass("Cavalier", 2, 4, 0, 3,
new HashDictionary<string, int>{{"Spear Training", 10},
{"Leadership", 7},
{"Charge", 7}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 4}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Pike", 2), new Item ("Heavy Armor", 2), new Item ("Banner", 1)
})},
{"Chaser", new GuildClass("Chaser", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Charge", 10},
{"Spear Training", 7},
{"Scan", 7},
{"Vigilance", 5},
{"Animal Bane", 5}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Javelins", 1), new Item ("Totem", 1)
})},
{"Cheesemaker", new GuildClass("Cheesemaker", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Food Crafting", 10},
{"Cleanse", 7},
{"Knife Training", 7},
{"Herbalism", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Cooking Kit", 2), new Item ("Tiny Knife", 1), new Item ("Cheeses", 1), new Item ("Breads", 1)
})},
{"Chronomancer", new GuildClass("Chronomancer", 1, 6, 3, 1,
new HashDictionary<string, int>{{"Time Magic", 10},
{"Cosmic Magic", 10},
{"Vigilance", 6},
{"Persistent Arcana", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Talisman", 1), new Item ("Elixir", 1)
})},
{"Churchless Cleric", new GuildClass("Churchless Cleric", 3, 6, 3, 1,
new HashDictionary<string, int>{{"Heal Magic", 8},
{"Light Magic", 7},
{"Insight", 6},
{"Vigilance", 5},
{"Cleanse", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Staff", 1), new Item ("Pure Robes", 1)
})},
{"Confectioner", new GuildClass("Confectioner", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Food Crafting", 10},
{"Hasten", 7},
{"Adhere", 7},
{"Bludgeon Training", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Cooking Kit", 2), new Item ("Club", 1), new Item ("Cakes", 1), new Item ("Candies", 1)
})},
{"Conjurer", new GuildClass("Conjurer", 2, 6, 2, 1,
new HashDictionary<string, int>{{"Earth Magic", 8},
{"Water Magic", 8},
{"Cosmic Magic", 8},
{"Ritual Magic", 5},
{"Persistent Arcana", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Amulet", 1), new Item ("Ring", 1)
})},
{"Crossbowman", new GuildClass("Crossbowman", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Bow Training", 10},
{"Quick Draw", 9},
{"Vigilance", 8},
{"Scan", 7}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Crossbow", 2)
})},
{"Daggermaster", new GuildClass("Daggermaster", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Knife Training", 10},
{"Knife Techniques", 10},
{"Quick Draw", 6},
{"Acrobatics", 5}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 3}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Daggers", 2), new Item ("Stealth Garb", 1), new Item ("Light Armor", 1)
})},
{"Dark Mage", new GuildClass("Dark Mage", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Dark Magic", 10},
{"Anathema Magic", 9},
{"Vanish", 6},
{"Potent Arcana", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Wand", 2), new Item ("Stealth Garb", 1)
})},
{"Dawnblade", new GuildClass("Dawnblade", 3, 6, 1, 1,
new HashDictionary<string, int>{{"Saber Training", 10},
{"Shining Weapon", 7},
{"Saber Techniques", 6},
{"Counterattack", 5}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 2}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Saber", 2), new Item ("Crystal", 1)
})},
{"Demonwearer", new GuildClass("Demonwearer", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Whip Training", 10},
{"Counterattack", 10},
{"Terrify", 6},
{"Whip Techniques", 5}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Cape", 2), new Item ("Mask", 1), new Item ("Meats", 1)
})},
{"Diplomat", new GuildClass("Diplomat", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Persuasion", 8},
{"Leadership", 7},
{"Teach", 6},
{"Linguistics", 6},
{"Vigilance", 6},
{"Confusion", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Official Marque", 2), new Item ("Suit", 1), new Item ("Encyclopedia", 1)
})},
{"Disciple", new GuildClass("Disciple", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Saber Training", 8},
{"Telekinesis", 8},
{"Acrobatics", 6},
{"Insight", 6},
{"Saber Techniques", 5},
{"Counterattack", 5}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Saber", 2), new Item ("Cloak", 1)
})},
{"Diviner", new GuildClass("Diviner", 2, 6, 2, 1,
new HashDictionary<string, int>{{"Time Magic", 8},
{"Insight", 8},
{"Gun Training", 8},
{"Scan", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Cannon", 2), new Item ("Orb", 1)
})},
{"Doctor", new GuildClass("Doctor", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Potion Crafting", 8},
{"Herbalism", 8},
{"Poison Lore", 7},
{"Scan", 6},
{"Knife Training", 5}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Potion Satchel", 1), new Item ("Tiny Knife", 1), new Item ("Brewing Kit", 1), new Item ("Elixir", 1), new Item ("Hard Liquor", 1)
})},
{"Doomsayer", new GuildClass("Doomsayer", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Malevolence", 10},
{"Terrify", 10},
{"Vigilance", 6},
{"Insight", 6}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 1}, {"Spell", 3}
},
new HashBag<Item>{new Item ("Banner", 1), new Item ("Crystal", 1), new Item ("Hat", 1)
})},
{"Dracomancer", new GuildClass("Dracomancer", 1, 6, 3, 1,
new HashDictionary<string, int>{{"Bludgeon Training", 10},
{"Fire Magic", 10},
{"Charm Magic", 6},
{"Leadership", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Light Armor", 1), new Item ("Staff", 1)
})},
{"Dressmaker", new GuildClass("Dressmaker", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Garment Crafting", 10},
{"Scan", 8},
{"Knife Training", 7}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Clothworking Kit", 2), new Item ("Tiny Knife", 1), new Item ("Elegant Garb", 1), new Item ("Pure Robes", 1)
})},
{"Druid", new GuildClass("Druid", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Wild Morph", 10},
{"Cleanse", 8},
{"Herbalism", 7},
{"Roar", 5}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 0}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Cloak", 1), new Item ("Totem", 1)
})},
{"Duskblade", new GuildClass("Duskblade", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Saber Training", 8},
{"Charge", 7},
{"Counterattack", 6},
{"Saber Techniques", 6},
{"Acrobatics", 5},
{"Vigilance", 5}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 3}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Saber", 2), new Item ("Light Armor", 1), new Item ("Cloak", 1)
})},
{"Earth Mage", new GuildClass("Earth Mage", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Earth Magic", 10},
{"Fortify", 9},
{"Repair", 6},
{"Breaker", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Wand", 2), new Item ("Heavy Armor", 1)
})},
{"Elite Trooper", new GuildClass("Elite Trooper", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Unarmed Training", 10},
{"General Training", 10},
{"Unarmed Techniques", 6},
{"Vigilance", 6}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 3}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Maul", 1), new Item ("Heavy Armor", 1), new Item ("Crossbow", 1)
})},
{"Empath", new GuildClass("Empath", 1, 6, 3, 1,
new HashDictionary<string, int>{{"Charm Magic", 10},
{"Insight", 10},
{"Telekinesis", 6},
{"Mentalism", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Amulet", 1), new Item ("Crystal", 1)
})},
{"Eternalist", new GuildClass("Eternalist", 3, 6, 3, 1,
new HashDictionary<string, int>{{"Cosmic Magic", 7},
{"Heal Magic", 7},
{"Potion Crafting", 6},
{"Vigilance", 6},
{"Teach", 4}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Amulet", 1), new Item ("Light Armor", 1), new Item ("Brewing Kit", 1), new Item ("Potion Satchel", 1), new Item ("Shield", 1)
})},
{"Evangelist", new GuildClass("Evangelist", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Leadership", 8},
{"Persuasion", 7},
{"Insight", 6},
{"Confusion", 6},
{"Teach", 6},
{"Benevolence", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Holy Water", 2), new Item ("Breads", 1)
})},
{"Evoker", new GuildClass("Evoker", 1, 6, 3, 1,
new HashDictionary<string, int>{{"Fire Magic", 10},
{"Storm Magic", 10},
{"Precise Arcana", 6},
{"Potent Arcana", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Wand", 1), new Item ("Light Armor", 1)
})},
{"Exemplar", new GuildClass("Exemplar", 2, 6, 2, 1,
new HashDictionary<string, int>{{"Eldritch Rift", 10},
{"Unarmed Training", 10},
{"Mentalism", 7},
{"Acrobatics", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Crystal", 1), new Item ("Crown", 1)
})},
{"Exorcist", new GuildClass("Exorcist", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Binding Seal", 8},
{"Radiance", 8},
{"Cleanse", 8},
{"Insight", 5},
{"Fiend Bane", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Holy Water", 1), new Item ("Pure Robes", 1), new Item ("Crystal", 1)
})},
{"Fencer", new GuildClass("Fencer", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Saber Training", 10},
{"Taunt", 8},
{"Counterattack", 7},
{"Saber Techniques", 5}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Saber", 1), new Item ("Elegant Garb", 1), new Item ("Forged Marque", 1)
})},
{"Feytouched", new GuildClass("Feytouched", 3, 6, 3, 1,
new HashDictionary<string, int>{{"Charm Magic", 7},
{"Fortune Magic", 7},
{"Cleanse", 6},
{"Acrobatics", 6},
{"Confusion", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Wand", 1), new Item ("Pure Robes", 1)
})},
{"Fire Mage", new GuildClass("Fire Mage", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Fire Magic", 10},
{"Potent Arcana", 8},
{"Breaker", 7},
{"Plant Bane", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Wand", 2), new Item ("Ring", 1), new Item ("Oils", 1)
})},
{"Fisherman", new GuildClass("Fisherman", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Whip Training", 8},
{"Vigilance", 7},
{"Food Crafting", 7},
{"Scan", 6},
{"Animal Bane", 5}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Net", 1), new Item ("Fish", 1), new Item ("Cooking Kit", 1), new Item ("Beer", 1), new Item ("Trapping Tools", 1)
})},
{"Fishmonger", new GuildClass("Fishmonger", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Taunt", 10},
{"Food Crafting", 8},
{"Knife Training", 7}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Fish", 2), new Item ("Carving Knife", 1), new Item ("Cooking Kit", 1), new Item ("Banner", 1)
})},
{"Fist Fighter", new GuildClass("Fist Fighter", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Unarmed Training", 10},
{"Counterattack", 10},
{"Unarmed Techniques", 6},
{"Charge", 6}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 3}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Light Armor", 1), new Item ("Hard Liquor", 1)
})},
{"Gadgeteer", new GuildClass("Gadgeteer", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Gun Training", 8},
{"Prototype Crafting", 8},
{"Scan", 6},
{"Quick Draw", 6},
{"Repair", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Musket", 2), new Item ("Utility Harness", 1), new Item ("Trapping Tools", 1), new Item ("Smithing Kit", 1), new Item ("Thievery Tools", 1)
})},
{"Gambler", new GuildClass("Gambler", 1, 6, 3, 1,
new HashDictionary<string, int>{{"Fortune Magic", 10},
{"Insight", 10},
{"Taunt", 6},
{"Persistent Arcana", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Talisman", 1), new Item ("Forged Marque", 1)
})},
{"Gatekeeper", new GuildClass("Gatekeeper", 2, 9, 2, 2,
new HashDictionary<string, int>{{"Binding Seal", 8},
{"Ritual Magic", 7},
{"Insight", 7},
{"Persistent Arcana", 4}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 4}
},
new HashBag<Item>{new Item ("Staff", 1), new Item ("Heavy Armor", 1), new Item ("Nuts", 1)
})},
{"Gatherer", new GuildClass("Gatherer", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Herbalism", 10},
{"Scan", 8},
{"Food Crafting", 8},
{"Vigilance", 7}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Cooking Kit", 1), new Item ("Totem", 1), new Item ("Nuts", 1)
})},
{"Geometer", new GuildClass("Geometer", 2, 6, 2, 1,
new HashDictionary<string, int>{{"Insight", 8},
{"Ritual Magic", 8},
{"Cleanse", 8},
{"Persistent Arcana", 6},
{"Scan", 4}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Staff", 1), new Item ("Pure Robes", 1)
})},
{"Gladiator", new GuildClass("Gladiator", 1, 6, 0, 1,
new HashDictionary<string, int>{{"General Training", 10},
{"Counterattack", 10},
{"Charge", 6},
{"Taunt", 6}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Spear", 1), new Item ("Net", 1), new Item ("Heavy Armor", 1)
})},
{"Haberdasher", new GuildClass("Haberdasher", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Garment Crafting", 10},
{"Leadership", 7},
{"Persuasion", 6},
{"Knife Training", 4},
{"Scan", 4},
{"Insight", 4}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Clothworking Kit", 2), new Item ("Hat", 1), new Item ("Crown", 1), new Item ("Tiny Knife", 1)
})},
{"Harlequin", new GuildClass("Harlequin", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Terrify", 10},
{"Bomb Training", 10},
{"Confusion", 6},
{"Taunt", 6}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 3}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Bomb Satchel", 1), new Item ("Acid Satchel", 1), new Item ("Cakes", 1)
})},
{"Havoc Mage", new GuildClass("Havoc Mage", 2, 6, 2, 1,
new HashDictionary<string, int>{{"Anathema Magic", 8},
{"Whip Training", 8},
{"Fire Magic", 7},
{"Potent Arcana", 5},
{"Whip Techniques", 4}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Flail", 1), new Item ("Amulet", 1), new Item ("Light Armor", 1), new Item ("Cape", 1)
})},
{"Hedge Mage", new GuildClass("Hedge Mage", 2, 6, 2, 1,
new HashDictionary<string, int>{{"Insight", 8},
{"Potion Crafting", 8},
{"Herbalism", 8},
{"Ritual Magic", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Staff", 1), new Item ("Brewing Kit", 1), new Item ("Potion Satchel", 1)
})},
{"Herder", new GuildClass("Herder", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Vigilance", 10},
{"Scan", 8},
{"Bludgeon Training", 8},
{"Leadership", 7}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Pole", 1), new Item ("Totem", 1)
})},
{"Hive Caller", new GuildClass("Hive Caller", 1, 6, 3, 1,
new HashDictionary<string, int>{{"Poison Lore", 10},
{"Wind Magic", 10},
{"Herbalism", 6},
{"Precise Arcana", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Light Armor", 1), new Item ("Staff", 1)
})},
{"Holy Cleric", new GuildClass("Holy Cleric", 3, 6, 3, 1,
new HashDictionary<string, int>{{"Heal Magic", 7},
{"Cleanse", 7},
{"Insight", 6},
{"Leadership", 6},
{"Persuasion", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Staff", 1)
})},
{"Hooligan", new GuildClass("Hooligan", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Backstab", 9},
{"Bludgeon Training", 8},
{"Taunt", 6},
{"Breaker", 6}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Beer", 1), new Item ("Club", 1), new Item ("Stealth Garb", 1), new Item ("Thievery Tools", 1)
})},
{"Hoplite", new GuildClass("Hoplite", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Spear Training", 10},
{"Vigilance", 10},
{"Counterattack", 6},
{"Charge", 6}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Spear", 1), new Item ("Shield", 1), new Item ("Heavy Armor", 1)
})},
{"Hunter", new GuildClass("Hunter", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Bow Training", 10},
{"Scan", 8},
{"Backstab", 7},
{"Animal Bane", 6},
{"Herbalism", 3},
{"Food Crafting", 3}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Bow", 1), new Item ("Totem", 1), new Item ("Stealth Garb", 1)
})},
{"Hurler", new GuildClass("Hurler", 2, 5, 1, 2,
new HashDictionary<string, int>{{"General Training", 10},
{"Quick Draw", 8},
{"Scan", 7},
{"Vigilance", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 3}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Handaxes", 1), new Item ("Light Armor", 1), new Item ("Utility Harness", 1)
})},
{"Ice Mage", new GuildClass("Ice Mage", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Ice Magic", 10},
{"Precise Arcana", 8},
{"Vigilance", 7},
{"Insight", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Wand", 2), new Item ("Light Armor", 1), new Item ("Cloak", 1)
})},
{"Ice Stalker", new GuildClass("Ice Stalker", 2, 6, 2, 1,
new HashDictionary<string, int>{{"Ice Magic", 8},
{"Spear Training", 8},
{"Vanish", 8},
{"Backstab", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Javelins", 1), new Item ("Orb", 1), new Item ("Cloak", 1), new Item ("Light Armor", 1), new Item ("Trapping Tools", 1)
})},
{"Illusionist", new GuildClass("Illusionist", 3, 6, 3, 1,
new HashDictionary<string, int>{{"Light Magic", 7},
{"Dark Magic", 7},
{"Charm Magic", 7},
{"Insight", 6},
{"Persistent Arcana", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Orb", 1), new Item ("Elegant Garb", 1)
})},
{"Impaler", new GuildClass("Impaler", 3, 4, 1, 1,
new HashDictionary<string, int>{{"Spear Training", 10},
{"Spear Techniques", 7},
{"Counterattack", 7},
{"Charge", 4}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Pike", 2), new Item ("Heavy Armor", 1)
})},
{"Inquisitor", new GuildClass("Inquisitor", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Bow Training", 8},
{"Sacred Weapon", 8},
{"Insight", 7},
{"Vigilance", 6},
{"Fiend Bane", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Crossbow", 1), new Item ("Light Armor", 1), new Item ("Cloak", 1), new Item ("Utility Harness", 1)
})},
{"Jeweler", new GuildClass("Jeweler", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Artifact Crafting", 10},
{"Repair", 7},
{"Persuasion", 7},
{"Insight", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Smithing Kit", 2), new Item ("Crown", 1), new Item ("Amulet", 1), new Item ("Thievery Tools", 1), new Item ("Crystal", 1)
})},
{"Juggernaut", new GuildClass("Juggernaut", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Bow Training", 10},
{"Unarmed Training", 10},
{"Counterattack", 6},
{"Vigilance", 5}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 4}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Crossbow", 1), new Item ("Heavy Armor", 1), new Item ("Armor Spikes", 1)
})},
{"Justiciar", new GuildClass("Justiciar", 2, 4, 1, 3,
new HashDictionary<string, int>{{"Bludgeon Training", 10},
{"Vigilance", 7},
{"Counterattack", 7}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 2}, {"Spell", 3}
},
new HashBag<Item>{new Item ("Heavy Armor", 1), new Item ("Shield", 1), new Item ("Warhammer", 1), new Item ("Official Marque", 1)
})},
{"Kensei", new GuildClass("Kensei", 3, 4, 1, 1,
new HashDictionary<string, int>{{"Blade Training", 10},
{"Blade Techniques", 7},
{"Quick Draw", 6},
{"Counterattack", 6}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Katana", 2)
})},
{"Khan", new GuildClass("Khan", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Bow Training", 9},
{"Leadership", 8},
{"Vigilance", 6},
{"Bow Techniques", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 4}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Bow", 1), new Item ("Light Armor", 1), new Item ("Meats", 1), new Item ("Cloak", 1)
})},
{"Kineticist", new GuildClass("Kineticist", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Telekinesis", 10},
{"Precise Arcana", 10},
{"Mentalism", 6},
{"Insight", 6}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 4}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Utility Harness", 1), new Item ("Thievery Tools", 1), new Item ("Crystal", 1)
})},
{"Lasher", new GuildClass("Lasher", 3, 4, 1, 1,
new HashDictionary<string, int>{{"Whip Training", 10},
{"Whip Techniques", 7},
{"Taunt", 6},
{"Backstab", 5}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 3}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Whip", 2), new Item ("Light Armor", 1), new Item ("Official Marque", 1)
})},
{"Liberator", new GuildClass("Liberator", 2, 4, 1, 3,
new HashDictionary<string, int>{{"Blade Training", 10},
{"Insight", 7},
{"Persuasion", 7}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 3}, {"Spell", 3}
},
new HashBag<Item>{new Item ("Broadsword", 1), new Item ("Light Armor", 1), new Item ("Thievery Tools", 1)
})},
{"Light Mage", new GuildClass("Light Mage", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Light Magic", 10},
{"Scan", 10},
{"Precise Arcana", 5},
{"Insight", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Wand", 2), new Item ("Elegant Garb", 1), new Item ("Ring", 1)
})},
{"Lorekeeper", new GuildClass("Lorekeeper", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Teach", 9},
{"Linguistics", 7},
{"Insight", 6},
{"Persuasion", 6},
{"Herbalism", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Trapping Tools", 1), new Item ("Cloak", 1), new Item ("Encyclopedia", 1), new Item ("Light Armor", 1)
})},
{"Lurker", new GuildClass("Lurker", 3, 4, 1, 1,
new HashDictionary<string, int>{{"Vanish", 10},
{"Knife Training", 7},
{"Mentalism", 6},
{"Backstab", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 3}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Stealth Garb", 2), new Item ("Daggers", 1), new Item ("Thievery Tools", 1)
})},
{"Magekiller", new GuildClass("Magekiller", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Binding Seal", 10},
{"Knife Training", 10},
{"Insight", 6},
{"Dispel", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 4}
},
new HashBag<Item>{new Item ("Daggers", 1), new Item ("Stealth Garb", 1), new Item ("Trapping Tools", 1)
})},
{"Maharaja", new GuildClass("Maharaja", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Leadership", 9},
{"Persuasion", 7},
{"Insight", 6},
{"Cleanse", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 3}
},
new HashBag<Item>{new Item ("Elegant Garb", 2), new Item ("Pure Robes", 1), new Item ("Potion Satchel", 1), new Item ("Official Marque", 1), new Item ("Holy Water", 1)
})},
{"Marauder", new GuildClass("Marauder", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Axe Training", 10},
{"Quick Draw", 8},
{"Charge", 8},
{"Axe Techniques", 7}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Handaxes", 1), new Item ("Light Armor", 1)
})},
{"Marksman", new GuildClass("Marksman", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Bow Training", 10},
{"Bow Techniques", 10},
{"Quick Draw", 6},
{"Scan", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 4}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Paired Crossbows", 2), new Item ("Light Armor", 1), new Item ("Utility Harness", 1)
})},
{"Martial Savant", new GuildClass("Martial Savant", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Unarmed Training", 10},
{"Unarmed Techniques", 10},
{"Counterattack", 6},
{"Acrobatics", 5}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 4}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Cloak", 2)
})},
{"Mask Maker", new GuildClass("Mask Maker", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Artifact Crafting", 10},
{"Terrify", 8},
{"Insight", 6},
{"Persuasion", 4}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 3}
},
new HashBag<Item>{new Item ("Mask", 2), new Item ("Totem", 1), new Item ("Woodworking Kit", 1), new Item ("Shield", 1)
})},
{"Mastermind", new GuildClass("Mastermind", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Teach", 8},
{"Insight", 8},
{"Taunt", 6},
{"Persuasion", 6},
{"Linguistics", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Trapping Tools", 1), new Item ("Forged Marque", 1), new Item ("Thievery Tools", 1), new Item ("Utility Harness", 1), new Item ("Encyclopedia", 1)
})},
{"Merchant", new GuildClass("Merchant", 4, 6, 1, 2,
new HashDictionary<string, int>{{"Persuasion", 8},
{"Vigilance", 8},
{"Scan", 6},
{"Insight", 6},
{"Repair", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Banner", 2), new Item ("Net", 1), new Item ("Utility Harness", 1), new Item ("Broadsword", 1)
})},
{"Metal Mage", new GuildClass("Metal Mage", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Metal Magic", 10},
{"Repair", 8},
{"Precise Arcana", 7}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Wand", 2), new Item ("Heavy Armor", 1), new Item ("Smithing Kit", 1)
})},
{"Metamancer", new GuildClass("Metamancer", 3, 6, 3, 1,
new HashDictionary<string, int>{{"Cosmic Magic", 7},
{"Ritual Magic", 7},
{"Precise Arcana", 6},
{"Persistent Arcana", 6},
{"Potent Arcana", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Grimoire", 1), new Item ("Ring", 1)
})},
{"Mindblade", new GuildClass("Mindblade", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Mentalism", 8},
{"Knife Training", 8},
{"Telekinesis", 8},
{"Backstab", 5},
{"Insight", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Daggers", 1), new Item ("Stealth Garb", 1), new Item ("Crystal", 1)
})},
{"Mirror Mage", new GuildClass("Mirror Mage", 2, 9, 2, 2,
new HashDictionary<string, int>{{"Ward Magic", 7},
{"Light Magic", 7},
{"Hex Magic", 7},
{"Precise Arcana", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 2}, {"Spell", 4}
},
new HashBag<Item>{new Item ("Talisman", 1), new Item ("Heavy Armor", 1), new Item ("Shield", 1)
})},
{"Mob Boss", new GuildClass("Mob Boss", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Persuasion", 8},
{"Bludgeon Training", 8},
{"Vigilance", 6},
{"Backstab", 6},
{"Scan", 6}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 2}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Suit", 1), new Item ("Forged Marque", 1), new Item ("Club", 1)
})},
{"Monk", new GuildClass("Monk", 3, 4, 1, 1,
new HashDictionary<string, int>{{"Unarmed Training", 10},
{"Unarmed Techniques", 7},
{"Counterattack", 6},
{"Acrobatics", 6}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 2}, {"Spell", 2}
},
new HashBag<Item>{
})},
{"Mugger", new GuildClass("Mugger", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Knife Training", 7},
{"Backstab", 7},
{"Persuasion", 7},
{"Scan", 7}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 4}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Daggers", 2), new Item ("Stealth Garb", 1), new Item ("Light Armor", 1)
})},
{"Musketeer", new GuildClass("Musketeer", 3, 4, 1, 1,
new HashDictionary<string, int>{{"Gun Training", 10},
{"Scan", 8},
{"Repair", 6},
{"Gun Techniques", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 4}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Musket", 2), new Item ("Light Armor", 1), new Item ("Utility Harness", 1), new Item ("Smithing Kit", 1)
})},
{"Mutant", new GuildClass("Mutant", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Atomic Blast", 10},
{"Mentalism", 8},
{"Potent Arcana", 8},
{"Vigilance", 8}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Crystal", 1)
})},
{"Mystic", new GuildClass("Mystic", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Unarmed Training", 8},
{"Unarmed Techniques", 7},
{"Insight", 7},
{"Fiery Weapon", 6},
{"Cleanse", 6}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 0}, {"Spell", 4}
},
new HashBag<Item>{
})},
{"Natural Cleric", new GuildClass("Natural Cleric", 3, 6, 3, 1,
new HashDictionary<string, int>{{"Heal Magic", 7},
{"Bludgeon Training", 7},
{"Insight", 6},
{"Herbalism", 5},
{"Cleanse", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Staff", 1), new Item ("Cloak", 1), new Item ("Light Armor", 1)
})},
{"Necromancer", new GuildClass("Necromancer", 1, 6, 3, 1,
new HashDictionary<string, int>{{"Anathema Magic", 10},
{"Dark Magic", 10},
{"Potent Arcana", 6},
{"Defile", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Grimoire", 1), new Item ("Foul Robes", 1)
})},
{"Nihang", new GuildClass("Nihang", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Blade Training", 9},
{"Quick Draw", 8},
{"Vigilance", 8},
{"Insight", 4},
{"Blade Techniques", 4}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Chakrams", 1), new Item ("Shortsword", 1), new Item ("Light Armor", 1)
})},
{"Ninja", new GuildClass("Ninja", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Knife Training", 7},
{"Vanish", 7},
{"Backstab", 7},
{"Acrobatics", 7}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 2}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Daggers", 1), new Item ("Shortsword", 1), new Item ("Stealth Garb", 1), new Item ("Utility Harness", 1), new Item ("Thievery Tools", 1)
})},
{"Noble", new GuildClass("Noble", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Leadership", 10},
{"Persuasion", 8},
{"Linguistics", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Crown", 2), new Item ("Official Marque", 2), new Item ("Encyclopedia", 1), new Item ("Elegant Garb", 1)
})},
{"Officer", new GuildClass("Officer", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Leadership", 8},
{"Saber Training", 8},
{"Vigilance", 7},
{"Persuasion", 6}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 2}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Saber", 1), new Item ("Light Armor", 1), new Item ("Banner", 1), new Item ("Official Marque", 1)
})},
{"Orthodox Cleric", new GuildClass("Orthodox Cleric", 2, 9, 2, 2,
new HashDictionary<string, int>{{"Ritual Magic", 7},
{"Heal Magic", 6},
{"Leadership", 6},
{"Cleanse", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 4}
},
new HashBag<Item>{new Item ("Staff", 2), new Item ("Heavy Armor", 1), new Item ("Holy Water", 1), new Item ("Potion Satchel", 1)
})},
{"Paladin", new GuildClass("Paladin", 2, 4, 1, 3,
new HashDictionary<string, int>{{"Blade Training", 10},
{"Sacred Weapon", 7},
{"Heal Magic", 7}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 2}, {"Spell", 3}
},
new HashBag<Item>{new Item ("Broadsword", 1), new Item ("Heavy Armor", 1), new Item ("Amulet", 1)
})},
{"Pantheon Acolyte", new GuildClass("Pantheon Acolyte", 3, 6, 3, 1,
new HashDictionary<string, int>{{"Ritual Magic", 8},
{"Cleanse", 6},
{"Radiance", 6},
{"Precise Arcana", 6},
{"Insight", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Pure Robes", 1), new Item ("Amulet", 1)
})},
{"Phantom", new GuildClass("Phantom", 2, 6, 2, 1,
new HashDictionary<string, int>{{"Dark Magic", 9},
{"Knife Training", 9},
{"Vanish", 7},
{"Backstab", 4},
{"Scan", 4}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Stealth Garb", 1), new Item ("Carving Knife", 1)
})},
{"Philosopher", new GuildClass("Philosopher", 4, 5, 1, 2,
new HashDictionary<string, int>{{"Teach", 7},
{"Persuasion", 7},
{"Insight", 7},
{"Confusion", 7},
{"Linguistics", 6}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 4}
},
new HashBag<Item>{new Item ("Encyclopedia", 2), new Item ("Wine", 1)
})},
{"Pirate", new GuildClass("Pirate", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Saber Training", 8},
{"Bomb Training", 8},
{"Terrify", 7},
{"Backstab", 6}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Saber", 1), new Item ("Bomb Satchel", 1), new Item ("Light Armor", 1), new Item ("Hard Liquor", 1)
})},
{"Pistoleer", new GuildClass("Pistoleer", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Gun Training", 10},
{"Quick Draw", 10},
{"Gun Techniques", 6},
{"Scan", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 4}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Pistol", 2), new Item ("Utility Harness", 1), new Item ("Cloak", 1)
})},
{"Plains Ravager", new GuildClass("Plains Ravager", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Axe Training", 8},
{"Quick Draw", 8},
{"Terrify", 8},
{"Scan", 5},
{"Vigilance", 5}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Handaxes", 1), new Item ("Totem", 1)
})},
{"Poison Oracle", new GuildClass("Poison Oracle", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Poison Lore", 10},
{"Ritual Magic", 10},
{"Insight", 5},
{"Food Crafting", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Talisman", 2), new Item ("Cooking Kit", 1), new Item ("Meats", 1)
})},
{"Pole Master", new GuildClass("Pole Master", 3, 4, 1, 1,
new HashDictionary<string, int>{{"Bludgeon Training", 10},
{"Bludgeon Techniques", 7},
{"Counterattack", 6},
{"Vigilance", 6}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Pole", 2)
})},
{"Powderwitch", new GuildClass("Powderwitch", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Prototype Crafting", 10},
{"Gun Training", 10},
{"Baleful Weapon", 6},
{"Defile", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Brewing Kit", 2), new Item ("Smithing Kit", 2), new Item ("Crystal", 1), new Item ("Foul Robes", 1), new Item ("Pistol", 1)
})},
{"Prophet", new GuildClass("Prophet", 1, 6, 0, 1,
new HashDictionary<string, int>{{"Vigilance", 10},
{"Insight", 10},
{"Mentalism", 6},
{"Scan", 5}
},
new HashDictionary<string, int>{{"Melee", 3}, {"Ranged", 4}, {"Spell", 3}
},
new HashBag<Item>{new Item ("Totem", 1)
})},
{"Quilter", new GuildClass("Quilter", 3, 6, 1, 3,
new HashDictionary<string, int>{{"Garment Crafting", 10},
{"Whip Training", 9},
{"Insight", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Clothworking Kit", 2), new Item ("Cloak", 1), new Item ("Cape", 1), new Item ("Net", 1)
})},
{"Raider", new GuildClass("Raider", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Axe Training", 9},
{"Backstab", 7},
{"Scan", 7},
{"Fury", 6}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Greataxe", 1), new Item ("Light Armor", 1), new Item ("Stealth Garb", 1), new Item ("Hard Liquor", 1)
})},
{"Ranger", new GuildClass("Ranger", 3, 4, 1, 1,
new HashDictionary<string, int>{{"Blade Training", 10},
{"Vigilance", 7},
{"Herbalism", 5},
{"Blade Techniques", 5},
{"Scan", 5}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Paired Swords", 2), new Item ("Light Armor", 1), new Item ("Stealth Garb", 1)
})},
{"Remaker", new GuildClass("Remaker", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Fortify", 10},
{"Earthen Wall", 8},
{"Rising Tide", 8},
{"Persistent Arcana", 8}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Crystal", 1)
})},
{"Revolutionary", new GuildClass("Revolutionary", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Persuasion", 7},
{"Blade Training", 7},
{"Counterattack", 7},
{"Charge", 7}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 4}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Broadsword", 1), new Item ("Light Armor", 1), new Item ("Utility Harness", 1), new Item ("Cloak", 1), new Item ("Banner", 1)
})},
{"Rogue", new GuildClass("Rogue", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Knife Training", 8},
{"Backstab", 7},
{"Scan", 6},
{"Quick Draw", 6},
{"Taunt", 6},
{"Knife Techniques", 6}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 2}, {"Spell", 2}
},
new HashBag<Item>{new Item ("Daggers", 1), new Item ("Stealth Garb", 1), new Item ("Utility Harness", 1)
})},
{"Runeknife", new GuildClass("Runeknife", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Knife Training", 8},
{"Otherworldly Weapon", 8},
{"Binding Seal", 8},
{"Quick Draw", 5},
{"Knife Techniques", 4}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Daggers", 1), new Item ("Crystal", 1), new Item ("Utility Harness", 1)
})},
{"Samurai", new GuildClass("Samurai", 2, 4, 1, 3,
new HashDictionary<string, int>{{"Blade Training", 10},
{"Blade Techniques", 7},
{"Vigilance", 7}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 3}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Katana", 2), new Item ("Heavy Armor", 1), new Item ("Official Marque", 1)
})},
{"Scar Warrior", new GuildClass("Scar Warrior", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Pain Surge", 10},
{"Vigilance", 9},
{"Spear Training", 7},
{"Counterattack", 3},
{"Fury", 3}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 1}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Shield", 1), new Item ("Spear", 1), new Item ("Potion Satchel", 1)
})},
{"Scout", new GuildClass("Scout", 4, 6, 0, 3,
new HashDictionary<string, int>{{"Bow Training", 8},
{"Scan", 7},
{"Vigilance", 7},
{"Vanish", 7},
{"Herbalism", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 3}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Bow", 1), new Item ("Stealth Garb", 1), new Item ("Cloak", 1), new Item ("Trapping Tools", 1), new Item ("Nuts", 1)
})},
{"Sentinel", new GuildClass("Sentinel", 2, 4, 1, 3,
new HashDictionary<string, int>{{"Vigilance", 10},
{"Bludgeon Training", 7},
{"Persuasion", 5}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 4}, {"Spell", 4}
},
new HashBag<Item>{new Item ("Pole", 1), new Item ("Heavy Armor", 1), new Item ("Holy Water", 1)
})},
{"Shaman", new GuildClass("Shaman", 3, 6, 3, 1,
new HashDictionary<string, int>{{"Ritual Magic", 7},
{"Insight", 6},
{"Hex Magic", 6},
{"Potion Crafting", 6},
{"Herbalism", 6}
},
new HashDictionary<string, int>{{"Melee", 1}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Staff", 1), new Item ("Totem", 1), new Item ("Brewing Kit", 1)
})},
{"Shaper", new GuildClass("Shaper", 2, 6, 2, 1,
new HashDictionary<string, int>{{"Weapon Crafting", 8},
{"Artifact Crafting", 8},
{"Armor Crafting", 7},
{"Adhere", 5},
{"Insight", 4}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Crystal", 1), new Item ("Smithing Kit", 1), new Item ("Woodworking Kit", 1), new Item ("Trapping Tools", 1), new Item ("Elixir", 1)
})},
{"Shapeshifter", new GuildClass("Shapeshifter", 1, 8, 0, 1,
new HashDictionary<string, int>{{"Wild Morph", 10},
{"Unarmed Training", 8},
{"Charge", 7},
{"Unarmed Techniques", 7}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 2}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Totem", 1)
})},
{"Shieldbearer", new GuildClass("Shieldbearer", 2, 4, 1, 3,
new HashDictionary<string, int>{{"Blade Training", 8},
{"Vigilance", 8},
{"Counterattack", 7}
},
new HashDictionary<string, int>{{"Melee", 4}, {"Ranged", 4}, {"Spell", 0}
},
new HashBag<Item>{new Item ("Shield", 2), new Item ("Longsword", 1), new Item ("Heavy Armor", 1)
})},
{"Shock Mage", new GuildClass("Shock Mage", 3, 10, 2, 2,
new HashDictionary<string, int>{{"Storm Magic", 10},
{"Precise Arcana", 9},
{"Potent Arcana", 6},
{"Acrobatics", 5}
},
new HashDictionary<string, int>{{"Melee", 0}, {"Ranged", 1}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Wand", 2), new Item ("Trapping Tools", 1)
})},
{"Skin Mage", new GuildClass("Skin Mage", 2, 5, 1, 2,
new HashDictionary<string, int>{{"Pain Surge", 10},
{"Regenerate", 7},
{"Garment Crafting", 7},
{"Whip Training", 4},
{"Malevolence", 4}
},
new HashDictionary<string, int>{{"Melee", 2}, {"Ranged", 0}, {"Spell", 1}
},
new HashBag<Item>{new Item ("Cape", 1), new Item ("Cloak", 1), new Item ("Clothworking Kit", 1), new Item ("Potion Satchel", 1)
})}
};
}
}
