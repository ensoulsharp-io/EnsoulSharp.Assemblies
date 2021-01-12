namespace SPrediction
{
    using System;

    /// <summary>
    /// Enum for collision flags
    /// </summary>
    [Flags]
    public enum CollisionFlags
    {
        None = 0,
        Minions = 1,
        AllyChampions = 2,
        EnemyChampions = 4,
        Wall = 8,
        YasuoWall = 16,
    }
}
