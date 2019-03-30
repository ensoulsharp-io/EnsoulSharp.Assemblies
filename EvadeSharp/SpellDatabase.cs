// Copyright 2014 - 2014 Esk0r
// SpellDatabase.cs is part of Evade.
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
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Core.Utils;

#endregion

namespace Evade
{
    public static class SpellDatabase
    {
        public static List<SpellData> Spells = new List<SpellData>();

        static SpellDatabase()
        {
            // Add spells to the database

            #region Test

            if (Config.TestOnAllies)
            {
                Spells.Add(
                    new SpellData
                    {
                        ChampionName = ObjectManager.Player.CharacterName,
                        SpellName = "TestCircleSkillShot",
                        DisplayName = "TestCircleSkillShot",
                        Slot = SpellSlot.R,
                        Type = SkillShotType.SkillshotCircle,
                        Delay = 6000,
                        Range = 650,
                        Radius = 500,
                        MissileSpeed = int.MaxValue,
                        FixedRange = false,
                        AddHitbox = true,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "TestCircleSkillShot",
                        DontCross = true
                    });

                Spells.Add(
                    new SpellData
                    {
                        ChampionName = ObjectManager.Player.CharacterName,
                        SpellName = "TestLineSkillShot",
                        DisplayName = "TestLineSkillShot",
                        Slot = SpellSlot.R,
                        Type = SkillShotType.SkillshotMissileLine,
                        Delay = 1000,
                        Range = 1200,
                        Radius = 100,
                        MissileSpeed = 100,
                        FixedRange = true,
                        AddHitbox = true,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "TestLineSkillShot",
                        DontCross = true
                    });
            }

            #endregion

            #region Aatrox

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Aatrox",
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Aatrox Q1",
                    FixedRange = true,
                    FollowCaster = true,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "AatroxQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 150,
                    Range = 650
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Aatrox",
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Aatrox Q2",
                    FixedRange = true,
                    FollowCaster = true,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "AatroxQ2",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCone,
                    Radius = 70,
                    Range = 550
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Aatrox",
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Aatrox Q3",
                    FollowCaster = true,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "AatroxQ3",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 300,
                    Range = 300
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Aatrox",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Aatrox W",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1800,
                    MissileSpellName = "AatroxW",
                    SpellName = "AatroxW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 825
                });

            #endregion

