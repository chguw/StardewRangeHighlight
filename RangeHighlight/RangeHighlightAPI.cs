﻿// Copyright 2020 Jamie Taylor
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Buildings;

namespace RangeHighlight {
    public class RangeHighlightAPI : IRangeHighlightAPI {
        private ModConfig config;
        private RangeHighlighter rangeHighlighter;

        public RangeHighlightAPI(ModEntry mod) {
            // can't take these types as args directly because they are less
            // visible than this class (which must be public because SMAPI
            // doesn't currently support using a non-public implementation
            // class for a mod's API).
            this.config = mod.config;
            this.rangeHighlighter = mod.highlighter;
        }

        // ----- Getters for the currently configured tint colors ----

        public Color GetJunimoRangeTint() {
            return config.JunimoRangeTint;
        }

        public Color GetScarecrowRangeTint() {
            return config.ScarecrowRangeTint;
        }

        public Color GetSprinklerRangeTint() {
            return config.SprinklerRangeTint;
        }

        public Color GetBeehouseRangeTint() {
            return config.BeehouseRangeTint;
        }

        // ----- Helpers for making highlight shapes -----

        private bool[,] GetCircle(int radius, bool excludeCenter, Func<int, int, int> CalcDistance) {
            int size = 2 * radius + 1;
            bool[,] result = new bool[size, size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    int dist = CalcDistance(i, j);
                    result[i, j] = dist <= radius;
                }
            }
            if (excludeCenter) {
                result[radius, radius] = false;
            }
            return result;
        }

        public bool[,] GetCartesianCircle(uint radius, bool excludeCenter = true) {
            int r = (int)radius;
            return GetCircle(r, excludeCenter, (i, j) =>
                (int)Math.Truncate(Math.Sqrt((r - i) * (r - i) + (r - j) * (r - j))));
        }

        public bool[,] GetManhattanCircle(uint radius, bool excludeCenter = true) {
            int r = (int)radius;
            return GetCircle(r, excludeCenter, (i, j) =>
                (int)(Math.Abs(r - i) + Math.Abs(r - j)));
        }

        public bool[,] GetSquareCircle(uint radius, bool excludeCenter = true) {
            int r = (int)radius;
            return GetCircle(r, excludeCenter, (i, j) =>
                (int)(Math.Max(Math.Abs(r - i), Math.Abs(r - j))));
        }

        // ----- Hooks for applying highlights ----

        void IRangeHighlightAPI.AddBuildingRangeHighlighter(string uniqueId, SButton? hotkey, Func<Building, Tuple<Color, bool[,], int, int>> highlighter) {
            rangeHighlighter.AddBuildingHighlighter(uniqueId, hotkey, highlighter);
        }

        void IRangeHighlightAPI.AddItemRangeHighlighter(string uniqueId, SButton? hotkey, Func<string, Tuple<Color, bool[,]>> highlighter) {
            rangeHighlighter.AddItemHighlighter(uniqueId, hotkey, highlighter);
        }

        public void RemoveBuildingRangeHighlighter(string uniqueId) {
            rangeHighlighter.RemoveBuildingHighlighter(uniqueId);
        }

        public void RemoveItemRangeHighlighter(string uniqueId) {
            rangeHighlighter.RemoveItemHighlighter(uniqueId);
        }
    }
}
