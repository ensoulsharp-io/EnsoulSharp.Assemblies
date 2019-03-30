// Copyright 2014 - 2014 Esk0r
// SpellData.cs is part of Evade.
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

using EnsoulSharp;

#endregion

namespace Evade
{
    public class SpellData
    {
        public bool AddHitbox;
        public bool CanBeRemoved;
        public bool Centered;
        public string ChampionName;
        public bool CollisionExceptMini;
        public CollisionObjectTypes[] CollisionObjects = { };
        public int DangerValue;
        public int Delay;
        public bool DisabledByDefault;
        public bool DisableFowDetection;
        public string DisplayName;
        public bool DontAddExtraDuration;
        public bool DontCheckForDuplicates;
        public bool DontCross;
        public EarlyObjects[] EarlyEvade = { };
        public int ExtraDuration;
        public string[] ExtraMissileSpellNames = { };
        public int ExtraRange = -1;
        public string[] ExtraSpellNames = { };
        public bool FixedRange;
        public bool FollowCaster;
        public string FromObject = string.Empty;
        public string[] FromObjects = { };
        public bool Invert;
        public bool IsDangerous;
        public int MissileAccel;
        public bool MissileDelayed;
        public bool MissileFollowsUnit;
        public int MissileMaxSpeed;
        public int MissileMinSpeed;
        public int MissileSpeed;
        public string MissileSpellName = string.Empty;
        public float MultipleAngle;
        public int MultipleNumber = -1;
        public int RingRadius;
        public string SourceObjectName;
        public string SpellName = string.Empty;
        public SpellSlot Slot;
        public bool TakeClosestPath;
        public string ToggleParticleName = string.Empty;
        public SkillShotType Type;

        private int _radius;
        private int _range;

        public string MenuItemName
        {
            get { return ChampionName + "-" + DisplayName; }
        }

        public int Radius
        {
            get
            {
                return (!AddHitbox)
                    ? _radius + Config.SkillShotsExtraRadius
                    : _radius + Config.SkillShotsExtraRadius + (int)ObjectManager.Player.BoundingRadius;
            }
            set { _radius = value; }
        }

        public int RawRadius
        {
            get { return _radius; }
        }

        public int Range
        {
            get
            {
                return _range +
                    ((Type == SkillShotType.SkillshotLine || Type == SkillShotType.SkillshotMissileLine)
                    ? Config.SkillShotsExtraRange
                    : 0);
            }
            set { _range = value; }
        }

        public int RawRange
        {
            get { return _range; }
        }
    }
}