            #region Ahri

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ahri",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Ahri Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2500,
                    MissileSpellName = "AhriOrbMissile",
                    SpellName = "AhriOrbofDeception",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 100,
                    Range = 880
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ahri",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Ahri Q Wayback",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileAccel = 1900,
                    MissileFollowsUnit = true,
                    MissileMaxSpeed = 2600,
                    MissileSpeed = 60,
                    MissileSpellName = "AhriOrbReturn",
                    SpellName = "AhriOrbReturn",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 100,
                    Range = 880
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ahri",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Ahri E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1550,
                    MissileSpellName = "AhriSeduceMissile",
                    SpellName = "AhriSeduce",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 975
                });

            #endregion

            #region Akali

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Akali",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Akali E",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1900,
                    MissileSpellName = "AkaliEMis",
                    SpellName = "AkaliE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 775
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Akali",
                    DangerValue = 3,
                    DisplayName = "Akali R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1500,
                    SpellName = "AkaliR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 600
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Akali",
                    DangerValue = 3,
                    DisplayName = "Akali R2",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 3000,
                    SpellName = "AkaliRb",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 750
                });

            #endregion

            #region Amumu

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Amumu",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Amumu Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 2000,
                    MissileSpellName = "SadMummyBandageToss",
                    SpellName = "BandageToss",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Amumu",
                    DangerValue = 5,
                    Delay = 250,
                    DisplayName = "Amumu R",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "CurseoftheSadMummy",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 550,
                    Range = 0
                });

            #endregion

            #region Anivia

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Anivia",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Anivia Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 850,
                    MissileSpellName = "FlashFrostSpell",
                    SpellName = "FlashFrost",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 110,
                    Range = 1075
                });

            #endregion

            #region Annie

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Annie",
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Annie W",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "AnnieW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCone,
                    Radius = 50,
                    Range = 590
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Annie",
                    DangerValue = 5,
                    Delay = 250,
                    DisplayName = "Annie R",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "AnnieR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 250,
                    Range = 600
                });

            #endregion

            #region Ashe

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ashe",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Ashe W",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1500,
                    MissileSpellName = "VolleyAttack",
                    MultipleAngle = 5f * (float)Math.PI / 180,
                    MultipleNumber = 9,
                    SpellName = "Volley",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 20,
                    Range = 1200
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ashe",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    Delay = 250,
                    DisplayName = "Ashe R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1600,
                    MissileSpellName = "EnchantedCrystalArrow",
                    SpellName = "EnchantedCrystalArrow",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 130,
                    Range = 25000
                });

            #endregion

            #region AurelionSol

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "AurelionSol",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    Delay = 350,
                    DisplayName = "AurelionSol R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 4500,
                    MissileSpellName = "AurelionSolRBeamMissile",
                    SpellName = "AurelionSolR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 140,
                    Range = 1750
                });

            #endregion

            #region Azir

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Azir",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Azir Q",
                    IsDangerous = false,
                    MissileSpeed = 1600,
                    MissileSpellName = "AzirSoldierMissile",
                    SpellName = "AzirSoldierMissile",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 2550
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    Centered = true,
                    ChampionName = "Azir",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 4,
                    DisplayName = "Azir R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1400,
                    SpellName = "AzirR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 290,
                    Range = 1000
                });

            #endregion

            #region Bard

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Bard",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Bard Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1500,
                    MissileSpellName = "BardQMissile",
                    SpellName = "BardQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 850
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Bard",
                    DangerValue = 2,
                    Delay = 500,
                    DisplayName = "Bard R",
                    IsDangerous = false,
                    MissileSpeed = 2100,
                    SpellName = "BardR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 350,
                    Range = 3400
                });

            #endregion

            #region Blitzcrank

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Blitzcrank",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Blitzcrank Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1800,
                    MissileSpellName = "RocketGrabMissile",
                    SpellName = "RocketGrab",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 925
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Blitzcrank",
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Blitzcrank R",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "StaticField",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 600,
                    Range = 0
                });

            #endregion

            #region Brand

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Brand",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Brand Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1600,
                    MissileSpellName = "BrandQMissile",
                    SpellName = "BrandQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 1050
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Brand",
                    DangerValue = 2,
                    Delay = 850,
                    DisplayName = "Brand W",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "BrandW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 240,
                    Range = 900
                });

            #endregion

            #region Braum

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Braum",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Braum Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    MissileSpellName = "BraumQMissile",
                    SpellName = "BraumQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 1000
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Braum",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Braum R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1400,
                    MissileSpellName = "BraumRMissile",
                    SpellName = "BraumRWrapper",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 125,
                    Range = 1200
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Braum",
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Braum R Range",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "BraumRRange",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 250,
                    Range = 0
                });

            #endregion

            #region Caitlyn

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Caitlyn",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 630,
                    DisplayName = "Caitlyn Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2200,
                    MissileSpellName = "CaitlynPiltoverPeacemaker",
                    SpellName = "CaitlynPiltoverPeacemaker",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 1250
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Caitlyn",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 150,
                    DisplayName = "Caitlyn E",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1600,
                    MissileSpellName = "CaitlynEntrapmentMissile",
                    SpellName = "CaitlynEntrapment",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 750
                });

            #endregion

            #region Camille

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Camille",
                    DangerValue = 2,
                    Delay = 900,
                    DisplayName = "Camille W",
                    FollowCaster = true,
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "CamilleW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCone,
                    Radius = 70,
                    Range = 650
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Camille",
                    DangerValue = 3,
                    DisplayName = "Camille E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1430,
                    SpellName = "CamilleEDash2",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 75,
                    Range = 800
                });

            #endregion

            #region Cassiopeia

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Cassiopeia",
                    DangerValue = 2,
                    Delay = 750,
                    DisplayName = "Cassiopeia Q",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "CassiopeiaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 160,
                    Range = 850
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Cassiopeia",
                    DangerValue = 5,
                    Delay = 500,
                    DisplayName = "Cassiopeia R",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "CassiopeiaR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCone,
                    Radius = 80,
                    Range = 825
                });

            #endregion

            #region Chogath

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Chogath",
                    DangerValue = 3,
                    Delay = 1200,
                    DisplayName = "Chogath Q",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "Rupture",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 230,
                    Range = 950
                });

            #endregion

            #region Corki

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Corki",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Corki Q",
                    IsDangerous = false,
                    MissileSpeed = 1000,
                    MissileSpellName = "PhosphorusBombMissile",
                    SpellName = "PhosphorusBomb",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 250,
                    Range = 825
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Corki",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 180,
                    DisplayName = "Corki R",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2000,
                    MissileSpellName = "MissileBarrageMissile",
                    SpellName = "MissileBarrageMissile",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 40,
                    Range = 1225
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Corki",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 180,
                    DisplayName = "Corki R Super",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2000,
                    MissileSpellName = "MissileBarrageMissile2",
                    SpellName = "MissileBarrageMissile2",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 40,
                    Range = 1225
                });

            #endregion

            #region Darius

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Darius",
                    DangerValue = 3,
                    Delay = 750,
                    DisplayName = "Darius Q",
                    FollowCaster = true,
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "DariusCleave",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 420,
                    Range = 0
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Darius",
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Darius E",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "DariusAxeGrabCone",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCone,
                    Radius = 50,
                    Range = 535
                });

            #endregion

            #region Diana

            Spells.Add(
                new SpellData
                {
                    CanBeRemoved = true,
                    ChampionName = "Diana",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Diana Q Range",
                    IsDangerous = true,
                    MissileSpeed = 1400,
                    MissileSpellName = "DianaArcThrow",
                    SpellName = "DianaArc",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 195,
                    Range = 900
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Diana",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Diana Q",
                    IsDangerous = true,
                    MissileSpeed = 1400,
                    SpellName = "DianaArcArc",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotArc,
                    Radius = 195,
                    Range = 900
                });

            #endregion

            #region Draven

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Draven",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Draven E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1400,
                    MissileSpellName = "DravenDoubleShotMissile",
                    SpellName = "DravenDoubleShot",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 130,
                    Range = 1050
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Draven",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Draven R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 2000,
                    MissileSpellName = "DravenR",
                    SpellName = "DravenRCast",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 160,
                    Range = 20000
                });

            #endregion

            #region DrMundo

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "DrMundo",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "DrMundo Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2000,
                    MissileSpellName = "InfectedCleaverMissile",
                    SpellName = "InfectedCleaverMissileCast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 75,
                    Range = 975
                });

            #endregion

            #region Ekko

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ekko",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Ekko Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1650,
                    MissileSpellName = "EkkoQMis",
                    SpellName = "EkkoQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 160,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ekko",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Ekko Q Return",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileFollowsUnit = true,
                    MissileSpeed = 2300,
                    MissileSpellName = "EkkoQReturn",
                    SpellName = "EkkoQReturn",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 160,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    CanBeRemoved = true,
                    ChampionName = "Ekko",
                    DangerValue = 3,
                    Delay = 3750,
                    DisplayName = "Ekko W",
                    IsDangerous = true,
                    MissileSpeed = 1650,
                    MissileSpellName = "EkkoWMis",
                    SpellName = "EkkoW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 375,
                    Range = 1600
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    DangerValue = 3,
                    Delay = 800,
                    DisplayName = "Ekko R",
                    FromObject = "Ekko_.+_R_TrailEnd",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "EkkoR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 375,
                    Range = 25000
                });

            #endregion

            #region Elise

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Elise",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 4,
                    Delay = 250,
                    DisplayName = "Elise E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1600,
                    MissileSpellName = "EliseHumanE",
                    SpellName = "EliseHumanE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 55,
                    Range = 1075
                });

            #endregion

            #region Evelynn

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Evelynn",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Evelynn Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2400,
                    MissileSpellName = "EvelynnQ",
                    SpellName = "EvelynnQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 40,
                    Range = 800
                });

            #endregion

            #region Ezreal

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ezreal",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Ezreal Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2000,
                    MissileSpellName = "EzrealQ",
                    SpellName = "EzrealQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 1150
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ezreal",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Ezreal W",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    MissileSpellName = "EzrealW",
                    SpellName = "EzrealW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 1150
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ezreal",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 1000,
                    DisplayName = "Ezreal R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 2000,
                    MissileSpellName = "EzrealR",
                    SpellName = "EzrealR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 160,
                    Range = 25000
                });

            #endregion

            #region Fiora

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Fiora",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 550,
                    DisplayName = "Fiora W",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 3200,
                    MissileSpellName = "FioraWMissile",
                    SpellName = "FioraW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 95,
                    Range = 750
                });

            #endregion

            #region Fizz

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Fizz",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    Delay = 250,
                    DisplayName = "Fizz R",
                    IsDangerous = true,
                    MissileSpeed = 1300,
                    MissileSpellName = "FizzRMissile",
                    SpellName = "FizzR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 1300
                });

            #endregion

            #region Galio

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Galio",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Wall },
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Galio E",
                    IsDangerous = true,
                    MissileSpeed = 2300,
                    SpellName = "GalioE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 160,
                    Range = 650
                });

            #endregion

            #region Gnar

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Gnar",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Gnar Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileAccel = -3000,
                    MissileMinSpeed = 1400,
                    MissileSpeed = 2500,
                    MissileSpellName = "GnarQMissile",
                    SpellName = "GnarQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 55,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Gnar",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Gnar Q Return",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileAccel = 800,
                    MissileMaxSpeed = 2600,
                    MissileSpeed = 60,
                    MissileSpellName = "GnarQMissileReturn",
                    SpellName = "GnarQMissileReturn",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 55,
                    Range = 3000
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Gnar",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 500,
                    DisplayName = "Gnar Big Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2100,
                    MissileSpellName = "GnarBigQMissile",
                    SpellName = "GnarBigQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Gnar",
                    DangerValue = 3,
                    Delay = 600,
                    DisplayName = "Gnar Big W",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "GnarBigW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 80,
                    Range = 525
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Gnar",
                    DangerValue = 2,
                    DisplayName = "Gnar E",
                    IsDangerous = false,
                    MissileSpeed = 900,
                    SpellName = "GnarE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 150,
                    Range = 475
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Gnar",
                    DangerValue = 2,
                    DisplayName = "Gnar Big E",
                    IsDangerous = false,
                    MissileSpeed = 1000,
                    SpellName = "GnarBigE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 350,
                    Range = 600
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Gnar",
                    DangerValue = 5,
                    Delay = 250,
                    DisplayName = "Gnar R",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "GnarR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 450,
                    Range = 0
                });

            #endregion

            #region Gragas

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Gragas",
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Gragas Q",
                    DontCross = true,
                    ExtraDuration = 4500,
                    IsDangerous = false,
                    MissileSpeed = 1000,
                    MissileSpellName = "GragasQMissile",
                    SpellName = "GragasQ",
                    Slot = SpellSlot.Q,
                    ToggleParticleName = "Gragas_.+_Q_(Enemy|Ally)",
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 250,
                    Range = 850
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    DangerValue = 3,
                    DisplayName = "Gragas E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 910,
                    SpellName = "GragasE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 180,
                    Range = 800
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Gragas",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    Delay = 250,
                    DisplayName = "Gragas R",
                    IsDangerous = true,
                    MissileSpeed = 1900,
                    MissileSpellName = "GragasRBoom",
                    SpellName = "GragasR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 350,
                    Range = 1000
                });

            #endregion

            #region Graves

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Graves",
                    CollisionObjects = new[] { CollisionObjectTypes.Wall, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Graves Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 3000,
                    MissileSpellName = "GravesQLineMis",
                    SpellName = "GravesQLineSpell",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 75,
                    Range = 830
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Graves",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Graves Q Return",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1600,
                    MissileSpellName = "GravesQReturn",
                    SpellName = "GravesQReturn",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 190,
                    Range = 830
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Graves",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Graves W",
                    DontCross = true,
                    ExtraDuration = 4000,
                    IsDangerous = false,
                    MissileSpeed = 1500,
                    MissileSpellName = "GravesSmokeGrenadeBoom",
                    SpellName = "GravesSmokeGrenade",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 225,
                    Range = 950
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Graves",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Graves R",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2100,
                    MissileSpellName = "GravesChargeShotShot",
                    SpellName = "GravesChargeShot",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 100,
                    Range = 1000
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Graves",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 350,
                    DisplayName = "Graves R Range",
                    IsDangerous = false,
                    MissileSpeed = 2100,
                    SpellName = "GravesRRange",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCone,
                    Radius = 60,
                    Range = 800
                });

            #endregion

            #region Heimerdinger

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Heimerdinger",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Heimerdinger W",
                    ExtraMissileSpellNames = new[] { "HeimerdingerWAttack2Ult" },
                    FixedRange = true,
                    IsDangerous = false,
                    MissileAccel = 4000,
                    MissileMaxSpeed = 3000,
                    MissileSpeed = 750,
                    MissileSpellName = "HeimerdingerWAttack2",
                    SpellName = "HeimerdingerWAttack2",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 1350
                });

            Spells.Add(
                new SpellData
                {
                    CanBeRemoved = true,
                    ChampionName = "Heimerdinger",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Heimerdinger E",
                    IsDangerous = true,
                    MissileSpeed = 1200,
                    MissileSpellName = "HeimerdingerESpell",
                    SpellName = "HeimerdingerE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 250,
                    Range = 970
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Heimerdinger",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Heimerdinger E Ult",
                    IsDangerous = true,
                    MissileSpeed = 1200,
                    MissileSpellName = "HeimerdingerESpell_ult",
                    SpellName = "HeimerdingerEUlt",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 300,
                    Range = 925
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Heimerdinger",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    DisplayName = "Heimerdinger E Ult 2",
                    IsDangerous = true,
                    MissileSpeed = 750,
                    MissileSpellName = "HeimerdingerESpell_ult2",
                    SpellName = "HeimerdingerESpell_ult2",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 300,
                    Range = 925
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Heimerdinger",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    DisplayName = "Heimerdinger E Ult 3",
                    IsDangerous = true,
                    MissileSpeed = 750,
                    MissileSpellName = "HeimerdingerESpell_ult3",
                    SpellName = "HeimerdingerESpell_ult3",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 300,
                    Range = 925
                });

            #endregion

            #region Illaoi

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Illaoi",
                    DangerValue = 3,
                    Delay = 750,
                    DisplayName = "Illaoi Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "IllaoiQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 105,
                    Range = 825
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Illaoi",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Illaoi E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1900,
                    MissileSpellName = "IllaoiEMis",
                    SpellName = "IllaoiE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 900
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Illaoi",
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Illaoi R",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "IllaoiR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 450,
                    Range = 0
                });

            #endregion

            #region Irelia

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Irelia",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Irelia W",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 15500,
                    MissileSpellName = "IreliaW2",
                    SpellName = "IreliaW2",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 100,
                    Range = 825
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Irelia",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    DisplayName = "Irelia E",
                    IsDangerous = true,
                    MissileSpeed = 1500,
                    MissileSpellName = "IreliaEParticleMissile",
                    SpellName = "IreliaEParticleMissile",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 5000
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Irelia",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 400,
                    DisplayName = "Irelia R",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2000,
                    MissileSpellName = "IreliaR",
                    SpellName = "IreliaR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 160,
                    Range = 950
                });

            #endregion

            #region Ivern

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ivern",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Ivern Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1300,
                    MissileSpellName = "IvernQ",
                    SpellName = "IvernQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 75,
                    Range = 1075
                });

            #endregion

            #region Janna

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Janna",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    DisplayName = "Janna Q",
                    IsDangerous = true,
                    MissileSpeed = 1166,
                    MissileSpellName = "HowlingGaleSpell",
                    SpellName = "HowlingGaleSpell",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 120,
                    Range = 1750
                });

            #endregion

            #region JarvanIV

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "JarvanIV",
                    DangerValue = 3,
                    Delay = 400,
                    DisplayName = "JarvanIV Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "JarvanIVDragonStrike",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 70,
                    Range = 770
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "JarvanIV",
                    DangerValue = 2,
                    Delay = 500,
                    DisplayName = "JarvanIV E",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "JarvanIVDemacianStandard",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 175,
                    Range = 860
                });

            #endregion

            #region Jayce

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Jayce",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 210,
                    DisplayName = "Jayce Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1450,
                    MissileSpellName = "JayceShockBlastMis",
                    SpellName = "JayceShockBlast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 1050
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Jayce",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    DisplayName = "Jayce Q Accel",
                    IsDangerous = true,
                    MissileSpeed = 2350,
                    MissileSpellName = "JayceShockBlastWallMis",
                    SpellName = "JayceShockBlastWallMis",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 2100
                });

            #endregion

            #region Jhin

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Jhin",
                    DangerValue = 2,
                    Delay = 750,
                    DisplayName = "Jhin W",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "JhinW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 60,
                    Range = 2500
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Jhin",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Jhin R",
                    ExtraMissileSpellNames = new[] { "JhinRShotMis4" },
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 5000,
                    MissileSpellName = "JhinRShotMis",
                    SpellName = "JhinRShot",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 3500
                });

            #endregion

            #region Jinx

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Jinx",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Jinx W",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 3300,
                    MissileSpellName = "JinxWMissile",
                    SpellName = "JinxW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 1450
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Jinx",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    DisplayName = "Jinx E",
                    DontCross = true,
                    ExtraDuration = 5000,
                    IsDangerous = true,
                    MissileSpeed = 1530,
                    MissileSpellName = "JinxEHit",
                    SpellName = "JinxEHit",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 60,
                    Range = 900
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Jinx",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    Delay = 600,
                    DisplayName = "Jinx R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1700,
                    MissileSpellName = "JinxR",
                    SpellName = "JinxR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 140,
                    Range = 25000
                });

            #endregion

            #region Kaisa

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Kaisa",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 400,
                    DisplayName = "Kaisa W",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1750,
                    MissileSpellName = "KaisaW",
                    SpellName = "KaisaW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 3000
                });

            #endregion

            #region Kalista

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Kalista",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Kalista Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2400,
                    MissileSpellName = "KalistaMysticShotMisTrue",
                    SpellName = "KalistaMysticShot",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 40,
                    Range = 1150
                });

            #endregion

            #region Karma

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Karma",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Karma Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    MissileSpellName = "KarmaQMissile",
                    SpellName = "KarmaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 950
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Karma",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Karma Q Mantra",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    MissileSpellName = "KarmaQMissileMantra",
                    SpellName = "KarmaQMissileMantra",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 950
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Karma",
                    DangerValue = 2,
                    DisplayName = "Karma Q Mantra Range",
                    ExtraDuration = 1500,
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    SpellName = "KarmaQMantraRange",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 260,
                    Range = 950
                });

            #endregion

            #region Karthus

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Karthus",
                    DangerValue = 2,
                    Delay = 900,
                    DisplayName = "Karthus Q",
                    ExtraSpellNames = new[] { "KarthusLayWasteDeadA" },
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "KarthusLayWasteA",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 160,
                    Range = 875
                });

            #endregion

            #region Kassadin

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Kassadin",
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Kassadin E",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "ForcePulse",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCone,
                    Radius = 78,
                    Range = 640
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kassadin",
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Kassadin R",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "RiftWalk",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 270,
                    Range = 500
                });

            #endregion

            #region Kayle

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Kayle",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 500,
                    DisplayName = "Kayle Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2000,
                    MissileSpellName = "KayleQMis",
                    SpellName = "KayleQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 850
                });

            #endregion

            #region Kayn

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Kayn",
                    DangerValue = 3,
                    Delay = 550,
                    DisplayName = "Kayn W",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "KaynW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 100,
                    Range = 700
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Kayn",
                    DangerValue = 3,
                    Delay = 600,
                    DisplayName = "Kayn W Ass",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "KaynAssW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 100,
                    Range = 900
                });

            #endregion

            #region Kennen

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Kennen",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 180,
                    DisplayName = "Kennen Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    MissileSpellName = "KennenShurikenHurlMissile1",
                    SpellName = "KennenShurikenHurlMissile1",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 950
                });

            #endregion

            #region Khazix

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Khazix",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Khazix W",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    MissileSpellName = "KhazixWMissile",
                    SpellName = "KhazixW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 1000
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Khazix",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Khazix W Long",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    MissileSpellName = "KhazixWMissile",
                    MultipleAngle = 22f * (float)Math.PI / 180,
                    MultipleNumber = 3,
                    SpellName = "KhazixWLong",
                    Slot = SpellSlot.W, 
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 1000
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Khazix",
                    DangerValue = 2,
                    DisplayName = "Khazix E",
                    IsDangerous = false,
                    MissileSpeed = 1260,
                    SpellName = "KhazixE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 300,
                    Range = 700
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Khazix",
                    DangerValue = 2,
                    DisplayName = "Khazix E Long",
                    IsDangerous = false,
                    MissileSpeed = 1210,
                    SpellName = "KhazixELong",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 300,
                    Range = 900
                });

            #endregion

            #region Kled

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Kled",
                    CollisionExceptMini = true,
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Kled Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1600,
                    MissileSpellName = "KledQMissile",
                    SpellName = "KledQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 40,
                    Range = 750
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Kled",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Kled Q Rider",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 3000,
                    MissileSpellName = "KledRiderQMissile",
                    MultipleAngle = 5f * (float)Math.PI / 180,
                    MultipleNumber = 5,
                    SpellName = "KledRiderQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 40,
                    Range = 700
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Kled",
                    DangerValue = 2,
                    DisplayName = "Kled E",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 945,
                    SpellName = "KledE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 100,
                    Range = 600
                });

            #endregion

            #region KogMaw

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "KogMaw",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "KogMaw Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1650,
                    MissileSpellName = "KogMawQ",
                    SpellName = "KogMawQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 1175
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "KogMaw",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "KogMaw E",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1400,
                    MissileSpellName = "KogMawVoidOozeMissile",
                    SpellName = "KogMawVoidOoze",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 120,
                    Range = 1200
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "KogMaw",
                    DangerValue = 2,
                    Delay = 1000,
                    DisplayName = "KogMaw R",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "KogMawLivingArtillery",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 240,
                    Range = 1800
                });

            #endregion

            #region Leblanc

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Leblanc",
                    DangerValue = 2,
                    DisplayName = "Leblanc W",
                    IsDangerous = false,
                    MissileSpeed = 1450,
                    SpellName = "LeblancW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 240,
                    Range = 600
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Leblanc",
                    DangerValue = 2,
                    DisplayName = "Leblanc RW",
                    IsDangerous = false,
                    MissileSpeed = 1450,
                    SpellName = "LeblancRW",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 220,
                    Range = 600
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Leblanc",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Leblanc E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1750,
                    MissileSpellName = "LeblancEMissile",
                    SpellName = "LeblancE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 55,
                    Range = 925
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Leblanc",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Leblanc RE",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1750,
                    MissileSpellName = "LeblancREMissile",
                    SpellName = "LeblancRE",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 55,
                    Range = 925
                });

            #endregion

            #region LeeSin

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "LeeSin",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "LeeSin Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1800,
                    MissileSpellName = "BlindMonkQOne",
                    SpellName = "BlindMonkQOne",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 1100
                });

            #endregion

            #region Leona

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Leona",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Leona E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 2000,
                    MissileSpellName = "LeonaZenithBladeMissile",
                    SpellName = "LeonaZenithBlade",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 875
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Leona",
                    DangerValue = 5,
                    Delay = 900,
                    DisplayName = "Leona R",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "LeonaSolarFlare",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 300,
                    Range = 1200
                });

            #endregion

            #region Lissandra

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Lissandra",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Lissandra Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2200,
                    MissileSpellName = "LissandraQMissile",
                    SpellName = "LissandraQMissile",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 75,
                    Range = 725
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Lissandra",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Lissandra E",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1000,
                    MissileSpellName = "LissandraEMissile",
                    SpellName = "LissandraEMissile",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 110,
                    Range = 1050
                });

            #endregion

            #region Lucian

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Lucian",
                    DangerValue = 2,
                    DisplayName = "Lucian Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "LucianQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 60,
                    Range = 900
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Lucian",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Lucian W",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1600,
                    MissileSpellName = "LucianWMissile",
                    SpellName = "LucianW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 900
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Lucian",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisabledByDefault = true,
                    DisplayName = "Lucian R",
                    DontCheckForDuplicates = true,
                    ExtraMissileSpellNames = new[] { "LucianRMissileOffhand" },
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2800,
                    MissileSpellName = "LucianRMissile",
                    SpellName = "LucianRMissile",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 110,
                    Range = 1200
                });

            #endregion

            #region Lulu

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Lulu",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Lulu Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1450,
                    MissileSpellName = "LuluQMissile",
                    SpellName = "LuluQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 925
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Lulu",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Lulu Q Pix",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1450,
                    MissileSpellName = "LuluQMissileTwo",
                    SpellName = "LuluQMissileTwo",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 925
                });

            #endregion

            #region Lux

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Lux",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Lux Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1200,
                    MissileSpellName = "LuxLightBindingMis",
                    SpellName = "LuxLightBinding",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 1175
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lux",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Lux E",
                    DontCross = true,
                    ExtraDuration = 5000,
                    IsDangerous = false,
                    MissileSpeed = 1200,
                    MissileSpellName = "LuxLightStrikeKugel",
                    SpellName = "LuxLightStrikeKugel",
                    Slot = SpellSlot.E,
                    ToggleParticleName = "Lux_.+_E_tar_aoe_green",
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 295,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lux",
                    DangerValue = 5,
                    Delay = 1000,
                    DisplayName = "Lux R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "LuxMaliceCannon",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 190,
                    Range = 3340
                });

            #endregion

            #region Malphite

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Malphite",
                    DangerValue = 5,
                    DisplayName = "Malphite R",
                    IsDangerous = true,
                    MissileSpeed = 1800,
                    SpellName = "UFSlash",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 270,
                    Range = 1000
                });

            #endregion

            #region Malzahar

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Malzahar",
                    DangerValue = 2,
                    Delay = 750,
                    DisplayName = "Malzahar Q",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "MalzaharQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 100,
                    Range = 900
                });

            #endregion

            #region Maokai

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Maokai",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 380,
                    DisplayName = "Maokai Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1600,
                    MissileSpellName = "MaokaiQMissile",
                    SpellName = "MaokaiQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 150,
                    Range = 600
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Maokai",
                    DangerValue = 2,
                    Delay = 380,
                    DisplayName = "Maokai Q Range",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "MaokaiQRange",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 300,
                    Range = 0
                });

            #endregion

            #region Morgana

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Morgana",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Morgana Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1200,
                    MissileSpellName = "MorganaQ",
                    SpellName = "MorganaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 1250
                });

            #endregion

            #region Nami

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nami",
                    DangerValue = 3,
                    Delay = 950,
                    DisplayName = "Nami Q",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "NamiQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 200,
                    Range = 875
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Nami",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Nami R",
                    IsDangerous = true,
                    MissileSpeed = 850,
                    MissileSpellName = "NamiRMissile",
                    SpellName = "NamiR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 325,
                    Range = 2550
                });

            #endregion

            #region Nautilus

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Nautilus",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.Wall, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Nautilus Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 2000,
                    MissileSpellName = "NautilusAnchorDragMissile",
                    SpellName = "NautilusAnchorDrag",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 950
                });

            #endregion

            #region Neeko

            Spells.Add(
                new SpellData
                {
                    CanBeRemoved = true,
                    ChampionName = "Neeko",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Neeko Q",
                    IsDangerous = false,
                    MissileSpeed = 2000,
                    MissileSpellName = "NeekoQ",
                    SpellName = "NeekoQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 225,
                    Range = 725
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Neeko",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Neeko E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1300,
                    MissileSpellName = "NeekoE",
                    SpellName = "NeekoE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 1000
                });

            #endregion

            #region Nidalee

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Nidalee",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Nidalee Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1300,
                    MissileSpellName = "JavelinToss",
                    SpellName = "JavelinToss",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 40,
                    Range = 1500
                });

            #endregion

            #region Nocturne

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Nocturne",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Nocturne Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1600,
                    MissileSpellName = "NocturneDuskbringer",
                    SpellName = "NocturneDuskbringer",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 1125
                });

            #endregion

            #region Olaf

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Olaf",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Olaf Q",
                    IsDangerous = false,
                    MissileSpeed = 1600,
                    MissileSpellName = "OlafAxeThrow",
                    SpellName = "OlafAxeThrowCast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 1000
                });

            #endregion

            #region Orianna

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Orianna",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Orianna Q",
                    IsDangerous = false,
                    MissileSpeed = 1400,
                    MissileSpellName = "OrianaIzuna",
                    SpellName = "OrianaIzunaCommand-",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 2000
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Orianna",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Orianna Q Range",
                    IsDangerous = false,
                    MissileSpeed = 1400,
                    MissileSpellName = "OrianaIzuna",
                    SpellName = "OriannaQRange",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 145,
                    Range = 2000
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Orianna",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Orianna E",
                    IsDangerous = false,
                    MissileSpeed = 1850,
                    MissileSpellName = "OrianaRedact",
                    SpellName = "OrianaRedactCommand-",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 2200
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    DangerValue = 5,
                    Delay = 500,
                    DisplayName = "Orianna R",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "OrianaDetonateCommand-",
                    Slot = SpellSlot.R,
                    SourceObjectName = "R_VacuumIndicator",
                    Radius = 410,
                    Range = 0
                });

            #endregion

            #region Ornn

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ornn",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 300,
                    DisplayName = "Ornn Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1800,
                    MissileSpellName = "OrnnQ",
                    SpellName = "OrnnQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 65,
                    Range = 800
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Ornn",
                    CollisionObjects = new[] { CollisionObjectTypes.Wall },
                    DangerValue = 3,
                    Delay = 350,
                    DisplayName = "Ornn E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1600,
                    SpellName = "OrnnE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 150,
                    Range = 750
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ornn",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Ornn R",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1250, // accelerate from 450 to 1250 between 570 ms by eight irregular interval accelerations at 100 acceleration
                    MissileSpellName = "OrnnRWave",
                    SpellName = "OrnnRWave",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 340,
                    Range = 3500
                });

            Spells.Add(
                new SpellData
                {
                    CanBeRemoved = true,
                    ChampionName = "Ornn",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    DisplayName = "Ornn R Wayback",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1650,
                    MissileSpellName = "OrnnRWave2",
                    SpellName = "OrnnRWave2",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 340,
                    Range = 3500
                });

            #endregion

            #region Poppy

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Poppy",
                    DangerValue = 2,
                    Delay = 1400,
                    DisplayName = "Poppy Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "PoppyQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 100,
                    Range = 430
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Poppy",
                    DangerValue = 5,
                    Delay = 330,
                    DisplayName = "Poppy R Short",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "PoppyR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 90,
                    Range = 450
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Poppy",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    DisplayName = "Poppy R Long",
                    IsDangerous = true,
                    MissileSpeed = 2000,
                    MissileSpellName = "PoppyRMissile",
                    SpellName = "PoppyRMissile",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 1200
                });

            #endregion

            #region Pyke

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Pyke",
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Pyke Q Short",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "PykeQMelee",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 100,
                    Range = 400
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Pyke",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 200,
                    DisplayName = "Pyke Q Long",
                    IsDangerous = true,
                    MissileSpeed = 2000,
                    MissileSpellName = "PykeQRange",
                    SpellName = "PykeQRange",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Pyke",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    DisplayName = "Pyke E",
                    IsDangerous = true,
                    MissileFollowsUnit = true,
                    MissileSpeed = 3000,
                    MissileSpellName = "PykeEMissile",
                    SpellName = "PykeEMissile",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 550
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Pyke",
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Pyke R",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "PykeR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 50,
                    Range = 750
                });

            #endregion

            #region Quinn

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Quinn",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Quinn Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1550,
                    MissileSpellName = "QuinnQ",
                    SpellName = "QuinnQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 1050
                });

            #endregion

            #region Rakan

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Rakan",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Rakan Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1850,
                    MissileSpellName = "RakanQMis",
                    SpellName = "RakanQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 65,
                    Range = 850
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rakan",
                    DangerValue = 3,
                    Delay = 300,
                    DisplayName = "Rakan W",
                    IsDangerous = true,
                    MissileSpeed = 1500,
                    SpellName = "RakanW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 275,
                    Range = 650
                });

            #endregion

            #region RekSai

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "RekSai",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 130,
                    DisplayName = "RekSai Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1950,
                    MissileSpellName = "RekSaiQBurrowedMis",
                    SpellName = "RekSaiQBurrowed",
                    Slot = SpellSlot.Q, 
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 1450
                });

            #endregion

            #region Rengar

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Rengar",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Rengar E",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1500,
                    MissileSpellName = "RengarEMis",
                    SpellName = "RengarE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 1000
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Rengar",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Rengar E Emp",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1500,
                    MissileSpellName = "RengarEEmpMis",
                    SpellName = "RengarEEmp",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 1000
                });

            #endregion

            #region Riven

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Riven",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    Delay = 250,
                    DisplayName = "Riven R",
                    ExtraMissileSpellNames = new[] { "RivenWindslashMissileLeft", "RivenWindslashMissileRight" },
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1600,
                    MissileSpellName = "RivenWindslashMissileCenter",
                    MultipleAngle = 8f * (float)Math.PI / 180,
                    MultipleNumber = 3,
                    SpellName = "RivenIzunaBlade",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 100,
                    Range = 1100
                });

            #endregion

            // from now on ... spell has proper radius & range

            #region Rumble

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Rumble",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Rumble E",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2000,
                    MissileSpellName = "RumbleGrenadeMissile",
                    SpellName = "RumbleGrenade",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 950
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Rumble",
                    DangerValue = 4,
                    Delay = 580,
                    DisplayName = "Rumble R",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileDelayed = true,
                    MissileSpeed = 1600,
                    MissileSpellName = "RumbleCarpetBombMissile",
                    SpellName = "RumbleCarpetBomb-",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 200,
                    Range = 1200
                });

            #endregion

            #region Ryze

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ryze",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Ryze Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    MissileSpellName = "RyzeQ",
                    SpellName = "RyzeQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 55,
                    Range = 1000
                });

            #endregion

            #region Sejuani

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Sejuani",
                    DangerValue = 3,
                    DisplayName = "Sejuani Q",
                    IsDangerous = true,
                    MissileSpeed = 1000,
                    SpellName = "SejuaniQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 75,
                    Range = 650
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Sejuani",
                    DangerValue = 2,
                    Delay = 1000,
                    DisplayName = "Sejuani W",
                    FixedRange = true,
                    FollowCaster = true,
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "SejuaniW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 75,
                    Range = 600
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Sejuani",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    Delay = 250,
                    DisplayName = "Sejuani R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1600,
                    MissileSpellName = "SejuaniRMissile",
                    SpellName = "SejuaniR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 120,
                    Range = 1300
                });

            #endregion

            #region Shen

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Shen",
                    DangerValue = 3,
                    DisplayName = "Shen E",
                    IsDangerous = true,
                    MissileSpeed = 1155,
                    SpellName = "ShenE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 600
                });

            #endregion

            #region Shyvana

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Shyvana",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Shyvana E",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1575,
                    MissileSpellName = "ShyvanaFireballMissile",
                    SpellName = "ShyvanaFireball",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 950
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Shyvana",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 330,
                    DisplayName = "Shyvana E Dragon",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1575,
                    MissileSpellName = "ShyvanaFireballDragonMissile",
                    SpellName = "ShyvanaFireballDragon2",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 975
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Shyvana",
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Shyvana R",
                    ExtraRange = 200,
                    IsDangerous = true,
                    MissileSpeed = 1100,
                    SpellName = "ShyvanaTransformCast",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 160,
                    Range = 850
                });

            #endregion

            #region Sion

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Sion",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Sion E",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1800,
                    MissileSpellName = "SionEMissile",
                    SpellName = "SionE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 800
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Sion",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions },
                    DangerValue = 3,
                    Delay = 130,
                    DisplayName = "Sion R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 950,
                    SpellName = "SionR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 100,
                    Range = 400
                });

            #endregion

            #region Sivir

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Sivir",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Sivir Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1350,
                    MissileSpellName = "SivirQMissile",
                    SpellName = "SivirQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 1250
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Sivir",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Sivir Q Return",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileFollowsUnit = true,
                    MissileSpeed = 1350,
                    MissileSpellName = "SivirQMissileReturn",
                    SpellName = "SivirQMissileReturn",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 100,
                    Range = 1250
                });

            #endregion

            #region Skarner

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Skarner",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Skarner E",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1500,
                    MissileSpellName = "SkarnerFractureMissile",
                    SpellName = "SkarnerFracture",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 1000
                });

            #endregion

            #region Sona

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Sona",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    Delay = 250,
                    DisplayName = "Sona R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 2400,
                    MissileSpellName = "SonaR",
                    SpellName = "SonaR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 140,
                    Range = 1000
                });

            #endregion

            #region Soraka

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Soraka",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Soraka Q",
                    IsDangerous = false,
                    MissileSpeed = 1100,
                    MissileSpellName = "SorakaQMissile",
                    SpellName = "SorakaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 230,
                    Range = 810
                });

            #endregion

            #region Swain

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Swain",
                    DangerValue = 2,
                    Delay = 1500,
                    DisplayName = "Swain W",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "SwainW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 300,
                    Range = 3500
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Swain",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Swain E",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1400, // average
                    MissileSpellName = "SwainE",
                    SpellName = "SwainE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 85,
                    Range = 850
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Swain",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Swain E Return",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileDelayed = true,
                    MissileFollowsUnit = true,
                    MissileSpeed = 1800, // average
                    MissileSpellName = "SwainEReturnMissile",
                    SpellName = "SwainEReturnMissile",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 85,
                    Range = 950
                });

            #endregion

            #region Sylas

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sylas",
                    DangerValue = 2,
                    Delay = 400,
                    DisplayName = "Sylas Q",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "SylasQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 70,
                    Range = 775
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Sylas",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Sylas E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 2500,
                    MissileSpellName = "SylasE2",
                    SpellName = "SylasE2",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 850
                });

            #endregion

            #region Syndra

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Syndra",
                    DangerValue = 2,
                    Delay = 600,
                    DisplayName = "Syndra Q",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "SyndraQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 180,
                    Range = 800
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Syndra",
                    DangerValue = 2,
                    DisplayName = "Syndra W",
                    IsDangerous = false,
                    MissileSpeed = 1500,
                    SpellName = "SyndraWCast",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 195,
                    Range = 950
                });

            #endregion

            #region TahmKench

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "TahmKench",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "TahmKench Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 2800,
                    MissileSpellName = "TahmKenchQMissile",
                    SpellName = "TahmKenchQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 875
                });

            #endregion

            #region Taliyah

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Taliyah",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisabledByDefault = true,
                    DisplayName = "Taliyah Q",
                    DontCheckForDuplicates = true,
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 3600,
                    MissileSpellName = "TaliyahQMis",
                    SpellName = "TaliyahQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 100,
                    Range = 950
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Taliyah",
                    DangerValue = 3,
                    Delay = 800,
                    DisplayName = "Taliyah W",
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "TaliyahWVC",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 200,
                    Range = 900
                });

            #endregion

            #region Talon

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Talon",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Talon W",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2350,
                    MissileSpellName = "TalonWMissileOne",
                    MultipleAngle = 11f * (float)Math.PI / 180,
                    MultipleNumber = 3,
                    SpellName = "TalonW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 75,
                    Range = 900
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Talon",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Talon W Return",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileFollowsUnit = true,
                    MissileSpeed = 3000,
                    MissileSpellName = "TalonWMissileTwo",
                    SpellName = "TalonWMissileTwo",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 75,
                    Range = 900
                });

            #endregion

            #region Taric

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Taric",
                    DangerValue = 3,
                    Delay = 950,
                    DisplayName = "Taric E",
                    FixedRange = true,
                    FollowCaster = true,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    SpellName = "TaricE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 100,
                    Range = 610
                });

            #endregion

            #region Thresh

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Thresh",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Thresh Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1900,
                    MissileSpellName = "ThreshQMissile",
                    SpellName = "ThreshQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    Centered = true,
                    ChampionName = "Thresh",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    DisplayName = "Thresh E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 2000,
                    MissileSpellName = "ThreshEMissile1",
                    SpellName = "ThreshE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 110,
                    Range = 1075
                });

            #endregion

            #region Tristana

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Tristana",
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Tristana W",
                    IsDangerous = false,
                    MissileSpeed = 1110,
                    SpellName = "TristanaW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 270,
                    Range = 900
                });

            #endregion

            #region Tryndamere

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Tryndamere",
                    DangerValue = 2,
                    DisplayName = "Tryndamere E",
                    IsDangerous = false,
                    MissileSpeed = 900,
                    SpellName = "TryndamereE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 160,
                    Range = 650
                });

            #endregion

            #region TwistedFate

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "TwistedFate",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "TwistedFate Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1000,
                    MissileSpellName = "SealFateMissile",
                    MultipleAngle = 28f * (float)Math.PI / 180,
                    MultipleNumber = 3,
                    SpellName = "WildCards",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 40,
                    Range = 1450
                });

            #endregion

            #region Twitch

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Twitch",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Twitch W",
                    IsDangerous = false,
                    MissileSpeed = 1400,
                    MissileSpellName = "TwitchVenomCaskMissile",
                    SpellName = "TwitchVenomCask",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 275,
                    Range = 950
                });

            #endregion

            #region Urgot

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Urgot",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 125,
                    DisplayName = "Urgot Q",
                    IsDangerous = false,
                    MissileSpeed = 3200,
                    MissileSpellName = "UrgotQMissile",
                    SpellName = "UrgotQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 180,
                    Range = 800
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Urgot",
                    DangerValue = 3,
                    Delay = 450,
                    DisplayName = "Urgot E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1530,
                    SpellName = "UrgotE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 100,
                    Range = 475
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Urgot",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    Delay = 500,
                    DisplayName = "Urgot R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 3200,
                    MissileSpellName = "UrgotR",
                    SpellName = "UrgotR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 2500
                });

            #endregion

            #region Varus

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Varus",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Varus Q",
                    IsDangerous = false,
                    MissileSpeed = 1900,
                    MissileSpellName = "VarusQMissile",
                    SpellName = "VarusQMissile",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 1525
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Varus",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 240,
                    DisplayName = "Varus E",
                    IsDangerous = false,
                    MissileSpeed = 1500,
                    MissileSpellName = "VarusEMissile",
                    SpellName = "VarusE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 235,
                    Range = 925
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Varus",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 5,
                    Delay = 240,
                    DisplayName = "Varus R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1950,
                    MissileSpellName = "VarusRMissile",
                    SpellName = "VarusR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 120,
                    Range = 1250
                });

            #endregion

            #region Veigar

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Veigar",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Veigar Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2200,
                    MissileSpellName = "VeigarBalefulStrikeMis",
                    SpellName = "VeigarBalefulStrike",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 950
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Veigar",
                    DangerValue = 2,
                    Delay = 1200,
                    DisplayName = "Veigar W",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "VeigarDarkMatter",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 225,
                    Range = 900
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Veigar",
                    DangerValue = 3,
                    Delay = 500,
                    DisplayName = "Veigar E",
                    DontAddExtraDuration = true,
                    DontCross = true,
                    ExtraDuration = 3500,
                    IsDangerous = true,
                    MissileSpeed = int.MaxValue,
                    RingRadius = 80,
                    SpellName = "VeigarEventHorizon",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotRing,
                    Radius = 350,
                    Range = 725
                });

            #endregion

            #region Velkoz

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Velkoz",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Velkoz Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1300,
                    MissileSpellName = "VelkozQMissile",
                    SpellName = "VelkozQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Velkoz",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Velkoz Q Split",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 2100,
                    MissileSpellName = "VelkozQMissileSplit",
                    SpellName = "VelkozQSplit",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 45,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Velkoz",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Velkoz W",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    MissileSpellName = "VelkozWMissile",
                    SpellName = "VelkozW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 88,
                    Range = 1200
                });

            Spells.Add(
                new SpellData
                {
                    CanBeRemoved = true,
                    ChampionName = "Velkoz",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Velkoz E",
                    IsDangerous = true,
                    MissileSpeed = 1400,
                    MissileSpellName = "VelkozEMissile",
                    SpellName = "VelkozE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 225,
                    Range = 810
                });

            #endregion

            #region Vi

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Vi",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions },
                    DangerValue = 3,
                    DisplayName = "Vi Q",
                    IsDangerous = true,
                    MissileSpeed = 1500,
                    MissileSpellName = "ViQMissile",
                    SpellName = "ViQMissile",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 775
                });

            #endregion

            #region Viktor

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Viktor",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Viktor E",
                    ExtraMissileSpellNames = new[] { "ViktorEAugMissile" },
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1050,
                    MissileSpellName = "ViktorDeathRayMissile",
                    SpellName = "ViktorDeathRayMissile",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 700
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Viktor",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Viktor E Particle",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1500,
                    MissileSpellName = "ViktorDeathRayMissile2",
                    SpellName = "ViktorDeathRayMissile2",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 700
                });

            #endregion

            #region Warwick

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Warwick",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions },
                    DangerValue = 5,
                    Delay = 100,
                    DisplayName = "Warwick R",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 2000,
                    SpellName = "WarwickR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 670
                });

            #endregion

            #region Xayah

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Xayah",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Xayah Q1",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileDelayed = true,
                    MissileSpeed = 400,
                    MissileSpellName = "XayahQMissile1",
                    SpellName = "XayahQMissile1",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Xayah",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 500,
                    DisplayName = "Xayah Q2",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileDelayed = true,
                    MissileSpeed = 400,
                    MissileSpellName = "XayahQMissile2",
                    SpellName = "XayahQMissile2",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Xayah",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    DisplayName = "Xayah E",
                    IsDangerous = true,
                    MissileFollowsUnit = true,
                    MissileSpeed = 4000,
                    MissileSpellName = "XayahEMissile",
                    SpellName = "XayahEMissile",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Xayah",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Xayah R",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 4000,
                    MissileSpellName = "XayahRMissile",
                    SpellName = "XayahRMissile",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 20,
                    Range = 1100
                });

            #endregion

            #region Xerath

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Xerath",
                    DangerValue = 2,
                    Delay = 500,
                    DisplayName = "Xerath Q",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "XerathArcanopulse2",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 90,
                    Range = 1700
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Xerath",
                    DangerValue = 2,
                    Delay = 800,
                    DisplayName = "Xerath W",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "XerathArcaneBarrage2",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 250,
                    Range = 1100
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Xerath",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Xerath E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1400,
                    MissileSpellName = "XerathMageSpearMissile",
                    SpellName = "XerathMageSpear",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 60,
                    Range = 1125
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Xerath",
                    DangerValue = 2,
                    Delay = 600,
                    DisplayName = "Xerath R",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "XerathRMissileWrapper",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 225,
                    Range = 5600
                });

            #endregion

            #region XinZhao

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "XinZhao",
                    DangerValue = 2,
                    Delay = 500,
                    DisplayName = "XinZhao W",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "XinZhaoW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 50,
                    Range = 850
                });

            #endregion

            #region Yasuo

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Yasuo",
                    DangerValue = 2,
                    Delay = 350,
                    DisplayName = "Yasuo Q1",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "YasuoQ1Wrapper",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 55,
                    Range = 475
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Yasuo",
                    DangerValue = 2,
                    Delay = 350,
                    DisplayName = "Yasuo Q2",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "YasuoQ2Wrapper",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 55,
                    Range = 475
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Yasuo",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 350,
                    DisplayName = "Yasuo Q3",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1200,
                    MissileSpellName = "YasuoQ3Mis",
                    SpellName = "YasuoQ3Wrapper",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 90,
                    Range = 1100
                });

            #endregion

            #region Zac

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Zac",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 330,
                    DisplayName = "Zac Q",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 2800,
                    MissileSpellName = "ZacQMissile",
                    SpellName = "ZacQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 80,
                    Range = 800
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zac",
                    DangerValue = 3,
                    DisplayName = "Zac E",
                    IsDangerous = true,
                    MissileSpeed = 1000,
                    SpellName = "ZacE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 240,
                    Range = 1900
                });

            #endregion

            #region Zed

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Zed",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Zed Q",
                    FixedRange = true,
                    FromObjects = new[] { "Zed_.+_W_cloneswap_buf", "Zed_.+_R_cloneswap_buf" },
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    MissileSpellName = "ZedQMissile",
                    SpellName = "ZedQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 925
                });

            #endregion

            #region Ziggs

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ziggs",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Ziggs Q",
                    IsDangerous = false,
                    MissileSpeed = 1700,
                    MissileSpellName = "ZiggsQSpell",
                    SpellName = "ZiggsQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 125,
                    Range = 850
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ziggs",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Ziggs Q Bounce1",
                    IsDangerous = false,
                    MissileSpeed = 500,
                    MissileSpellName = "ZiggsQSpell2",
                    SpellName = "ZiggsQSpell2",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 125,
                    Range = 850
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ziggs",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Ziggs Q Bounce2",
                    IsDangerous = false,
                    MissileSpeed = 500,
                    MissileSpellName = "ZiggsQSpell3",
                    SpellName = "ZiggsQSpell3",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 125,
                    Range = 850
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ziggs",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Ziggs W",
                    IsDangerous = true,
                    MissileSpeed = 1750,
                    MissileSpellName = "ZiggsW",
                    SpellName = "ZiggsW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 275,
                    Range = 1000
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Ziggs",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Ziggs E",
                    IsDangerous = false,
                    MissileSpeed = 1550,
                    MissileSpellName = "ZiggsE2",
                    SpellName = "ZiggsE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 235,
                    Range = 900
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Ziggs",
                    DangerValue = 2,
                    Delay = 380,
                    DisplayName = "Ziggs R",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "ZiggsR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 500,
                    Range = 5000
                });

            #endregion

            #region Zilean

            Spells.Add(
                new SpellData
                {
                    CanBeRemoved = true,
                    ChampionName = "Zilean",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Zilean Q",
                    IsDangerous = false,
                    MissileSpeed = 1000,
                    MissileSpellName = "ZileanQMissile",
                    SpellName = "ZileanQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Radius = 150,
                    Range = 900
                });

            #endregion

            #region Zoe

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Zoe",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    Delay = 250,
                    DisplayName = "Zoe Q",
                    FixedRange = true,
                    IsDangerous = false,
                    MissileSpeed = 1200,
                    MissileSpellName = "ZoeQMissile",
                    SpellName = "ZoeQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 800
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Zoe",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 2,
                    DisplayName = "Zoe Q Recast",
                    IsDangerous = false,
                    MissileSpeed = 2500,
                    MissileSpellName = "ZoeQMis2",
                    SpellName = "ZoeQMis2",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 10000
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Zoe",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 300,
                    DisplayName = "Zoe E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1850,
                    MissileSpellName = "ZoeE",
                    SpellName = "ZoeE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 800
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Zoe",
                    CollisionObjects = new[] { CollisionObjectTypes.Champions, CollisionObjectTypes.Minion, CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    DisplayName = "Zoe E Cross",
                    IsDangerous = true,
                    MissileSpeed = 1850,
                    MissileSpellName = "ZoeEMis",
                    SpellName = "ZoeEMis",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 50,
                    Range = 10000
                });

            #endregion

            #region Zyra

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    ChampionName = "Zyra",
                    DangerValue = 2,
                    Delay = 850,
                    DisplayName = "Zyra Q",
                    IsDangerous = false,
                    MissileSpeed = int.MaxValue,
                    SpellName = "ZyraQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Radius = 125,
                    Range = 800
                });

            Spells.Add(
                new SpellData
                {
                    AddHitbox = true,
                    CanBeRemoved = true,
                    ChampionName = "Zyra",
                    CollisionObjects = new[] { CollisionObjectTypes.YasuoWall },
                    DangerValue = 3,
                    Delay = 250,
                    DisplayName = "Zyra E",
                    FixedRange = true,
                    IsDangerous = true,
                    MissileSpeed = 1150,
                    MissileSpellName = "ZyraE",
                    SpellName = "ZyraE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Radius = 70,
                    Range = 1150
                });

            #endregion

            if (Config.PrintSpellData)
            {
                Logging.Write(false, true, "EV")(LogLevel.Info, "[EvadeSharp] Added {0} spells into SpellDatabase.", Spells.Count);
            }
        }

        public static SpellData GetBySourceObjectName(string objectName)
        {
            objectName = objectName.ToLower();
            foreach (var spellData in Spells)
            {
                if (spellData.SourceObjectName != null && objectName.Contains(spellData.SourceObjectName.ToLower()))
                {
                    return spellData;
                }
            }
            return null;
        }

        public static SpellData GetByName(string spellName)
        {
            spellName = spellName.ToLower();
            foreach (var spellData in Spells)
            {
                if (spellData.SpellName != null && spellData.SpellName.ToLower() == spellName || spellData.ExtraSpellNames.Contains(spellName, StringComparer.OrdinalIgnoreCase))
                {
                    return spellData;
                }
                if (spellData.SpellName != null && spellData.SpellName == "KarthusLayWasteA")
                {
                    if (spellName.Contains(spellData.SpellName.ToLower()) || spellData.ExtraSpellNames.Any(item => spellName.Contains(item.ToLower())))
                    {
                        return spellData;
                    }
                }
            }
            return null;
        }

        public static SpellData GetByMissileName(string missileSpellName)
        {
            missileSpellName = missileSpellName.ToLower();
            foreach (var spellData in Spells)
            {
                if (spellData.MissileSpellName != null && spellData.MissileSpellName.ToLower() == missileSpellName || spellData.ExtraMissileSpellNames.Contains(missileSpellName, StringComparer.OrdinalIgnoreCase))
                {
                    return spellData;
                }
                if (spellData.MissileSpellName != null && spellData.MissileSpellName == "HowlingGaleSpell" && missileSpellName.Contains(spellData.MissileSpellName.ToLower()))
                {
                    return spellData;
                }
            }
            return null;
        }
    }
}