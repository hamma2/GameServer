﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using LeagueSandbox.GameServer.Enet;
using LeagueSandbox.GameServer.Packets;
using LeagueSandbox.GameServer.Players;

namespace LeagueSandbox.GameServer
{
    static class Extensions
    {
        public static float SqrLength(this Vector2 v)
        {
            return v.X * v.X + v.Y * v.Y;
        }

        public static long ElapsedNanoSeconds(this Stopwatch watch)
        {
            return watch.ElapsedTicks * 1000000000 / Stopwatch.Frequency;
        }

        public static long ElapsedMicroSeconds(this Stopwatch watch)
        {
            return watch.ElapsedTicks * 1000000 / Stopwatch.Frequency;
        }

        public static void Add(this List<byte> list, int val)
        {
            list.AddRange(PacketHelper.intToByteArray(val));
        }

        public static void Add(this List<byte> list, string val)
        {
            list.AddRange(Encoding.BigEndianUnicode.GetBytes(val));
        }

        public static void fill(this BinaryWriter list, byte data, int length)
        {
            for (var i = 0; i < length; ++i)
            {
                list.Write(data);
            }
        }

        public static Vector2 Rotate(this Vector2 v, Vector2 origin, float angle)
        {
            // Rotating (px,py) around (ox, oy) with angle a
            // p'x = cos(a) * (px-ox) - sin(a) * (py-oy) + ox
            // p'y = sin(a) * (px-ox) + cos(a) * (py-oy) + oy
            angle = (float) -DegreeToRadian(angle);
            var x = (float) (Math.Cos(angle) * (v.X - origin.X) - Math.Sin(angle) * (v.Y - origin.Y) + origin.X);
            var y = (float) (Math.Sin(angle) * (v.X - origin.X) + Math.Cos(angle) * (v.Y - origin.Y) + origin.Y);
            return new Vector2(x, y);
        }

        public static Vector2 Rotate(this Vector2 v, float angle)
        {
            return v.Rotate(new Vector2(0, 0), angle);
        }

        public static float AngleBetween(this Vector2 v, Vector2 vectorToGetAngle, Vector2 origin)
        {
            // Make other vectors relative to the origin
            v.X -= origin.X;
            vectorToGetAngle.X -= origin.X;
            v.Y -= origin.Y;
            vectorToGetAngle.Y -= origin.Y;

            // Normalize the vectors
            v = Vector2.Normalize(v);
            vectorToGetAngle = Vector2.Normalize(vectorToGetAngle);

            // Get the angle
            var ang = Vector2.Dot(v, vectorToGetAngle);
            var returnVal = (float) RadianToDegree(Math.Acos(ang));
            if (vectorToGetAngle.X < v.X)
            {
                returnVal = 360 - returnVal;
            }

            return returnVal;
        }

        public static float AngleBetween(this Vector2 v, Vector2 vectorToGetAngle)
        {
            return v.AngleBetween(vectorToGetAngle, new Vector2(0, 0));
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }

    public class PairList<TKey, TValue> : List<Pair<TKey, TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            Add(new Pair<TKey, TValue>(key, value));
        }

        public bool ContainsKey(TKey key)
        {
            foreach (var v in this)
                if (v.Item1.Equals(key))
                    return true;

            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                foreach (var v in this)
                {
                    if (v.Item1.Equals(key))
                    {
                        return v.Item2;
                    }
                }

                return default(TValue);
            }
            set
            {
                foreach (var v in this)
                {
                    if (v.Item1.Equals(key))
                    {
                        v.Item2 = value;
                    }
                }
            }
        }
    }

    public static class CustomConvert
    {
        private static PlayerManager _playerManager = Program.ResolveDependency<PlayerManager>();

        public static Team ToTeamId(int i)
        {
            var dic = new Dictionary<int, Team>
            {
                {0, Team.Order},
                {(int) Team.Order, Team.Order},
                {1, Team.Chaos},
                {(int) Team.Chaos, Team.Chaos}
            };

            if (!dic.ContainsKey(i))
            {
                return (Team) 2;
            }

            return dic[i];
        }

        public static int FromTeamId(Team team)
        {
            var dic = new Dictionary<Team, int>
            {
                {Team.Order, 0},
                {Team.Chaos, 1}
            };

            if (!dic.ContainsKey(team))
            {
                return 2;
            }

            return dic[team];
        }

        public static Team GetEnemyTeam(Team team)
        {
            var dic = new Dictionary<Team, Team>
            {
                {Team.Order, Team.Chaos},
                {Team.Chaos, Team.Order}
            };

            if (!dic.ContainsKey(team))
            {
                return (Team) 2;
            }

            return dic[team];
        }
    }
}