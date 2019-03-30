// Copyright 2014 - 2014 Esk0r
// EvadeSpellData.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using EnsoulSharp;
using EnsoulSharp.SDK.Core.UI.IMenu.Values;

#endregion

namespace Evade
{
    public enum SpellValidTargets
    {
        AllyMinions,
        EnemyMinions,

        AllyWards,
        EnemyWards,

        AllyChampions,
        EnemyChampions
    }

    /// <summary>
    /// Class containing the needed info about the evading spells.
    /// </summary>
    internal class EvadeSpellData
    {
        public delegate float MoveSpeedAmount();

        public bool CanShieldAllies;
        public string CheckSpellName = "";
        public int Delay;
        public bool FixedRange;
        public bool Invert;

        public bool IsBlink;
        public bool IsDash;
        public bool IsInvulnerability;
        public bool IsMovementSpeedBuff;
        public bool IsShield;
        public bool IsSpellShield;
        public bool IsSummonerSpell;

        public float MaxRange;
        public float MinRange;
        public MoveSpeedAmount MoveSpeedTotalAmount;
        public string Name;
        public bool PositionOnly;
        public bool RequiresPreMove;
        public bool SelfCast;
        public SpellSlot Slot;

        public int Speed;
        public SpellValidTargets[] ValidTargets;

        public int _dangerLevel;

        public EvadeSpellData() { }

        public EvadeSpellData(string name, int dangerLevel)
        {
            Name = name;
            _dangerLevel = dangerLevel;
        }

        public bool IsTargetted
        {
            get { return ValidTargets != null; }
        }

        public int DangerLevel
        {
            get
            {
                return (Config.evadeSpells?[Name]?.GetValue<MenuSlider>("DangerLevel" + Name)?.Value ?? new Nullable<int>(_dangerLevel)).Value;
            }
        }

        public bool Enabled
        {
            get
            {
                return (Config.evadeSpells?[Name]?.GetValue<MenuBool>("Enabled" + Name)?.Value ?? new Nullable<bool>(true)).Value;
            }
        }

        public bool IsReady()
        {
            return (CheckSpellName == "" || ObjectManager.Player.Spellbook.GetSpell(Slot).Name == CheckSpellName) &&
                    ObjectManager.Player.Spellbook.CanUseSpell(Slot) == SpellState.Ready;
        }
    }

    internal class DashData : EvadeSpellData
    {
        public DashData(string name, SpellSlot slot, float range, bool fixedRange, int delay, int speed, int dangerLevel)
        {
            Name = name;
            MaxRange = range;
            Slot = slot;
            FixedRange = fixedRange;
            Delay = delay;
            Speed = speed;
            _dangerLevel = dangerLevel;
            IsDash = true;
        }
    }

    internal class BlinkData : EvadeSpellData
    {
        public BlinkData(string name, SpellSlot slot, float range, int delay, int dangerLevel, bool isSummonerSpell = false)
        {
            Name = name;
            MaxRange = range;
            Slot = slot;
            Delay = delay;
            _dangerLevel = dangerLevel;
            IsSummonerSpell = isSummonerSpell;
            IsBlink = true;
        }
    }

    internal class InvulnerabilityData : EvadeSpellData
    {
        public InvulnerabilityData(string name, SpellSlot slot, int delay, int dangerLevel)
        {
            Name = name;
            Slot = slot;
            Delay = delay;
            _dangerLevel = dangerLevel;
            IsInvulnerability = true;
        }
    }

    internal class ShieldData : EvadeSpellData
    {
        public ShieldData(string name, SpellSlot slot, int delay, int dangerLevel, bool isSpellShield = false)
        {
            Name = name;
            Slot = slot;
            Delay = delay;
            _dangerLevel = dangerLevel;
            IsSpellShield = isSpellShield;
            IsShield = !IsSpellShield;
        }
    }

    internal class MoveBuffData : EvadeSpellData
    {
        public MoveBuffData(string name, SpellSlot slot, int delay, int dangerLevel, MoveSpeedAmount amount)
        {
            Name = name;
            Slot = slot;
            Delay = delay;
            _dangerLevel = dangerLevel;
            MoveSpeedTotalAmount = amount;
            IsMovementSpeedBuff = true;
        }
    }
}